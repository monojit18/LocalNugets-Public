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
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Auth;
using Subsystems.ShareQuickComponent.Internal;
using Subsystems.CustomAccountStore.External;

namespace Subsystems.ShareQuickComponent.External
{
    public class CMPTwitterShareProxy
    {

        private CMPTwitterShareService _twitterShareService;

        public void CreateTwitterService(string customerKeyString, string customerSecretKeyString,
                                         IShareAuthenticationCallbacks authenticationCallbacks,
                                         IAccountStoreConfiguration accountStoreConfiguration)
        {

            _twitterShareService = new CMPTwitterShareService(customerKeyString, customerSecretKeyString,
                                                              authenticationCallbacks, accountStoreConfiguration);

        }

        public async Task<Tuple<bool, CMPShareError>> AuthenticateUserAsync()
        {

            var authenticationResultInfo = await _twitterShareService.AuthenticateUserAsync();
            return authenticationResultInfo;

        }

        public async Task<Tuple<string, CMPShareError>> GetFollowersListAsync(Dictionary<string, string>
                                                                              parametersDictionary = null)
        {

            var followersResultInfo = await _twitterShareService.GetFollowersListAsync(parametersDictionary);
            return followersResultInfo;

        }

        public async Task<Tuple<string, CMPShareError>> GetFriendsListAsync(Dictionary<string, string>
                                                                            parametersDictionary = null)
        {

            var friendsResultInfo = await _twitterShareService.GetFriendsListAsync(parametersDictionary);
            return friendsResultInfo;

        }

        public async Task<Tuple<string, CMPShareError>> GetHomeTweetsListAsync(Dictionary<string, string>
                                                                               parametersDictionary)
        {

            var homeTweetsListResultInfo = await _twitterShareService.GetHomeTweetsListAsync(parametersDictionary);
            return homeTweetsListResultInfo;

        }

        public async Task<Tuple<string, CMPShareError>> GetUserTweetsListAsync(Dictionary<string, string>
                                                                               parametersDictionary)
        {

            var userTweetsListResultInfo = await _twitterShareService.GetUserTweetsListAsync(parametersDictionary);
            return userTweetsListResultInfo;

        }

        public async Task<Tuple<string, CMPShareError>> GetMentionsTweetsListAsync(Dictionary<string, string>
                                                                                   parametersDictionary)
        {

            var mentionsTweetsListResultInfo = await _twitterShareService.GetMentionsTweetsListAsync(parametersDictionary);
            return mentionsTweetsListResultInfo;

        }

        public async Task<Tuple<string, CMPShareError>> GetTweetDetailsAsync(string tweetIdString,
                                                                             Dictionary<string, string>
                                                                             parametersDictionary = null)
        {

            var tweetDetailsResultInfo = await _twitterShareService.GetTweetDetailsAsync(tweetIdString,
                                                                                         parametersDictionary);
            return tweetDetailsResultInfo;

        }

        public async Task<Tuple<string, CMPShareError>> LookupTweetsAsync(Dictionary<string, string>
                                                                          parametersDictionary)
        {

            var lookupTweetsResultInfo = await _twitterShareService.LookupTweetsAsync(parametersDictionary);
            return lookupTweetsResultInfo;

        }

        public async Task<Tuple<string, CMPShareError>> GetReTweetDetailsAsync(string tweetIdString,
                                                                               Dictionary<string, string>
                                                                               parametersDictionary = null)
        {

            var reTweetDetailsResultInfo = await _twitterShareService.GetReTweetDetailsAsync(tweetIdString,
                                                                                             parametersDictionary);
            return reTweetDetailsResultInfo;

        }

        public async Task<Tuple<string, CMPShareError>> GetReTweetsSelfAsync(Dictionary<string, string>
                                                                             parametersDictionary = null)
        {

            var reTweetsSelfResultInfo = await _twitterShareService.GetReTweetsSelfAsync(parametersDictionary);
            return reTweetsSelfResultInfo;

        }

        public async Task<Tuple<string, CMPShareError>> GetReTweetersAsync(Dictionary<string, string>
                                                                           parametersDictionary = null)
        {

            var reTweetersResultInfo = await _twitterShareService.GetReTweetersAsync(parametersDictionary);
            return reTweetersResultInfo;

        }

        public async Task<Tuple<string, CMPShareError>> ReTweetAsync(string tweetIdString,
                                                                     Dictionary<string, string>
                                                                     parametersDictionary = null)
        {

            var reTweetResultInfo = await _twitterShareService.ReTweetAsync(tweetIdString, parametersDictionary);
            return reTweetResultInfo;

        }

        public async Task<Tuple<string, CMPShareError>> UnReTweetAsync(string tweetIdString,
                                                                       Dictionary<string, string>
                                                                       parametersDictionary = null)
        {

            var unReTweetResultInfo = await _twitterShareService.UnReTweetAsync(tweetIdString, parametersDictionary);
            return unReTweetResultInfo;

        }

        public async Task<Tuple<string, CMPShareError>> SendTweetAsync(Dictionary<string, string> parametersDictionary,
                                                                       byte[] imageBytesArray)
        {

            var tweetResultInfo = await _twitterShareService.SendTweetAsync(parametersDictionary, imageBytesArray);
            return tweetResultInfo;

        }

        public async Task<Tuple<string, CMPShareError>> DestroyTweetAsync(string tweetIdString,
                                                                          Dictionary<string, string>
                                                                          parametersDictionary = null)
        {

            var destroyResultInfo = await _twitterShareService.DestroyTweetAsync(tweetIdString, parametersDictionary);
            return destroyResultInfo;

        }

        public async Task<Tuple<string, CMPShareError>> GetShowMessageForIdAsync(string idString,
                                                                                 Dictionary<string, string>
                                                                                 parametersDictionary = null)
        {

            var showMessageInfo = await _twitterShareService.GetShowMessageForIdAsync(idString,
                                                                                      parametersDictionary);
            return showMessageInfo;

        }

        public async Task<Tuple<string, CMPShareError>> GetDirectMessagesAsync(Dictionary<string, string>
                                                                               parametersDictionary = null)
        {

            var directMessagesListInfo = await _twitterShareService.GetDirectMessagesAsync(parametersDictionary);
            return directMessagesListInfo;

        }

        public async Task<Tuple<string, CMPShareError>> SendDirectMessageAsync(string recipientIdString,
                                                                               string directMessageTextString = null,
                                                                               byte[] imageBytesArray = null)
        {

            var directMessageResultInfo = await _twitterShareService.SendDirectMessageAsync(recipientIdString,
                                                                                            directMessageTextString,
                                                                                            imageBytesArray);
            return directMessageResultInfo;

        }

    }
}
