﻿using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Provider;
using Android.Support.CustomTabs;
using Android.Support.V4.Content;
using Android.Telephony;
using Android.Widget;
using Java.IO;
using Java.Text;
using Java.Util;
using PlayTube.Helpers.Utils;
using Console = System.Console;
using Uri = Android.Net.Uri;

namespace PlayTube.Helpers.Controller
{
    public class IntentController
    {
        //############################# DON'T MODIFY HERE ##########################

        private readonly Activity Context;
        public static string CurrentPhotoPath;
        public static string CurrentVideoPath;

        public IntentController(Activity context)
        {
            try
            {
                Context = context;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //############################# Special for application ##########################

        //################################# General #################################
        /// <summary>
        /// Open intent Image Gallery when the request code of result is 500
        /// </summary>
        /// <param name="title"></param>
        /// <param name="allowMultiple"></param>
        public void OpenIntentImageGallery(string title, bool allowMultiple = true)
        {
            try
            {
                Intent intent;
                if ((int)Build.VERSION.SdkInt <= 25)
                    intent = new Intent(Intent.ActionPick, MediaStore.Images.Media.ExternalContentUri);
                else
                    intent = Android.OS.Environment.GetExternalStorageState(null).Equals(Android.OS.Environment.MediaMounted) ? new Intent(Intent.ActionPick, MediaStore.Images.Media.ExternalContentUri) : new Intent(Intent.ActionPick, MediaStore.Images.Media.InternalContentUri);

                intent.SetType("image/*");

                if (AppSettings.ImageCropping)
                {
                    intent.PutExtra("crop", "true");
                    var myUri = Uri.FromFile(new File(Methods.Path.FolderDiskImage, Methods.GetTimestamp(DateTime.Now) + ".jpeg"));
                    intent.PutExtra(MediaStore.ExtraOutput, myUri);
                    intent.PutExtra("outputFormat", Bitmap.CompressFormat.Jpeg.ToString());
                    intent.PutExtra("return-data", true); //added snippet
                }

                if (allowMultiple)
                    intent.PutExtra(Intent.ExtraAllowMultiple, true);

                Context.StartActivityForResult(Intent.CreateChooser(intent, title), 500);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Open intent video Gallery when the request code of result is 501
        /// </summary>
        public void OpenIntentVideoGallery(string title = "video")
        {
            try
            {
                //var intent = new Intent(Intent.ActionPick, MediaStore.Video.Media.ExternalContentUri);
                ////intent.SetAction(Intent.ActionGetContent);
                //intent.SetType("video/*");
                //intent.PutExtra("return-data", true); //added snippet
                //Context.StartActivityForResult(Intent.CreateChooser(intent, title), 501);

                Intent intent;
                if ((int)Build.VERSION.SdkInt <= 25)
                    intent = new Intent(Intent.ActionPick, MediaStore.Video.Media.ExternalContentUri);
                else
                    intent = Android.OS.Environment.GetExternalStorageState(null).Equals(Android.OS.Environment.MediaMounted) ? new Intent(Intent.ActionPick, MediaStore.Video.Media.ExternalContentUri) : new Intent(Intent.ActionPick, MediaStore.Video.Media.InternalContentUri);

                //  In this example we will set the type to video
                intent.SetType("video/*");
                intent.PutExtra("return-data", true); //added snippet
                intent.AddFlags(ActivityFlags.GrantReadUriPermission);
                Context.StartActivityForResult(Intent.CreateChooser(intent, title), 501);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Open intent Location when the request code of result is 502
        /// </summary>
        public void OpenIntentLocation()
        {

            //try
            //{
            //    Intent intent = new Intent(Context, typeof(LocationActivity));
            //    Context.StartActivityForResult(intent, 502);
            //}
            //catch (GooglePlayServicesRepairableException e)
            //{
            //    Console.WriteLine(e);
            //    Toast.MakeText(Context, "Google Play Services is not available.", ToastLength.Short).Show();
            //}
            //catch (GooglePlayServicesNotAvailableException e)
            //{
            //    Console.WriteLine(e);
            //    Toast.MakeText(Context, "Google Play Services is not available", ToastLength.Short).Show();
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //    Toast.MakeText(Context, "Google Play Services e", ToastLength.Short).Show();
            //}
        }

        private File CreateImageFile()
        {
            // Create an image file name
            string timeStamp = new SimpleDateFormat("yyyyMMdd_HHmmss").Format(new Date());
            string imageFileName = "JPEG_" + timeStamp + "_";
            string storageDir = Methods.Path.FolderDcimImage;

            try
            {
                File image = File.CreateTempFile(imageFileName, ".jpg", new File(storageDir));

                // Save a file: path for use with ACTION_VIEW intents
                CurrentPhotoPath = image.AbsolutePath;
                return image;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                File image = new File(storageDir + "/" + imageFileName, ".jpg");
                // Save a file: path for use with ACTION_VIEW intents
                CurrentPhotoPath = image.AbsolutePath;
                return image;
            }
        }

        /// <summary>
        /// Open intent Camera when the request code of result is 503
        /// </summary>
        public void OpenIntentCamera()
        {
            try
            {
                if (Methods.MultiMedia.IsCameraAvailable())
                {
                    Intent takePictureIntent = new Intent(MediaStore.ActionImageCapture);
                    // Ensure that there's a camera activity to handle the intent
                    var packageManager = takePictureIntent.ResolveActivity(Context.PackageManager);
                    if (packageManager != null)
                    {
                        // Create the File where the photo should go
                        File photoFile;
                        try
                        {
                            photoFile = CreateImageFile();
                        }
                        catch (Exception ex)
                        {
                            string timeStamp = new SimpleDateFormat("yyyyMMdd_HHmmss").Format(new Date());
                            photoFile = new File(Methods.Path.FolderDcimImage + "/" + timeStamp + ".jpg");
                            CurrentPhotoPath = photoFile.AbsolutePath;

                            // Error occurred while create  ...
                            Console.WriteLine(ex);
                        }

                        // Continue only if the File was successfully created
                        if (photoFile != null)
                        {
                            var photoUri = FileProvider.GetUriForFile(Context, Context.PackageName + ".fileprovider", photoFile);
                            takePictureIntent.PutExtra(MediaStore.ExtraOutput, photoUri);
                        }
                    }
                    Context.StartActivityForResult(takePictureIntent, 503);
                }
                else
                {
                    Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_Camera_Not_Available), ToastLength.Short).Show();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private File CreateVideoFile()
        {
            // Create an image file name
            string timeStamp = new SimpleDateFormat("yyyyMMdd_HHmmss").Format(new Date());
            string videoFileName = "Video_" + timeStamp + "_";
            string storageDir = Methods.Path.FolderDcimVideo;

            File video = new File(storageDir + "/" + videoFileName + ".mp4");

            // Save a file: path for use with ACTION_VIEW intents
            CurrentVideoPath = video.AbsolutePath;
            return video;
        }

        /// <summary>
        /// Open intent Video Camera when the request code of result is 513
        /// </summary>
        public void OpenIntentVideoCamera()
        {
            try
            {
                if (Methods.MultiMedia.IsCameraAvailable())
                {
                    Intent intent = new Intent(MediaStore.ActionVideoCapture);

                    File mediaFile = null;
                    try
                    {
                        mediaFile = CreateVideoFile();
                    }
                    catch (Exception ex)
                    {
                        string timeStamp = new SimpleDateFormat("yyyyMMdd_HHmmss").Format(new Date());
                        mediaFile = new File(Methods.Path.FolderDcimVideo + "/" + timeStamp + ".mp4");
                        CurrentVideoPath = mediaFile.AbsolutePath;

                        // Error occurred while create  ...
                        Console.WriteLine(ex);
                    }

                    if (mediaFile != null)
                    {
                        //var videoUri = FileProvider.GetUriForFile(Context, Context.PackageName + ".fileprovider", mediaFile);
                        //intent.PutExtra(MediaStore.ExtraOutput, videoUri);
                    }

                    Context.StartActivityForResult(intent, 513);
                }
                else
                {
                    Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_Camera_Not_Available), ToastLength.Short).Show();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Open intent File when the request code of result is 504
        /// </summary>
        /// <param name="title"></param>
        public void OpenIntentFile(string title)
        {
            try
            {
                Intent intent;
                if (Build.Manufacturer.ToLower().Equals("samsung"))
                {
                    intent = new Intent("com.sec.android.app.myfiles.PICK_DATA");
                    intent.PutExtra("CONTENT_TYPE", "*/*");
                    intent.AddCategory(Intent.CategoryDefault);
                }
                else
                {
                    string[] mimeTypes =
                    {"application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", // .doc & .docx
                        "application/vnd.ms-powerpoint", "application/vnd.openxmlformats-officedocument.presentationml.presentation", // .ppt & .pptx
                        "application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", // .xls & .xlsx
                        "text/plain",
                        "application/pdf",
                        "application/zip", "application/vnd.android.package-archive"};

                    intent = new Intent(Intent.ActionGetContent); // or ACTION_OPEN_DOCUMENT
                    intent.SetType("*/*");
                    intent.PutExtra(Intent.ExtraMimeTypes, mimeTypes);
                    intent.AddCategory(Intent.CategoryOpenable);
                    intent.PutExtra(Intent.ExtraLocalOnly, true);
                }
                Context.StartActivityForResult(Intent.CreateChooser(intent, title), 504);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                var fileIntent = new Intent(Intent.ActionPick);
                fileIntent.SetAction(Intent.ActionGetContent);
                fileIntent.SetType("*/*");
                Context.StartActivityForResult(Intent.CreateChooser(fileIntent, title), 504);
            }
        }

        /// <summary>
        /// Open intent Audio when the request code of result is 505
        /// </summary>
        public void OpenIntentAudio()
        {
            try
            {
                Intent intent;
                if ((int)Build.VERSION.SdkInt <= 25)
                    intent = new Intent(Intent.ActionPick, MediaStore.Audio.Media.ExternalContentUri);
                else
                    intent = Android.OS.Environment.GetExternalStorageState(null).Equals(Android.OS.Environment.MediaMounted) ? new Intent(Intent.ActionPick, MediaStore.Audio.Media.ExternalContentUri) : new Intent(Intent.ActionPick, MediaStore.Audio.Media.InternalContentUri);
                //intent.SetType("audio/*");
                Context.StartActivityForResult(intent, 505);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Open intent Get Contact Number Phone when the request code of result is 506
        /// </summary>
        public void OpenIntentGetContactNumberPhone()
        {
            try
            {
                Intent pickcontact = new Intent(Intent.ActionPick, ContactsContract.Contacts.ContentUri);
                pickcontact.SetType(ContactsContract.CommonDataKinds.Phone.ContentType);
                Context.StartActivityForResult(pickcontact, 506);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Open intent Gps when the request code of result is 1050
        /// </summary>
        /// <param name="locationManager"></param>
        public void OpenIntentGps(LocationManager locationManager)
        {
            try
            {
                if (!locationManager.IsProviderEnabled(LocationManager.GpsProvider) && !locationManager.IsProviderEnabled(LocationManager.NetworkProvider))
                {
                    Context.StartActivityForResult(new Intent(Settings.ActionLocationSourceSettings), 1050);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Open intent Send Sms
        /// </summary>
        /// <param name="phoneNumber">any number </param>
        /// <param name="textMessages">Example : Hello Xamarin This is My Test SMS</param>
        /// <param name="openIntent">true or false >> If it is false the message will be sent in a hidden manner .. don't open intent </param>
        public void OpenIntentSendSms(string phoneNumber, string textMessages, bool openIntent = true)
        {
            try
            {
                if (openIntent)
                {
                    var smsUri = Uri.Parse("smsto:" + phoneNumber);
                    var intent = new Intent(Intent.ActionSendto, smsUri);
                    intent.PutExtra("sms_body", textMessages);
                    intent.AddFlags(ActivityFlags.NewTask);
                    Context.StartActivity(intent);
                }
                else
                {
                    //Or use this code
                    SmsManager.Default.SendTextMessage(phoneNumber, null, textMessages, null, null);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Open intent Save Contact Number
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <param name="detailedInformation">true or false </param>
        public void OpenIntentSaveContacts(string phoneNumber, string name, string email, bool detailedInformation = false)
        {
            try
            {
                if (detailedInformation)
                {
                    Intent intent = new Intent(ContactsContract.Intents.Insert.Action);
                    intent.SetType(ContactsContract.RawContacts.ContentType);
                    intent.PutExtra(ContactsContract.Intents.Insert.Phone, phoneNumber);
                    intent.PutExtra(ContactsContract.Intents.Insert.Name, name);
                    intent.PutExtra(ContactsContract.Intents.Insert.Email, email);
                    Context.StartActivity(intent);
                }
                else
                {
                    var contactUri = Uri.Parse("tel:" + phoneNumber);
                    Intent intent = new Intent(ContactsContract.Intents.ShowOrCreateContact, contactUri);
                    intent.PutExtra(ContactsContract.Intents.ExtraRecipientContactName, true);
                    Context.StartActivity(intent);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="email"></param>
        /// <param name="cc"></param>
        /// <param name="subject"></param>
        /// <param name="text"></param>
        public void OpenIntentSendEmail(string email, string cc = " ", string subject = " ", string text = " ")
        {
            try
            {
                var intent = new Intent(Intent.ActionSend);
                intent.PutExtra(Intent.ExtraEmail, email);
                intent.PutExtra(Intent.ExtraCc, cc);
                intent.PutExtra(Intent.ExtraSubject, subject);
                intent.PutExtra(Intent.ExtraText, text);
                intent.SetType("message/rfc822");
                Context.StartActivity(intent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="phoneNumber"></param>
        public void OpenIntentSendPhoneCall(string phoneNumber)
        {
            try
            {
                var urlNumber = Uri.Parse("tel:" + phoneNumber);
                var intent = new Intent(Intent.ActionCall);
                intent.SetData(urlNumber);
                intent.AddFlags(ActivityFlags.NewTask);
                Context.StartActivity(intent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Open intent Browser From Phone using Url
        /// </summary>
        /// <param name="website"></param>
        public void OpenBrowserFromPhone(string website)
        {
            try
            {
                var uri = Uri.Parse(website);
                var intent = new Intent(Intent.ActionView, uri);
                intent.AddFlags(ActivityFlags.NewTask);
                Context.StartActivity(intent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Open intent Browser From App using Url
        /// </summary>
        /// <param name="url"></param>
        public void OpenBrowserFromApp(string url)
        {
            try
            {
                CustomTabsIntent.Builder builder = new CustomTabsIntent.Builder();
                CustomTabsIntent customTabsIntent = builder.Build();
                customTabsIntent.Intent.AddFlags(ActivityFlags.NewTask);
                customTabsIntent.LaunchUrl(Context, Uri.Parse(url));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Open app PackageName by Google play
        /// </summary>
        /// <param name="appPackageName">from Context or Activity object</param>
        public void OpenAppOnGooglePlay(string appPackageName)
        {
            try
            {
                try
                {
                    Context.StartActivity(new Intent(Intent.ActionView, Uri.Parse("market://details?id=" + appPackageName)));
                }
                catch (ActivityNotFoundException exception)
                {
                    Context.StartActivity(new Intent(Intent.ActionView, Uri.Parse("https://play.google.com/store/apps/details?id=" + appPackageName)));
                    Console.WriteLine(exception);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static Intent GetOpenFacebookIntent(Context context, string name)
        {
            try
            {
                context.PackageManager.GetPackageInfo("com.facebook.katana", 0); //Checks if FB is even installed.
                //return new Intent(Intent.ActionView,Uri.Parse("fb://profile/" + name)); //Try's to make intent with FB is URI
                return new Intent(Intent.ActionView, Uri.Parse("fb://facewebmodal/f?href=https://www.facebook.com/" + name)); //Try's to make intent with FB is URI
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new Intent(Intent.ActionView, Uri.Parse("https://www.facebook.com/" + name)); //catches and opens a url to the desired page
            }
        }

        public void OpenFacebookIntent(Context context, string name)
        {
            try
            {
                Intent facebookIntent = GetOpenFacebookIntent(context, name);
                Context.StartActivity(facebookIntent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OpenTwitterIntent(string name)
        {
            try
            {
                Context.StartActivity(new Intent(Intent.ActionView, Uri.Parse("twitter://user?screen_name=" + name)));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                OpenBrowserFromApp("https://twitter.com/#!/" + name);
                //Context.StartActivity(new Intent(Intent.ActionView, Uri.Parse("https://twitter.com/#!/" + name)));
            }
        }

        public void OpenLinkedInIntent(string name)
        {
            try
            {
                string url = "https://www.linkedin.com/in/" + name;
                Intent linkedInAppIntent = new Intent(Intent.ActionView, Uri.Parse(url));
                linkedInAppIntent.AddFlags(ActivityFlags.ClearWhenTaskReset);
                Context.StartActivity(linkedInAppIntent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OpenInstagramIntent(string name)
        {
            try
            {
                Intent likeIng = new Intent(Intent.ActionView, Uri.Parse("http://instagram.com/_u/" + name));
                likeIng.SetPackage("com.instagram.android");

                try
                {
                    Context.StartActivity(likeIng);
                }
                catch (ActivityNotFoundException e)
                {
                    Console.WriteLine(e);
                    Context.StartActivity(new Intent(Intent.ActionView, Uri.Parse("http://instagram.com/" + name)));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OpenYoutubeIntent(string channelId)
        {
            try
            {
                Intent likeIng = new Intent(Intent.ActionView, Uri.Parse("vnd.youtube://user/channel/" + channelId));

                try
                {
                    Context.StartActivity(likeIng);
                }
                catch (ActivityNotFoundException e)
                {
                    Console.WriteLine(e);
                    Context.StartActivity(new Intent(Intent.ActionView, Uri.Parse("http://www.youtube.com/" + channelId)));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OpenVkontakteIntent(string friendId)
        {
            try
            {
                Intent likeIng = new Intent(Intent.ActionView, Uri.Parse("vkontakte://profile/%d" + friendId));

                try
                {
                    Context.StartActivity(likeIng);
                }
                catch (ActivityNotFoundException e)
                {
                    Console.WriteLine(e);
                    Context.StartActivity(new Intent(Intent.ActionView, Uri.Parse("http://vk.com/" + friendId)));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

    }
}