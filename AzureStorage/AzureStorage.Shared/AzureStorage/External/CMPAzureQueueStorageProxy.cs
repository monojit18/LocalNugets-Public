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
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;
using Subsystems.AzureStorage.Internal;

namespace Subsystems.AzureStorage.External
{
    public class CMPAzureQueueStorageProxy
    {

        private CMPQueueStorageService _queueStorageService;

        public CMPAzureQueueStorageProxy(string connectionString, string queueNameString)
        {

            _queueStorageService = new CMPQueueStorageService(connectionString, queueNameString);

        }

        public async Task<int> GetMessagesCountAsync(CMPQueueStorageOptions queueStorageOptions)
        {

            if (_queueStorageService == null)
                return -1;

            var messagesCount = await _queueStorageService.GetMessagesCountAsync(queueStorageOptions);
            return messagesCount;

        }

        public async Task<Tuple<bool, CMPStorageError>> AddMessageAsync(string messageString,
                                                                        string popupTokenString,
                                                                        CMPQueueStorageOptions queueStorageOptions =
                                                                        null)
        {

            if (_queueStorageService == null)
                return (new Tuple<bool, CMPStorageError>(false, null));

            var addMessageInfo = await _queueStorageService.AddMessageAsync(messageString, popupTokenString,
                                                                            queueStorageOptions);
            return addMessageInfo;

        }

        public async Task<Tuple<string, string, CMPStorageError>> PeekMessageAsync(CMPQueueStorageOptions
                                                                                   queueStorageOptions = null)
        {

            if (_queueStorageService == null)
                return (new Tuple<string, string, CMPStorageError>(null, null, null));

            var peekMessageInfo = await _queueStorageService.PeekMessageAsync(queueStorageOptions);
            return peekMessageInfo;

        }

        public async Task<Tuple<bool, CMPStorageError>> DeQueueMessageAsync(TimeSpan visibilityTimeOut,
                                                                            CMPQueueStorageOptions queueStorageOptions)
        {

            if (_queueStorageService == null)
                return (new Tuple<bool, CMPStorageError>(false, null));

            var dequeueMessageInfo = await _queueStorageService.DeQueueMessageAsync(visibilityTimeOut,
                                                                                    queueStorageOptions);
            return dequeueMessageInfo;

        }

        public async Task<Tuple<bool, CMPStorageError>> DeQueueBatchMessagesAsync(TimeSpan visibilityTimeOut,
                                                                                  int deQueueCount,
                                                                                  CMPQueueStorageOptions
                                                                                  queueStorageOptions = null)
        {

            if (_queueStorageService == null)
                return (new Tuple<bool, CMPStorageError>(false, null));

            var dequeueBatchMessageInfo = await _queueStorageService.DeQueueBatchMessagesAsync(visibilityTimeOut,
                                                                                               deQueueCount,
                                                                                               queueStorageOptions);
            return dequeueBatchMessageInfo;

        }

        public async Task<Tuple<bool, CMPStorageError>> UpdateMessageAsync(string updateMessageString,
                                                                           TimeSpan visibilityTimeOut,
                                                                           CMPQueueStorageOptions
                                                                           queueStorageOptions = null)
        {

            if (_queueStorageService == null)
                return (new Tuple<bool, CMPStorageError>(false, null));

            var updateMessageInfo = await _queueStorageService.UpdateMessageAsync(updateMessageString,
                                                                                  visibilityTimeOut,
                                                                                  queueStorageOptions);
            return updateMessageInfo;

        }

        public async Task<Tuple<bool, CMPStorageError>> DeleteMessageAsync(CloudQueueMessage queueMessage,
                                                                           CMPQueueStorageOptions
                                                                           queueStorageOptions = null)
        {

            if (_queueStorageService == null)
                return (new Tuple<bool, CMPStorageError>(false, null));

            var deleteMessageInfo = await _queueStorageService.DeleteMessageAsync(queueMessage, queueStorageOptions);
            return deleteMessageInfo;

        }

        public async Task<Tuple<bool, CMPStorageError>> DeleteBatchMessagesAsync(List<CloudQueueMessage>
                                                                                 queueMessagesList,
                                                                                 CMPQueueStorageOptions
                                                                                 queueStorageOptions = null)
        {

            if (_queueStorageService == null)
                return (new Tuple<bool, CMPStorageError>(false, null));

            var deleteMessageInfo = await _queueStorageService.DeleteBatchMessagesAsync(queueMessagesList,
                                                                                        queueStorageOptions);
            return deleteMessageInfo;


        }

        public async Task<Tuple<bool, CMPStorageError>> DeleteQueueAsync(CMPQueueStorageOptions queueStorageOptions =
                                                                         null)
        {

            if (_queueStorageService == null)
                return (new Tuple<bool, CMPStorageError>(false, null));

            var deleteQueueInfo = await _queueStorageService.DeleteQueueAsync(queueStorageOptions);
            return deleteQueueInfo;

        }


    }
}
