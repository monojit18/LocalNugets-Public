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
    public class CMPHttpWebProxy : IWebProxy
    {

        #region Private Variables(CMPHttpWebProxy)
        private string _addressString;
        private string _userNameString;
        private string _passwordString;
        private int _portNumber;
        private string _proxyURIString;
        private NetworkCredential _networkCredential;
        private WebProxy _proxy;
        #endregion

        #region Public Methods(CMPHttpWebProxy)
        public CMPHttpWebProxy() { }

        public CMPHttpWebProxy Address(string proxyAddressString)
        {

            _addressString = string.Copy(proxyAddressString);
            return this;

        }

        public CMPHttpWebProxy UserName(string proxyUserNameString)
        {

            _userNameString = string.Copy(proxyUserNameString);
            return this;

        }

        public CMPHttpWebProxy Password(string proxyPasswordString)
        {

            _passwordString = string.Copy(proxyPasswordString);
            return this;

        }

        public CMPHttpWebProxy Port(int portNumber)
        {

            _portNumber = portNumber;
            return this;

        }

        public CMPHttpWebProxy Build()
        {

            _networkCredential = new NetworkCredential(_userNameString, _passwordString);
            _proxyURIString = string.Format("{0}:{1}", _addressString, _portNumber.ToString());
            _proxy = new WebProxy(_proxyURIString, false)
            {

                Credentials = _networkCredential,
                UseDefaultCredentials = false

            };

            return this;

        }

        public ICredentials Credentials
        {

            get => _networkCredential;
            set
            {

                _networkCredential = value as NetworkCredential;

            }

        }


        public Uri GetProxy(Uri destination)
        {

            return (new Uri(_proxyURIString));

        }

        public bool IsBypassed(Uri host)
        {

            return _proxy.IsBypassed(host);

        }
        #endregion


    }
}
