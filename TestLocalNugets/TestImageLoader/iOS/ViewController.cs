using System;
using System.Threading.Tasks;
using UIKit;
using Subsystems.Cache.External;
using Subsystems.ImageLoader.External;


namespace TestImageLoader.iOS
{
	public partial class ViewController : UIViewController
	{
		int count = 1;

        private CMPCacheProxy _cacheProxy;
        private CMPImageLoaderProxy _imageLoaderProxy;

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


            var baseFolderPathString = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            _cacheProxy = new CMPCacheProxy(baseFolderPathString, 5 * 1024);
            _cacheProxy.ExpiryDays(1);

            _imageLoaderProxy = new CMPImageLoaderProxy("<Image_URL>", _cacheProxy);

			// Perform any additional setup after loading the view, typically from a nib.
			Button.AccessibilityIdentifier = "myButton";
			Button.TouchUpInside += async delegate
			{
				var title = string.Format("{0} clicks!", count++);
				Button.SetTitle(title, UIControlState.Normal);
                await _imageLoaderProxy.LoadImageAsync();

			};
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.		
		}
	}
}
