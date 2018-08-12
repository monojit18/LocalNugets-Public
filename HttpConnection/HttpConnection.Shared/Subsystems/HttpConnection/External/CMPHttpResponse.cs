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
using System.Net;
using System.Net.Http;

namespace Subsystems.HttpConnection.External
{
    public class CMPHttpResponse
    {

        public string _reponseString;
        public string ResponseString
        {
            get
            {
                return _reponseString;
            }

        }

        public HttpStatusCode _statusCode;
        public HttpStatusCode StatusCode
        {
            get
            {
                return _statusCode;
            }

        }

        public WebExceptionStatus _exceptionStatus;
        public WebExceptionStatus ExceptionStatus
        {
            get
            {
                return _exceptionStatus;
            }

        }

        public CMPHttpResponse(string responseString, HttpStatusCode statusCode,
                               WebExceptionStatus exceptionStatus)
        {

            _reponseString = string.Copy(responseString);
            _statusCode = statusCode;
            _exceptionStatus = exceptionStatus;

        }

    }
}
