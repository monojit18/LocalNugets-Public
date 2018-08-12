using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using UIKit;
using Foundation;
using Subsystems.AzureStorage.External;

namespace TestAzureStorage.iOS
{
    public partial class ViewController : UIViewController
    {

        private CMPAzureBlobStorageProxy _azureBlobStorageProxy;

        private async Task PerformUploadToBlobAsync()
        {
            
            string str = NSBundle.MainBundle.PathForResource("image1", "jpg");
            var data = NSData.FromFile(str);
            var imageBytes = data.ToArray();

            var res = await _azureBlobStorageProxy.AddBytesToBlobAsync(imageBytes, "ocrcontactblob");
            Console.WriteLine(res.Item1.ToString());


        }

        public ViewController(IntPtr handle) : base(handle)
        {

            _azureBlobStorageProxy = CMPAzureStorageFactory.CreateBlobStorage("<connection_string>", "ocrcontactblob");

        }


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Perform any additional setup after loading the view, typically from a nib.
            Button.AccessibilityIdentifier = "myButton";
            Button.TouchUpInside += async delegate
            {

                await PerformUploadToBlobAsync();

            };
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.        
        }
    }
}
