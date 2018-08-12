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
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Threading.Tasks;
using UIKit;
using Foundation;
using ZXing.Mobile;
using ZXing.Common;

namespace Subsystems.CustomQRcodeiOS.External
{

    public class CMPZXingQR
    {

        #region Private/Protected Variables
        private BarcodeWriter _qrCodeWriter;
        private readonly MobileBarcodeScanner _qrScanner;
        private readonly MobileBarcodeScanningOptions _qrScannerOptions;
        #endregion

        #region Public Methods
        public CMPZXingQR()
        {

            _qrCodeWriter = new BarcodeWriter()
            {
                Format = ZXing.BarcodeFormat.QR_CODE
            };

            _qrScanner = new MobileBarcodeScanner();
            _qrScannerOptions = new MobileBarcodeScanningOptions()
            {

                PossibleFormats = new List<ZXing.BarcodeFormat>()
                {
                    ZXing.BarcodeFormat.QR_CODE
                },
                AutoRotate = true
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
            byte[] imageBytesArray = null;
            string qrCodeString = null;

            NSData imageData = encodedImage.AsJPEG();
            imageBytesArray = new byte[imageData.Length];
            Marshal.Copy(imageData.Bytes, imageBytesArray, 0, Convert.ToInt32(imageData.Length));

            if (imageBytesArray != null)
                qrCodeString = Convert.ToBase64String(imageBytesArray);

            return qrCodeString;

        }

        public async Task<string> RetrieveFromQRAsync()
        {

            var scanTask = _qrScanner.Scan(_qrScannerOptions);
            await scanTask;
            if (scanTask.Result == null)
                return string.Empty;

            // _qrScanner.Cancel();
            return scanTask.Result.Text;

        }
        #endregion
    }
}
