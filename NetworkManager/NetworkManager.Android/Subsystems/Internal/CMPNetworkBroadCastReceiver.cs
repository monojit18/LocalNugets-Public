using System;
using System.Threading;
using Android;
using Android.Content;
using Android.Net;
using Subsystems.NetworkManager.Shared.External;

namespace Subsystems.NetworkManager.Internal
{

    [BroadcastReceiver(Enabled = true, Exported = false)]
    public sealed class CMPNetworkBroadCastReceiver : BroadcastReceiver      
    {

        private ConnectivityManager _connectivityManager;

        public const string KNetworkStatusChanged = "NETWORK_STATUS_CHANGED";
        public const string KNetworkStatus = "network_status";

        public event EventHandler<CMPNetworkEventArgs> NetworkStatusChanged;

        public CMPNetworkBroadCastReceiver() {}

        public CMPNetworkBroadCastReceiver(ConnectivityManager connectivityManager)
        {

            _connectivityManager = connectivityManager;

        }

        public override void OnReceive(Context context, Intent intent)
        {

            if (NetworkStatusChanged == null)
                return;

            var networkState = intent.GetIntExtra(KNetworkStatus,
                                                  (int)(NetworkState.Unknown));
            var networkEventArgs = new CMPNetworkEventArgs((NetworkState)networkState);

            if (NetworkStatusChanged != null)
                NetworkStatusChanged.Invoke(this, networkEventArgs);

        }

    }
}
