using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Auth;
using Subsystems.ShareQuickComponent.Internal;
using Subsystems.CustomAccountStore.External;

namespace Subsystems.ShareQuickComponent.External
{

    public class CMPFacebookShareProxy
    {

        private CMPFacebookShareService _facebookShareService;

        public void CreateFacebookService(string customerKeyString, string customerSecretKeyString,
                                          IShareAuthenticationCallbacks authenticationCallbacks,
                                          IAccountStoreConfiguration accountStoreConfiguration)
        {

            _facebookShareService = new CMPFacebookShareService(customerKeyString, customerSecretKeyString,
                                                                authenticationCallbacks, accountStoreConfiguration);

        }

        public CMPFacebookShareProxy AddToScope(string scopeString)
        {

            _facebookShareService.AddToScope(scopeString);
            return this;

        }

        public async Task<Tuple<bool, CMPShareError>> AuthenticateUserAsync()
        {

            var authenticationResultInfo = await _facebookShareService.AuthenticateUserAsync();
            return authenticationResultInfo;

        }

        public async Task<Tuple<string, CMPShareError>> GetFriendsListAsync()
        {

            var friendsResultInfo = await _facebookShareService.GetFriendsListAsync();
            return friendsResultInfo;

        }

        public async Task<Tuple<string, CMPShareError>> PostStatusAsync(Dictionary<string, string>
                                                                        parametersDictionary)
        {

            var postResultInfo = await _facebookShareService.PostStatusAsync(parametersDictionary);
            return postResultInfo;

        }

        public async Task<Tuple<string, CMPShareError>> PostPhotoWithMessageAsync(byte[] imageBytesArray)
        {

            var photoResultInfo = await _facebookShareService.PostPhotoWithMessageAsync(imageBytesArray);
            return photoResultInfo;

        }

        public async Task<Tuple<string, CMPShareError>> UpdatePostAsync(string postIdString,
                                                                        Dictionary<string, string>
                                                                        parametersDictionary)
        {

            var updatePostInfo = await _facebookShareService.UpdatePostAsync(postIdString, parametersDictionary);
            return updatePostInfo;

        }

        //public async Task<Tuple<string, CMPShareError>> LikePostAsync(string postIdString,
        //                                                              Dictionary<string, string>
        //                                                              parametersDictionary = null)
        //{

        //    var likePostInfo = await _facebookShareService.LikePostAsync(postIdString, parametersDictionary);
        //    return likePostInfo;

        //}

        //public async Task<Tuple<string, CMPShareError>> CommentOnPostAsync(string postIdString,
        //                                                                   Dictionary<string, string>
        //                                                                   parametersDictionary)
        //{

        //    var commentOnPostInfo = await _facebookShareService.CommentOnPostAsync(postIdString, parametersDictionary);
        //    return commentOnPostInfo;

        //}

        public async Task<Tuple<string, CMPShareError>> RemovePostAsync(string postIdString)
        {

            var removePostInfo = await _facebookShareService.RemovePostAsync(postIdString);
            return removePostInfo;

        }

        public async Task<Tuple<string, CMPShareError>> GetAllAlbumsAsync()
        {

            var getAlbumResultInfo = await _facebookShareService.GetAllAlbumsAsync();
            return getAlbumResultInfo;

        }

        public async Task<Tuple<string, CMPShareError>> CreateAlbumAsync(Dictionary<string, string>
                                                                         parametersDictionary = null)
        {

            var createAlbumResultInfo = await _facebookShareService.CreateAlbumAsync(parametersDictionary);
            return createAlbumResultInfo;

        }

        public async Task<Tuple<string, CMPShareError>> UploadPhotoForAlbumAsync(byte[] imageBytesarray,
                                                                                 string albumIdString,
                                                                                 Dictionary<string, string>
                                                                                 photoParametersDictionary)
        {

            var uploadedAlbumResultInfo = await _facebookShareService.UploadPhotoForAlbumAsync(imageBytesarray,
                                                                                               albumIdString,
                                                                                               photoParametersDictionary);
            return uploadedAlbumResultInfo;

        }

        public bool LogOut()
        {

            var couldDelete = _facebookShareService.LogOut();
            return couldDelete;

        }

    }
}
