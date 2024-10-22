using DBQueue.Model;

namespace GINotificationManager.Tasks
{
    public interface IWorkTask<T>
    {
        Task RunAsync(Message message);
    }
}
