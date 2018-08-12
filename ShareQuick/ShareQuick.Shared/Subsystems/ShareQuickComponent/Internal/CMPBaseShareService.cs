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
using Diagonistics = System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Auth;
using Subsystems.ShareQuickComponent.External;
using Subsystems.HttpConnection.External;

namespace Subsystems.ShareQuickComponent.Internal
{
    public abstract class CMPBaseShareService
    {

        protected const string KHttpMethodGETString = "GET";
        protected const string KHttpMethodPOSTString = "POST";
        protected string _customerKeyString;
        protected string _customerSecretKeyString;
        protected Account _authenticatedAccount;
        protected CancellationTokenSource _cancellationTokenSource;
        protected CMPHttpConnectionProxy _httpConnectionProxy;

        protected abstract void SaveTokensToKeyStore();

        protected async Task<Tuple<string, CMPShareError>> PerformGraphAsync(Request graphRequest)
        {

            if (graphRequest == null)
                return (new Tuple<string, CMPShareError>(null, null));

            var graphResponseTask = Task.Run(async () =>
            {

                try
                {

                    var graphResponse = await graphRequest.GetResponseAsync(_cancellationTokenSource.Token);
                    var responseTextString = await graphResponse.GetResponseTextAsync();

                    if (graphResponse.StatusCode != System.Net.HttpStatusCode.OK)
                    {

                        var responseError = PrepareGraphErrorFromResponse(graphResponse.StatusCode, responseTextString);
                        return (new Tuple<string, CMPShareError>(null, responseError));
                    }

                    return (new Tuple<string, CMPShareError>(responseTextString, null));

                }
                catch (AggregateException exception)
                {

                    Diagonistics.Debug.WriteLine(exception.StackTrace);
                    var responseError = PrepareGraphErrorFromException(exception);
                    return (new Tuple<string, CMPShareError>(null, responseError));

                }
                catch (Exception exception)
                {

                    Diagonistics.Debug.WriteLine(exception.StackTrace);
                    var responseError = PrepareGraphErrorFromException(exception);
                    return (new Tuple<string, CMPShareError>(null, responseError));

                }

            });

            await graphResponseTask;
            return graphResponseTask.Result;

        }

        protected CMPShareError PrepareGraphErrorFromResponse(HttpStatusCode statusCode, string responseTextString)
        {

            var responseError = new CMPShareError()
            {

                ErrorCode = (int)statusCode,
                ErrorMessageString = responseTextString,
                UserMessageString = responseTextString

            };

            return responseError;

        }

        protected CMPShareError PrepareGraphErrorFromException(Exception exception)
        {

            var responseError = new CMPShareError()
            {

                ErrorCode = 0,
                ErrorMessageString = exception?.Message,
                UserMessageString = exception?.InnerException?.Message

            };

            return responseError;

        }

        public CMPBaseShareService(string customerKeyString, string customerSecretKeyString)
        {

            _customerKeyString = string.Copy(customerKeyString);
            _customerSecretKeyString = string.Copy(customerSecretKeyString);
            _httpConnectionProxy = new CMPHttpConnectionProxy();

        }

    }

}
