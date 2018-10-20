using System;
using System.Threading;
using Android;
using Android.Graphics;
using Android.Content;
using Android.Net;
using Subsystems.MediaCapture.Shared.External;

namespace Subsystems.MediaCapture.Internal
{

    [BroadcastReceiver(Enabled = true, Exported = false)]
    public class CMPMediaBroadcastReceiver : BroadcastReceiver
    {

        public event EventHandler<CMPMediaEventArgs> FinishedPickingMedia;

        public CMPMediaBroadcastReceiver()
        {
        }

        public override void OnReceive(Context context, Intent intent)
        {

            var imageBytesArray = intent
                                    .GetByteArrayExtra(CMPMediaCaptureSharedProxy
                                                    .KFinishedPickingMedia);

            FinishedPickingMedia?.Invoke(this,
                                         new CMPMediaEventArgs(imageBytesArray));

        }
    }

}
