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

namespace Subsystems.AzureStorage.External
{
    public class CMPAzureStorageFactory
    {

        private CMPAzureStorageFactory() {}

        public static CMPAzureTableStorageProxy CreateTableStorage(string connectionString)
        {

            var azureTableStorageProxy = new CMPAzureTableStorageProxy(connectionString);
            return azureTableStorageProxy;

        }

        public static CMPAzureBlobStorageProxy CreateBlobStorage(string connectionString, string containerNameString)
        {

            var azureBlobStorageProxy = new CMPAzureBlobStorageProxy(connectionString, containerNameString);
            return azureBlobStorageProxy;

        }

        public static CMPAzureQueueStorageProxy CreateQueueStorage(string connectionString, string queueameString)
        {

            var azureQueueStorageProxy = new CMPAzureQueueStorageProxy(connectionString, queueameString);
            return azureQueueStorageProxy;

        }
    }
}
