using System.Threading;
using System.Threading.Tasks;

namespace Templates.TodoApp.Host.HostedService
{
    public interface IScopedProcessingService
    {
        Task DoWorkAsync(CancellationToken stoppingToken);
    }
}
