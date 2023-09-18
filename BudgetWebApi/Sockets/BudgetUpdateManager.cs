using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

using static BudgetWebApi.Sockets.SocketListener;

namespace BudgetWebApi.Sockets;

public class BudgetUpdateManager: IUpdateManager
{
    private readonly ConcurrentDictionary<string, List<WebSocket>> _sockets = new();
    private readonly ILogger<BudgetUpdateManager> _logger;

    public BudgetUpdateManager(ILogger<BudgetUpdateManager> logger)
    {
        _logger = logger;
    }

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
        int examined = 0;
        int left = 0;
        foreach (var sockets in _sockets.Values)
        {
            examined += sockets.Count;
            sockets.RemoveAll(s => s.State != WebSocketState.Open);
            left += sockets.Count;
        }

        string checkStatus = $"Removing dead sockets: {examined} checked, {left} left open";
        Console.WriteLine(checkStatus);
        _logger.Log(LogLevel.Information, checkStatus);
    }

    
}