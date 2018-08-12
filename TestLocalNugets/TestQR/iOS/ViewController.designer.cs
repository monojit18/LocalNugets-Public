// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace TestQR.iOS
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        UIKit.UIButton Button { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton GenerateButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView QRImageView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField QRTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ScanButton { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (GenerateButton != null) {
                GenerateButton.Dispose ();
                GenerateButton = null;
            }

            if (QRImageView != null) {
                QRImageView.Dispose ();
                QRImageView = null;
            }

            if (QRTextField != null) {
                QRTextField.Dispose ();
                QRTextField = null;
            }

            if (ScanButton != null) {
                ScanButton.Dispose ();
                ScanButton = null;
            }
        }
    }
}