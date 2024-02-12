using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

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
        // Check that there are any open sockets for this budget id
        if (!_sockets.TryGetValue(budgetId, out List<WebSocket>? clients)) return;
        
        IEnumerable<Task> tasks = clients.Where(client => client.State == WebSocketState.Open).Select(async s =>
        {
            ReadOnlyMemory<byte> message =  new(Encoding.ASCII.GetBytes($"Update in budget {budgetId}"));
            await s.SendAsync(message,
                WebSocketMessageType.Text,
                WebSocketMessageFlags.EndOfMessage,
                CancellationToken.None);
        });

        await Task.WhenAll(tasks);

    }
    public void RemoveDeadSockets()
    {
        foreach (List<WebSocket> sockets in _sockets.Values)
        {
            sockets.RemoveAll(s => s.State != WebSocketState.Open);
        }
    }

    
}