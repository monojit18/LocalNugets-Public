using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Subsystems.NetworkManagerShared.External;

namespace TestNetworkManager.Droid
{
    [Activity(Label = "TestNetworkManager", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        int count = 1;
        private CMPNetworkManagerSharedProxy _networkManagerProxy;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);



            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            _networkManagerProxy = new CMPNetworkManagerSharedProxy(this);
            _networkManagerProxy.NetworkStatusChanged += (object sender,
                                                          CMPNetworkEventArgs e) =>
            {

                Console.WriteLine(e.State);

            };

            _networkManagerProxy.StartMonitoring();

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.myButton);

            button.Click += delegate { _networkManagerProxy.StopMonitoring(); };
        }
    }
}

