using DBQueue.Model;

namespace GINotificationManager.Tasks
{
    public class ScartaNotificaTask : IWorkTask<ScartaNotificaTask>
    {
        private readonly ILogger<ScartaNotificaTask> _logger;
        private readonly DBQueue.IQueueProvider _queueProvider;

        public ScartaNotificaTask(ILogger<ScartaNotificaTask> logger, DBQueue.IQueueProvider queueProvider)
        {
            _logger = logger;
            _queueProvider = queueProvider;
        }

        public async Task RunAsync(Message message)
        {
            _logger.LogInformation("Run Scarto Notifica Task");
            await Task.Delay(500);

            DBQueue.IConsumer consumer = _queueProvider.GetQueueConsumer();
            await consumer.DequeueAsync(message.Header.MessageId);

        }
    }
}
