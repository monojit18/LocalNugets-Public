using System;
using System.Threading;
using Android;
using Android.Content;
using Android.Net;
using Subsystems.NetworkManager.Shared.External;
using Subsystems.NetworkManager.Shared.Internal;

namespace Subsystems.NetworkManager.Internal
{
    public sealed class CMPNetworkManager : INetworkManager
    {

        private Context _applicationContext;
        private ConnectivityManager _connectivityManager;
        private CMPNetworkBroadCastReceiver _networkBroadcastReceiver;
        private SemaphoreSlim _networkSemaphore;

        public event EventHandler<CMPNetworkEventArgs> NetworkStatusChanged
        {

            add
            {

                _networkSemaphore.Wait();
                _networkBroadcastReceiver.NetworkStatusChanged += value;
                _networkSemaphore.Release();

            }

            remove
            {

                _networkSemaphore.Wait();
                _networkBroadcastReceiver.NetworkStatusChanged -= value;
                _networkSemaphore.Release();

            }


        }

        public static NetworkState CheckNetworkState(NetworkInfo networkInfo)
        {

            if (networkInfo == null)
                return NetworkState.Unknown;

            if (networkInfo.IsConnected == false)
                return NetworkState.NoNetwork;


            if (networkInfo.Subtype == ConnectivityType.Wifi)
                return NetworkState.WifiNetwork;

            if (networkInfo.Subtype == ConnectivityType.Mobile)
                return NetworkState.CarrierNetwork;

            return NetworkState.Unknown;

        }

        public CMPNetworkManager(object applicationContext)
        {

            _applicationContext = applicationContext as Context;
            if (_applicationContext == null)
                return;

            _connectivityManager = _applicationContext
                                    .GetSystemService(Context.ConnectivityService)
                                                      as ConnectivityManager;

            _networkBroadcastReceiver = new CMPNetworkBroadCastReceiver(
                                        _connectivityManager);
            _networkSemaphore = new SemaphoreSlim(1);

        }

        public bool IsNetworkReachable()
        {

            if (_connectivityManager == null)
                return false;

            var activeNetworkInfo = _connectivityManager.ActiveNetworkInfo;
            if (activeNetworkInfo == null)
                return false;

            var networkState = CheckNetworkState(activeNetworkInfo);
            return (networkState != NetworkState.Unknown && networkState != NetworkState.NoNetwork);        

        }

        public void StartMonitoring()
        {

            if ((_applicationContext == null)
                || (_networkBroadcastReceiver == null))
                return;

            var networkRequest = new NetworkRequest.Builder()
                                                   .AddTransportType(TransportType.Cellular)
                                                   .AddTransportType(TransportType.Wifi)
                                                   .AddTransportType(TransportType.WifiAware)
                                                   .Build();

            var networkCallback = new CMPNetworkCallback(_applicationContext,
                                                         _connectivityManager);
            _connectivityManager.RegisterNetworkCallback(networkRequest,
                                                         networkCallback);

            var intentFilter = new IntentFilter(CMPNetworkBroadCastReceiver
                                                .KNetworkStatusChanged);
            _applicationContext.RegisterReceiver(_networkBroadcastReceiver,
                                                 intentFilter);

        }

        public void StopMonitoring()
        {

            if ((_applicationContext == null) ||  (_networkBroadcastReceiver == null))
                return;

            _applicationContext.UnregisterReceiver(_networkBroadcastReceiver);

        }

    }

    public sealed class CMPNetworkCallback : ConnectivityManager.NetworkCallback
    {

        private ConnectivityManager _connectivityManager;
        private Context _applicationContext;

        private void SendNetworkBoradcast()
        {


            var activeNetworkInfo = _connectivityManager.ActiveNetworkInfo;
            var networkState = CMPNetworkManager.CheckNetworkState(activeNetworkInfo);

            var networkIntent = new Intent(CMPNetworkBroadCastReceiver
                                           .KNetworkStatusChanged);
            networkIntent.PutExtra(CMPNetworkBroadCastReceiver.KNetworkStatus,
                                   (int)networkState);
            _applicationContext.SendBroadcast(networkIntent);

        }

        public CMPNetworkCallback(Context context, ConnectivityManager connectivityManager)
        {

            _applicationContext = context;
            _connectivityManager = connectivityManager;

        }

        public override void OnAvailable(Network network)
        {

            base.OnAvailable(network);
            SendNetworkBoradcast();

        }

        public override void OnUnavailable()
        {

            SendNetworkBoradcast();
            base.OnUnavailable();

        }

    }


}