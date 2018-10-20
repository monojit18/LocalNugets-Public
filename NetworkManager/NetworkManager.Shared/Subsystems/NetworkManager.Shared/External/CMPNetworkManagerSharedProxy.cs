using System;
using Autofac;
using Subsystems.NetworkManager.Shared.Internal;
using Subsystems.NetworkManager.Internal;

namespace Subsystems.NetworkManager.Shared.External
{

    public class CMPNetworkManagerSharedProxy
    {

        private IContainer _container;
        private INetworkManager _networkManager;

        public event EventHandler<CMPNetworkEventArgs> NetworkStatusChanged
        {

            add
            {

                var networkManager = Resolve();
                networkManager.NetworkStatusChanged += value;

            }

            remove
            {

                var networkManager = Resolve();
                networkManager.NetworkStatusChanged -= value;

            }


        }

        private INetworkManager Resolve()
        {

            if (_networkManager != null)
                return _networkManager;

            using (var scope = _container?.BeginLifetimeScope())
            {

                _networkManager = _container.Resolve<INetworkManager>();
                return _networkManager;

            }


        }

        public CMPNetworkManagerSharedProxy(object parameters)            
        {

            var builder = new ContainerBuilder();
            builder.Register((componentContext) => new CMPNetworkManager(parameters))
                   .As<INetworkManager>();
            _container = builder.Build();

        }

        public bool IsNetworkReachable()
        {

            var networkManager = Resolve();
            var isReachable = networkManager?.IsNetworkReachable();
            return isReachable ?? false;

        }

        public void StartMonitoring()
        {

            var networkManager = Resolve();
            networkManager?.StartMonitoring();

        }

        public void StopMonitoring()
        {

            var networkManager = Resolve();
            networkManager?.StopMonitoring();

        }

    }
}
