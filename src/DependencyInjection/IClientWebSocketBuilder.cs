using System.Net.WebSockets;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// A builder for configuring named <see cref="ClientWebSocket"/> instances returned by <see cref="IClientWebSocketFactory"/>.
    /// </summary>
    public interface IClientWebSocketBuilder
    {
        string Name { get; }

        IServiceCollection Services { get; }
    }
}
