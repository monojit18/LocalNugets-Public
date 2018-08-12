using System;
using System.Threading.Tasks;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using Subsystems.CustomQRcodeDroid.External;

namespace TestQR.Droid
{
    [Activity(Label = "TestQR", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        int count = 1;
        private CMPZXingQR _qr;

        private async Task RetieveAsync()
        {

            var qrResult = await _qr.RetrieveFromQRAsync();
            var toast = Toast.MakeText(this, qrResult, ToastLength.Short);
            toast.Show();


        }

        private void GenerateQR()
        {

            var qrString = _qr.GenerateQR("Testing QRcode", 400, 400);
            var qrBytes = Convert.FromBase64String(qrString);
            var bmp = BitmapFactory.DecodeByteArray(qrBytes, 0, qrBytes.Length);
            var imageView = FindViewById<ImageView>(Resource.Id.imageView1);
            imageView.SetImageBitmap(bmp);

        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            _qr = new CMPZXingQR(Application);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.myButton);

            //button.Click +=  async (object sender, System.EventArgs e) => 
            //{

            //    await RetieveAsync();

            //};

            button.Click += (object sender, System.EventArgs e) =>
            {

                GenerateQR();

            };
        }
    }
}

