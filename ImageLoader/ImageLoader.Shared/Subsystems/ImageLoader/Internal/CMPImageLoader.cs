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
using System.Threading.Tasks;
using Subsystems.Cache.External;
using Subsystems.HttpConnection.External;

namespace Subsystems.ImageLoader.Internal
{
    public class CMPImageLoader
    {

        private readonly string _imageURLString;
        private readonly CMPCacheProxy _cacheProxy;
        private readonly CMPHttpConnectionProxy _httpConnectionProxy;

        public CMPCacheProxy CacheProxy
        {

            get => _cacheProxy;

        }

        public CMPImageLoader(string imageURLString, CMPCacheProxy cacheProxy)
		{

            if (string.IsNullOrEmpty(imageURLString) == true)
                return;

            _cacheProxy = cacheProxy;
			_httpConnectionProxy = new CMPHttpConnectionProxy();
            _imageURLString = string.Copy(imageURLString);

		}

		public async Task<byte[]> LoadImageAsync()
		{

			if (string.IsNullOrEmpty(_imageURLString) == true)
                return null;

			var cachedBytes = await _cacheProxy?.RetieveItemAsync(_imageURLString);
            if (cachedBytes != null)
                return cachedBytes;

			var responseBytes = await _httpConnectionProxy?.GetContentsFromURL(_imageURLString);
			if (responseBytes == null)
				return null;

            var cacheItem = await _cacheProxy?.CacheItemAsync(responseBytes, _imageURLString);
            return ((cacheItem == true) ? responseBytes : null);

		}


	}
}
