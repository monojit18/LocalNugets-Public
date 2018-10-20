using System;

namespace Subsystems.MediaCapture.Shared.External
{
    public class CMPMediaEventArgs : EventArgs
    {

        public byte[] ImageBytes { get; private set; }

        public CMPMediaEventArgs(byte[] imageBytesArray)
        {

            if ((imageBytesArray == null) || (imageBytesArray.Length == 0))
                return;

            Array.Copy(imageBytesArray, ImageBytes, imageBytesArray.Length);

        }

    }
}
