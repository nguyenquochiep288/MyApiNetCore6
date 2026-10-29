using DatabaseTHP;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApiNetCore6.Data;
using THP.Catalogs;

namespace MyApiNetCore6.Controllers
{
    [ApiController, Route("api/[controller]"), Authorize(Roles = "User")]
    public class CatalogFlagsController : ControllerBase
    {
        private readonly dbTrangHiepPhatContext context;
        private readonly IConfiguration configuration;
        public CatalogFlagsController(dbTrangHiepPhatContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        public class CatalogFlagUpdateRequest
        {
            public string FIELD { get; set; } = "";
            public bool? ENABLED { get; set; }
            public string? USER_ID { get; set; }
        }

        [HttpPut("{catalog}/{locId}/{id}")]
        public async Task<IActionResult> Update(string catalog, string locId, string id,
            [FromBody] CatalogFlagUpdateRequest request, CancellationToken cancellationToken)
        {
            var type = CatalogFlagDefinitions.Find(catalog);
            if (!CatalogFlagDefinitions.Allows(type, request.FIELD) || !request.ENABLED.HasValue
                || string.IsNullOrWhiteSpace(locId) || locId.Length > 50
                || string.IsNullOrWhiteSpace(id) || id.Length > 50 || request.USER_ID?.Length > 50)
                return BadRequest(new ApiResponse { Success = false, Message = "Yêu cầu cập nhật danh mục không hợp lệ." });

            var entityType = context.Model.FindEntityType(type)!;
            var keys = entityType.FindPrimaryKey()!.Properties.Select(p =>
                (object)(p.Name == "LOC_ID" ? locId : id)).ToArray();
            var entity = await context.FindAsync(type, keys, cancellationToken);
            if (entity == null)
                return NotFound(new ApiResponse { Success = false, Message = "Không tìm thấy dòng danh mục." });

            // Only the requested flag and audit fields are changed, including on system rows.
            var entry = context.Entry(entity);
            entry.Property(request.FIELD).CurrentValue = request.ENABLED.Value;
            entry.Property(request.FIELD).IsModified = true;
            if (entityType.FindProperty("THOIGIANSUA") != null)
                entry.Property("THOIGIANSUA").CurrentValue = DateTime.UtcNow.AddHours(7);
            if (entityType.FindProperty("ID_NGUOISUA") != null)
                entry.Property("ID_NGUOISUA").CurrentValue = request.USER_ID;
            new AuditLogController(context, configuration).InserAuditLog();
            await context.SaveChangesAsync(cancellationToken);
            return Ok(new ApiResponse { Success = true, Message = "Đã lưu thay đổi.",
                Data = new { ID = id, FIELD = request.FIELD, ENABLED = request.ENABLED.Value } });
        }
    }
}
