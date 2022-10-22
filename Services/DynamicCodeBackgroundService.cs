using Backend.Enties;
using Furion.DatabaseAccessor;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Backend.Services {
    public class DynamicCodeBackgroundService : BackgroundService {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            while (!stoppingToken.IsCancellationRequested) {
                var taskFactory = new TaskFactory(TaskScheduler.Current);
                await taskFactory.StartNew(async () => {
                    var dynamicCodeService = new DynamicCodeService(Db.GetRepository<DynamicCode>());
                    await dynamicCodeService.DeleteExpiredCode();
                }, stoppingToken);
                await Task.Delay(60 * 1000, stoppingToken);
            }
        }
    }
}
