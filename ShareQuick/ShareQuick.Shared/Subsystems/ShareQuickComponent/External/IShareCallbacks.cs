using System;
using Xamarin.Auth;

namespace Subsystems.ShareQuickComponent.External
{
    public interface IShareAuthenticationCallbacks
    {

        void PerformAuthentication(WebRedirectAuthenticator authenticator);

    }
}
