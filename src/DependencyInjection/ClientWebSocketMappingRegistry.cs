using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    internal class ClientWebSocketMappingRegistry
    {
        public Dictionary<string, Type> NamedClientRegistrations { get; } = new Dictionary<string, Type>();
    }
}
