using Microsoft.Extensions.DependencyInjection;
using System.Threading;

namespace System.Net.WebSockets
{
    internal class DefaultTypedClientWebSocketFactory<TClientWebSocket> : ITypedClientWebSocketFactory<TClientWebSocket>
    {
        private readonly Cache _cache;
        private readonly IServiceProvider _services;

        public DefaultTypedClientWebSocketFactory(Cache cache, IServiceProvider services)
        {
            if (cache == null)
            {
                throw new ArgumentNullException(nameof(cache));
            }

            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            _cache = cache;
            _services = services;
        }

        public TClientWebSocket CreateClient(ClientWebSocket client)
        {
            client = client ?? throw new ArgumentNullException(nameof(client));
            return (TClientWebSocket)_cache.Activator(_services, new object[] { client });
        }

        public class Cache
        {
            private static readonly Func<ObjectFactory> _createActivator = () => ActivatorUtilities.CreateFactory(typeof(TClientWebSocket), new Type[] { typeof(ClientWebSocket), });

            private ObjectFactory _activator;
            private bool _initialized;
            private object _lock;

            public ObjectFactory Activator => LazyInitializer.EnsureInitialized(
                ref _activator,
                ref _initialized,
                ref _lock,
                _createActivator);
        }
    }
}
