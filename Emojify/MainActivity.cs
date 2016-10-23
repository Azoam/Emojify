using Android.App;
using Android.Widget;
using Android.OS;
using Android;
using Google.Apis.Json;
using Google.Apis.Requests;
using Google.Apis.Util;
using Google.Apis.Vision.v1;
using Android.Graphics;
using System;
using Android.Content;
using Android.Content.PM;
using Android.Provider;
using Java.IO;
using System.Collections.Generic;
using Android.Views;

namespace Emojify
{
    [Activity(Label = "Emojify", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        public static readonly int PickImageID = 1000;
        private ImageView _imageView;
        Button uploadButton;
        public static class App
        {
            public static File _file;
            public static File _dir;
            public static Bitmap bitmap;
        }
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures();

                Button button = FindViewById<Button>(Resource.Id.takePicture);
                button.Click += TakeAPicture;

            }
            _imageView = FindViewById<ImageView>(Resource.Id.imageView1);
            uploadButton = FindViewById<Button>(Resource.Id.uploadButton);
            uploadButton.Click += BrowseOnClick;
        }
        private void CreateDirectoryForPictures()
        {
            App._dir = new File(
                Android.OS.Environment.GetExternalStoragePublicDirectory(
                    Android.OS.Environment.DirectoryPictures), "Emojify");
            if (!App._dir.Exists())
            {
                App._dir.Mkdirs();
            }
        }

        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities =
                PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }
        private void TakeAPicture(object sender, EventArgs eventArgs)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            App._file = new System.Drawing.Bitmap(String.Format("myPhoto_{0}.jpg", Guid.NewGuid()),10,30);
            App._file = new File(App._dir, String.Format("myPhoto_{0}.jpg", Guid.NewGuid()));
            intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(App._file));
            StartActivityForResult(intent, 0);
        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if ((requestCode == PickImageID) && (resultCode == Result.Ok) && (data != null))
            {
                Object uri = data.Data;
                _imageView.SetImageURI((Android.Net.Uri)uri);
            }
            base.OnActivityResult(requestCode, resultCode, data);

            // Make it available in the gallery

            Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
            Android.Net.Uri contentUri = Android.Net.Uri.FromFile(App._file);
            mediaScanIntent.SetData(contentUri);
            SendBroadcast(mediaScanIntent);

            // Display in ImageView. We will resize the bitmap to fit the display.
            // Loading the full sized image will consume to much memory
            // and cause the application to crash.

            // Dispose of the Java side bitmap.
            GC.Collect();
        }
		private void BrowseOnClick(object sender, EventArgs e)
		{
			Intent.SetType("");
			Intent = new Intent();
			Intent.SetType("image/*");
			Intent.SetAction(Intent.ActionGetContent);
			StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), PickImageID);
		}

    }
    }
 