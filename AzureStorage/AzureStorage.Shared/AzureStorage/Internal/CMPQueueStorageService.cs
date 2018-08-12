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
using System.Linq;
using System.Collections.Generic;
using Diagonistics = System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Subsystems.AzureStorage.External;

namespace Subsystems.AzureStorage.Internal
{

    public delegate void ProcessQueueMessage(CloudQueueMessage queueMessage);
    public delegate void ProcessQueueBatchMessages(List<CloudQueueMessage> queueMessagesList);

    public class CMPQueueStorageService : CMPStorageServiceBase
    {

        private string _queueNameString;
        private CloudQueueClient _cloudQueueClient;
        private CloudQueue _queueReference;

        private void PrepareQueueReference()
        {

            if (string.IsNullOrEmpty(_queueNameString) == true)
                return;

            try
            {

                _queueReference = _cloudQueueClient.GetQueueReference(_queueNameString);

            }
            catch (StorageException exception)
            {

                Diagonistics.Debug.WriteLine(exception.Message);
                throw;

            }
            catch (ArgumentException exception)
            {

                Diagonistics.Debug.WriteLine(exception.Message);
                throw;

            }

        }

        public ProcessQueueMessage ProcessQueueMessage { get; set; }
        public ProcessQueueBatchMessages ProcessQueueBatchMessages { get; set; }

        public CMPQueueStorageService(string connectionString, string queueNameString) : base(connectionString)
        {

            var couldParse = CloudStorageAccount.TryParse(connectionString, out _cloudStorageAccount);
            if (couldParse == false)
                return;

            _queueNameString = string.Copy(queueNameString);
            _cloudQueueClient = _cloudStorageAccount.CreateCloudQueueClient();

        }

        public async Task<int> GetMessagesCountAsync(CMPQueueStorageOptions queueStorageOptions)
        {

            try
            {

                if (_queueReference == null)
                    return -1;

                if (queueStorageOptions == null)
                    await _queueReference.FetchAttributesAsync();
                else
                    await _queueReference.FetchAttributesAsync(queueStorageOptions.QueueRequestOptions,
                                                               queueStorageOptions.OperationContext,
                                                               _tokenSource.Token);

                return ((int)(_queueReference.ApproximateMessageCount));

            }
            catch (StorageException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return -1;

            }


        }

        public async Task<Tuple<bool, CMPStorageError>> CreateQueueAsync(CMPQueueStorageOptions queueStorageOptions =
                                                                         null)
        {

            try
            {

                if (_queueReference == null)
                    return (new Tuple<bool, CMPStorageError>(false, null));

                var couldCreate = false;
                if (queueStorageOptions == null)
                    couldCreate = await _queueReference.CreateIfNotExistsAsync();
                else
                    couldCreate = await _queueReference.CreateIfNotExistsAsync(queueStorageOptions.QueueRequestOptions,
                                                                               queueStorageOptions.OperationContext,
                                                                               _tokenSource.Token);
                
                return (new Tuple<bool, CMPStorageError>(couldCreate, null));

            }
            catch (StorageException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<bool, CMPStorageError>(false, error));

            }

        }

        public async Task<Tuple<bool, CMPStorageError>> AddMessageAsync(string messageString,
                                                                        string popupTokenString,
                                                                        CMPQueueStorageOptions queueStorageOptions =
                                                                        null)
        {

            if (string.IsNullOrEmpty(messageString) == true)
                return (new Tuple<bool, CMPStorageError>(false, null));

            try
            {

                if (_queueReference == null)
                    return (new Tuple<bool, CMPStorageError>(false, null));

                CloudQueueMessage queueMessage = null;
                if (string.IsNullOrEmpty(popupTokenString) == true)
                    queueMessage = new CloudQueueMessage(messageString);
                else
                    queueMessage = new CloudQueueMessage(messageString, popupTokenString);

                if (queueStorageOptions == null)
                    await _queueReference.AddMessageAsync(queueMessage);
                else
                    await _queueReference.AddMessageAsync(queueMessage, queueStorageOptions.TimeToLive,
                                                          queueStorageOptions.VisibilityTimeoutOrDelay,
                                                          queueStorageOptions.QueueRequestOptions,
                                                          queueStorageOptions.OperationContext, _tokenSource.Token);
                
                return (new Tuple<bool, CMPStorageError>(true, null));

            }
            catch (StorageException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<bool, CMPStorageError>(false, error));

            }
            catch (ArgumentException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<bool, CMPStorageError>(false, error));

            }
            catch (OperationCanceledException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<bool, CMPStorageError>(false, error));

            }
        }

        public async Task<Tuple<bool, CMPStorageError>> UpdateMessageAsync(string messageString,
                                                                           string messageIdString,
                                                                           string popupTokenString,
                                                                           CMPQueueStorageOptions queueStorageOptions)
        {

            if (queueStorageOptions == null)
                return (new Tuple<bool, CMPStorageError>(false, null));
            
            if ((string.IsNullOrEmpty(messageIdString) == true) || (string.IsNullOrEmpty(popupTokenString) == true))
                return (new Tuple<bool, CMPStorageError>(false, null));
            
            try
            {

                if (_queueReference == null)
                    return (new Tuple<bool, CMPStorageError>(false, null));

                CloudQueueMessage queueMessage = new CloudQueueMessage(messageIdString, popupTokenString);
                queueMessage.SetMessageContent((queueStorageOptions.MessageUpdateFields ==
                                                    MessageUpdateFields.Content) ? messageString : string.Empty);

                await _queueReference.UpdateMessageAsync(queueMessage, queueStorageOptions.VisibilityTimeoutOrDelay,
                                                         queueStorageOptions.MessageUpdateFields);

                return (new Tuple<bool, CMPStorageError>(true, null));

            }
            catch (StorageException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<bool, CMPStorageError>(false, error));

            }
            catch (ArgumentException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<bool, CMPStorageError>(false, error));

            }
            catch (OperationCanceledException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<bool, CMPStorageError>(false, error));

            }
        }

        public async Task<Tuple<string, string, CMPStorageError>> PeekMessageAsync(CMPQueueStorageOptions
                                                                                   queueStorageOptions = null)
        {

            try
            {
                
                if (_queueReference == null)
                    return (new Tuple<string, string, CMPStorageError>(null, null, null));

                CloudQueueMessage queueMessage = null;
                if (queueStorageOptions == null)
                    await _queueReference.PeekMessageAsync();
                else
                    await _queueReference.PeekMessageAsync(queueStorageOptions.QueueRequestOptions,
                                                           queueStorageOptions.OperationContext, _tokenSource.Token);

                return (new Tuple<string, string, CMPStorageError>(queueMessage.AsString, queueMessage.PopReceipt,
                                                                   null));

            }
            catch (StorageException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<string, string, CMPStorageError>(null, null, error));

            }
            catch (ArgumentException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<string, string, CMPStorageError>(null, null, error));

            }
            catch (OperationCanceledException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<string, string, CMPStorageError>(null, null, error));

            }
        }

        public async Task<Tuple<bool, CMPStorageError>> DeQueueMessageAsync(TimeSpan visibilityTimeOut,
                                                                            CMPQueueStorageOptions
                                                                            queueStorageOptions = null)
        {

            try
            {

                if (_queueReference == null)
                    return (new Tuple<bool, CMPStorageError>(false, null));

                CloudQueueMessage queueMessage = null;
                if (queueStorageOptions == null)
                    queueMessage = await _queueReference.GetMessageAsync();
                else
                    queueMessage = await _queueReference.GetMessageAsync(visibilityTimeOut,
                                                                         queueStorageOptions.QueueRequestOptions,
                                                                         queueStorageOptions.OperationContext,
                                                                         _tokenSource.Token);

                ProcessQueueMessage?.Invoke(queueMessage);
                var couldDeleteInfo = await DeleteMessageAsync(queueMessage, queueStorageOptions);
                return couldDeleteInfo;

            }
            catch (StorageException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<bool, CMPStorageError>(false, error));

            }
            catch (ArgumentException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<bool, CMPStorageError>(false, error));

            }
            catch (OperationCanceledException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<bool, CMPStorageError>(false, error));

            }
        }

        public async Task<Tuple<bool, CMPStorageError>> DeQueueBatchMessagesAsync(TimeSpan visibilityTimeOut,
                                                                                  int deQueueCount,
                                                                                  CMPQueueStorageOptions
                                                                                  queueStorageOptions = null)
        {

            try
            {

                if (_queueReference == null)
                    return (new Tuple<bool, CMPStorageError>(false, null));

                if (deQueueCount == 0)
                    deQueueCount = await GetMessagesCountAsync(queueStorageOptions);

                int batchCount = (deQueueCount / 10);
                if (batchCount == 0)
                    batchCount = deQueueCount;

                do
                {

                    IEnumerable<CloudQueueMessage> queueMessages = null;
                    if (queueStorageOptions == null)
                        queueMessages = await _queueReference.GetMessagesAsync(batchCount);
                    else
                        queueMessages = await _queueReference.GetMessagesAsync(batchCount, visibilityTimeOut,
                                                                               queueStorageOptions.QueueRequestOptions,
                                                                               queueStorageOptions.OperationContext,
                                                                               _tokenSource.Token);

                    var queueMessagesList = queueMessages.ToList();
                    ProcessQueueBatchMessages?.Invoke(queueMessagesList);
                    await DeleteBatchMessagesAsync(queueMessagesList, queueStorageOptions);

                } while ((deQueueCount -= batchCount) > 0);

                return (new Tuple<bool, CMPStorageError>(true, null));

            }
            catch (StorageException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<bool, CMPStorageError>(false, null));

            }
            catch (ArgumentException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<bool, CMPStorageError>(false, null));

            }
            catch (OperationCanceledException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<bool, CMPStorageError>(false, null));

            }
        }

        public async Task<Tuple<bool, CMPStorageError>> UpdateMessageAsync(string updateMessageString,
                                                                           TimeSpan visibilityTimeOut,
                                                                           CMPQueueStorageOptions
                                                                           queueStorageOptions = null)
        {

            try
            {

                if (_queueReference == null)
                    return (new Tuple<bool, CMPStorageError>(false, null));

                var cloudMessage = await _queueReference.GetMessageAsync();
                cloudMessage?.SetMessageContent(updateMessageString);

                if (queueStorageOptions == null)
                    await _queueReference.UpdateMessageAsync(cloudMessage, visibilityTimeOut,
                                                             MessageUpdateFields.Content | MessageUpdateFields.Visibility);
                else
                    await _queueReference.UpdateMessageAsync(cloudMessage, visibilityTimeOut,
                                                             MessageUpdateFields.Content | MessageUpdateFields.Visibility,
                                                             queueStorageOptions.QueueRequestOptions,
                                                             queueStorageOptions.OperationContext, _tokenSource.Token);
                
                return (new Tuple<bool, CMPStorageError>(true, null));

            }
            catch (StorageException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<bool, CMPStorageError>(false, error));

            }
            catch (ArgumentException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<bool, CMPStorageError>(false, error));

            }
            catch (OperationCanceledException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<bool, CMPStorageError>(false, error));

            }
        }

        public async Task<Tuple<bool, CMPStorageError>> DeleteMessageAsync(CloudQueueMessage queueMessage,
                                                                           CMPQueueStorageOptions
                                                                           queueStorageOptions = null)
        {

            try
            {

                if (_queueReference == null)
                    return (new Tuple<bool, CMPStorageError>(false, null));

                var cloudMessage = await _queueReference.GetMessageAsync();
                if (queueStorageOptions == null)
                    await _queueReference.DeleteMessageAsync(queueMessage);
                else
                    await _queueReference.DeleteMessageAsync(queueMessage, queueStorageOptions.QueueRequestOptions,
                                                             queueStorageOptions.OperationContext);

                return (new Tuple<bool, CMPStorageError>(true, null));

            }
            catch (StorageException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<bool, CMPStorageError>(false, error));

            }
            catch (ArgumentException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<bool, CMPStorageError>(false, error));

            }
            catch (OperationCanceledException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<bool, CMPStorageError>(false, error));

            }
        }

        public async Task<Tuple<bool, CMPStorageError>> DeleteMessageAsync(string deleteMessageString,
                                                                           string popupTokenString,
                                                                           CMPQueueStorageOptions
                                                                           queueStorageOptions = null)
        {

            try
            {

                if (_queueReference == null)
                    return (new Tuple<bool, CMPStorageError>(false, null));

                var cloudMessage = await _queueReference.GetMessageAsync();
                if (queueStorageOptions == null)
                    await _queueReference.DeleteMessageAsync(deleteMessageString, popupTokenString);
                else
                    await _queueReference.DeleteMessageAsync(deleteMessageString, popupTokenString,
                                                             queueStorageOptions.QueueRequestOptions,
                                                             queueStorageOptions.OperationContext, _tokenSource.Token);

                return (new Tuple<bool, CMPStorageError>(true, null));

            }
            catch (StorageException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<bool, CMPStorageError>(false, error));

            }
            catch (ArgumentException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<bool, CMPStorageError>(false, error));

            }
            catch (OperationCanceledException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<bool, CMPStorageError>(false, error));

            }
        }

        public async Task<Tuple<bool, CMPStorageError>> DeleteBatchMessagesAsync(List<CloudQueueMessage>
                                                                                 queueMessagesList,
                                                                                 CMPQueueStorageOptions
                                                                                 queueStorageOptions = null)
        {

            try
            {

                if (_queueReference == null)
                    return (new Tuple<bool, CMPStorageError>(false, null));

                await Task.Run(() => 
                {

                    var deleteTasksArray = queueMessagesList.Select(async (CloudQueueMessage queueMessageInfo) =>
                    {

                        if (queueMessageInfo == null)
                            return;

                        await DeleteMessageAsync(queueMessageInfo, queueStorageOptions);

                    }).ToArray();

                    Task.WaitAll(deleteTasksArray, _tokenSource.Token);

                });

                return (new Tuple<bool, CMPStorageError>(true, null));

            }
            catch (StorageException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<bool, CMPStorageError>(false, error));

            }
            catch (ArgumentException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<bool, CMPStorageError>(false, error));

            }
            catch (OperationCanceledException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<bool, CMPStorageError>(false, error));

            }
        }

        public async Task<Tuple<bool, CMPStorageError>> DeleteQueueAsync(CMPQueueStorageOptions queueStorageOptions =
                                                                         null)
        {
            

            try
            {

                if (_queueReference == null)
                    return (new Tuple<bool, CMPStorageError>(false, null));

                var couldDelete = false;
                if (queueStorageOptions == null)
                    couldDelete = await _queueReference.DeleteIfExistsAsync();
                else
                    couldDelete = await _queueReference.DeleteIfExistsAsync(queueStorageOptions.QueueRequestOptions,
                                                                            queueStorageOptions.OperationContext,
                                                                            _tokenSource.Token);

                return (new Tuple<bool, CMPStorageError>(couldDelete, null));

            }
            catch (StorageException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<bool, CMPStorageError>(false, error));

            }
            catch (ArgumentException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<bool, CMPStorageError>(false, error));

            }
        }

    }
}
