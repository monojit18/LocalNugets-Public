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
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Android.Graphics;
using Android.App;
using ZXing.Mobile;
using ZXing.Common;

namespace Subsystems.CustomQRcodeDroid.External
{
    
    public class CMPZXingQR : IDisposable
    {

        #region Private/Protected Variables
        private BarcodeWriter _qrCodeWriter;
        private readonly MobileBarcodeScanner _qrScanner;
        private readonly MobileBarcodeScanningOptions _qrScannerOptions;
        private readonly Application _application;
        #endregion

        #region Public Methods
        public CMPZXingQR(Application application)
        {

            _application = application;

            _qrCodeWriter = new BarcodeWriter()
            {
                Format = ZXing.BarcodeFormat.QR_CODE
            };

            _qrScanner = new MobileBarcodeScanner();
            MobileBarcodeScanner.Initialize(application);

            _qrScannerOptions = new MobileBarcodeScanningOptions()
            {

                PossibleFormats = new List<ZXing.BarcodeFormat>()
                {
                    ZXing.BarcodeFormat.QR_CODE
                },

                AutoRotate = true,
                TryHarder = true
            };

        }

        public string GenerateQR(string encodeString, int width, int height)
        {

            _qrCodeWriter.Options = new EncodingOptions()
            {
                Width = width,
                Height = height

            };

            var encodedImage = _qrCodeWriter.Write(encodeString);
            byte[] imageBytes = null;
            string qrCodeString = null;

            var imageStream = new MemoryStream();
            var couldCompress = encodedImage.Compress(Bitmap.CompressFormat.Jpeg, 0, imageStream);
            if (couldCompress)
                imageBytes = imageStream.ToArray();

            if (imageBytes != null)
                qrCodeString = Convert.ToBase64String(imageBytes);

            return qrCodeString;

        }

        public async Task<string> RetrieveFromQRAsync()
        {

            var scanTask = _qrScanner.Scan(_qrScannerOptions);
            await scanTask;
            _qrScanner.Cancel();

            if (scanTask.Result == null)
                return string.Empty;

            return scanTask.Result.Text;

        }

        public void Dispose()
        {

            MobileBarcodeScanner.Uninitialize(_application);

        }
        #endregion
    }
}
