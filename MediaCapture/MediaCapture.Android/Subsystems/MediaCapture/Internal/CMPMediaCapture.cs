using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Graphics;
using Android.Content;
using Android.Provider;
using Subsystems.MediaCapture.Shared.Internal;
using Subsystems.MediaCapture.Shared.External;

namespace Subsystems.MediaCapture.Internal
{
    public class CMPMediaCapture : IMediaCapture
    {

        private Activity _hostActivity;
        private TaskCompletionSource<Bitmap> _taskSource;
        private CMPMediaBroadcastReceiver _mediaReceiver;

        private async Task<byte[]> SelectMediaAsync(string actionString)
        {
        
            var cameraIntent = new Intent();
            cameraIntent.SetAction(actionString);
            _hostActivity.StartActivityForResult(cameraIntent, 1);

            byte[] bitmapData = null;
            await Task.Run(() =>
            {

                var bitmap = _taskSource.Task.Result;

                using (var stream = new MemoryStream())
                {
                    bitmap.Compress(Bitmap.CompressFormat.Jpeg, 0, stream);
                    bitmapData = stream.ToArray();
                }

            });

            return bitmapData;

        }

        public CMPMediaCapture(object context)
        {

            _hostActivity = context as Activity;
            _taskSource = new TaskCompletionSource<Bitmap>();

            _mediaReceiver = new CMPMediaBroadcastReceiver();
            _mediaReceiver.FinishedPickingMedia += (object sender,
                                                    CMPMediaEventArgs
                                                    eventArgs) => 
            {

                var imageBytesArray = eventArgs.ImageBytes;
                if ((imageBytesArray == null) || (imageBytesArray.Length == 0))
                    return;

                var bitmap = BitmapFactory.DecodeByteArray(imageBytesArray, 0,
                                                           imageBytesArray.Length);
                _taskSource.SetResult(bitmap);

            };

        }

        public async Task<byte[]> CaptureMediaAsync()
        {

            var mediaBytesArray = await SelectMediaAsync(MediaStore
                                                         .ActionImageCapture);
            return mediaBytesArray;

        }

        public async Task<byte[]> ChooseMediaAsync()
        {

            var mediaBytesArray = await SelectMediaAsync(Intent
                                                         .ActionPick);
            return mediaBytesArray;

        }
    }
}
