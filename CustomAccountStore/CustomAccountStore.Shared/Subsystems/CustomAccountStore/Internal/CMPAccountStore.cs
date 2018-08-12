/*
 * 
 * Copyright 2018 Monojit Datta

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Auth;
using Subsystems.CustomAccountStore.External;

namespace Subsystems.CustomAccountStore.Internal
{
    public class CMPAccountStore
    {

        private Account _account;
        private readonly AccountStore _accountStore;
        private readonly string _serviceIdstring;

        public CMPAccountStore(IAccountStoreConfiguration configuration)
        {

            _account = new Account();

            var configurationInfo = configuration.GetAccountStore();
            if (configurationInfo == null)
                return;

            _accountStore = configurationInfo.Item1;
            _serviceIdstring = string.Copy(configurationInfo.Item2);

        }

        public bool Save(string keyString, string valueString)
        {

            try
            {

                _account.Properties?.Add(keyString, valueString);
                _accountStore.Save(_account, _serviceIdstring);
                return true;

            }
            catch (AccountStoreException exception)
            {

                Console.WriteLine(exception.StackTrace);
                return false;

            }

        }

        public bool Save(Dictionary<string, string> itemDictionary)
        {

            try
            {

                foreach (var item in itemDictionary)
                    _account.Properties?.Add(item.Key, item.Value);

                _accountStore.Save(_account, _serviceIdstring);
                return true;

            }
            catch (AccountStoreException exception)
            {

                Console.WriteLine(exception.StackTrace);
                return false;

            }

        }

        public string Fetch(string keyString)
        {

            try
            {

                var fetchedAccount = _accountStore.FindAccountsForService(_serviceIdstring).FirstOrDefault();
                if (fetchedAccount == null)
                    return null;

                _account = fetchedAccount;

                var hasKey = _account.Properties?.ContainsKey(keyString);
                if (hasKey == false)
                    return null;

                var valueString = _account.Properties?[keyString];
                return valueString;

            }
            catch (AccountStoreException exception)
            {

                Console.WriteLine(exception.StackTrace);
                return null;

            }

        }

        public bool Delete()
        {

            try
            {

                _account.Properties.Clear();
                _accountStore.Delete(_account, _serviceIdstring);
                return true;

            }
            catch(AccountStoreException exception)
            {

                Console.WriteLine(exception.StackTrace);
                return false;

            }

        }

    }
}
