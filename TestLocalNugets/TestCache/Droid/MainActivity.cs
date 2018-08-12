using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Subsystems.Cache.External;

namespace TestCache.Droid
{
	[Activity(Label = "TestCache", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		int count = 1;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button>(Resource.Id.myButton);

			button.Click += delegate
            {

                button.Text = string.Format("{0} clicks!", count++);
            
                var cachedFolderPathString = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
                var cacheProxy = new CMPCacheProxy(cachedFolderPathString, 5000);
                cacheProxy.ExpiryDays(1);

                string cacheString = "Test String";
                byte[] retrievedBytes = cacheProxy.RetieveItem("testfile");
                if (retrievedBytes != null)
                {
                    cacheString = System.Text.Encoding.UTF8.GetString(retrievedBytes);
                    Console.WriteLine("cacheString:" + cacheString);
                }
                else
                {

                    byte[] cacheBytes = System.Text.Encoding.UTF8.GetBytes(cacheString);
                    cacheProxy.CacheItem(cacheBytes, "testfile");

                }
            
            };
		}
	}
}

