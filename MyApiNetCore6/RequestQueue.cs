// Decompiled with JetBrains decompiler
// Type: API_QuanLyTHP.RequestQueue
// Assembly: API_QuanLyTHP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC050ACB-EFEA-4AC7-80CD-78C98E6478D1
// Assembly location: G:\MyApiNetCore6-03_Authentication_New\Publish_API\API_QuanLyTHP.dll

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace API_QuanLyTHP;

public class RequestQueue
{
  private readonly ConcurrentQueue<Func<Task>> _queue = new ConcurrentQueue<Func<Task>>();
  private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

  public async Task Enqueue(Func<Task> task)
  {
    this._queue.Enqueue(task);
    await this.ProcessQueue();
  }

  private async Task ProcessQueue()
  {
    Func<Task> task;
    if (!this._queue.TryDequeue(out task))
    {
      task = (Func<Task>) null;
    }
    else
    {
      await this._semaphore.WaitAsync();
      try
      {
        await task();
      }
      finally
      {
        this._semaphore.Release();
        await this.ProcessQueue();
      }
      task = (Func<Task>) null;
    }
  }
}
