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
using System.Net.Http;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Auth;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Subsystems.ShareQuickComponent.External;
using Subsystems.CustomAccountStore.External;
using Subsystems.HttpConnection.External;

namespace Subsystems.ShareQuickComponent.Internal
{
    public class CMPTwitterShareService : CMPBaseShareService
    {
        
        private const string KRequestTokenURLString = "https://api.twitter.com/oauth/request_token";
        private const string KAuthorizeURLString = "https://api.twitter.com/oauth/authorize";
        private const string KAccessTokenURLString = "https://api.twitter.com/oauth/access_token";
        private const string KRedirectUriString = "https://codemongers.wordpress.com/";
        private const string KUploadMediaURLString = "https://upload.twitter.com/1.1/media/upload.json";
        private const string KFriendsListURLString = "https://api.twitter.com/1.1/friends/list.json";
        private const string KFollowersListURLString = "https://api.twitter.com/1.1/followers/list.json";
        private const string KHomeTweetsListURLString = "https://api.twitter.com/1.1/statuses/home_timeline.json";
        private const string KUserTweetsListURLString = "https://api.twitter.com/1.1/statuses/user_timeline.json";
        private const string KMenionsTweetsListURLString = "https://api.twitter.com/1.1/statuses/menions_timeline.json";
        private const string KTweetDetailsURLString = "https://api.twitter.com/1.1/statuses/show{0}.json";
        private const string KLookupTweetURLString = "https://api.twitter.com/1.1/statuses/lookup.json";
        private const string KReTweetDetailsURLString = "https://api.twitter.com/1.1/statuses/retweets/{0}.json";
        private const string KReTweetersURLString = "https://api.twitter.com/1.1/statuses/retweeters/ids.json";
        private const string KReTweetSelfURLString = "https://api.twitter.com/1.1/statuses/retweets_of_me.json";
        private const string KReTweetURLString = "https://api.twitter.com/1.1/statuses/retweet/{0}.json";
        private const string KUnReTweetURLString = "https://api.twitter.com/1.1/statuses/unretweet/{0}.json";
        private const string KStatusUpdateURLString = "https://api.twitter.com/1.1/statuses/update.json";
        private const string KDestroyTweetURLString = "https://api.twitter.com/1.1/statuses/destroy/{0}.json";
        private const string KShowMessageURLString = "https://api.twitter.com/1.1/direct_messages/events/show.json";
        private const string KRetrieveDirectMessagesURLString = "https://api.twitter.com/1.1/direct_messages/events/list.json";
        private const string KDirectMessageURLString = "https://api.twitter.com/1.1/direct_messages/events/new.json";


        private const string KAccessTokenKeyString = "oauth_token";
        private const string KAccessTokenSecretKeyString = "oauth_token_secret";
        private const string KConsumerKeyString = "oauth_consumer_key";
        private const string KConsumerSecretKeyString = "oauth_consumer_secret";
        private const string KScreenNameKeyString = "screen_name";
        private const string KUserIdKeyString = "user_id";
        private const string KExpiresInKeyString = "x_auth_expires";
        private const string KTWAccessTokenKeyString = "TWAccessToken";
        private const string KTWAccessTokenSecretKeyString = "TWAccessTokenSecret";
        private const string KTWConsumerKeyString = "TWConsumerKey";
        private const string KTWConsumerSecretKeyString = "TWConsumerSecretKey";
        private const string KTWScreenNameKeyString = "TWScreenName";
        private const string KTWUserIdKeyString = "TWUserId";
        private const string KTWExpiresInKeyString = "TWExpiresIn";
        private const string KMediaDataKeyString = "media_data";
        private const string KMediaIdsKeyString = "media_ids";
        private const string KResponseMediaIdKeyString = "media_id";
        private const string KStatusKeyString = "status";
        private const string KDirectEventKeyString = "event";
        private const string KDirectMessageCreateKeyString = "message_create";
        private const string KMediaKeyString = "media";

        private OAuth1Authenticator _authenticator;
        private CMPAccountStoreProxy _accountStoreProxy;
        private IShareAuthenticationCallbacks _authenticationCallbacks;
        private TaskCompletionSource<Tuple<bool, CMPShareError>> _authenticationTask;

        private OAuth1Authenticator PrepareAuthenticator()
        {

            var requestUri = new Uri(KRequestTokenURLString);
            var authorizeUri = new Uri(KAuthorizeURLString);
            var accessTokenUri = new Uri(KAccessTokenURLString);
            var redirectUri = new Uri(KRedirectUriString);

            var authenticator = new OAuth1Authenticator(_customerKeyString, _customerSecretKeyString, requestUri,
                                                        authorizeUri, accessTokenUri, redirectUri);
            return authenticator;

        }

        private Account CheckUserAuthentication()
        {

            if (_accountStoreProxy == null)
                return null;

            var accessTokenString = _accountStoreProxy.Fetch(KTWAccessTokenKeyString);
            if (string.IsNullOrEmpty(accessTokenString) == true)
                return null;

            var accessTokenSecretString = _accountStoreProxy.Fetch(KTWAccessTokenSecretKeyString);
            var consumerKeyString = _accountStoreProxy.Fetch(KTWConsumerKeyString);
            var consumerSecretString = _accountStoreProxy.Fetch(KTWConsumerSecretKeyString);
            var screenNameString = _accountStoreProxy.Fetch(KTWScreenNameKeyString);
            var userIdString = _accountStoreProxy.Fetch(KTWUserIdKeyString);
            var expiresInString = _accountStoreProxy.Fetch(KTWExpiresInKeyString);

            var accountProperties = new Dictionary<string, string>();
            accountProperties[KAccessTokenKeyString] = accessTokenString;
            accountProperties[KAccessTokenSecretKeyString] = accessTokenSecretString;
            accountProperties[KConsumerKeyString] = consumerKeyString;
            accountProperties[KConsumerSecretKeyString] = consumerSecretString;
            accountProperties[KScreenNameKeyString] = screenNameString;
            accountProperties[KUserIdKeyString] = userIdString;
            accountProperties[KExpiresInKeyString] = expiresInString;

            var userAccount = new Account(KUserIdKeyString ?? string.Empty, accountProperties);
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

        private async Task<Tuple<string, CMPShareError>> UploadMediaAsync(byte[] imageBytesArray)
        {

            var parametersDictionary = new Dictionary<string, string>();
            parametersDictionary.Add(KMediaDataKeyString, Convert.ToBase64String(imageBytesArray));

            var graphRequest = new OAuth1Request(KHttpMethodPOSTString, new Uri(KUploadMediaURLString),
            parametersDictionary, _authenticatedAccount, true);

            var graphResponse = await PerformGraphAsync(graphRequest);
            return graphResponse;

        }

        private string ExtractUploadedMediaId(Tuple<string, CMPShareError> uploadResponseInfo)
        {

            try
            {

                var uploadResponseError = uploadResponseInfo.Item2;
                if (uploadResponseError != null)
                    return null;
                
                var uploadMediaResponseString = uploadResponseInfo.Item1;
                if (string.IsNullOrEmpty(uploadMediaResponseString) == true)
                    return null;
                
                var uploadResponse = JObject.Parse(uploadMediaResponseString);
                var mediaIdToken = uploadResponse[KResponseMediaIdKeyString] as JToken;
                return mediaIdToken.ToString();

            }
            catch(Exception exception)
            {

                Diagonistics.Debug.WriteLine(exception.StackTrace);
                return null;

            }

        }

        private async Task<Tuple<string, CMPShareError>> GetTweetsListAsync(string urlString,
                                                                            Dictionary<string, string>
                                                                            parametersDictionary)
        {

            var graphRequest = new OAuth1Request(KHttpMethodGETString, new Uri(urlString), parametersDictionary,
                                                 _authenticatedAccount);
            var graphResponse = await PerformGraphAsync(graphRequest);
            return graphResponse;

        }

        private CMPDirectMessage PrepareDirectMessage(string recipientIdString, string directMessageTextString,
                                                      string mediaIdString)
        {

            if (string.IsNullOrEmpty(recipientIdString) == true)
                return null;
                    
            var directMessageModel = new CMPDirectMessage();
            directMessageModel.type = KDirectMessageCreateKeyString;

            directMessageModel.message_create = new MessageCreate();
            directMessageModel.message_create.target = new Target();
            directMessageModel.message_create.target.recipient_id = recipientIdString;

            directMessageModel.message_create.message_data = new MessageData();
            directMessageModel.message_create.message_data.text = (directMessageTextString) ?? string.Empty;

            if (string.IsNullOrEmpty(mediaIdString) == true)
                return directMessageModel;
            
            var messageData = directMessageModel.message_create.message_data;
            var attachment = new Attachment()
            {

                type = KMediaKeyString,
                media = new Media()
                {

                    id = mediaIdString
                }
            };

            messageData.attachment = attachment;
            return directMessageModel;

        }

        protected override void SaveTokensToKeyStore()
        {

            if (_authenticatedAccount == null)
                return;

            if (_authenticatedAccount.Properties?.ContainsKey(KAccessTokenKeyString) == true)
            {

                var accessTokenString = _authenticatedAccount.Properties?[KAccessTokenKeyString];
                _accountStoreProxy.Save(KTWAccessTokenKeyString, accessTokenString);

            }

            if (_authenticatedAccount.Properties?.ContainsKey(KAccessTokenSecretKeyString) == true)
            {

                var accessTokenSecretString = _authenticatedAccount.Properties?[KAccessTokenSecretKeyString];
                _accountStoreProxy.Save(KTWAccessTokenSecretKeyString, accessTokenSecretString);

            }

            if (_authenticatedAccount.Properties?.ContainsKey(KConsumerKeyString) == true)
            {

                var consumerKeyString = _authenticatedAccount.Properties?[KConsumerKeyString];
                _accountStoreProxy.Save(KTWConsumerKeyString, consumerKeyString);

            }

            if (_authenticatedAccount.Properties?.ContainsKey(KConsumerSecretKeyString) == true)
            {

                var consumerSecretString = _authenticatedAccount.Properties?[KConsumerSecretKeyString];
                _accountStoreProxy.Save(KTWConsumerSecretKeyString, consumerSecretString);

            }

            if (_authenticatedAccount.Properties?.ContainsKey(KScreenNameKeyString) == true)
            {

                var screenNameString = _authenticatedAccount.Properties?[KScreenNameKeyString];
                _accountStoreProxy.Save(KTWScreenNameKeyString, screenNameString);

            }

            if (_authenticatedAccount.Properties?.ContainsKey(KUserIdKeyString) == true)
            {

                var userIdString = _authenticatedAccount.Properties?[KUserIdKeyString];
                _accountStoreProxy.Save(KTWUserIdKeyString, userIdString);

            }

            if (_authenticatedAccount.Properties?.ContainsKey(KExpiresInKeyString) == true)
            {

                var expiresInString = _authenticatedAccount.Properties?[KExpiresInKeyString];
                _accountStoreProxy.Save(KTWExpiresInKeyString, expiresInString);

            }


        }

        public CMPTwitterShareService(string customerKeyString, string customerSecretKeyString,
                                      IShareAuthenticationCallbacks authenticationCallbacks,
                                      IAccountStoreConfiguration accountStoreConfiguration)
            : base(customerKeyString, customerSecretKeyString)
        {

            _authenticationCallbacks = authenticationCallbacks;
            _accountStoreProxy = new CMPAccountStoreProxy(accountStoreConfiguration);
            _cancellationTokenSource = new CancellationTokenSource();

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

        public async Task<Tuple<string, CMPShareError>> GetFollowersListAsync(Dictionary<string, string>
                                                                              parametersDictionary = null)
        {

            var graphRequest = new OAuth1Request(KHttpMethodGETString, new Uri(KFollowersListURLString),
                                                 parametersDictionary, _authenticatedAccount);
            var graphResponse = await PerformGraphAsync(graphRequest);
            return graphResponse;

        }

        public async Task<Tuple<string, CMPShareError>> GetFriendsListAsync(Dictionary<string, string>
                                                                            parametersDictionary = null)
        {

            var graphRequest = new OAuth1Request(KHttpMethodGETString, new Uri(KFriendsListURLString),
                                                 parametersDictionary, _authenticatedAccount);
            var graphResponse = await PerformGraphAsync(graphRequest);
            return graphResponse;

        }

        public async Task<Tuple<string, CMPShareError>> GetHomeTweetsListAsync(Dictionary<string, string>
                                                                               parametersDictionary)
        {

            var homeTweetsInfo = await GetTweetsListAsync(KHomeTweetsListURLString, parametersDictionary);
            return homeTweetsInfo;

        }

        public async Task<Tuple<string, CMPShareError>> GetUserTweetsListAsync(Dictionary<string, string>
                                                                               parametersDictionary)
        {

            var userTweetsInfo = await GetTweetsListAsync(KUserTweetsListURLString, parametersDictionary);
            return userTweetsInfo;

        }

        public async Task<Tuple<string, CMPShareError>> GetMentionsTweetsListAsync(Dictionary<string, string>
                                                                                   parametersDictionary)
        {

            var menionsTweetsInfo = await GetTweetsListAsync(KMenionsTweetsListURLString, parametersDictionary);
            return menionsTweetsInfo;

        }

        public async Task<Tuple<string, CMPShareError>> GetTweetDetailsAsync(string tweetIdString,
                                                                             Dictionary<string, string>
                                                                             parametersDictionary = null)
        {

            if (string.IsNullOrEmpty(tweetIdString) == true)
                throw (new ArgumentNullException());

            var urlString = string.Format(KTweetDetailsURLString, tweetIdString);
            var graphRequest = new OAuth1Request(KHttpMethodGETString, new Uri(urlString), parametersDictionary,
                                                 _authenticatedAccount);
            var graphResponse = await PerformGraphAsync(graphRequest);
            return graphResponse;

        }

        public async Task<Tuple<string, CMPShareError>> LookupTweetsAsync(Dictionary<string, string>
                                                                          parametersDictionary)
        {

            if ((parametersDictionary == null) || (parametersDictionary.Count == 0))
                throw (new ArgumentNullException());
            
            var graphRequest = new OAuth1Request(KHttpMethodGETString, new Uri(KLookupTweetURLString), parametersDictionary,
                                                 _authenticatedAccount);
            var graphResponse = await PerformGraphAsync(graphRequest);
            return graphResponse;

        }

        public async Task<Tuple<string, CMPShareError>> GetReTweetDetailsAsync(string tweetIdString,
                                                                               Dictionary<string, string>
                                                                               parametersDictionary = null)
        {

            if (string.IsNullOrEmpty(tweetIdString) == true)
                throw (new ArgumentNullException());

            var urlString = string.Format(KTweetDetailsURLString, tweetIdString);
            var graphRequest = new OAuth1Request(KHttpMethodGETString, new Uri(urlString), parametersDictionary,
                                                 _authenticatedAccount);
            var graphResponse = await PerformGraphAsync(graphRequest);
            return graphResponse;

        }

        public async Task<Tuple<string, CMPShareError>> GetReTweetsSelfAsync(Dictionary<string, string>
                                                                             parametersDictionary = null)
        {

            var graphRequest = new OAuth1Request(KHttpMethodGETString, new Uri(KReTweetSelfURLString), parametersDictionary,
                                                 _authenticatedAccount);
            var graphResponse = await PerformGraphAsync(graphRequest);
            return graphResponse;

        }

        public async Task<Tuple<string, CMPShareError>> GetReTweetersAsync(Dictionary<string, string>
                                                                           parametersDictionary = null)
        {

            var graphRequest = new OAuth1Request(KHttpMethodGETString, new Uri(KReTweetersURLString), parametersDictionary,
                                                 _authenticatedAccount);
            var graphResponse = await PerformGraphAsync(graphRequest);
            return graphResponse;

        }

        public async Task<Tuple<string, CMPShareError>> ReTweetAsync(string tweetIdString,
                                                                     Dictionary<string, string>
                                                                     parametersDictionary = null)
        {

            if (string.IsNullOrEmpty(tweetIdString) == true)
                throw (new ArgumentNullException());

            var urlString = string.Format(KDestroyTweetURLString, tweetIdString);
            var graphRequest = new OAuth1Request(KHttpMethodPOSTString, new Uri(urlString), parametersDictionary,
                                                 _authenticatedAccount);
            var graphResponse = await PerformGraphAsync(graphRequest);
            return graphResponse;

        }

        public async Task<Tuple<string, CMPShareError>> UnReTweetAsync(string tweetIdString,
                                                                       Dictionary<string, string>
                                                                       parametersDictionary = null)
        {

            if (string.IsNullOrEmpty(tweetIdString) == true)
                throw (new ArgumentNullException());

            var urlString = string.Format(KUnReTweetURLString, tweetIdString);
            var graphRequest = new OAuth1Request(KHttpMethodPOSTString, new Uri(urlString), parametersDictionary,
                                                 _authenticatedAccount);
            var graphResponse = await PerformGraphAsync(graphRequest);
            return graphResponse;

        }

        public async Task<Tuple<string, CMPShareError>> SendTweetAsync(Dictionary<string, string> parametersDictionary,
                                                                       byte[] imageBytesArray)
        {

            if ((parametersDictionary == null) || (parametersDictionary.Count == 0))
                throw (new ArgumentNullException());
            
            string mediaIdString = string.Empty;
            if ((imageBytesArray != null) && (imageBytesArray.Length > 0))
            {

                var uploadResultInfo = await UploadMediaAsync(imageBytesArray);
                mediaIdString = ExtractUploadedMediaId(uploadResultInfo);

            }

            if (string.IsNullOrEmpty(mediaIdString) == false)
                parametersDictionary.Add(KMediaIdsKeyString, mediaIdString);
            
            var graphRequest = new OAuth1Request(KHttpMethodPOSTString, new Uri(KStatusUpdateURLString),
                                                 parametersDictionary, _authenticatedAccount,
                                                 !(string.IsNullOrEmpty(mediaIdString) == true));
            
            var graphResponse = await PerformGraphAsync(graphRequest);
            return graphResponse;

        }

        public async Task<Tuple<string, CMPShareError>> DestroyTweetAsync(string tweetIdString,
                                                                          Dictionary<string, string>
                                                                          parametersDictionary = null)
        {

            if (string.IsNullOrEmpty(tweetIdString) == true)
                throw (new ArgumentNullException());

            var urlString = string.Format(KDestroyTweetURLString, tweetIdString);
            var graphRequest = new OAuth1Request(KHttpMethodPOSTString, new Uri(urlString), parametersDictionary,
                                                 _authenticatedAccount);
            var graphResponse = await PerformGraphAsync(graphRequest);
            return graphResponse;

        }

        public async Task<Tuple<string, CMPShareError>> GetShowMessageForIdAsync(string idString,
                                                                                 Dictionary<string, string>
                                                                                 parametersDictionary = null)
        {

            var graphRequest = new OAuth1Request(KHttpMethodGETString, new Uri(KRetrieveDirectMessagesURLString),
                                                 parametersDictionary, _authenticatedAccount);
            var graphResponse = await PerformGraphAsync(graphRequest);
            return graphResponse;

        }

        public async Task<Tuple<string, CMPShareError>> GetDirectMessagesAsync(Dictionary<string, string>
                                                                               parametersDictionary = null)
        {

            var graphRequest = new OAuth1Request(KHttpMethodGETString, new Uri(KRetrieveDirectMessagesURLString),
                                                 parametersDictionary, _authenticatedAccount);
            var graphResponse = await PerformGraphAsync(graphRequest);
            return graphResponse;

        }

        //public async Task<Tuple<string, CMPShareError>> SendDirectMessageAsync(CMPDirectMessage directMessage,
                                                                               //byte[] imageBytesArray)
        public async Task<Tuple<string, CMPShareError>> SendDirectMessageAsync(string recipientIdString,
                                                                               string directMessageTextString = null,
                                                                               byte[] imageBytesArray = null)
        {

            if (string.IsNullOrEmpty(recipientIdString) == true)
                return null;

            string mediaIdString = string.Empty;
            if ((imageBytesArray != null) && (imageBytesArray.Length > 0))
            {

                var uploadResultInfo = await UploadMediaAsync(imageBytesArray);
                mediaIdString = ExtractUploadedMediaId(uploadResultInfo);

            }

            var directMessageModel = PrepareDirectMessage(recipientIdString, directMessageTextString, mediaIdString);
            if (directMessageModel == null)
                return (new Tuple<string, CMPShareError>(null, null));
            
            var parametersDictionary = new Dictionary<string, CMPDirectMessage>();
            parametersDictionary.Add(KDirectEventKeyString, directMessageModel);
            var directMessageResponseString = JsonConvert.SerializeObject(parametersDictionary);

            var graphRequest = new DirectMessageRequest(KHttpMethodPOSTString, new Uri(KDirectMessageURLString),
                                                        _authenticatedAccount, directMessageResponseString);
            
            var graphResponse = await PerformGraphAsync(graphRequest);
            return graphResponse;

        }

    }

    public class DirectMessageRequest : OAuth1Request
    {


        private string _directMessageString;

        public DirectMessageRequest(string method, Uri url, Account account, string directMessageString)
            : base(method, url, null, account, false)
        {

            _directMessageString = string.Copy(directMessageString);

        }

        public override Task<Response> GetResponseAsync(CancellationToken cancellationToken)
        {
            
            var directMessageRequest = GetPreparedWebRequest();
            directMessageRequest.Content = new StringContent(_directMessageString, System.Text.Encoding.UTF8,
                                                             "application/json");
            return base.GetResponseAsync(cancellationToken);

        }

    }
}
