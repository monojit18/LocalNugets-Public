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
using Subsystems.AzureStorage.Internal;

namespace Subsystems.AzureStorage.External
{

    public delegate void FetchProgressCallback<T>(List<T> fetchedResults) where T : CMPTableStorageModel, new();

    public class CMPAzureTableStorageProxy
    {

        private CMPTableStorageService _tableStorageService;

        public CMPAzureTableStorageProxy(string connectionString)
        {

            _tableStorageService = new CMPTableStorageService(connectionString);

        }

        public async Task<Tuple<bool, CMPStorageError>> CreateTableAsync(string tableNameString)
        {

            if (_tableStorageService == null)
                return (new Tuple<bool, CMPStorageError>(false, null));

            var createInfo = await _tableStorageService.CreateTableAsync(tableNameString);
            return createInfo;

        }

        public async Task<Tuple<bool, CMPStorageError>> DeleteTableAsync(string tableNameString)
        {

            if (_tableStorageService == null)
                return (new Tuple<bool, CMPStorageError>(false, null));

            var deleteInfo = await _tableStorageService.DeleteTableAsync(tableNameString);
            return deleteInfo;

        }

        public async Task<Tuple<T, CMPStorageError>> InsertRowAsync<T>(T insertModel, string tableNameString)
            where T : CMPTableStorageModel, new()
        {

            if (_tableStorageService == null)
                return (new Tuple<T, CMPStorageError>(null, null));

            var insertedRow = await _tableStorageService.InsertRowAsync<T>(insertModel, tableNameString);
            return insertedRow;

        }

        public async Task<Tuple<List<T>, CMPStorageError>> InsertRowsInBatchAsync<T>(List<T> insertModelsList,
                                                                                     string tableNameString)
            where T : CMPTableStorageModel, new()
        {

            if (_tableStorageService == null)
                return (new Tuple<List<T>, CMPStorageError>(null, null));

            var insertedResults = await _tableStorageService.InsertRowsInBatchAsync<T>(insertModelsList,
                                                                                       tableNameString);
            return insertedResults;

        }

        public async Task<Tuple<T, CMPStorageError>> UpdateRowAsync<T>(string tableNameString, T updateModel)
            where T : CMPTableStorageModel, new()
        {

            if (_tableStorageService == null)
                return null;

            var updatedResults = await _tableStorageService.UpdateRowAsync<T>(tableNameString, updateModel);
            return updatedResults;

        }

        public async Task<List<Tuple<T, CMPStorageError>>> UpdateRowsAsync<T>(string tableNameString,
                                                                              string partitionKeyString)
            where T : CMPTableStorageModel, new()
        {

            if (_tableStorageService == null)
                return null;

            var updatedResults = await _tableStorageService.UpdateRowsAsync<T>(tableNameString, partitionKeyString);
            return updatedResults;

        }

        public async Task<Tuple<T, CMPStorageError>> DeleteRowAsync<T>(string tableNameString, T deleteModel)
           where T : CMPTableStorageModel, new()
        {

            if (_tableStorageService == null)
                return null;

            var deletedResults = await _tableStorageService.DeleteRowAsync<T>(tableNameString, deleteModel);
            return deletedResults;

        }

        public async Task<List<Tuple<T, CMPStorageError>>> DeleteRowsAsync<T>(string tableNameString,
                                                                              string partitionKeyString)
            where T : CMPTableStorageModel, new()
        {

            if (_tableStorageService == null)
                return null;

            var deletedResults = await _tableStorageService.DeleteRowsAsync<T>(tableNameString, partitionKeyString);
            return deletedResults;

        }

        public async Task<Tuple<List<T>, CMPStorageError>> FetchAllAsync<T>(string tableNameString,
                                                                            FetchProgressCallback<T>
                                                                            fetchProgressCallback)
            where T : CMPTableStorageModel, new()
        {

            if (_tableStorageService == null)
                return null;

            var fetchedResults = await _tableStorageService.FetchAllAsync<T>(tableNameString, fetchProgressCallback);
            return fetchedResults;

        }

        public async Task<Tuple<List<T>, CMPStorageError>> FetchAsync<T>(string tableNameString,
                                                                         string partitionKeyString,
                                                                         FetchProgressCallback<T> fetchProgressCallback)
            where T : CMPTableStorageModel, new()
        {

            if (_tableStorageService == null)
                return null;

            var fetchedResults = await _tableStorageService.FetchAsync<T>(tableNameString, partitionKeyString,
                                                                          fetchProgressCallback);
            return fetchedResults;

        }

        public async Task<Tuple<T, CMPStorageError>> FetchAsync<T>(string tableNameString,
                                                                         string partitionKeyString,
                                                                         string rowKeyString)
            where T : CMPTableStorageModel, new()
        {

            if (_tableStorageService == null)
                return null;

            var fetchedResult = await _tableStorageService.FetchAsync<T>(tableNameString, partitionKeyString,
                                                                         rowKeyString);
            return fetchedResult;

        }

    }
}
