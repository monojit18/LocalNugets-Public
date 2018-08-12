using System;
using UIKit;
using Foundation;
using Subsystems.CustomQRcodeiOS.External;

namespace TestQR.iOS
{
    public partial class ViewController : UIViewController
    {
        int count = 1;
        CMPZXingQR _pQRcoder;

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

            _pQRcoder = new CMPZXingQR();

            GenerateButton.TouchUpInside += (object sender, EventArgs e) => 
            {

                var qrImageString = _pQRcoder.GenerateQR(QRTextField.Text, (int)(QRImageView.Frame.Width),
                                                   (int)(QRImageView.Frame.Height));
                var qrBytes = Convert.FromBase64String(qrImageString);
                QRImageView.Image = UIImage.LoadFromData(NSData.FromArray(qrBytes));

                QRTextField.ResignFirstResponder();

            };

            ScanButton.TouchUpInside += async (object sender, EventArgs e) =>
            {

                var qrTextResult = await _pQRcoder.RetrieveFromQRAsync();
                QRTextField.Text = qrTextResult;


            };


        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.        
        }
    }
}
