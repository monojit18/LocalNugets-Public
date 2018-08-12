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

using System.Threading.Tasks;
using Subsystems.Cache.Internal;

namespace Subsystems.Cache.External
{
	public class CMPCacheProxy
	{

        #region Private Variables
        private readonly CMPCache _cache;
        #endregion

        #region Public Methods
        public CMPCacheProxy(string cacheFolderPathString, long sizeLimitInKB)
        {

            _cache = new CMPCache(cacheFolderPathString, sizeLimitInKB);

        }

        public CMPCacheProxy ExpiryDays(int days)
        {

            _cache.ExpiryDays(days);
            return this;

        }

        public CMPCacheProxy ExpiryMonths(int months)
        {

            _cache.ExpiryMonths(months);
            return this;

        }

        public CMPCacheProxy ExpiryYears(int years)
        {

            _cache.ExpiryYears(years);
            return this;

        }

        public CMPCacheProxy ExpiryHours(int hours)
        {

            _cache.ExpiryHours(hours);
            return this;

        }

        public CMPCacheProxy ExpiryMinutes(int minutes)
        {

            _cache.ExpiryMinutes(minutes);
            return this;

        }

        public CMPCacheProxy ExpirySeconds(int seconds)
        {

            _cache.ExpirySeconds(seconds);
            return this;

        }

        public bool CacheItem(byte[] cacheBytes, string fileNameString)
        {

            bool couldCache = _cache.CacheItem(cacheBytes, fileNameString);
            return couldCache;

        }

        public async Task<bool> CacheItemAsync(byte[] cacheBytes, string fileNameString)
        {

            Task<bool> cacheItemTask = _cache.CacheItemAsync(cacheBytes, fileNameString);
            await cacheItemTask;
            return cacheItemTask.Result;

        }

        public byte[] RetieveItem(string fileNameString, bool shouldAlwaysRetrieve = false)
        {

            if (string.IsNullOrEmpty(fileNameString))
                return null;

            byte[] retrievedBytes = _cache.RetieveItem(fileNameString, shouldAlwaysRetrieve);
            return retrievedBytes;

        }

        public async Task<byte[]> RetieveItemAsync(string fileNameString, bool shouldAlwaysRetrieve = false)
        {

            if (string.IsNullOrEmpty(fileNameString))
                return null;

            Task<byte[]> retrieveItemTask = _cache.RetieveItemAsync(fileNameString, shouldAlwaysRetrieve);
            await retrieveItemTask;
            return retrieveItemTask.Result;

        }

        public bool DeleteItem(string fileNameString)
        {

            if (string.IsNullOrEmpty(fileNameString))
                return false;

            bool couldDelete = _cache.DeleteItem(fileNameString);
            return couldDelete;

        }

        public async Task<bool> DeleteItemAsync(string fileNameString)
        {

            if (string.IsNullOrEmpty(fileNameString))
                return false;

            Task<bool> deleteItemTask = _cache.DeleteItemAsync(fileNameString);
            await deleteItemTask;
            return deleteItemTask.Result;

        }
		#endregion

	}
}
