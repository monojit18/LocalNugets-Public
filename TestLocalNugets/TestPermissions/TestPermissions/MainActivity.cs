using System;
using System.Linq;
using System.Collections.Generic;
using Android;
using Android.App;
using Android.Widget;
using Android.OS;
using Subsystems.CustomPermissionsDroid.External;

namespace TestPermissions
{
    [Activity(Label = "TestPermissions", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        int count = 1;

        private CMPPermissionsProxy _permissionsProxy;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.myButton);

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

        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {

            Console.WriteLine("OnRequestPermissionsResult");


        } 
    }
}

