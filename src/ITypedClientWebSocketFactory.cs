namespace System.Net.WebSockets
{
    public interface ITypedClientWebSocketFactory<TClientWebSocket>
    {
        TClientWebSocket CreateClient(ClientWebSocket client);
    }
}
