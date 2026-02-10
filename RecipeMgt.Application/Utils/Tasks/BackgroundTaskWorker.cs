using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Utils.Tasks
{
    public class BackgroundTaskWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IBackgroundTaskQueue _queue;
        private ILogger<BackgroundTaskWorker> _logger;

        public BackgroundTaskWorker(IServiceProvider serviceProvider, IBackgroundTaskQueue queue, ILogger<BackgroundTaskWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _queue = queue;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if(_queue.TryDequeue(out var workItem))
                {
                    try
                    {
                        using var scope = _serviceProvider.CreateScope();
                        await workItem(scope.ServiceProvider);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Background worker failed");
                    }
                }
                await Task.Delay(50, stoppingToken);
            }
        }
    }
}
