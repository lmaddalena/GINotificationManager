using DBQueue.Model;

namespace GINotificationManager.Tasks
{
    public class CambioStatoTask : IWorkTask<CambioStatoTask>
    {
        private readonly ILogger<CambioStatoTask> _logger;
        private readonly DBQueue.IQueueProvider _queueProvider;

        public CambioStatoTask(ILogger<CambioStatoTask> logger, DBQueue.IQueueProvider queueProvider)
        {
            _logger = logger;
            _queueProvider = queueProvider;
        }

        public async Task RunAsync(Message message)
        {
            _logger.LogInformation("Run Cambio Stato Task");
            await Task.Delay(15000);

            DBQueue.IConsumer consumer = _queueProvider.GetQueueConsumer();
            await consumer.DequeueAsync(message.Header.MessageId);

            _logger.LogInformation("Exit Cambio Stato Task");

        }
    }
}
