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
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Subsystems.AzureStorage.External;

namespace Subsystems.AzureStorage.Internal
{
    
    public class CMPTableStorageService : CMPStorageServiceBase
    {

        private const string KPartitionKeystring = "PartitionKey";
        private const string KRowKeystring = "RowKey";
        private CloudTableClient _cloudTableClient;

        private async Task<Tuple<List<T>, CMPStorageError>> FetchForFilterAsync<T>(CloudTable tableReference,
                                                                                   string filterConditionString,
                                                                                   FetchProgressCallback<T>
                                                                                   fetchProgressCallback = null)
            where T : CMPTableStorageModel, new()
        {

            if ((tableReference == null) || (string.IsNullOrEmpty(filterConditionString) == true))
                return (new Tuple<List<T>, CMPStorageError>(null, null));

            var tableQuery = new TableQuery<T>().Where(filterConditionString);
            TableContinuationToken token = null;
            var fetchedResultsList = new List<T>();

            do
            {

                var tableQuerySegment = await tableReference.ExecuteQuerySegmentedAsync(tableQuery, token);

                var currentResultsList = tableQuerySegment?.Results?.Cast<T>().ToList();
                if (fetchProgressCallback != null)
                    fetchProgressCallback.Invoke(currentResultsList);
                
                fetchedResultsList.AddRange(currentResultsList);
                token = tableQuerySegment.ContinuationToken;

            } while (token != null);

            return (new Tuple<List<T>, CMPStorageError>(fetchedResultsList, null));

        }

        public CMPTableStorageService(string connectionString) : base(connectionString)
        {


            var couldParse = CloudStorageAccount.TryParse(connectionString, out _cloudStorageAccount);
            if (couldParse == false)
                return;

            _cloudTableClient = _cloudStorageAccount.CreateCloudTableClient();
            
        }

        public async Task<Tuple<bool, CMPStorageError>> CreateTableAsync(string tableNameString, 
                                                                         CMPTableStorageOptions tableStorageOptions =
                                                                         null)
        {

            if (_cloudTableClient == null)
                return (new Tuple<bool, CMPStorageError>(false, null));

            try
            {

                var tableReference = _cloudTableClient.GetTableReference(tableNameString);
                if (tableReference == null)
                    return (new Tuple<bool, CMPStorageError>(false, null));
                
                var couldCreate = false;
                if (tableStorageOptions == null)
                    couldCreate = await tableReference.CreateIfNotExistsAsync();
                else
                    couldCreate = await tableReference.CreateIfNotExistsAsync(tableStorageOptions.TableRequestOptions,
                                                                              tableStorageOptions.OperationContext,
                                                                              _tokenSource.Token);
                
                return (new Tuple<bool, CMPStorageError>(true, null));

            }
            catch(StorageException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<bool, CMPStorageError>(false, error));

            }

        }

        public async Task<Tuple<bool, CMPStorageError>> DeleteTableAsync(string tableNameString,
                                                                         CMPTableStorageOptions tableStorageOptions =
                                                                         null)
        {

            if (_cloudTableClient == null)
                return (new Tuple<bool, CMPStorageError>(false, null));

            try
            {

                var tableReference = _cloudTableClient.GetTableReference(tableNameString);
                if (tableReference == null)
                    return (new Tuple<bool, CMPStorageError>(false, null));
                
                var couldDelete = false;
                if (tableStorageOptions == null)
                    couldDelete = await tableReference.DeleteIfExistsAsync();
                else
                    couldDelete = await tableReference.DeleteIfExistsAsync(tableStorageOptions.TableRequestOptions,
                                                                           tableStorageOptions.OperationContext,
                                                                           _tokenSource.Token);
                
                return (new Tuple<bool, CMPStorageError>(false, null));

            }
            catch (StorageException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<bool, CMPStorageError>(false, error));

            }

        }

        public async Task<Tuple<T, CMPStorageError>> InsertRowAsync<T>(T insertModel, string tableNameString,
                                                                       CMPTableStorageOptions tableStorageOptions =
                                                                       null)
            where T : CMPTableStorageModel, new()
        {


            if (_cloudTableClient == null)
                return (new Tuple<T, CMPStorageError>(null, null));
            
            if (string.IsNullOrEmpty(tableNameString) == true)
                return (new Tuple<T, CMPStorageError>(null, null));

            if ((insertModel == null) || (_cloudTableClient == null))
                return (new Tuple<T, CMPStorageError>(null, null));

            try
            {

                var tableReference = _cloudTableClient.GetTableReference(tableNameString);
                if (tableReference == null)
                    return (new Tuple<T, CMPStorageError>(null, null));
                
                var insertOperation = TableOperation.Insert(insertModel);
                TableResult insertResult = null;

                if (tableStorageOptions == null)
                    insertResult = await tableReference.ExecuteAsync(insertOperation);
                else
                    insertResult = await tableReference.ExecuteAsync(insertOperation,
                                                                     tableStorageOptions.TableRequestOptions,
                                                                     tableStorageOptions.OperationContext,
                                                                     _tokenSource.Token);
                
                CMPStorageError error = null;
                if (insertResult.HttpStatusCode != 200)
                    error = CMPStorageError.CreateError(insertResult.HttpStatusCode.ToString(), string.Empty);

                return (new Tuple<T, CMPStorageError>(insertResult.Result as T,
                                                                         error));

            }
            catch (StorageException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<T, CMPStorageError>(null, error));

            }
            catch (ArgumentException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<T, CMPStorageError>(null, error));

            }

        }

        public async Task<Tuple<List<T>, CMPStorageError>> InsertRowsInBatchAsync<T>(List<T> insertModelsList,
                                                                                     string tableNameString,
                                                                                     CMPTableStorageOptions
                                                                                     tableStorageOptions = null)
            where T : CMPTableStorageModel, new()

        {

            if (_cloudTableClient == null)
                return (new Tuple<List<T>, CMPStorageError>(null, null));
            
            if (string.IsNullOrEmpty(tableNameString) == true)
                return (new Tuple<List<T>, CMPStorageError>(null, null));

            if ((insertModelsList == null) || (insertModelsList.Count == 0) || (_cloudTableClient == null))
                return (new Tuple<List<T>, CMPStorageError>(null, null));

            try
            {

                var tableReference = _cloudTableClient.GetTableReference(tableNameString);
                if (tableReference == null)
                    return (new Tuple<List<T>, CMPStorageError>(null, null));
                
                var insertInBatchOperation = new TableBatchOperation();
                foreach (var insertModel in insertModelsList)
                    insertInBatchOperation.Insert(insertModel);

                IList<TableResult> insertInBatchResult = null;
                if (tableStorageOptions == null)
                    insertInBatchResult = await tableReference.ExecuteBatchAsync(insertInBatchOperation);
                else
                    insertInBatchResult = await tableReference.ExecuteBatchAsync(insertInBatchOperation,
                                                                                 tableStorageOptions.TableRequestOptions,
                                                                                 tableStorageOptions.OperationContext,
                                                                                 _tokenSource.Token);
                
                var insertResultsList = insertInBatchResult.Select((TableResult insertResult) =>
                {

                    return (insertResult?.Result as T);

                }).ToList();

                return (new Tuple<List<T>, CMPStorageError>(insertResultsList, null));

            }
            catch (StorageException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<List<T>, CMPStorageError>(null, error));

            }
            catch (ArgumentException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<List<T>, CMPStorageError>(null, error));

            }

        }

        public async Task<Tuple<T, CMPStorageError>> UpdateRowAsync<T>(string tableNameString, T updateModel,
                                                                       CMPTableStorageOptions tableStorageOptions =
                                                                       null)
            where T : CMPTableStorageModel, new()
        {


            if (_cloudTableClient == null)
                return (new Tuple<T, CMPStorageError>(null, null));

            if (string.IsNullOrEmpty(tableNameString) == true)
                return (new Tuple<T, CMPStorageError>(null, null));

            if (updateModel == null)
                return (new Tuple<T, CMPStorageError>(null, null));

            try
            {

                var tableReference = _cloudTableClient.GetTableReference(tableNameString);
                if (tableReference == null)
                    return (new Tuple<T, CMPStorageError>(null, null));

                var retrieveOperation = TableOperation.Retrieve<T>(updateModel.PartitionKey, updateModel.RowKey);
                var retrieveResult = await tableReference.ExecuteAsync(retrieveOperation);

                var retrievedModel = retrieveResult?.Result as T;
                if (retrievedModel == null)
                    return (new Tuple<T, CMPStorageError>(null, null));

                updateModel.ETag = retrievedModel.ETag;
                var updateOperation = TableOperation.Replace(updateModel);

                TableResult updateResult = null;
                if (tableStorageOptions == null)
                    updateResult = await tableReference.ExecuteAsync(updateOperation);
                else
                    updateResult = await tableReference.ExecuteAsync(updateOperation,
                                                                     tableStorageOptions.TableRequestOptions,
                                                                     tableStorageOptions.OperationContext,
                                                                     _tokenSource.Token);

                return (new Tuple<T, CMPStorageError>(updateResult?.Result as T, null));

            }
            catch (StorageException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<T, CMPStorageError>(null, error));

            }
            catch (ArgumentException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<T, CMPStorageError>(null, error));

            }

        }

        public async Task<List<Tuple<T, CMPStorageError>>> UpdateRowsAsync<T>(string tableNameString,
                                                                              string partitionKeyString,
                                                                              CMPTableStorageOptions
                                                                              tableStorageOptions = null)
            where T : CMPTableStorageModel, new()
        {


            if (_cloudTableClient == null)
                return null;

            if (string.IsNullOrEmpty(tableNameString) == true)
                return null;

            try
            {

                var tableReference = _cloudTableClient.GetTableReference(tableNameString);
                if (tableReference == null)
                    return null;

                string filterConditionString = TableQuery.GenerateFilterCondition(KPartitionKeystring,
                                                                                  QueryComparisons.Equal,
                                                                                  partitionKeyString);

                var fetchedResultsInfo = await FetchForFilterAsync<T>(tableReference, filterConditionString);
                var fetchedResultsList = fetchedResultsInfo.Item1;

                if ((fetchedResultsList == null) || (fetchedResultsList.Count == 0))
                    return null;

                var updateResultsInfoList = new List<Tuple<T, CMPStorageError>>();
                var updateTasksArray = fetchedResultsList.Select(async (T updateModel) =>
                {

                    var updateResultInfo = await UpdateRowAsync<T>(tableNameString, updateModel, tableStorageOptions);
                    updateResultsInfoList.Add(updateResultInfo);


                }).ToArray();

                await Task.WhenAll(updateTasksArray);
                return null;

            }
            catch (StorageException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                var errorInfo = new Tuple<T, CMPStorageError>(null, error);
                return (new List<Tuple<T, CMPStorageError>>()
                {
                    errorInfo

                });

            }
            catch (ArgumentException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                var errorInfo = new Tuple<T, CMPStorageError>(null, error);
                return (new List<Tuple<T, CMPStorageError>>()
                {
                    errorInfo

                });

            }

        }

        public async Task<Tuple<T, CMPStorageError>> DeleteRowAsync<T>(string tableNameString, T deleteModel,
                                                                       CMPTableStorageOptions tableStorageOptions =
                                                                       null)
            where T : CMPTableStorageModel, new()
        {


            if (_cloudTableClient == null)
                return (new Tuple<T, CMPStorageError>(null, null));

            if (string.IsNullOrEmpty(tableNameString) == true)
                return (new Tuple<T, CMPStorageError>(null, null));

            if (deleteModel == null)
                return (new Tuple<T, CMPStorageError>(null, null));

            try
            {

                var tableReference = _cloudTableClient.GetTableReference(tableNameString);
                if (tableReference == null)
                    return (new Tuple<T, CMPStorageError>(null, null));

                var retrieveOperation = TableOperation.Retrieve<T>(deleteModel.PartitionKey, deleteModel.RowKey);
                var retrieveResult = await tableReference.ExecuteAsync(retrieveOperation);

                var retrievedModel = retrieveResult?.Result as T;
                if (retrievedModel == null)
                    return (new Tuple<T, CMPStorageError>(null, null));

                var deleteOperation = TableOperation.Delete(retrievedModel);

                TableResult deleteResult = null;
                if (tableStorageOptions == null)
                    deleteResult = await tableReference.ExecuteAsync(deleteOperation);
                else
                    deleteResult = await tableReference.ExecuteAsync(deleteOperation,
                                                                     tableStorageOptions.TableRequestOptions,
                                                                     tableStorageOptions.OperationContext,
                                                                     _tokenSource.Token);

                return (new Tuple<T, CMPStorageError>(deleteResult?.Result as T, null));

            }
            catch (StorageException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<T, CMPStorageError>(null, error));

            }
            catch (ArgumentException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<T, CMPStorageError>(null, error));

            }

        }

        public async Task<List<Tuple<T, CMPStorageError>>> DeleteRowsAsync<T>(string tableNameString,
                                                                              string partitionKeyString,
                                                                              CMPTableStorageOptions
                                                                              tableStorageOptions = null)
            where T : CMPTableStorageModel, new()
        {


            if (_cloudTableClient == null)
                return null;

            if (string.IsNullOrEmpty(tableNameString) == true)
                return null;

            try
            {

                var tableReference = _cloudTableClient.GetTableReference(tableNameString);
                if (tableReference == null)
                    return null;
                
                string filterConditionString = TableQuery.GenerateFilterCondition(KPartitionKeystring,
                                                                                  QueryComparisons.Equal,
                                                                                  partitionKeyString);

                var fetchedResultsInfo = await FetchForFilterAsync<T>(tableReference, filterConditionString);
                var fetchedResultsList = fetchedResultsInfo.Item1;

                if ((fetchedResultsList == null) || (fetchedResultsList.Count == 0))
                    return null;

                var deleteResultsInfoList = new List<Tuple<T, CMPStorageError>>();
                var deleteTasksArray = fetchedResultsList.Select(async (T deleteModel) =>
                {

                    var deleteResultInfo = await DeleteRowAsync<T>(tableNameString, deleteModel, tableStorageOptions);
                    deleteResultsInfoList.Add(deleteResultInfo);


                }).ToArray();

                await Task.WhenAll(deleteTasksArray);
                return deleteResultsInfoList;

            }
            catch (StorageException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                var errorInfo = new Tuple<T, CMPStorageError>(null, error);
                return (new List<Tuple<T, CMPStorageError>>()
                {
                    errorInfo

                });

            }
            catch (ArgumentException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                var errorInfo = new Tuple<T, CMPStorageError>(null, error);
                return (new List<Tuple<T, CMPStorageError>>()
                {
                    errorInfo

                });

            }

        }

        public async Task<Tuple<List<T>, CMPStorageError>> FetchAllAsync<T>(string tableNameString,
                                                                            FetchProgressCallback<T>
                                                                            fetchProgressCallback)
            where T : CMPTableStorageModel, new()
        {

            if (_cloudTableClient == null)
                return (new Tuple<List<T>, CMPStorageError>(null, null));
            
            if (string.IsNullOrEmpty(tableNameString) == true)
                return (new Tuple<List<T>, CMPStorageError>(null, null));

            try
            {

                var tableReference = _cloudTableClient.GetTableReference(tableNameString);
                if (tableReference == null)
                    return (new Tuple<List<T>, CMPStorageError>(null, null));
                
                string filterConditionString = TableQuery.GenerateFilterCondition(KPartitionKeystring,
                                                                                  QueryComparisons.NotEqual,
                                                                                  string.Empty);
                var fetchedResultsInfo = await FetchForFilterAsync<T>(tableReference, filterConditionString,
                                                                      fetchProgressCallback);
                return fetchedResultsInfo;


            }
            catch (StorageException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<List<T>, CMPStorageError>(null, error));

            }

        }

        public async Task<Tuple<List<T>, CMPStorageError>> FetchAsync<T>(string tableNameString,
                                                                         string partitionKeyString,
                                                                         FetchProgressCallback<T> fetchProgressCallback)
            where T : CMPTableStorageModel, new()
        {

            if (_cloudTableClient == null)
                return (new Tuple<List<T>, CMPStorageError>(null, null));
            
            if (string.IsNullOrEmpty(tableNameString) == true)
                return (new Tuple<List<T>, CMPStorageError>(null, null));

            try
            {

                var tableReference = _cloudTableClient.GetTableReference(tableNameString);
                if (tableReference == null)
                    return (new Tuple<List<T>, CMPStorageError>(null, null));
                                                 
                string filterConditionString = TableQuery.GenerateFilterCondition(KPartitionKeystring,
                                                                                  QueryComparisons.Equal,
                                                                                  partitionKeyString);

                var fetchedResultsInfo = await FetchForFilterAsync<T>(tableReference, filterConditionString,
                                                                      fetchProgressCallback);
                return fetchedResultsInfo;

            }
            catch (StorageException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<List<T>, CMPStorageError>(null, error));

            }

        }

        public async Task<Tuple<T, CMPStorageError>> FetchAsync<T>(string tableNameString, string partitionKeyString,
                                                                   string rowKeyString,
                                                                   CMPTableStorageOptions tableStorageOptions = null)
            where T : CMPTableStorageModel, new()
        {

            if (_cloudTableClient == null)
                return (new Tuple<T, CMPStorageError>(null, null));
            
            if (string.IsNullOrEmpty(tableNameString) == true)
                return (new Tuple<T, CMPStorageError>(null, null));

            try
            {

                var tableReference = _cloudTableClient.GetTableReference(tableNameString);
                if (tableReference == null)
                    return (new Tuple<T, CMPStorageError>(null, null));
                
                var tableOperation = TableOperation.Retrieve<T>(partitionKeyString, rowKeyString);

                TableResult fetchedResultInfo = null;
                if (tableStorageOptions == null)
                    fetchedResultInfo = await tableReference.ExecuteAsync(tableOperation);
                else
                    fetchedResultInfo = await tableReference.ExecuteAsync(tableOperation,
                                                                          tableStorageOptions.TableRequestOptions,
                                                                          tableStorageOptions.OperationContext,
                                                                          _tokenSource.Token);
                
                return (new Tuple<T, CMPStorageError>(fetchedResultInfo.Result as T, null));

            }
            catch (StorageException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<T, CMPStorageError>(null, error));

            }

        }

    }
}
