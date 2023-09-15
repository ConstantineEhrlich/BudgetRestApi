using System.Net.WebSockets;

namespace BudgetWebApi.Sockets;

public interface IUpdateManager
{
    public void RemoveDeadSockets();
}