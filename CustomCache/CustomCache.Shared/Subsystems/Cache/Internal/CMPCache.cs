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
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Subsystems.CustomFileManager.External;

namespace Subsystems.Cache.Internal
{
	public class CMPCache
	{

        #region Private Variables
        private const string _kCachedFolderNameString = "/CachedFiles/";
        private string _cacheFolderPathString;
        private long _sizeLimitInKB;
        private readonly SemaphoreSlim _cacheSemaphore;
        private readonly CMPFileManagerProxy _fileManagerProxy;
        private int _expiryDays;
        private int _expiryMonths;
        private int _expiryYears;
        private int _expiryHours;
        private int _expiryMinutes;
        private int _expirySeconds;
		#endregion

        #region Private/Protected Methods
        private string PrepareFoldePath(string cacheFolderPathString)
        {

            if (string.IsNullOrEmpty(cacheFolderPathString))
                return null;

            var baseFolderPathString = cacheFolderPathString + _kCachedFolderNameString;
            var couldPrepare = CMPFileManagerProxy.PrepareFoldePath(baseFolderPathString);
            return (couldPrepare == true) ? baseFolderPathString : null;

        }

        private string ConvertFileNameToBase64(string fileNameString)
        {

            if (string.IsNullOrEmpty(fileNameString))
                return null;

            byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileNameString);
            string fileNameWithBase64String = Convert.ToBase64String(fileNameBytes);
            return fileNameWithBase64String;

        }

        private bool ShouldCacheItem(string fileNameString)
        {

            if (string.IsNullOrEmpty(fileNameString))
                return false;

            var DoesExists = _fileManagerProxy.DoesFileExist(fileNameString);
            if (DoesExists == false)
                return true;

            long folderSize = _fileManagerProxy.AdjustFolderSizeWithLRULimit(_sizeLimitInKB);
            return (folderSize != -1);

        }

        private bool ShouldRetrieveItem(string fileNameString, bool shouldCheckExpiry = true)
        {

            if (string.IsNullOrEmpty(fileNameString))
                return false;

            var DoesExists = _fileManagerProxy.DoesFileExist(fileNameString);
            if (DoesExists == false)
                return false;

            if (shouldCheckExpiry == false)
                return true;

            bool HasExpired = _fileManagerProxy.CheckExpiry(fileNameString, _expiryDays,
                                                            _expiryMonths, _expiryYears,
                                                            _expiryHours, _expiryMinutes,
                                                            _expirySeconds);

            return (HasExpired == false);

        }

        private bool ShouldDeleteItem(string fileNameString)
        {

            if (string.IsNullOrEmpty(fileNameString))
                return false;

            var DoesExists = _fileManagerProxy.DoesFileExist(fileNameString);
            return DoesExists;

        }
        #endregion

        #region Public Methods
        public CMPCache(string cacheFolderPathString, long sizeLimitInKB)
        {

            _cacheFolderPathString = PrepareFoldePath(cacheFolderPathString);
            _sizeLimitInKB = sizeLimitInKB;
            _cacheSemaphore = new SemaphoreSlim(1);
            _fileManagerProxy = new CMPFileManagerProxy(_cacheFolderPathString);

        }

        public CMPCache ExpiryDays(int days)
        {

            _expiryDays = days;
            return this;

        }

        public CMPCache ExpiryMonths(int months)
        {

            _expiryMonths = months;
            return this;

        }

        public CMPCache ExpiryYears(int years)
        {

            _expiryYears = years;
            return this;

        }

        public CMPCache ExpiryHours(int hours)
        {

            _expiryHours = hours;
            return this;

        }

        public CMPCache ExpiryMinutes(int minutes)
        {

            _expiryMinutes = minutes;
            return this;

        }

        public CMPCache ExpirySeconds(int seconds)
        {

            _expirySeconds = seconds;
            return this;

        }

        public FileInfo GetLRUCachedItem()
        {

            if (string.IsNullOrEmpty(_cacheFolderPathString) == true)
                return null;

            FileInfo lruFileInfo = _fileManagerProxy.GetLRUFileItem(_cacheFolderPathString);
            return lruFileInfo;

        }

        public bool CacheItem(byte[] cacheBytes, string fileNameString)
        {

            if (cacheBytes.Length == 0 || string.IsNullOrEmpty(fileNameString))
                return false;

            fileNameString = ConvertFileNameToBase64(fileNameString);
            bool shouldCacheItem = ShouldCacheItem(fileNameString);
            if (shouldCacheItem == false)
                return false;

            var couldCreate = _fileManagerProxy.CreateFile(fileNameString, cacheBytes, FileCreationType.Overwrite);
            return couldCreate;

        }

        public async Task<bool> CacheItemAsync(byte[] cacheBytes, string fileNameString)
        {

            if (cacheBytes.Length == 0 || string.IsNullOrEmpty(fileNameString))
                return false;

            fileNameString = ConvertFileNameToBase64(fileNameString);
            bool shouldCacheItem = ShouldCacheItem(fileNameString);
            if (shouldCacheItem == false)
            {

                _cacheSemaphore.Release();
                return false;

            }

            await _cacheSemaphore.WaitAsync();
            var couldCreate = await _fileManagerProxy.CreateFileAsync(fileNameString, cacheBytes,
                                                                      FileCreationType.Overwrite);
            _cacheSemaphore.Release();
            return couldCreate;

        }

        public byte[] RetieveItem(string fileNameString, bool shouldAlwaysRetrieve = false)
        {

            if (string.IsNullOrEmpty(fileNameString))
                return null;

            fileNameString = ConvertFileNameToBase64(fileNameString);
            bool shouldRetrieveItem = ShouldRetrieveItem(fileNameString, (shouldAlwaysRetrieve == false));
            if (shouldRetrieveItem == false)
                return null;

            var cachedBytesArray = _fileManagerProxy.GetContents(fileNameString);
            return cachedBytesArray;

        }

        public async Task<byte[]> RetieveItemAsync(string fileNameString, bool shouldAlwaysRetrieve = false)
        {

            if (string.IsNullOrEmpty(fileNameString))
                return null;

            fileNameString = ConvertFileNameToBase64(fileNameString);
            bool shouldRetrieveItem = ShouldRetrieveItem(fileNameString, (shouldAlwaysRetrieve == false));
            if (shouldRetrieveItem == false)
                return null;

            await _cacheSemaphore.WaitAsync();
            var cachedBytesArray = await _fileManagerProxy.GetContentsAsync(fileNameString);
            _cacheSemaphore.Release();
            return cachedBytesArray;

        }

        public bool DeleteItem(string fileNameString)
        {

            if (string.IsNullOrEmpty(fileNameString))
                return false;

            fileNameString = ConvertFileNameToBase64(fileNameString);
            bool shouldDeleteItem = ShouldDeleteItem(fileNameString);
            if (shouldDeleteItem == false)
                return false;

            bool couldDelete = _fileManagerProxy.RemoveFile(fileNameString);
            return couldDelete;

        }

        public async Task<bool> DeleteItemAsync(string fileNameString)
        {

            if (string.IsNullOrEmpty(fileNameString))
                return false;

            fileNameString = ConvertFileNameToBase64(fileNameString);
            bool shouldDeleteItem = ShouldDeleteItem(fileNameString);
            if (shouldDeleteItem == false)
                return false;

            await _cacheSemaphore.WaitAsync();
            var couldDelete = await _fileManagerProxy.RemoveFileAsync(fileNameString);
            _cacheSemaphore.Release();
            return couldDelete;

        }
		#endregion

	}
}
