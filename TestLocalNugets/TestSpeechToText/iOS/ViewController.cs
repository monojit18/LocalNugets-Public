using System;

using UIKit;

using Subsystems.CustomSpeechToText.iOS.External;

namespace TestSpeechToText.iOS
{
    public partial class ViewController : UIViewController
    {
        int count = 1;
        private CMPSpeechToTextIOS _speechToTextIOS;

        public ViewController(IntPtr handle) : base(handle)
        {

            _speechToTextIOS = new CMPSpeechToTextIOS();
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
            };
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.        
        }
    }
}
