# CustomAccountStore
*A Xamarin wrapper binding for AccountStore of the Xamarin.Auth component*

## Usage
        public class AccountConfiguration : IAccountStoreConfiguration
        {
            public Tuple<AccountStore, string> GetAccountStore()
            {

                var accountStore = AccountStore.Create();
                return (new Tuple<AccountStore, string>(accountStore, "<"));
            }        
        }
        ....
        ....
        
        var accountStoreConfiguration = new AccountConfiguration();
        _accountStoreProxy = new CMPAccountStoreProxy(accountStoreConfiguration);
        ....
        ....
        _accountStoreProxy.Save(<Store_Key>, <Store_Value>);
        ....
        var valueString = _accountStoreProxy.Fetch(<Store_Key>);
        ....
        _accountStoreProxy.Delete();
