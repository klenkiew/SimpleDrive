using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AuthenticationService.Extensions
{
    internal static class TaskExtensions
    {
        internal static void LogErrors<T>(
            this Task task, 
            ILogger<T> logger, 
            string message = "Fire & forget asynchronous operation failed")
        {
            task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                    logger.LogError(t.Exception, message);
            });
        }
    }
}