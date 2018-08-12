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
using System.Threading;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAzure.Storage;

namespace Subsystems.AzureStorage.Internal
{

    public class CMPStorageServiceBase
    {

        protected string _connectionString;
        protected CloudStorageAccount _cloudStorageAccount;
        protected CancellationTokenSource _tokenSource;

        public CMPStorageServiceBase(string connectionString)
        {

            _connectionString = string.Copy(connectionString);
            _tokenSource = new CancellationTokenSource();

        }

        public void Cancel()
        {

            _tokenSource.Cancel(true);

        }

    }
}
