using My.Framework.Foundation.Redis;
using StackExchange.Redis;
using System.Collections;

namespace My.Framework.RedisAccessor.ServerIteration
{
    public class ServerEnumerable : IEnumerable<IServer>
    {
        private readonly ConnectionMultiplexer _multiplexer;
        private readonly ServerEnumerationStrategy.TargetRoleOptions _targetRole;
        private readonly ServerEnumerationStrategy.UnreachableServerActionOptions _unreachableServerAction;

        public ServerEnumerable(
            ConnectionMultiplexer multiplexer,
            ServerEnumerationStrategy.TargetRoleOptions targetRole,
            ServerEnumerationStrategy.UnreachableServerActionOptions unreachableServerAction)
        {
            _multiplexer = multiplexer;
            _targetRole = targetRole;
            _unreachableServerAction = unreachableServerAction;
        }

        public IEnumerator<IServer> GetEnumerator()
        {
            foreach (var endPoint in _multiplexer.GetEndPoints())
            {
                var server = _multiplexer.GetServer(endPoint);
                if (_targetRole == ServerEnumerationStrategy.TargetRoleOptions.PreferSlave)
                {
                    if (!server.IsSlave)
                        continue;
                }
                if (_unreachableServerAction == ServerEnumerationStrategy.UnreachableServerActionOptions.IgnoreIfOtherAvailable)
                {
                    if (!server.IsConnected || !server.Features.Scan)
                        continue;
                }

                yield return server;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
