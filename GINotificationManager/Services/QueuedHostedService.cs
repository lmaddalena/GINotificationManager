
using GINotificationManager.Tasks;

namespace GINotificationManager.Services
{
    public class QueuedHostedService : BackgroundService
    {
        private readonly ILogger<QueuedHostedService> _logger;
        private readonly DBQueue.IQueueProvider _queueProvider;
        private readonly IWorkTask<DispatchNotificaGITask> _dispatchNotificaGITask;
        private readonly IWorkTask<CambioStatoTask> _cambioStatoTask;
        private readonly IWorkTask<ScartaNotificaTask> _scartoNotificaTask;
        public QueuedHostedService(
            ILogger<QueuedHostedService> logger, 
            DBQueue.IQueueProvider queueProvider,
            IWorkTask<DispatchNotificaGITask> dispatchNotificaGITask,
            IWorkTask<CambioStatoTask> cambioStatoTask,
            IWorkTask<ScartaNotificaTask> scartoNotificaTask)
        {
            _logger = logger;
            _queueProvider = queueProvider;
            _dispatchNotificaGITask = dispatchNotificaGITask;
            _cambioStatoTask = cambioStatoTask;
            _scartoNotificaTask = scartoNotificaTask;

            _logger.LogInformation(
                $"Queued Hosted Serviceis instantiated.{Environment.NewLine}");

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                    $"Notifiche Dispatcher Hosted Serviceis running.{Environment.NewLine}");

            await BackgroundProcessing(stoppingToken);

        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var message =
                    await _queueProvider.GetQueueConsumer().GetMessageFromQueueAsync();

                try
                {
                    if (message != null)
                    {
                        switch (message.Header.Tag)
                        {
                            case "NotificaGI":
                                _ = _dispatchNotificaGITask.RunAsync(message);
                                break;

                            case "NotificaCambioStato":
                                _ = _cambioStatoTask.RunAsync(message);
                                break;


                            default:
                                await _scartoNotificaTask.RunAsync(message);
                                break;
                                
                        }
                    }
                    else
                    {
                        await Task.Delay(500);
                    }

                }
                catch (Exception ex)
                {
                    if(message != null)
                        _logger.LogError(ex,
                            "Error occurred executing MessageId:{0}.", message.Header.MessageId);
                    else
                        _logger.LogError(ex,
                            "Error occurred in BackgroundProcessing");

                }

            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Queued Hosted Service is stopping.");

            await base.StopAsync(stoppingToken);
        }

    }
}
