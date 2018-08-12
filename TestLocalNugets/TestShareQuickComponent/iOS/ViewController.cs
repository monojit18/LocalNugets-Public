using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using UIKit;
using Foundation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Auth;
using Subsystems.ShareQuickComponent.External;
using Subsystems.CustomAccountStore.External;

namespace TestShareQuickComponent.iOS
{
    public partial class ViewController : UIViewController, IShareAuthenticationCallbacks, IAccountStoreConfiguration
    {
        int count = 1;
        private CMPFacebookShareProxy _shareFacebookProxy;
        private CMPTwitterShareProxy _shareTwitterProxy;

        private async Task PerformFacebookAsync()
        {

            var authenticationResult = await _shareFacebookProxy.AuthenticateUserAsync();
            DismissViewController(true, null);

            var paramsDictionary = new Dictionary<string, string>();
            paramsDictionary.Add("message", "This is a Another Test Message");

            var postResultInfo = await _shareFacebookProxy.PostStatusAsync(paramsDictionary);
            var idTokenInfo = JObject.Parse(postResultInfo.Item1);
            var idString = idTokenInfo["id"] as JToken;

            paramsDictionary.Clear();
            paramsDictionary.Add("message", "This is an Updated Message");
            var updateResultInfo = await _shareFacebookProxy.UpdatePostAsync(idString.ToString(), paramsDictionary);
            Console.WriteLine(updateResultInfo);

            var removeResultInfo = await _shareFacebookProxy.RemovePostAsync(idString.ToString());
            Console.WriteLine(removeResultInfo);






        }
        
        private async Task PerformTwitterAsync()
        {

            var authenticationResult = await _shareTwitterProxy.AuthenticateUserAsync();
            DismissViewController(true, null);


            var obj = new CMPDirectMessage();
            obj.type = "message_create";
            
            obj.message_create = new MessageCreate();
            obj.message_create.type = new Target();
            obj.message_create.type.recipient_id = "743827432818384896";

            obj.message_create.message_data = new MessageData();
            obj.message_create.message_data.text = "Hi Rakhi";
            obj.message_create.message_data.attachment = new Attachment();
            obj.message_create.message_data.attachment.type = "media";
            obj.message_create.message_data.attachment.media = new Media();
            obj.message_create.message_data.attachment.media.id = "";


            var str = JsonConvert.SerializeObject(obj);
            Console.WriteLine(str);

            var path = NSBundle.MainBundle.PathForResource("Contacts_69", "png");
            var img = UIImage.FromFile(path);
            var strm = img.AsPNG().AsStream();
            var memstrm = new MemoryStream();
            strm.CopyTo(memstrm);
            var bytes = memstrm.GetBuffer();

            var param = new Dictionary<string, string>();
            param.Add("status", "This is nice quick test of Tweek with image! Thanks!!");

            var res = await _shareTwitterProxy.SendTweetAsync(param, bytes);
            Console.WriteLine(res);

        }

        public ViewController(IntPtr handle) : base(handle)
        {

            _shareFacebookProxy = new CMPFacebookShareProxy();
            _shareFacebookProxy.CreateFacebookService("266081350162253", string.Empty, this, this);
            _shareFacebookProxy.AddToScope("email")
                               .AddToScope("public_profile")
                               .AddToScope("publish_actions");

            _shareTwitterProxy = new CMPTwitterShareProxy();
            _shareTwitterProxy.CreateTwitterService("d0xmwoCQS4vswrfY2oXDzSZ4S",
                                                    "7EzJItJs3E7omg4nnNNpuD96we4pCszoqeE5PS2UW33mg59CuG",
                                                    this, this);
            
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Perform any additional setup after loading the view, typically from a nib.
            Button.AccessibilityIdentifier = "myButton";
            Button.TouchUpInside += async delegate
            {
                var title = string.Format("{0} clicks!", count++);
                Button.SetTitle(title, UIControlState.Normal);

                // await PerformTwitterAsync();
                await PerformFacebookAsync();

            };
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.        
        }

        public void PerformAuthentication(WebRedirectAuthenticator authenticator)
        {

            if (authenticator == null)
                return;
            
            var authViewController = authenticator.GetUI();
            PresentViewController(authViewController, true, null);

        }

        public Tuple<AccountStore, string> GetAccountStore()
        {

            var accountStore = AccountStore.Create();
            return (new Tuple<AccountStore, string>(accountStore, "com.monojit.development.TestShareComponent"));

        }

    }
}
