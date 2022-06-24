using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Armut.Messaging.Infrastructure.Hubs
{
    public static class ConnectionMapping
    {
        private static readonly Dictionary<string, HashSet<string>> _connections = new Dictionary<string, HashSet<string>>();

        public static void Add(string key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    connections = new HashSet<string>();
                    _connections.Add(key, connections);
                }

                lock (connections)
                {
                    connections.Add(connectionId);
                }
            }
        }

        public static void Remove(string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                return;
            }

            _connections.Remove(key);
        }

        public static IEnumerable<string> GetConnections(string key)
        {
            HashSet<string> connections;
            if (_connections.TryGetValue(key, out connections))
            {
                return connections;
            }

            return Enumerable.Empty<string>();
        }

    }
}
