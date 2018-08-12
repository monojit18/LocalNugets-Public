using System;
using UIKit;
using Subsystems.Cache.External;

namespace TestCache.iOS
{
	public partial class ViewController : UIViewController
	{
		int count = 1;

		public ViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			// Code to start the Xamarin Test Cloud Agent
#if ENABLE_TEST_CLOUD
			Xamarin.Calabash.Start ();
#endif

			// Perform any additional setup after loading the view, typically from a nib.
			Button.AccessibilityIdentifier = "myButton";
			Button.TouchUpInside += delegate
			{
				var title = string.Format("{0} clicks!", count++);
				Button.SetTitle(title, UIControlState.Normal);

				var cachedFolderPathString = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				var cacheProxy = new CMPCacheProxy(cachedFolderPathString, 1024);
                cacheProxy.ExpirySeconds(60);

                //var str = new byte[1];

                //str[2] = (byte)'c';

                string cacheString = null;				
				byte[] retrievedBytes = cacheProxy.RetieveItem("testfile81");
				if (retrievedBytes != null)
				{
                    cacheString = System.Text.Encoding.UTF8.GetString(retrievedBytes);
					Console.WriteLine("cacheString:" + cacheString);
				}
				else
                {

                    cacheString = "Test String38";
                    byte[] cacheBytes = System.Text.Encoding.UTF8.GetBytes(cacheString);
                    cacheProxy.CacheItem(cacheBytes, "testfile81");

                }
			};
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.		
		}
	}
}
