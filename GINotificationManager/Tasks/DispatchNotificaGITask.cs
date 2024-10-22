using DBQueue.Model;
using GINotificationManager.Model;
using GINotificationManager.Services;
using Microsoft.AspNetCore.SignalR;

namespace GINotificationManager.Tasks
{
    public class DispatchNotificaGITask : IWorkTask<DispatchNotificaGITask>
    {
        private readonly ILogger<DispatchNotificaGITask> _logger;
        private readonly DBQueue.IQueueProvider _queueProvider;

        public DispatchNotificaGITask(ILogger<DispatchNotificaGITask> logger, DBQueue.IQueueProvider queueProvider)
        {
            _logger = logger;
            _queueProvider = queueProvider;
        }

        public async Task RunAsync(Message message)
        {

            try
            {
                if (message == null)
                    throw new ArgumentNullException("Message is null");

                if (message.Body == null)
                    throw new ArgumentNullException("Message id {0} don't have a body");

                if (message.Body.Text == null || message.Body.Text == string.Empty)
                    throw new ArgumentNullException($"Message id {message.Header.MessageId} don't have any text in MessageBody");

                var ngi = System.Text.Json.JsonSerializer.Deserialize<NotificaGI>(message.Body.Text);

                if (ngi != null && ngi.TipoNotificaGI != null)
                {
                    foreach (var t in ngi.TipoNotificaGI)
                    {
                        NotificaGISingola n = new NotificaGISingola() { 
                            TipoNotificaGI = t, 
                            DataOraNotifica = ngi.DataOraNotifica, 
                            PrgVersioneGI = ngi.PrgVersioneGI, 
                            ProtocolloGI = ngi.ProtocolloGI 
                        };

                        string j = System.Text.Json.JsonSerializer.Serialize(n);

                        // add to queue
                        DBQueue.IPublisher pub = _queueProvider.GetQueuePublisher();

                        switch (t)
                        {
                            case 2:
                                // enque new message for post elaboration
                                Message newMessage = await pub.EnqueueAsync("NotificaCambioStato", j);
                                break;

                            default:
                                _logger.LogWarning("unknown");
                                break;
                        }

                    }

                    // deque current message
                    DBQueue.IConsumer consumer = _queueProvider.GetQueueConsumer();
                    await consumer.DequeueAsync(message.Header.MessageId);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

        }
    }
}
