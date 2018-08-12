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
using Diagonistics = System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Subsystems.AzureStorage.External;

namespace Subsystems.AzureStorage.Internal
{
    public class CMPBlobStorageService : CMPStorageServiceBase
    {

        private string _containerNameString;
        private CloudBlobClient _cloudBlobClient;

        private CloudBlockBlob PrepareBlockBlobReference(string blobNameString)
        {

            if ((string.IsNullOrEmpty(_containerNameString) == true) || (string.IsNullOrEmpty(blobNameString) == true))
                return null;

            try
            {

                var blobContainerReference = _cloudBlobClient.GetContainerReference(_containerNameString);
                if (blobContainerReference == null)
                    return null;

                var blobReference = blobContainerReference.GetBlockBlobReference(blobNameString);
                return blobReference;

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

        public CMPBlobStorageService(string connectionString, string containerNameString) : base(connectionString)
        {
            
            var couldParse = CloudStorageAccount.TryParse(connectionString, out _cloudStorageAccount);
            if (couldParse == false)
                return;

            _containerNameString = string.Copy(containerNameString);
            _cloudBlobClient = _cloudStorageAccount.CreateCloudBlobClient();

        }

        public async Task<Tuple<bool, CMPStorageError>> CreateBlobContainerAsync(CMPBlobStorageOptions
                                                                                 blobStorageOptions = null)
        {

            if (string.IsNullOrEmpty(_containerNameString) == true)
                return (new Tuple<bool, CMPStorageError>(false, null));

            try
            {

                var blobContainerReference = _cloudBlobClient.GetContainerReference(_containerNameString);
                if (blobContainerReference == null)
                    return (new Tuple<bool, CMPStorageError>(false, null));

                var couldCreate = false;
                if (blobStorageOptions == null)
                    couldCreate = await blobContainerReference.CreateIfNotExistsAsync();
                else                    
                    couldCreate = await blobContainerReference.CreateIfNotExistsAsync(blobStorageOptions.
                                                                                      BlobContainerPublicAccessType,
                                                                                      blobStorageOptions.
                                                                                      BlobRequestOptions,
                                                                                      blobStorageOptions.
                                                                                      OperationContext,
                                                                                      _tokenSource.Token);
                return (new Tuple<bool, CMPStorageError>(couldCreate, null));

            }
            catch (StorageException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<bool, CMPStorageError>(false, error));

            }

        }

        public async Task<Tuple<bool, CMPStorageError>> AddBytesToBlobAsync(byte[] blobBytesArray,
                                                                            string blobNameString,
                                                                            CMPBlobStorageOptions
                                                                            blobStorageOptions = null)
        {

            if ((blobBytesArray == null) || (blobBytesArray.Length == 0))
                return (new Tuple<bool, CMPStorageError>(false, null));

            try
            {

                var blobReference = PrepareBlockBlobReference(blobNameString);
                if (blobReference == null)
                    return (new Tuple<bool, CMPStorageError>(false, null));
                
                if (blobStorageOptions == null)
                    await blobReference.UploadFromByteArrayAsync(blobBytesArray, 0, blobBytesArray.Length);
                else
                    await blobReference.UploadFromByteArrayAsync(blobBytesArray, 0, blobBytesArray.Length,
                                                                 blobStorageOptions.AccessCondition,
                                                                 blobStorageOptions.BlobRequestOptions,
                                                                 blobStorageOptions.OperationContext,
                                                                 _tokenSource.Token);
                
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
        }

        public async Task<Tuple<bool, CMPStorageError>> AddStreamToBlobAsync(Stream blobStream, string blobNameString,
                                                                             CMPBlobStorageOptions
                                                                             blobStorageOptions = null)
        {

            if (blobStream == null)
                return (new Tuple<bool, CMPStorageError>(false, null));

            if (string.IsNullOrEmpty(blobNameString) == true)
                return (new Tuple<bool, CMPStorageError>(false, null));

            try
            {

                var blobReference = PrepareBlockBlobReference(blobNameString);
                if (blobReference == null)
                    return (new Tuple<bool, CMPStorageError>(false, null));

                if (blobStorageOptions == null)
                    await blobReference.UploadFromStreamAsync(blobStream, blobStream.Length);
                else
                    await blobReference.UploadFromStreamAsync(blobStream, blobStream.Length,
                                                              blobStorageOptions.AccessCondition,
                                                              blobStorageOptions.BlobRequestOptions,
                                                              blobStorageOptions.OperationContext, _tokenSource.Token);
                
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

        }

        public async Task<Tuple<bool, CMPStorageError>> DeleteBlobAsync(string blobNameString,
                                                                        CMPBlobStorageOptions
                                                                        blobStorageOptions = null)
        {

            if (string.IsNullOrEmpty(blobNameString) == true)
                return (new Tuple<bool, CMPStorageError>(false, null));

            try
            {

                var blobReference = PrepareBlockBlobReference(blobNameString);
                if (blobReference == null)
                    return (new Tuple<bool, CMPStorageError>(false, null));

                var couldDelete = false;
                if (blobStorageOptions == null)
                    couldDelete = await blobReference.DeleteIfExistsAsync();
                else
                    couldDelete = await blobReference.DeleteIfExistsAsync(blobStorageOptions.DeleteSnapshotsOption,
                                                                          blobStorageOptions.AccessCondition,
                                                                          blobStorageOptions.BlobRequestOptions,
                                                                          blobStorageOptions.OperationContext,
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

        public async Task<Tuple<bool, CMPStorageError>> DeleteBlobContainerAsync(CMPBlobStorageOptions
                                                                                 blobStorageOptions = null)
        {

            if (string.IsNullOrEmpty(_containerNameString) == true)
                return (new Tuple<bool, CMPStorageError>(false, null));

            try
            {

                var blobContainerReference = _cloudBlobClient.GetContainerReference(_containerNameString);
                if (blobContainerReference == null)
                    return (new Tuple<bool, CMPStorageError>(false, null));

                var couldDelete = false;
                if (blobStorageOptions == null)
                    couldDelete = await blobContainerReference.DeleteIfExistsAsync();
                else
                    couldDelete = await blobContainerReference.DeleteIfExistsAsync(blobStorageOptions.AccessCondition,
                                                                                   blobStorageOptions.BlobRequestOptions,
                                                                                   blobStorageOptions.OperationContext,
                                                                                   _tokenSource.Token);

                return (new Tuple<bool, CMPStorageError>(couldDelete, null));

            }
            catch (StorageException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<bool, CMPStorageError>(false, error));

            }
        }

        public async Task<Tuple<byte[], CMPStorageError>> DownloadFromBlobAsync(string blobNameString,
                                                                                CMPBlobStorageOptions
                                                                                blobStorageOptions = null)
        {

            if (string.IsNullOrEmpty(blobNameString) == true)
                return (new Tuple<byte[], CMPStorageError>(null, null));

            try
            {

                var blobReference = PrepareBlockBlobReference(blobNameString);
                if (blobReference == null)
                    return (new Tuple<byte[], CMPStorageError>(null, null));

                var memoryStream = new MemoryStream();
                if (blobStorageOptions == null)
                    await blobReference.DownloadToStreamAsync(memoryStream);
                else
                    await blobReference.DownloadToStreamAsync(memoryStream, blobStorageOptions.AccessCondition,
                                                              blobStorageOptions.BlobRequestOptions,
                                                              blobStorageOptions.OperationContext, _tokenSource.Token);  
                
                memoryStream.Close();
                var downloadedBytesArray = memoryStream.GetBuffer();
                return (new Tuple<byte[], CMPStorageError>(downloadedBytesArray, null));

            }
            catch (StorageException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<byte[], CMPStorageError>(null, null));

            }
            catch (ArgumentException exception)
            {

                var error = CMPStorageError.CreateErrorFromException(exception);
                return (new Tuple<byte[], CMPStorageError>(null, null));

            }
        }
    }
}
