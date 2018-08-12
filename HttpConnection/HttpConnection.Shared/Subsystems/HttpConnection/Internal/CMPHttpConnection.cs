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
using Diagnostics = System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using Subsystems.HttpConnection.External;

namespace Subsystems.HttpConnection.Internal
{

    public class CMPHttpConnection : IDisposable
    {

        #region Private Variables

        private const string KDefaultContentNameString = "source";
        private enum ContentTypeEnum
        {
            eApplicationJson,
            eApplicationDocumentJson,
            eXXXUrlEncoded,
            eByteArray,
            eMultipartFormData
        };
        private readonly HttpClient _httpClient;
        private HttpClientHandler _httpClientHandler;
        private HttpRequestMessage _requestMessage;
        private CancellationTokenSource _tokenSource;
        private string _httpURLString;
        private Dictionary<string, string> _headersDictionary;
        private Dictionary<string, string> _jsonBodyDictionary;
        private string _documentBodyString;
        private byte[] _byteBodyArray;
        private Dictionary<string, Tuple<HttpContent, string>> _multipartBodyDictionary;
        private ContentTypeEnum _enContentType;
        private int _chunkBufferSize;
        #endregion

        #region Private/Protected Methods
        private ContentTypeEnum ContentType
        {

            get
            {
                return _enContentType;
            }
            set
            {
                _enContentType = value;
            }

        }

        private void SetContentMessage()
        {

            switch (ContentType)
            {
                case ContentTypeEnum.eXXXUrlEncoded:
                    {
                        _requestMessage.Content = new FormUrlEncodedContent(_jsonBodyDictionary);
                    }
                    break;
                case ContentTypeEnum.eApplicationJson:
                    {

                        string bodyString = JsonConvert.SerializeObject(_jsonBodyDictionary);
                        _requestMessage.Content = new StringContent(bodyString, System.Text.Encoding.UTF8,
                                                                                    "application/json");
                    }
                    break;
                case ContentTypeEnum.eApplicationDocumentJson:
                    {
                        
                        _requestMessage.Content = new StringContent(_documentBodyString, System.Text.Encoding.UTF8,
                                                                                    "application/json");
                    }
                    break;
                case ContentTypeEnum.eByteArray:
                    {

                        var byteArrayContent = new ByteArrayContent(_byteBodyArray);
                        byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                        _requestMessage.Content = byteArrayContent;

                    }
                    break;
                case ContentTypeEnum.eMultipartFormData:
                    {

                        var multiPartFormDataContent = new MultipartFormDataContent();
                        if ((_multipartBodyDictionary == null) || (_multipartBodyDictionary.Count == 0))
                            return;
                        
                        foreach(var multiPartContent in _multipartBodyDictionary)
                        {

                            var contentaNameString = multiPartContent.Key;
                            var httpContent = multiPartContent.Value?.Item1;
                            var fileNameString = multiPartContent.Value?.Item2;

                            multiPartFormDataContent.Add(httpContent, contentaNameString, fileNameString);

                        }

                        _requestMessage.Content = multiPartFormDataContent;

                    }
                    break;
            }

        }

        private async Task<CMPHttpResponse> PerformAsync()
        {

            if (_requestMessage == null)
                return null;

            var httpResponseTask = Task.Run(async () =>
            {

                try
                {

                    HttpResponseMessage responseMessage = await _httpClient.SendAsync(_requestMessage, _tokenSource.Token);
                    string responseRead = null;
                    CMPHttpResponse httpResponse = null;
                    if (responseMessage == null)
                        return null;

                    if (responseMessage.StatusCode != HttpStatusCode.OK)
                        return new CMPHttpResponse(responseMessage.ReasonPhrase, responseMessage.StatusCode, 0);

                    responseRead = await responseMessage.Content?.ReadAsStringAsync();
                    httpResponse = new CMPHttpResponse(responseRead, responseMessage.StatusCode, 0);
                    return httpResponse;

                }
                catch (WebException exception)
                {

                    var httpResponse = new CMPHttpResponse(exception.Message, 0, exception.Status);
                    return httpResponse;

                }
                catch (AggregateException exception)
                {

                    var httpResponse = new CMPHttpResponse(exception.Message, 0, 0);
                    return httpResponse;

                }
                catch (Exception exception)
                {

                    var httpResponse = new CMPHttpResponse(exception.Message, 0, 0);
                    return httpResponse;

                }

            });

            await httpResponseTask;
            return httpResponseTask.Result;


        }

        private CMPHttpConnection MultipartFormDataBody(string contentNameString, HttpContent httpContent,
                                                        string fileNameString = null)
        {

            if ((string.IsNullOrEmpty(contentNameString) == true) || (httpContent == null))
                return this;

            ContentType = ContentTypeEnum.eMultipartFormData;

            if (_multipartBodyDictionary == null)
                _multipartBodyDictionary = new Dictionary<string, Tuple<HttpContent, string>>();

            _multipartBodyDictionary.Add(contentNameString, new Tuple<HttpContent, string>(httpContent,
                                                                                           fileNameString));
            return this;

        }

        #endregion

        #region Public Methods
        public CMPHttpConnection(HttpClientHandler httpClientHandler = null, int chunkBufferSize = 2048)
        {

            _httpClientHandler = httpClientHandler;
            if (_httpClientHandler != null)
            {

                _httpClientHandler = httpClientHandler;
                _httpClient = new HttpClient(_httpClientHandler);

            }
            else
                _httpClient = new HttpClient();

            _headersDictionary = null;
            _jsonBodyDictionary = null;
            _chunkBufferSize = chunkBufferSize;

        }

        public CMPHttpConnection URL(string urlString)
        {

            _httpURLString = string.Copy(urlString);
            return this;

        }

        public CMPHttpConnection Headers(string headerString, string valueString)
        {

            if (string.IsNullOrEmpty(headerString) == true)
                return this;

            if (string.IsNullOrEmpty(valueString) == true)
                return this;

            if (_headersDictionary == null)
                _headersDictionary = new Dictionary<string, string>();

            _headersDictionary.Add(headerString, valueString);
            return this;

        }

        public CMPHttpConnection JsonBody(string keyString, string valueString)
        {

            if ((string.IsNullOrEmpty(keyString) == true) || (string.IsNullOrEmpty(valueString) == true))
                return this;
            
            if (_jsonBodyDictionary == null)
                _jsonBodyDictionary = new Dictionary<string, string>();

            _jsonBodyDictionary.Add(keyString, valueString);
            return this;

        }

        public CMPHttpConnection JsonDocumentBody(string documentBodyString)
        {

            if ((string.IsNullOrEmpty(documentBodyString) == true)
                || (string.IsNullOrEmpty(documentBodyString) == true))
                return this;

            ContentType = ContentTypeEnum.eApplicationDocumentJson;
            _documentBodyString = string.Copy(documentBodyString);
            return this;

        }

        public CMPHttpConnection UrlEncodedBody(string keyString, string valueString)
        {

            ContentType = ContentTypeEnum.eXXXUrlEncoded;
            JsonBody(keyString, valueString);
            return this;

        }

        public CMPHttpConnection ByteArrayBody(byte[] byteBodyArray)
        {

            _byteBodyArray = new byte[byteBodyArray.Length];            
            ContentType = ContentTypeEnum.eByteArray;
            Array.Copy(byteBodyArray, _byteBodyArray, byteBodyArray.Length);
            return this;

        }

        public CMPHttpConnection MultipartByteBody(string contentNameString, byte[] httpContentBytes,
                                                   string fileNameString)
        {

            var byteArrayContent = new ByteArrayContent(httpContentBytes);
            MultipartFormDataBody(contentNameString, byteArrayContent, fileNameString);
            return this;

        }

        public CMPHttpConnection MultipartFormStreamBody(string contentNameString, Stream httpContentStream,
                                                         string fileNameString)
        {

            var streamContent = new StreamContent(httpContentStream);
            MultipartFormDataBody(contentNameString, streamContent, fileNameString);
            return this;

        }

        public CMPHttpConnection MultipartFormStringBody(string contentNameString, string httpContentString)
        {

            var stringContent = new StringContent(httpContentString);
            MultipartFormDataBody(contentNameString, stringContent);
            return this;

        }

        public void Build()
        {

            _tokenSource = new CancellationTokenSource();
            _requestMessage = new HttpRequestMessage()
            {
                RequestUri = new System.Uri(_httpURLString)
            };

            if (_headersDictionary != null)
            {

                foreach (KeyValuePair<string, string> header in _headersDictionary)
                    _requestMessage.Headers.Add(header.Key, header.Value);

            }

        }

        public async Task<byte[]> GetContentsFromURL(string urlString)
        {

            try
            {

                Task<byte[]> responseBytesTask = _httpClient.GetByteArrayAsync(urlString);
                await responseBytesTask;
                return responseBytesTask.Result;

            }
            catch (WebException exception)
            {

                Diagnostics.Debug.WriteLine(exception.StackTrace);
                return null;

            }
            catch (AggregateException exception)
            {

                Diagnostics.Debug.WriteLine(exception.StackTrace);
                return null;

            }
            catch (Exception exception)
            {

                Diagnostics.Debug.WriteLine(exception.StackTrace);
                return null;

            }

        }

        public async Task<CMPHttpResponse> GetAsync()
        {

            if (_requestMessage == null)
                return null;

            _requestMessage.Method = HttpMethod.Get;
            return (await PerformAsync());

        }

        public async Task GetBytesWithProgressAsync(ProgressCallback callback)
        {

            if ((_requestMessage == null) || (callback == null))
                return;

            int noOfBytesRead = 0;
            int totalBytesToRead = 0;

            try
            {

                Task<HttpResponseMessage> httpResponseTask = Task.Run(async () => await _httpClient
                                                                  .GetAsync(_httpURLString,
                                                                            HttpCompletionOption.ResponseHeadersRead,
                                                                            _tokenSource.Token));

                await httpResponseTask;
                HttpResponseMessage responseMessage = httpResponseTask.Result;

                Task<byte[]> responseByteTask = Task.Run(async () => await
                                                         responseMessage.Content.ReadAsByteArrayAsync());
                await responseByteTask;

                Task<Stream> responseStreamTask = Task.Run(async () => await responseMessage.Content.ReadAsStreamAsync());
                await responseStreamTask;

                Stream responseStream = responseStreamTask.Result;
                byte[] responseBytes = responseByteTask.Result;
                totalBytesToRead = responseBytes.Length;

                string responseString = null;
                byte[] buffer = null;
                CMPHttpResponse httpResponse = null;

                while (true)
                {

                    buffer = new byte[_chunkBufferSize];
                    int bytesRead = 0;

                    bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                        break;

                    noOfBytesRead += bytesRead;
                    responseString = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    httpResponse = new CMPHttpResponse(responseString, responseMessage.StatusCode, 0);
                    callback.Invoke(httpResponse, noOfBytesRead, totalBytesToRead);

                }

            }
            catch (WebException ex)
            {

                var httpResponse = new CMPHttpResponse(ex.Message, 0, ex.Status);
                callback.Invoke(httpResponse, noOfBytesRead, totalBytesToRead);

            }
            catch (AggregateException ex)
            {

                var httpResponse = new CMPHttpResponse(ex.Message, 0, 0);
                callback.Invoke(httpResponse, noOfBytesRead, totalBytesToRead);

            }
            catch (Exception ex)
            {

                var httpResponse = new CMPHttpResponse(ex.Message, 0, 0);
                callback.Invoke(httpResponse, noOfBytesRead, totalBytesToRead);

            }

        }

        public async Task<CMPHttpResponse> PostAsync()
        {

            if (_requestMessage == null)
                return null;

            _requestMessage.Method = HttpMethod.Post;
            SetContentMessage();

            return (await PerformAsync());

        }

        public async Task<CMPHttpResponse> PutAsync()
        {

            if (_requestMessage == null)
                return null;

            _requestMessage.Method = HttpMethod.Put;
            SetContentMessage();

            return (await PerformAsync());

        }

        public async Task<CMPHttpResponse> DeleteAsync()
        {

            if (_requestMessage == null)
                return null;

            _requestMessage.Method = HttpMethod.Delete;
            SetContentMessage();

            return (await PerformAsync());

        }

        public async Task PostBytesWithProgressAsync(byte[] byteContentsArray, ProgressCallback callback)
        {

            if ((_requestMessage == null) || (callback == null))
                return;

            if (ContentType != ContentTypeEnum.eMultipartFormData)
                return;

            if ((byteContentsArray == null) || (byteContentsArray.Length == 0))
                return;

            _requestMessage.Method = HttpMethod.Post;
            byte[] writeBuffer = new byte[_chunkBufferSize];
            using (Stream inputStream = new MemoryStream(byteContentsArray))
            {

                using (Stream outputStream = new MemoryStream())
                {

                    Task<int> readCountTask = null;
                    do
                    {

                        readCountTask = inputStream.ReadAsync(writeBuffer, 0, _chunkBufferSize);
                        await readCountTask;

                        Task writeTask = outputStream.WriteAsync(writeBuffer, 0, _chunkBufferSize);
                        await writeTask;

                        var multiPartContent = new MultipartFormDataContent();
                        multiPartContent.Add(new ByteArrayContent(writeBuffer), KDefaultContentNameString);

                        _requestMessage.Content = multiPartContent;

                        Task<CMPHttpResponse> responseTask = PerformAsync();
                        await responseTask;
                        callback.Invoke(responseTask.Result, _chunkBufferSize, byteContentsArray.Length);

                    } while (readCountTask.Result > 0);
                }
            }
        }

        public void Cancel()
        {

            if (_tokenSource == null)
                return;

            _tokenSource.Cancel();

        }

        public void Dispose()
        {

            if (_httpClient == null)
                return;

            _httpClient.Dispose();

        }
        #endregion
    }
}
