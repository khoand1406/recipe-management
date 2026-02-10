using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RecipeMgt.Application.Services.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Utils.Worker
{
    public class RelateDishBehaviorWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<RelateDishBehaviorWorker> _logger;

        public RelateDishBehaviorWorker(
            IServiceScopeFactory scopeFactory,
            ILogger<RelateDishBehaviorWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("RelatedDishBehaviorWorker started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var job = scope.ServiceProvider.GetRequiredService<IRelateBgJob>();

                    await job.RunAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "RelateDishBehaviorWorker failed");
                }

                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }
    }

}
