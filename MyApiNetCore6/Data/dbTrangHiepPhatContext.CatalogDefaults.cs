using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using DatabaseTHP;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using THP.Catalogs;

namespace MyApiNetCore6.Data
{
    public partial class dbTrangHiepPhatContext
    {
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            return SaveChangesAsync(acceptAllChangesOnSuccess, CancellationToken.None).GetAwaiter().GetResult();
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            // This boundary also covers the existing create/edit/import controllers.
            var changes = ChangeTracker.Entries().Where(e =>
                (e.State == EntityState.Added || e.State == EntityState.Modified)
                && CatalogFlagDefinitions.Allows(e.Metadata.ClrType, "ISDEFAULT")
                && (e.State == EntityState.Added || e.Property("ISDEFAULT").IsModified)).ToList();
            if (changes.Count == 0)
                return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken).ConfigureAwait(false);

            var scopes = changes.GroupBy(e => e.Metadata.ClrType.FullName + "|"
                    + Convert.ToString(e.Property("LOC_ID").CurrentValue)?.TrimEnd().ToUpperInvariant())
                .OrderBy(g => g.Key, StringComparer.Ordinal).ToList();
            IDbContextTransaction? ownedTransaction = null;
            try
            {
                if (Database.CurrentTransaction == null)
                    ownedTransaction = await Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
                // Serialize default writes for a catalog/branch before clearing and saving.
                // Transaction ownership releases locks on commit/rollback, also across API instances.
                foreach (var scope in scopes)
                {
                    var resource = "CatalogDefault:" + Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(scope.Key)));
                    await Database.ExecuteSqlInterpolatedAsync($@"
DECLARE @result int;
EXEC @result = sys.sp_getapplock @Resource={resource}, @LockMode='Exclusive',
    @LockOwner='Transaction', @LockTimeout=15000;
IF @result < 0 THROW 51000, N'Không thể khóa cập nhật mặc định. Vui lòng thử lại.', 1;",
                        cancellationToken).ConfigureAwait(false);
                }
                foreach (var scope in scopes)
                {
                    var winners = scope.Where(e => (bool)e.Property("ISDEFAULT").CurrentValue!).ToList();
                    if (winners.Count > 1)
                        throw new InvalidOperationException("Chỉ được chọn một mặc định trong mỗi danh mục và chi nhánh.");
                    if (winners.Count == 0) continue;
                    var winner = winners[0];
                    var locId = (string)winner.Property("LOC_ID").CurrentValue!;
                    var id = (string)winner.Property("ID").CurrentValue!;
                    var method = typeof(dbTrangHiepPhatContext).GetMethod(nameof(FindOtherDefaults), BindingFlags.NonPublic | BindingFlags.Instance)!
                        .MakeGenericMethod(winner.Metadata.ClrType);
                    var others = await ((Task<List<object>>)method.Invoke(this, new object[] { locId, id, cancellationToken })!)
                        .ConfigureAwait(false);
                    foreach (var other in others)
                    {
                        var entry = Entry(other);
                        if (entry.State == EntityState.Deleted) continue;
                        entry.Property("ISDEFAULT").CurrentValue = false;
                        entry.Property("ISDEFAULT").IsModified = true;
                        if (entry.Metadata.FindProperty("THOIGIANSUA") != null)
                            entry.Property("THOIGIANSUA").CurrentValue = DateTime.UtcNow.AddHours(7);
                        if (entry.Metadata.FindProperty("ID_NGUOISUA") != null)
                            entry.Property("ID_NGUOISUA").CurrentValue = Editor(winner);
                        AuditLog.Add(new AuditLog
                        {
                            ID_DINHKEM = Guid.NewGuid().ToString(),
                            ID_PHIEU = Convert.ToString(entry.Property("ID").CurrentValue),
                            ENTITYNAME = entry.Metadata.ClrType.Name,
                            OPERATION = "Modified",
                            DATA = JsonConvert.SerializeObject(entry.CurrentValues.ToObject()),
                            TIMESTAMP = DateTime.Now
                        });
                    }
                }
                var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken).ConfigureAwait(false);
                if (ownedTransaction != null)
                    await ownedTransaction.CommitAsync(cancellationToken).ConfigureAwait(false);
                return result;
            }
            catch
            {
                if (ownedTransaction != null)
                    await ownedTransaction.RollbackAsync(CancellationToken.None).ConfigureAwait(false);
                throw;
            }
            finally
            {
                if (ownedTransaction != null) await ownedTransaction.DisposeAsync().ConfigureAwait(false);
            }
        }

        private static object? Editor(EntityEntry entry)
        {
            var property = entry.State == EntityState.Added ? "ID_NGUOITAO" : "ID_NGUOISUA";
            return entry.Metadata.FindProperty(property) != null ? entry.Property(property).CurrentValue : null;
        }

        private async Task<List<object>> FindOtherDefaults<TEntity>(string locId, string id, CancellationToken cancellationToken)
            where TEntity : class
        {
            var rows = await Set<TEntity>().Where(e => EF.Property<string>(e, "LOC_ID") == locId
                && EF.Property<string>(e, "ID") != id && EF.Property<bool>(e, "ISDEFAULT"))
                .ToListAsync(cancellationToken).ConfigureAwait(false);
            return rows.Cast<object>().ToList();
        }
    }
}
