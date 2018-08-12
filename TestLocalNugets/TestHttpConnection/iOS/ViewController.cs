using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using UIKit;
using Subsystems.HttpConnection.External;

namespace TestHttpConnection.iOS
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
			Button.TouchUpInside += async delegate
			{
                
                string responseString = "";
                var httpConnectionProxy = new CMPHttpConnectionProxy();
                httpConnectionProxy.URL("http://www.google.com").Build();

                var httpResponse = await httpConnectionProxy.GetAsync();
                Console.WriteLine(httpResponse.ResponseString);

                await cl.GetBytesWithProgressAsync((httpResponse, progressBytes, totalBytes) =>
                {

                    responseString = string.Concat(responseString, httpResponse.ResponseString);
                    Console.WriteLine("Response:" + responseString);
                    Console.WriteLine("progressBytes: " + progressBytes + "\ntotalBytes: " + totalBytes);

                });await cl.GetBytesWithProgressAsync((httpResponse, progressBytes, totalBytes) =>
                {

                    responseString = string.Concat(responseString, httpResponse.ResponseString);
                    Console.WriteLine("Response:" + responseString);
                    Console.WriteLine("progressBytes: " + progressBytes + "\ntotalBytes: " + totalBytes);

                });

			};
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.		
		}
	}
}
