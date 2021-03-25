using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace System.Net.WebSockets
{
    internal class DefaultClientWebSocketFactory : IClientWebSocketFactory
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _services;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IOptionsMonitor<ClientWebSocketFactoryOptions> _optionsMonitor;

        public DefaultClientWebSocketFactory(
            IServiceProvider services, 
            IServiceScopeFactory scopeFactory,
            ILoggerFactory loggerFactory,
             IOptionsMonitor<ClientWebSocketFactoryOptions> optionsMonitor) {
            _services = services ?? throw new ArgumentNullException(nameof(services));

            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));

            _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));

            //_logger = loggerFactory?.CreateLogger<DefaultClientWebSocketFactory>();

        }

        public ClientWebSocket CreateClient(string name)
        {
            if (name == null) {
                throw new ArgumentNullException(nameof(name));
            }


            ClientWebSocket ws = new ClientWebSocket();

            ClientWebSocketFactoryOptions options = _optionsMonitor.Get(name);

            for (int i = 0; i < options.ClientWebSocketActions.Count; i++) {
                options.ClientWebSocketActions[i](ws);
            }

            return ws;

        }
    }
}
