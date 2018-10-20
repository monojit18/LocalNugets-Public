using System;
using System.Threading.Tasks;
using UIKit;
using Foundation;
using Subsystems.MediaCapture.Shared.Internal;

namespace Subsystems.MediaCapture.Internal

{
    public class CMPMediaCapture : IMediaCapture
    {

        private const string KPublicImage = "public.image"; 
        private UIImagePickerController _imagePicker;
        private TaskCompletionSource<byte[]> _imageTask;

        private async Task<byte[]> SelectMediaAsync(UIImagePickerControllerSourceType
                                                    sourceType)
        {

            _imageTask = new TaskCompletionSource<byte[]>();
            _imagePicker = new UIImagePickerController();
            _imagePicker.PrefersStatusBarHidden();
            _imagePicker.SourceType = sourceType;
            _imagePicker.FinishedPickingMedia += OnCaptureComplete;
            _imagePicker.Canceled += OnCaptureCancelled;

            StartCapture();

            var imageBytes = await _imageTask.Task;
            return imageBytes;

        }

        private void CancelCapture()
        {

            var captureViewController = UIApplication.SharedApplication
                                                     .KeyWindow
                                                     .RootViewController;
            captureViewController.DismissViewController(false, () => { });

        }

        private void StartCapture()
        {

            var captureViewController = UIApplication.SharedApplication
                                                     .KeyWindow
                                                     .RootViewController;
            captureViewController.PresentViewController(_imagePicker,
                                                        true, () => { });

        }

        public CMPMediaCapture(object context = null) {}

        public async Task<byte[]> CaptureMediaAsync()
        {

            var imageBytesArray = await
                SelectMediaAsync(UIImagePickerControllerSourceType
                                 .Camera);
            return imageBytesArray;

        }

        public async Task<byte[]> ChooseMediaAsync()
        {

            var imageBytesArray = await
                SelectMediaAsync(UIImagePickerControllerSourceType
                                 .PhotoLibrary);
            return imageBytesArray;

        }

        protected void OnCaptureComplete(object sender,
                                         UIImagePickerMediaPickedEventArgs
                                         eventArgs)
        {

            var mediaTypeString = eventArgs.Info[UIImagePickerController
                                                 .MediaType].ToString();
            if (string.Compare(mediaTypeString, KPublicImage, true) == -1)
                return;

            var mediaURL = eventArgs
                            .Info[new NSString("UIImagePickerControllerReferenceUrl")]
                            as NSUrl;

            if (mediaURL != null)
                Console.WriteLine("Url:" + mediaURL.ToString());

            UIImage originalImage = eventArgs
                                    .Info[UIImagePickerController
                                          .OriginalImage]
                                    as UIImage;
            if (originalImage != null)
            {

                var imageBytes = originalImage.AsJPEG().ToArray();
                _imageTask.SetResult(imageBytes);

            }

            CancelCapture();

        }

        void OnCaptureCancelled(object sender, EventArgs e)
        {

            CancelCapture();

        }
    }
}
