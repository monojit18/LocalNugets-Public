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

using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Subsystems.HttpConnection.Internal;

namespace Subsystems.HttpConnection.External
{
    
	public delegate void ProgressCallback(CMPHttpResponse response, int progressBytes,
                                          int totalBytes);

    public class CMPHttpConnectionProxy
    {
        
        #region Private Variables
        private CMPHttpConnection _httpConnection;
        #endregion

        #region Public Methods
        public CMPHttpConnectionProxy(HttpClientHandler httpClientHandler = null,
                                      int chunkBufferSize = 2048)
        {

            _httpConnection = new CMPHttpConnection(httpClientHandler, chunkBufferSize);

        }

        public CMPHttpConnectionProxy URL(string urlString)
        {

            if (_httpConnection == null)
                return null;

            _httpConnection.URL(urlString);
            return this;

        }

        public CMPHttpConnectionProxy Headers(string headerString, string valueString)
        {

            if (_httpConnection == null)
                return null;

            _httpConnection.Headers(headerString, valueString);
            return this;

        }

        public CMPHttpConnectionProxy JsonBody(string keyString, string valueString)
        {

            if (_httpConnection == null)
                return null;

            _httpConnection.JsonBody(keyString, valueString);
            return this;

        }

        public CMPHttpConnectionProxy JsonDocumentBody(string documentBodyString)
        {

            if (_httpConnection == null)
                return null;

            _httpConnection.JsonDocumentBody(documentBodyString);
            return this;

        }

        public CMPHttpConnectionProxy UrlEncodedBody(string keyString, string valueString)
        {

            if (_httpConnection == null)
                return null;

            _httpConnection.UrlEncodedBody(keyString, valueString);
            return this;

        }

        public CMPHttpConnectionProxy ByteArrayBody(byte[] byteBodyArray)
        {

            if (_httpConnection == null)
                return null;

            _httpConnection.ByteArrayBody(byteBodyArray);
            return this;

        }

        public CMPHttpConnectionProxy MultipartByteBody(string contentNameString, byte[] httpContentBytes,
                                                        string fileNameString)
        {

            if (_httpConnection == null)
                return null;

            _httpConnection.MultipartByteBody(contentNameString, httpContentBytes, fileNameString);
            return this;

        }

        public CMPHttpConnectionProxy MultipartFormStreamBody(string contentNameString, Stream httpContentStream,
                                                              string fileNameString)
        {

            if (_httpConnection == null)
                return null;

            _httpConnection.MultipartFormStreamBody(contentNameString, httpContentStream, fileNameString);
            return this;

        }

        public CMPHttpConnectionProxy MultipartFormStringBody(string contentNameString, string httpContentString)
        {

            if (_httpConnection == null)
                return null;

            _httpConnection.MultipartFormStringBody(contentNameString, httpContentString);
            return this;

        }

        public void Build()
        {

            if (_httpConnection == null)
                return;

            _httpConnection.Build();

        }

        public async Task<byte[]> GetContentsFromURL(string urlString)
        {

            if (_httpConnection == null)
                return null;

            return (await _httpConnection.GetContentsFromURL(urlString));

        }

        public async Task<CMPHttpResponse> GetAsync()
        {

            if (_httpConnection == null)
                return null;

            return (await _httpConnection.GetAsync());

        }

        public async Task GetBytesWithProgressAsync(ProgressCallback callback)
        {

            if (_httpConnection == null)
                return;

            await _httpConnection.GetBytesWithProgressAsync(callback);

        }

        public async Task<CMPHttpResponse> PostAsync()
        {

            if (_httpConnection == null)
                return null;

            return (await _httpConnection.PostAsync());

        }

        public async Task PostBytesWithProgressAsync(byte[] byteContentsArray, ProgressCallback callback)
        {

            if (_httpConnection == null)
                return;

            await _httpConnection.PostBytesWithProgressAsync(byteContentsArray, callback);

        }

        public async Task<CMPHttpResponse> PutAsync()
        {

            if (_httpConnection == null)
                return null;

            return (await _httpConnection.PutAsync());

        }

        public async Task<CMPHttpResponse> DeleteAsync()
        {

            if (_httpConnection == null)
                return null;

            return (await _httpConnection.DeleteAsync());

        }

        public void Cancel()
        {

            if (_httpConnection != null)
                _httpConnection.Cancel();

        }
        #endregion

    }
}
