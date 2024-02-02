using System.Net.WebSockets;

namespace BudgetWebApi.Sockets;

public static class SocketListener
{
    public static async Task Listen(WebSocket webSocket)
    {
        var buffer = new byte[1024*4];
        while (webSocket.State == WebSocketState.Open)
        {
            WebSocketReceiveResult incoming = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (incoming.MessageType == WebSocketMessageType.Close)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnected", CancellationToken.None);
            }
            
        }
    }

}