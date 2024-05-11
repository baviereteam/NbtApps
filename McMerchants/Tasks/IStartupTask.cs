using System.Threading;
using System.Threading.Tasks;

namespace McMerchants.Tasks
{
    public interface IStartupTask
    {
        Task ExecuteAsync(CancellationToken cancellationToken = default);
    }
}
