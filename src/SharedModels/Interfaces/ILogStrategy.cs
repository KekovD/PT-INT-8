using System.Threading.Tasks;

namespace SharedModels.Interfaces;

public interface ILogStrategy
{
    Task LogAsync(string message);
}