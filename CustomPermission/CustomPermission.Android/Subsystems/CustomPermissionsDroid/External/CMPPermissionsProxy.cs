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
using System.Linq;
using System.Collections.Generic;
using Android.App;
using Subsystems.CustomPermissionsDroid.Internal;

namespace Subsystems.CustomPermissionsDroid.External
{

    public delegate void ReasonResponseCallback(List<String> permissionsList);
    public delegate void ShowReasonCallback(List<CMPPermissionInfo> permissionInfoList,
                                            ReasonResponseCallback responseCallback);

    public class CMPPermissionsProxy
    {

        #region Private/Protected Variables
        private readonly CMPPermissions _permissions;
        #endregion

        #region Public Methods
        public static string[] ExtractPermissions(List<CMPPermissionInfo> permissionInfoList)
        {

            string[] permissionsArray = permissionInfoList.Where((CMPPermissionInfo permissionInfo) =>
            {

                return (permissionInfo.PermissionResult == Android.Content.PM.Permission.Denied);

            }).Select((CMPPermissionInfo permissionInfo) =>
            {

                return (permissionInfo.RequestedPermissionString);

            }).ToArray();

            if ((permissionsArray == null) || (permissionsArray.Length == 0))
            {

                permissionsArray = permissionInfoList.Select((CMPPermissionInfo permissionInfo) =>
                {

                    return (permissionInfo.RequestedPermissionString);

                }).ToArray();

            }

            return permissionsArray;

        }

        public CMPPermissionsProxy(Activity hostActivity)
        {
            _permissions = new CMPPermissions(hostActivity);
        }

        public void AddPermissionToWishList(string requestedPermissionString, string friendlyNameString)
        {

            if (_permissions == null)
                return;

            _permissions.AddPermissionToWishList(requestedPermissionString, friendlyNameString);

        }

        public PermissionError CheckForPermission(string friendlyNameString, int accessCode,
                                                  ShowReasonCallback showReasonCallback)
        {

            if (_permissions == null)
                return PermissionError.eFatalError;

            var response = _permissions.CheckForPermission(friendlyNameString, accessCode, showReasonCallback);
            return response;

        }

        public PermissionError CheckForListOfPermissions(List<string> friendlyNamesList, int accessCode,
                                                         ShowReasonCallback showReasonCallback)
        {

            if (_permissions == null)
                return PermissionError.eFatalError;

            var response = _permissions.CheckForListOfPermissions(friendlyNamesList, accessCode, showReasonCallback);
            return response;

        }
        #endregion
    }
}
