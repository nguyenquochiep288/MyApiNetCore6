using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace API_QuanLyTHP
{
    public class RequestQueue
    {
        private readonly ConcurrentQueue<Func<Task>> _queue = new ConcurrentQueue<Func<Task>>();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public async Task Enqueue(Func<Task> task)
        {
            _queue.Enqueue(task);
            await ProcessQueue();
        }

        private async Task ProcessQueue()
        {
            if (_queue.TryDequeue(out var task))
            {
                await _semaphore.WaitAsync();
                try
                {
                    await task(); // Xử lý yêu cầu hiện tại
                }
                finally
                {
                    _semaphore.Release();
                    await ProcessQueue(); // Xử lý tiếp yêu cầu tiếp theo
                }
            }
        }
    }
}
