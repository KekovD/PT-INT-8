using System.Threading.Tasks;

namespace Initiator.Services.Interfaces;

public interface IMessageQueueService
{
    Task SubscribeToMessages();
}