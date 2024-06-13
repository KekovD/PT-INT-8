using System.Threading.Tasks;

namespace SharedModels;

public interface ILogStrategy
{
    Task LogAsync(string message);
}