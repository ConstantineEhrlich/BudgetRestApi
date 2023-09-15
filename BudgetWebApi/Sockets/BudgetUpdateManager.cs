using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

using static BudgetWebApi.Sockets.SocketListener;

namespace BudgetWebApi.Sockets;

public class BudgetUpdateManager: IUpdateManager
{
    private readonly ConcurrentDictionary<string, List<WebSocket>> _sockets = new();

    public void AddSocket(string budgetId, WebSocket socket)
    {
        // New key
        if (!_sockets.ContainsKey(budgetId))
        {
            _sockets.TryAdd(budgetId, new List<WebSocket>() { socket });
        }
        else // existing key
        {
            _sockets[budgetId].Add(socket);
        }
    }

    public async void BroadcastUpdate(string budgetId)
    {
        if (_sockets.TryGetValue(budgetId, out List<WebSocket> clients))
        {
            clients.Where(client => client.State == WebSocketState.Open).ToList().ForEach(async s =>
            {
                ReadOnlyMemory<byte> message =  new(Encoding.ASCII.GetBytes($"Update in budget {budgetId}"));
                await s.SendAsync(message,
                    WebSocketMessageType.Text,
                    WebSocketMessageFlags.EndOfMessage,
                    CancellationToken.None);
            });
        }

    }
    public void RemoveDeadSockets()
    {
        foreach (var sockets in _sockets.Values)
        {
            sockets.RemoveAll(s => s.State != WebSocketState.Open);
        }
    }

    
}