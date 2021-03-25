namespace System.Net.WebSockets
{
    public interface IClientWebSocketFactory
    {
        ClientWebSocket CreateClient(string name);
    }
}
