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
using Subsystems.CustomAccountStore.Internal;

namespace Subsystems.CustomAccountStore.External
{
    public class CMPAccountStoreProxy
    {

        private CMPAccountStore _accountStore;

        public CMPAccountStoreProxy(IAccountStoreConfiguration configuration)
        {

            _accountStore = new CMPAccountStore(configuration); 

        }

        public bool Save(string keyString, string valueString)
        {

            if ((string.IsNullOrEmpty(keyString)) || (string.IsNullOrEmpty(valueString)))
                return false;
            
            var couldSave = _accountStore.Save(keyString, valueString);
            return couldSave;

        }

        public bool Save(Dictionary<string, string> itemDictionary)
        {

            if ((itemDictionary == null) || (itemDictionary.Count == 0))
                return false;
            
            var couldSave = _accountStore.Save(itemDictionary);
            return couldSave;

        }

        public string Fetch(string keyString)
        {

            if (string.IsNullOrEmpty(keyString))
                return null;
            
            var valueString = _accountStore.Fetch(keyString);
            return valueString;

        }

        public bool Delete()
        {

            var couldDelete = _accountStore.Delete();
            return couldDelete;

        }

    }
}
