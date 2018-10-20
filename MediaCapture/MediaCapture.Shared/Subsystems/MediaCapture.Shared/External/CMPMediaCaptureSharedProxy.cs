using System;
using System.Threading.Tasks;
using Autofac;
using Subsystems.MediaCapture.Shared.Internal;
using Subsystems.MediaCapture.Internal;

namespace Subsystems.MediaCapture.Shared.External
{
    public class CMPMediaCaptureSharedProxy
    {

        private IContainer _container;
        private IMediaCapture _mediaCapture;

        private IMediaCapture Resolve()
        {

            if (_mediaCapture != null)
                return _mediaCapture;

            using (var scope = _container?.BeginLifetimeScope())
            {

                _mediaCapture = _container.Resolve<IMediaCapture>();
                return _mediaCapture;

            }
        }

        public const string KFinishedPickingMedia = "OnFinishedPickingMedia";

        public CMPMediaCaptureSharedProxy(object context = null)
        {

            var builder = new ContainerBuilder();
            builder.Register((componentContext) =>
            {

                return (new CMPMediaCapture(context));

            }).As<IMediaCapture>();
            _container = builder.Build();

        }

        public async Task<byte[]> CaptureMediaAsync()
        {

            var mediaCapture = Resolve();
            var mediaBytesArray = await mediaCapture?.CaptureMediaAsync();
            return mediaBytesArray;

        }

        public async Task<byte[]> ChooseMediaAsync()
        {

            var mediaCapture = Resolve();
            var mediaBytesArray = await mediaCapture?.ChooseMediaAsync();
            return mediaBytesArray;

        }
    }
}
