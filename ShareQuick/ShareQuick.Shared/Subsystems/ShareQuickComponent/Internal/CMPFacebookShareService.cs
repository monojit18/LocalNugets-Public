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
using System.IO;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Auth;
using Newtonsoft.Json;
using Subsystems.ShareQuickComponent.External;
using Subsystems.CustomAccountStore.External;
using Subsystems.HttpConnection.External;

namespace Subsystems.ShareQuickComponent.Internal
{
    public class CMPFacebookShareService : CMPBaseShareService
    {

        private const string KAuthenticateUriString = "https://www.facebook.com/v2.12/dialog/oauth";
        private const string KRedirectURLString = "https://codemongers.wordpress.com/";
        private const string KFriendsURLString = "https://graph.facebook.com/v2.12/me/friends";
        private const string KPostDetailsURLString = "https://graph.facebook.com/v2.12/post-id";
        private const string KModifyPostURLString = "https://graph.facebook.com/v2.12/{0}";
        private const string KLikePostURLString = "https://graph.facebook.com/v2.12/{0}/likes";
        private const string KCommentPostURLString = "https://graph.facebook.com/v2.12/{0}/comments";
        private const string KShareStatusURLString = "https://graph.facebook.com/v2.12/me/feed";
        private const string KSharePhotoURLString = "https://graph.facebook.com/v2.12/me/photos";
        private const string KAllAlbumsURLString = "https://graph.facebook.com/v2.12/me/albums";
        private const string KCreateAlbumURLString = "https://graph.facebook.com/v2.12/me/albums";
        private const string KUploadPhotosToAlbumURLString = "https://graph.facebook.com/v2.12/{0}/photos";

        private const string KAccessTokenKeyString = "access_token";
        private const string KStateKeyString = "state";
        private const string KExpiresInKeyString = "expires_in";
        private const string KFBAccessTokenKeyString = "FBAccessToken";
        private const string KFBTokenStateKeyString = "FBTokenState";
        private const string KFBExpiresInKeyString = "FBTokenExpiresIn";
        private const string KFBUserNameKeyString = "FBTokenUserName";
        private const string KSourceKeyString = "source";
        private const string KBearerKeyString = "Bearer";
        private const string KAuthorizationKeyString = "Aauthorization";
        private const string KPNGMimeTypeKeyString = "image/png";
        private const string KUploadPNGFileNameString = "upload.png";


        private OAuth2Authenticator _authenticator;
        private IShareAuthenticationCallbacks _authenticationCallbacks;
        private List<string> _authenticationScopesList;
        private CMPAccountStoreProxy _accountStoreProxy;
        private TaskCompletionSource<Tuple<bool, CMPShareError>> _authenticationTask;
        private string _albumNameString;
        private string _albumCaptionString;
        private string _albumLocationString;
        private Dictionary<string, string> _privacyDictionary;

        private string PrepareScopes()
        {

            var scopesString = _authenticationScopesList?.Aggregate((string arg1, string arg2) => 
            {

                return (string.Concat(arg1, ",", arg2));

            });

            return scopesString;

        }

        private OAuth2Authenticator PrepareAuthenticator()
        {
            
            var authorizeUri = new Uri(KAuthenticateUriString);
            var redirectUri = new Uri(KRedirectURLString);
            var scopesString = PrepareScopes();

            var authenticator = new OAuth2Authenticator(_customerKeyString, scopesString, authorizeUri, redirectUri);
            return authenticator;

        }

        private Account CheckUserAuthentication()
        {

            if (_accountStoreProxy == null)
                return null;
            
            var accessTokenString = _accountStoreProxy.Fetch(KFBAccessTokenKeyString);
            if (string.IsNullOrEmpty(accessTokenString) == true)
                return null;

            var stateString = _accountStoreProxy.Fetch(KFBTokenStateKeyString);
            var expiresInString = _accountStoreProxy.Fetch(KFBExpiresInKeyString);
            var userNameString = _accountStoreProxy.Fetch(KFBUserNameKeyString);

            var accountProperties = new Dictionary<string, string>();
            accountProperties[KAccessTokenKeyString] = accessTokenString;
            accountProperties[KStateKeyString] = stateString;
            accountProperties[KExpiresInKeyString] = expiresInString;

            var userAccount = new Account(userNameString ?? string.Empty, accountProperties);
            return userAccount;

        }

        private void OnAuthenticationCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {

            if (e.IsAuthenticated)
            {

                _authenticatedAccount = e.Account;
                SaveTokensToKeyStore();
                _authenticationTask.TrySetResult(new Tuple<bool, CMPShareError>(true, null));

            }                
            else
                _authenticationTask.TrySetResult(new Tuple<bool, CMPShareError>(false, null));

        }

        private void OnAuthenticationError(object sender, AuthenticatorErrorEventArgs e)
        {

            var authenticationError = new CMPShareError()
            {

                ErrorCode = 0,
                ErrorMessageString = e.Message,
                UserMessageString = e.Exception?.InnerException?.Message

            };

            _authenticationTask.TrySetResult(new Tuple<bool, CMPShareError>(false, authenticationError));

        }

        private void PrepareUploadHttpConnection(string urlString, byte[] imageBytesArray,
                                                 Dictionary<string, string> parametersDictionary)
        {

            if (_httpConnectionProxy == null)
                return;

            if (string.IsNullOrEmpty(urlString) == true)
                return;

            if ((imageBytesArray == null) || (imageBytesArray.Length == 0))
                return;

            var headerString = $"{KBearerKeyString} {_authenticatedAccount.Properties?[KAccessTokenKeyString]}";
            _httpConnectionProxy.URL(urlString)
                                .Headers(KAuthorizationKeyString, headerString)
                                .MultipartByteBody(KSourceKeyString, imageBytesArray, KUploadPNGFileNameString);
            
            if ((parametersDictionary != null) && (parametersDictionary.Count > 0))
            {

                foreach (var keyValuePair in parametersDictionary)
                    _httpConnectionProxy.MultipartFormStringBody(keyValuePair.Key, keyValuePair.Value);

            }

            _httpConnectionProxy.Build();

        }

        protected override void SaveTokensToKeyStore()
        {

            if (_authenticatedAccount == null)
                return;

            if (_authenticatedAccount.Properties?.ContainsKey(KAccessTokenKeyString) == true)
            {

                var accessTokenString = _authenticatedAccount.Properties?[KAccessTokenKeyString];
                _accountStoreProxy.Save(KFBAccessTokenKeyString, accessTokenString);

            }

            if (_authenticatedAccount.Properties?.ContainsKey(KStateKeyString) == true)
            {

                var stateString = _authenticatedAccount.Properties?[KStateKeyString];
                _accountStoreProxy.Save(KFBTokenStateKeyString, stateString);

            }

            if (_authenticatedAccount.Properties?.ContainsKey(KExpiresInKeyString) == true)
            {

                var expiresInString = _authenticatedAccount.Properties?[KExpiresInKeyString];
                _accountStoreProxy.Save(KFBExpiresInKeyString, expiresInString);

            }

            var userNameString = _authenticatedAccount.Username;
            _accountStoreProxy.Save(KFBUserNameKeyString, userNameString);


        }

        public CMPFacebookShareService(string customerKeyString, string customerSecretKeyString,
                                      IShareAuthenticationCallbacks authenticationCallbacks,
                                      IAccountStoreConfiguration accountStoreConfiguration)
            : base(customerKeyString, customerSecretKeyString)
        {

            _authenticationCallbacks = authenticationCallbacks;
            _authenticationScopesList = new List<string>();
            _accountStoreProxy = new CMPAccountStoreProxy(accountStoreConfiguration);
            _cancellationTokenSource = new CancellationTokenSource();

        }

        public CMPBaseShareService AddToScope(string scopeString)
        {

            _authenticationScopesList.Add(scopeString);
            return this;

        }

        public async Task<Tuple<bool, CMPShareError>> AuthenticateUserAsync()
        {

            var userAccount = CheckUserAuthentication();
            if (userAccount != null)
            {

                _authenticatedAccount = userAccount;
                _authenticationCallbacks.PerformAuthentication(null);
                return (new Tuple<bool, CMPShareError>(true, null));

            }

            _authenticationTask = new TaskCompletionSource<Tuple<bool, CMPShareError>>();
            if (_authenticator == null)
            {

                _authenticator = PrepareAuthenticator();
                _authenticator.Completed += OnAuthenticationCompleted;
                _authenticator.Error += OnAuthenticationError;

            }

            _authenticationCallbacks.PerformAuthentication(_authenticator);
            var authenticationResult = await _authenticationTask.Task;
            return authenticationResult;

        }

        public async Task<Tuple<string, CMPShareError>> GetFriendsListAsync()
        {

            var graphRequest = new OAuth2Request(KHttpMethodGETString, new Uri(KFriendsURLString), null,
                                                 _authenticatedAccount);
            var graphResponse = await PerformGraphAsync(graphRequest);
            return graphResponse;

        }

        public async Task<Tuple<string, CMPShareError>> GetPostDetailsAsync(Dictionary<string, string>
                                                                            parametersDictionary)
        {

            if ((parametersDictionary == null) || (parametersDictionary.Count == 0))
                throw (new ArgumentNullException());
            
            var graphRequest = new OAuth2Request(KHttpMethodGETString, new Uri(KPostDetailsURLString), parametersDictionary,
                                                 _authenticatedAccount);
            var graphResponse = await PerformGraphAsync(graphRequest);
            return graphResponse;

        }

        public async Task<Tuple<string, CMPShareError>> PostStatusAsync(Dictionary<string, string>
                                                                        parametersDictionary)
        {

            if ((parametersDictionary == null) || (parametersDictionary.Count == 0))
                throw (new ArgumentNullException());
            
            var graphRequest = new OAuth2Request(KHttpMethodPOSTString, new Uri(KShareStatusURLString),
                                                 parametersDictionary, _authenticatedAccount);
            var graphResponse = await PerformGraphAsync(graphRequest);
            return graphResponse;

        }

        public async Task<Tuple<string, CMPShareError>> PostPhotoWithMessageAsync(byte[] imageBytesArray,
                                                                                  Dictionary<string, string>
                                                                                  parametersDictionary = null)
        {
            
            if (_httpConnectionProxy == null)
                return (new Tuple<string, CMPShareError>(null, null));

            PrepareUploadHttpConnection(KSharePhotoURLString, imageBytesArray, parametersDictionary); 

            var httpResponse = await _httpConnectionProxy.PostAsync();
            if (httpResponse?.StatusCode == HttpStatusCode.OK)
                return (new Tuple<string, CMPShareError>(httpResponse.ResponseString, null));

            var shareError = PrepareGraphErrorFromResponse(httpResponse.StatusCode, httpResponse.ResponseString);                
            return (new Tuple<string, CMPShareError>(null, shareError));

        }

        public async Task<Tuple<string, CMPShareError>> UpdatePostAsync(string postIdString,
                                                                        Dictionary<string, string>
                                                                        parametersDictionary)
        {

            if ((parametersDictionary == null) || (parametersDictionary.Count == 0))
                throw (new ArgumentNullException());

            if (string.IsNullOrEmpty(postIdString) == true)
                throw (new ArgumentNullException());

            var updatePostURLString = string.Format(KModifyPostURLString, postIdString);
            var graphRequest = new OAuth2Request(KHttpMethodPOSTString, new Uri(updatePostURLString), parametersDictionary,
                                                 _authenticatedAccount);
            var graphResponse = await PerformGraphAsync(graphRequest);
            return graphResponse;

        }

        public async Task<Tuple<string, CMPShareError>> LikePostAsync(string postIdString,
                                                                      Dictionary<string, string>
                                                                      parametersDictionary = null)
        {

            if (string.IsNullOrEmpty(postIdString) == true)
                throw (new ArgumentNullException());
            
            var likePostURLString = string.Format(KLikePostURLString, postIdString);
            var graphRequest = new OAuth2Request(KHttpMethodPOSTString, new Uri(likePostURLString), parametersDictionary,
                                                 _authenticatedAccount);
            var graphResponse = await PerformGraphAsync(graphRequest);
            return graphResponse;

        }

        public async Task<Tuple<string, CMPShareError>> CommentOnPostAsync(string postIdString,
                                                                           Dictionary<string, string>
                                                                           parametersDictionary)
        {

            if (string.IsNullOrEmpty(postIdString) == true)
                throw (new ArgumentNullException());
            
            if ((parametersDictionary == null) || (parametersDictionary.Count == 0))
                throw (new ArgumentNullException());

            var commentOnPostURLString = string.Format(KCommentPostURLString, postIdString);
            var graphRequest = new OAuth2Request(KHttpMethodPOSTString, new Uri(commentOnPostURLString), parametersDictionary,
                                                 _authenticatedAccount);
            var graphResponse = await PerformGraphAsync(graphRequest);
            return graphResponse;

        }

        public async Task<Tuple<string, CMPShareError>> RemovePostAsync(string postIdString)
        {

            if (string.IsNullOrEmpty(postIdString) == true)
                throw (new ArgumentNullException());

            var removePostURLString = string.Format(KModifyPostURLString, postIdString);
            var graphRequest = new OAuth2Request("DELETE", new Uri(removePostURLString), null, _authenticatedAccount);
            var graphResponse = await PerformGraphAsync(graphRequest);
            return graphResponse;

        }

        public async Task<Tuple<string, CMPShareError>> GetAllAlbumsAsync()
        {

            var graphRequest = new OAuth2Request(KHttpMethodGETString, new Uri(KAllAlbumsURLString), null,
                                                 _authenticatedAccount);
            var graphResponse = await PerformGraphAsync(graphRequest);
            return graphResponse;

        }

        public async Task<Tuple<string, CMPShareError>> CreateAlbumAsync(Dictionary<string, string>
                                                                         parametersDictionary = null)
        {

            var graphRequest = new OAuth2Request(KHttpMethodPOSTString, new Uri(KCreateAlbumURLString),
                                                 parametersDictionary, _authenticatedAccount);
            var graphResponse = await PerformGraphAsync(graphRequest);
            return graphResponse;

        }

        public async Task<Tuple<string, CMPShareError>> UploadPhotoForAlbumAsync(byte[] imageBytesArray,
                                                                                 string albumIdString,
                                                                                 Dictionary<string, string>
                                                                                 photoParametersDictionary = null)
        {

            var uploadURLString = string.Format(KUploadPhotosToAlbumURLString, albumIdString);
            var graphRequest = new OAuth2Request(KHttpMethodPOSTString, new Uri(uploadURLString), photoParametersDictionary,
                                                 _authenticatedAccount);

            var memoryStream = new MemoryStream(imageBytesArray);
            graphRequest.AddMultipartData(KSourceKeyString, memoryStream, KPNGMimeTypeKeyString,
                                          KUploadPNGFileNameString);
            
            var graphResponse = await PerformGraphAsync(graphRequest);
            return graphResponse;

        }

        public bool LogOut()
        {

            if (_accountStoreProxy == null)
                return false;

            var couldDelete = _accountStoreProxy.Delete();
            return couldDelete;

        }

    }
}
