using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Utils.Tasks
{
    public interface IBackgroundTaskQueue
    {
        void Enqueue(Func<IServiceProvider, Task> workItem);

        public bool TryDequeue(out Func<IServiceProvider, Task>? workItem);
    }
}
