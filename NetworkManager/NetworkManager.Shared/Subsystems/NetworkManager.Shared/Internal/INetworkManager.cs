using System;
using Subsystems.NetworkManager.Shared.External;
using Subsystems.NetworkManager.Internal;

namespace Subsystems.NetworkManager.Shared.Internal
{

    public interface INetworkManager
    {

        event EventHandler<CMPNetworkEventArgs> NetworkStatusChanged;

        bool IsNetworkReachable();
        void StartMonitoring();
        void StopMonitoring();

    }
}
