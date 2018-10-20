using System;
using System.Threading;
using System.Threading.Tasks;

namespace Subsystems.MediaCapture.Shared.Internal
{
    public interface IMediaCapture
    {

        Task<byte[]> CaptureMediaAsync();
        Task<byte[]> ChooseMediaAsync();


    }
}
