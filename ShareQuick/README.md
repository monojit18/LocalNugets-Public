# ShareQuick
*A Xamarin wrapper around  Xaamrin.Auth APIs to share in Social Media(Facebook & Twitter for now)*

## Usage

        using Newtonsoft.Json;
        using Newtonsoft.Json.Linq;
        using Xamarin.Auth;
        using Subsystems.ShareQuickComponent.External;
        using Subsystems.CustomAccountStore.External;
        ....
        ....
        
        public partial class ViewController : UIViewController, IShareAuthenticationCallbacks, IAccountStoreConfiguration
        ....
        ....
        
### IShareAuthenticationCallbacks - Overrides

        public void PerformAuthentication(WebRedirectAuthenticator authenticator)
        {

            if (authenticator == null)
                return;
            
            var authViewController = authenticator.GetUI();
            PresentViewController(authViewController, true, null);

        }
        
### IAccountStoreConfiguration - Overrides

        public Tuple<AccountStore, string> GetAccountStore()
        {

            var accountStore = AccountStore.Create();
            return (new Tuple<AccountStore, string>(accountStore, "com.monojit.development.TestShareComponent"));

        }
        
### Facebook (iOS)

        private CMPFacebookShareProxy _shareFacebookProxy;
        ....
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
        
        ....
        ....
        
        _shareFacebookProxy = new CMPFacebookShareProxy();
        _shareFacebookProxy.CreateFacebookService("<Customer_Key>","<Customer_Secret>", this, this);
        _shareFacebookProxy.AddToScope("email")
                           .AddToScope("public_profile")
                           .AddToScope("publish_actions");
                           
        PerformFacebookAsync();
        
        
### Twitter (iOS)

        private CMPTwitterShareProxy _shareTwitterProxy;
        ....
        private async Task GetTwitterFollowersAsync()
        {

            var authenticationResult = await _shareTwitterProxy.AuthenticateUserAsync();
            DismissViewController(true, null);

            var followersInfo = await _shareTwitterProxy.GetFollowersListAsync();
            Console.WriteLine(followersInfo);

            var dMUsersList = JsonConvert.DeserializeObject<DMUsers>(followersInfo.Item1);
            var dMUser = dMUsersList.UsersList[0];
            var idString = dMUser.UserIdString;
            await PerformDirectMessageAsync(idString);

        }
        
        private async Task RefreshMessageListAsync()
        {

            var authenticationResult = await _shareTwitterProxy.AuthenticateUserAsync();
            DismissViewController(true, null);
            
            var res = await _shareTwitterProxy.GetDirectMessagesAsync();
            Console.WriteLine(res);

        }
        
        private async Task PerformDirectMessageAsync(string idString, byte[] imageBytesArray = null)
        {

            var authenticationResult = await _shareTwitterProxy.AuthenticateUserAsync();
            DismissViewController(true, null);

            var path = NSBundle.MainBundle.PathForResource("Contacts_69", "png");
            var img = UIImage.FromFile(path);
            var strm = img.AsPNG().AsStream();
            var memstrm = new MemoryStream();
            strm.CopyTo(memstrm);
            var bytes = memstrm.GetBuffer();

            var res = await _shareTwitterProxy.SendDirectMessageAsync(idString, "hello How are you??", null);
            Console.WriteLine(res);

            var param = new Dictionary<string, string>();
            param.Add("status", "This is nice quick test of Tweek with image! Thanks!!");

            var res = await _shareTwitterProxy.SendTweetAsync(param, bytes);
            Console.WriteLine(res);

        }
        
        ....
        ....
        
        _shareTwitterProxy = new CMPTwitterShareProxy();
        _shareTwitterProxy.CreateTwitterService("<Customer_Key>","<Customer_Secret>", this, this);
        
