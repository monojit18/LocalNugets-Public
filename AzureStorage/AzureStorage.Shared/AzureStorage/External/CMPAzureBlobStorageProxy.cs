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
using System.Threading.Tasks;
using Subsystems.AzureStorage.Internal;

namespace Subsystems.AzureStorage.External
{
    public class CMPAzureBlobStorageProxy
    {

        private CMPBlobStorageService _blobStorageService;

        public CMPAzureBlobStorageProxy(string connectionString, string containerNameString)
        {

            _blobStorageService = new CMPBlobStorageService(connectionString, containerNameString);

        }

        public async Task<Tuple<bool, CMPStorageError>> CreateBlobContainerAsync()
        {

            if (_blobStorageService == null)
                return (new Tuple<bool, CMPStorageError>(false, null));

            var addToBlobInfo = await _blobStorageService.CreateBlobContainerAsync();
            return addToBlobInfo;

        }

        public async Task<Tuple<bool, CMPStorageError>> AddBytesToBlobAsync(byte[] blobBytesArray,
                                                                            string blobNameString)
        {

            if (_blobStorageService == null)
                return (new Tuple<bool, CMPStorageError>(false, null));

            var addToBlobInfo = await _blobStorageService.AddBytesToBlobAsync(blobBytesArray, blobNameString);
            return addToBlobInfo;

        }

        public async Task<Tuple<bool, CMPStorageError>> AddStreamToBlobAsync(Stream blobStream, string blobNameString)
        {

            if (_blobStorageService == null)
                return (new Tuple<bool, CMPStorageError>(false, null));

            var addToBlobInfo = await _blobStorageService.AddStreamToBlobAsync(blobStream, blobNameString);
            return addToBlobInfo;

        }

        public async Task<Tuple<bool, CMPStorageError>> DeleteBlobAsync(string blobNameString)
        {

            if (_blobStorageService == null)
                return (new Tuple<bool, CMPStorageError>(false, null));

            var deleteBlobInfo = await _blobStorageService.DeleteBlobAsync(blobNameString);
            return deleteBlobInfo;

        }

        public async Task<Tuple<bool, CMPStorageError>> DeleteBlobContainerAsync()
        {

            if (_blobStorageService == null)
                return (new Tuple<bool, CMPStorageError>(false, null));

            var deleteBlobContainerInfo = await _blobStorageService.DeleteBlobContainerAsync();
            return deleteBlobContainerInfo;

        }

        public async Task<Tuple<byte[], CMPStorageError>> DownloadFromBlobAsync(string blobNameString)
        {

            if (_blobStorageService == null)
                return (new Tuple<byte[], CMPStorageError>(null, null));

            var downloadBlobInfo = await _blobStorageService.DownloadFromBlobAsync(blobNameString);
            return downloadBlobInfo;

        }
    }
}
