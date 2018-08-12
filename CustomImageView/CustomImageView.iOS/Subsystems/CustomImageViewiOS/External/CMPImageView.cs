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
using UIKit;
using Foundation;
using Subsystems.ImageLoader.External;

namespace Subsystems.CustomImageViewiOS.External
{
    public class CMPImageView : IDisposable
    {

        private CMPImageLoaderProxy _imageLoaderProxy;
        private UIImageView _imageView;

        public CMPImageView(UIImageView imageView, string imageURLString)
        {
            
            _imageLoaderProxy = new CMPImageLoaderProxy(imageURLString);
            _imageLoaderProxy.ExpiryDays(7);
            _imageView = imageView;

        }

        public CMPImageView(UIImageView imageView, CMPImageLoaderProxy imageLoaderProxy)
        {
            
            _imageLoaderProxy = imageLoaderProxy;
            _imageView = imageView;

        }

        public CMPImageView ExpiryDays(int days)
        {

            _imageLoaderProxy.ExpiryDays(days);
            return this;

        }

        public CMPImageView ExpiryMonths(int months)
        {

            _imageLoaderProxy.ExpiryMonths(months);
            return this;

        }

        public CMPImageView ExpiryYears(int years)
        {

            _imageLoaderProxy.ExpiryYears(years);
            return this;

        }

        public CMPImageView ExpiryHours(int hours)
        {

            _imageLoaderProxy.ExpiryHours(hours);
            return this;

        }

        public CMPImageView ExpiryMinutes(int minutes)
        {

            _imageLoaderProxy.ExpiryMinutes(minutes);
            return this;

        }

        public CMPImageView ExpirySeconds(int seconds)
        {

            _imageLoaderProxy.ExpirySeconds(seconds);
            return this;

        }

        public async Task LoadImageAsync()
        {

            var imageBytesArray = await _imageLoaderProxy.LoadImageAsync();
            if ((imageBytesArray == null) || (imageBytesArray.Length == 0))
                return;

            var imageData = NSData.FromArray(imageBytesArray);
            if (imageData == null)
                return;
            
            _imageView.Image = UIImage.LoadFromData(imageData);

        }

        public void Dispose()
        {

            _imageView.Dispose();

        }
    }
}
