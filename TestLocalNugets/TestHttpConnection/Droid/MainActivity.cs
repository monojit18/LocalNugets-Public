using Android.App;
using Android.Widget;
using Android.OS;
using System;
using Subsystems.HttpConnection.External;

namespace TestHttpConnection.Droid
{
	[Activity(Label = "TestHttpConnection", MainLauncher = true, Icon = "@mipmap/icon")]
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

			string responseString = "";
			button.Click += async delegate
			{
				button.Text = string.Format("{0} clicks!", count++);

				CMPHttpConnectionProxy cl = new CMPHttpConnectionProxy();
				cl.URL("http://www.google.com").Build();
                await cl.GetBytesWithProgressAsync((httpResponse, progressBytes, totalBytes) =>
				{

					responseString = string.Concat(responseString, httpResponse.ResponseString);
					Console.WriteLine("Response:" + responseString);
					Console.WriteLine("progressBytes: " + progressBytes + "\ntotalBytes: " + totalBytes);

				});


			};
		}
	}
}

