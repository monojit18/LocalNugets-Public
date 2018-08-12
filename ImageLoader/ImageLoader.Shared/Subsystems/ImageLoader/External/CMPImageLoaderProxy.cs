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
using System.Threading.Tasks;
using Subsystems.ImageLoader.Internal;
using Subsystems.Cache.External;

namespace Subsystems.ImageLoader.External
{
	public class CMPImageLoaderProxy
	{

        private readonly CMPImageLoader _imageLoader;

        public CMPImageLoaderProxy(string imageURLString, long cacheSize = 5 * 1024)
		{

            var baseFolderPathString = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var cacheProxy = new CMPCacheProxy(baseFolderPathString, cacheSize);
            cacheProxy.ExpiryDays(7);

            _imageLoader = new CMPImageLoader(imageURLString, cacheProxy);

		}

        public CMPImageLoaderProxy(string imageURLString, CMPCacheProxy cacheProxy)
        {
        
            _imageLoader = new CMPImageLoader(imageURLString, cacheProxy);

        }

        public CMPImageLoaderProxy ExpiryDays(int days)
        {

            _imageLoader.CacheProxy.ExpiryDays(days);
            return this;

        }

        public CMPImageLoaderProxy ExpiryMonths(int months)
        {

            _imageLoader.CacheProxy.ExpiryMonths(months);
            return this;

        }

        public CMPImageLoaderProxy ExpiryYears(int years)
        {

            _imageLoader.CacheProxy.ExpiryYears(years);
            return this;

        }

        public CMPImageLoaderProxy ExpiryHours(int hours)
        {

            _imageLoader.CacheProxy.ExpiryHours(hours);
            return this;

        }

        public CMPImageLoaderProxy ExpiryMinutes(int minutes)
        {

            _imageLoader.CacheProxy.ExpiryMinutes(minutes);
            return this;

        }

        public CMPImageLoaderProxy ExpirySeconds(int seconds)
        {

            _imageLoader.CacheProxy.ExpirySeconds(seconds);
            return this;

        }

		public async Task<byte[]> LoadImageAsync()
		{

			if (_imageLoader == null)
				return null;

			var imageBytes = await _imageLoader.LoadImageAsync();
			return imageBytes;

		}
	}
}
