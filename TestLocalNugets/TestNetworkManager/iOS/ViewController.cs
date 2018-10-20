using System;

using UIKit;

using Subsystems.NetworkManagerShared.External;

namespace TestNetworkManager.iOS
{
    public partial class ViewController : UIViewController
    {
        int count = 1;

        private CMPNetworkManagerSharedProxy _networkManagerProxy;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _networkManagerProxy = new CMPNetworkManagerSharedProxy("www.google.com");
            _networkManagerProxy.NetworkStatusChanged += (object sender, CMPNetworkEventArgs e) =>
            {

                Console.WriteLine(e.State);


            };

            _networkManagerProxy.StartMonitoring();
            Console.WriteLine(_networkManagerProxy.IsNetworkReachable());

            // Perform any additional setup after loading the view, typically from a nib.
            Button.AccessibilityIdentifier = "myButton";
            Button.TouchUpInside += delegate
            {
                _networkManagerProxy.StopMonitoring();
            };
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.        
        }
    }
}
