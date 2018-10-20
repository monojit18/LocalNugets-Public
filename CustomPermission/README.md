# CustomPermission

*A Xamarin wrapper binding around Android permission model*

## Usage

        using Subsystems.CustomPermissionsDroid.External;
        ....
        private CMPPermissionsProxy _permissionsProxy;
        ....
        _permissionsProxy = new CMPPermissionsProxy(this);
        _permissionsProxy.AddPermissionToWishList(Manifest.Permission.ReadContacts, "RCN");
        _permissionsProxy.AddPermissionToWishList(Manifest.Permission.ReadCalendar, "RCL");

        var lst = new List<string>();
        lst.Add("RCN");
        lst.Add("RCL");

        _permissionsProxy.CheckForListOfPermissions(lst, 0, (permissionInfoList, responseCallback) => 
        {

            var permissionsArray = CMPPermissionsProxy.ExtractPermissions(permissionInfoList);
            
            // Can show reasons here, if needed, before calling callback
            responseCallback.Invoke(permissionsArray?.ToList());

        });
        
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Console.WriteLine("OnRequestPermissionsResult");
            
        }
