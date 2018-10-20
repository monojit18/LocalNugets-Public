using System;
using System.Net;
using SystemConfiguration;
using CoreFoundation;
using Subsystems.NetworkManager.Shared.External;
using Subsystems.NetworkManager.Shared.Internal;

namespace Subsystems.NetworkManager.Internal
{

    public sealed class CMPNetworkManager : INetworkManager
    {

        #region Private/Protected Variables
        private NetworkReachability _reachability;
        private string _hostAddressString;
        #endregion

        #region Private/Protected Methods
        private NetworkState CheckNetworkState(NetworkReachabilityFlags reachabilityFlags)
        {

            bool isReachable = ((reachabilityFlags
                                 &
                                 NetworkReachabilityFlags
                                 .Reachable) != 0);

            if (isReachable == false)
                return NetworkState.Unknown;

            if ((reachabilityFlags & NetworkReachabilityFlags.IsWWAN) != 0)
                return NetworkState.CarrierNetwork;

            bool doesConnectionRequired = ((reachabilityFlags
                                            & NetworkReachabilityFlags
                                            .ConnectionRequired) != 0);

            bool doesInterventionRequired = ((reachabilityFlags
                                              & NetworkReachabilityFlags
                                              .InterventionRequired) != 0);
            doesConnectionRequired = ((doesConnectionRequired == true)
                                      || (doesInterventionRequired == true));

            isReachable = (isReachable == true)
                && (doesConnectionRequired == false);

            if (isReachable == false)
                return NetworkState.NoNetwork;

            return NetworkState.WifiNetwork;

        }

        #endregion

        #region Public Methods
        public event EventHandler<CMPNetworkEventArgs> NetworkStatusChanged;

        public CMPNetworkManager(object hostAddressString)
        {

            _hostAddressString = string.Copy(hostAddressString as string);
            _reachability = new NetworkReachability(_hostAddressString);
            _reachability.SetNotification((NetworkReachabilityFlags
                                           reachabilityFlags) =>
            {

                var networkState = CheckNetworkState(reachabilityFlags);
                NetworkStatusChanged.Invoke(this,
                                            new CMPNetworkEventArgs(networkState));

            });

        }

        public bool IsNetworkReachable()
        {

            NetworkReachabilityFlags reachabilityFlags;
            var isReachable = _reachability.TryGetFlags(out reachabilityFlags);
            var networkState = CheckNetworkState(reachabilityFlags);
            var networkStateAvailable = (networkState
                                         != NetworkState.Unknown
                                         && networkState
                                         != NetworkState.NoNetwork);
            isReachable = (isReachable == true)
                && (networkStateAvailable == true);
            return isReachable;

        }

        public void StartMonitoring()
        {

            _reachability.Schedule(CFRunLoop.Current, CFRunLoop.ModeDefault);

        }

        public void StopMonitoring()
        {

            _reachability.Unschedule(CFRunLoop.Current, CFRunLoop.ModeDefault);

        }
        #endregion

    }
}
