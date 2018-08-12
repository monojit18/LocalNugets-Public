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
using Android.App;
using Android.Content.PM;
using Subsystems.CustomPermissionsDroid.External;

namespace Subsystems.CustomPermissionsDroid.Internal
{
    
    public class CMPPermissions
    {

        #region Private/Protected Variables
        private readonly Activity _hostActivity;
        private Dictionary<string, string> _requestedPermissionsDictionary;
        #endregion

        #region Private/Protected Methods
        private List<CMPPermissionInfo> PreparePermissionInfo(List<String> friendlyNamesList)
        {

            var permissionInfoList = new List<CMPPermissionInfo>();
            foreach (string friendlyNameString in friendlyNamesList)
            {

                string requestedPermissionString = _requestedPermissionsDictionary[friendlyNameString];
                Permission permissionResult = _hostActivity.CheckSelfPermission(requestedPermissionString);

                CMPPermissionInfo permissionInfo = new CMPPermissionInfo()
                {
                    FriendlyNameString = friendlyNameString,
                    RequestedPermissionString = requestedPermissionString,
                    PermissionResult = permissionResult

                };

                bool shouldShowRationale = _hostActivity.ShouldShowRequestPermissionRationale(permissionInfo
                                                                                              .RequestedPermissionString);
                permissionInfo.ShouldShowReason = shouldShowRationale;
                permissionInfoList.Add(permissionInfo);

            }

            return permissionInfoList;

        }

        private void ShowPermissions(List<CMPPermissionInfo> permissionInfoList, bool shouldShowReason, int accessCode,
                                     ShowReasonCallback showReasonCallback)
        {

            if (shouldShowReason == true)
            {

                showReasonCallback.Invoke(permissionInfoList, (List<string> permissionsList) =>
                {

                    String[] permissionsArray = null;
                    if (permissionsList == null)
                        permissionsArray = CMPPermissionsProxy.ExtractPermissions(permissionInfoList);
                    else
                        permissionsArray = permissionsList.ToArray();

                    if ((permissionsArray != null) && (permissionsArray.Length > 0))
                        _hostActivity.RequestPermissions(permissionsArray, accessCode);

                });
            }
            else
            {

                String[] permissionsArray = CMPPermissionsProxy.ExtractPermissions(permissionInfoList);
                if ((permissionsArray != null) && (permissionsArray.Length > 0))
                    _hostActivity.RequestPermissions(permissionsArray, accessCode);

            }

        }
        #endregion

        #region Public Methods
        public CMPPermissions(Activity hostActivity)
        {

            _hostActivity = hostActivity;

        }

        public void AddPermissionToWishList(string requestedPermissionString, string friendlyNameString)
        {

            if (_requestedPermissionsDictionary == null)
                _requestedPermissionsDictionary = new Dictionary<string, string>();

            _requestedPermissionsDictionary[friendlyNameString] = requestedPermissionString;

        }

        public PermissionError CheckForPermission(string friendlyNameString, int accessCode,
                                                  ShowReasonCallback showReasonCallback)
        {

            var friendlyNamesList = new List<string>();
            friendlyNamesList.Add(friendlyNameString);

            var permissionInfoList = PreparePermissionInfo(friendlyNamesList);
            var permissionInfo = permissionInfoList[0];

            if (permissionInfo.ShouldShowReason == true)
                ShowPermissions(permissionInfoList, permissionInfo.ShouldShowReason, accessCode, showReasonCallback);
            else
                _hostActivity?.RequestPermissions(new string[] { permissionInfo.RequestedPermissionString },
                                                  accessCode);

            return PermissionError.eAllOK;

        }

        public PermissionError CheckForListOfPermissions(List<string> friendlyNamesList, int accessCode,
                                                         ShowReasonCallback showReasonCallback)
        {

            if ((_hostActivity == null) || (friendlyNamesList == null) || (friendlyNamesList.Count == 0))
                return PermissionError.eFatalError;

            if ((_requestedPermissionsDictionary == null) || (_requestedPermissionsDictionary.Count == 0))
                return PermissionError.eFatalError;

            var permissionInfoList = PreparePermissionInfo(friendlyNamesList);
            if ((permissionInfoList == null) || (permissionInfoList.Count == 0))
                return PermissionError.eFatalError;

            var shouldShowReason = (permissionInfoList.Find((CMPPermissionInfo permissionInfo) =>
            {

                return (permissionInfo.ShouldShowReason == true);

            }) != null);

            ShowPermissions(permissionInfoList, shouldShowReason, accessCode, showReasonCallback);
            return PermissionError.eAllOK;

        }
        #endregion

    }

}
