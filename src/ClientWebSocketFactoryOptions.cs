using System.Collections.Generic;

namespace System.Net.WebSockets
{
    public class ClientWebSocketFactoryOptions
    {
        public IList<Action<ClientWebSocket>> ClientWebSocketActions { get; } = new List<Action<ClientWebSocket>>();

        /// <summary>
        /// Gets or sets a value that determines whether the <see cref="IClientWebSocketFactory"/> will
        /// create a dependency injection scope when building an <see cref="ClientWebSocketOptionsBuilder"/>.
        /// If <c>false</c> (default), a scope will be created, otherwise a scope will not be created.
        /// </summary>
        public bool SuppressBuilderScope { get; set; }
    }
}
