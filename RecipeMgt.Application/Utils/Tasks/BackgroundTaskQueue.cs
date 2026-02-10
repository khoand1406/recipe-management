using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Utils.Tasks
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly ConcurrentQueue<Func<IServiceProvider, Task>> _queue = new();
        public void Enqueue(Func<IServiceProvider, Task> workItem) => _queue.Enqueue(workItem);

        public bool TryDequeue(out Func<IServiceProvider, Task>? workItem)
        => _queue.TryDequeue(out workItem);

    }
}
