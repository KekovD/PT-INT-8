using System.Threading;
using System.Threading.Tasks;

namespace Initiator.Services.Interfaces;

public interface IHttpOperations
{
    Task PostAsJsonAsync<T>(string uri, T value, CancellationToken cancellationToken);
}