using System;
using System.Collections.Generic;
using System.Linq;
using AFollestad.MaterialDialogs;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads.DoubleClick;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using Com.Theartofdev.Edmodo.Cropper;
using Java.Lang;
using PlayTube.Helpers.Ads;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Fonts;
using PlayTube.Helpers.Models;
using PlayTube.Helpers.Utils;
using PlayTube.SQLite;
using PlayTubeClient.Classes.Global;
using PlayTubeClient.RestCalls;
using Exception = System.Exception;
using File = Java.IO.File;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Uri = Android.Net.Uri;

namespace PlayTube.Activities.Channel
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class EditMyChannelActivity : AppCompatActivity, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    {
        #region Variables Basic
        private PublisherAdView PublisherAdView;
        private TextView NameIcon, GenderIcon, SaveTextView, FacebookIcon, TwitterIcon, GoogleIcon;
        private EditText TxtFirstName, TxtLastName, TxtUsername, TxtAboutChannal, TxtEmail, TxtFavCategory;
       
        private LinearLayout ImageAvatarLiner, ImageCoverLiner;
        private EditText TxtFacebook, TxtTwitter, TxtGoogle;
        private string GenderStatus, ImageAvatar, ImageCover, ImageType, CategoryId, CategoryName;
        private List<string> CategorySelect = new List<string>();
        private string IdGender;

        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                Methods.App.FullScreenApp(this);

                SetTheme(AppSettings.SetTabDarkTheme ? Resource.Style.MyTheme_Dark_Base : Resource.Style.MyTheme_Base);

                // Create your application here
                SetContentView(Resource.Layout.Edit_MyChannel_layout);

                //Get Value And Set Toolbar
                InitComponent();
                InitToolbar();
                
                Get_Data_User();

                Methods.Path.Chack_MyFolder();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                AddOrRemoveEvent(true);
                PublisherAdView?.Resume();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnPause()
        {
            try
            {
                base.OnPause();
                AddOrRemoveEvent(false);
                PublisherAdView?.Pause();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #region Functions

        private void InitComponent()
        {
            try
            {
                NameIcon = FindViewById<TextView>(Resource.Id.Name_icon);
                TxtFirstName = FindViewById<EditText>(Resource.Id.FirstName_text);
                TxtLastName = FindViewById<EditText>(Resource.Id.LastName_text);

              
                TxtUsername = FindViewById<EditText>(Resource.Id.username_Edit);

                SaveTextView = FindViewById<TextView>(Resource.Id.toolbar_title);
              

                TxtAboutChannal = FindViewById<EditText>(Resource.Id.AboutChannal_Edit);

              
                TxtEmail = FindViewById<EditText>(Resource.Id.email_Edit);

              
                TxtFavCategory = FindViewById<EditText>(Resource.Id.favCategory_Edit);
                 
                GenderIcon = FindViewById<TextView>(Resource.Id.gender_icon);
              

                ImageAvatarLiner = FindViewById<LinearLayout>(Resource.Id.ImageAvatarLiner);
              

                ImageCoverLiner = FindViewById<LinearLayout>(Resource.Id.ImageCoverLiner);
              

                FacebookIcon = FindViewById<TextView>(Resource.Id.facebook_icon);
                TxtFacebook = FindViewById<EditText>(Resource.Id.facebook_Edit);

                TwitterIcon = FindViewById<TextView>(Resource.Id.twitter_icon);
                TxtTwitter = FindViewById<EditText>(Resource.Id.twitter_Edit);

                GoogleIcon = FindViewById<TextView>(Resource.Id.google_icon);
                TxtGoogle = FindViewById<EditText>(Resource.Id.google_Edit);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, NameIcon, IonIconsFonts.Person);
                NameIcon.SetTextColor(Color.ParseColor("#8c8a8a"));


                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, FacebookIcon, IonIconsFonts.SocialFacebook);
                FacebookIcon.SetTextColor(Color.ParseColor("#3b5999"));

                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, TwitterIcon, IonIconsFonts.SocialTwitter);
                TwitterIcon.SetTextColor(Color.ParseColor("#55acee"));

                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, GoogleIcon, IonIconsFonts.SocialGoogle);
                GoogleIcon.SetTextColor(Color.ParseColor("#dd4b39"));

                PublisherAdView = FindViewById<PublisherAdView>(Resource.Id.multiple_ad_sizes_view);
                AdsGoogle.InitPublisherAdView(PublisherAdView);

                Methods.SetFocusable(TxtFavCategory);
                Methods.SetFocusable(GenderIcon);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void InitToolbar()
        {
            try
            {
               var toolBar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (toolBar != null)
                {
                    toolBar.Title = GetText(Resource.String.Lbl_edit_MyChannal);

                    SetSupportActionBar(toolBar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }  
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void AddOrRemoveEvent(bool addEvent)
        {
            try
            {
                // true +=  // false -=
                if (addEvent)
                {
                  
                    ImageAvatarLiner.Click += ImageAvatarLinerOnClick;
                    ImageCoverLiner.Click += ImageCoverLinerOnClick;
                    SaveTextView.Click += SaveTextViewOnClick;
                    TxtFavCategory.Touch += TxtFavCategoryOnTouch;
                    GenderIcon.Touch += GenderIcon_Click;
                }
                else
                {
                   
                    ImageAvatarLiner.Click -= ImageAvatarLinerOnClick;
                    ImageCoverLiner.Click -= ImageCoverLinerOnClick;
                    SaveTextView.Click -= SaveTextViewOnClick;
                    TxtFavCategory.Touch -= TxtFavCategoryOnTouch;
                    GenderIcon.Touch -= GenderIcon_Click;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void GenderIcon_Click(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e.Event.Action != MotionEventActions.Down)
                {
                    return;
                }

                List<string> arrayAdapter = new List<string>();
                MaterialDialog.Builder dialogList = new MaterialDialog.Builder(this).Theme(AppSettings.SetTabDarkTheme ? AFollestad.MaterialDialogs.Theme.Dark : AFollestad.MaterialDialogs.Theme.Light);

                arrayAdapter.Add(GetText(Resource.String.Radio_Male));
                arrayAdapter.Add(GetText(Resource.String.Radio_Female));

                dialogList.Title(GetText(Resource.String.Lbl_Gender));
                dialogList.Items(arrayAdapter);
                dialogList.PositiveText(GetText(Resource.String.Lbl_Cancel)).OnPositive(this);
                dialogList.AlwaysCallSingleChoiceCallback();
                dialogList.ItemsCallback(this).Build().Show();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void SaveTextViewOnClick(object sender, EventArgs e)
        {
            SaveDataButtonOnClick();
        }

        #endregion

        public void OnSelection(MaterialDialog p0, View p1, int itemId, ICharSequence itemstring)
        {
            try
            {
                GenderIcon.Text = itemstring.ToString();
                GenderStatus = itemId == 0 ? "male" : "female";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        private async void Get_Data_User()
        {
            try
            {
                if (ListUtils.MyChannelList.Count == 0)
                    await ApiRequest.GetChannelData(this,UserDetails.UserId);

                var local = ListUtils.MyChannelList.FirstOrDefault();
                if (local != null)
                {
                    if (local.Gender == "male" || local.Gender == "Male")
                    {
                       
                        GenderStatus = "male";
                        GenderIcon.Text = "Male";
                    }
                    else
                    {
                       
                        GenderStatus = "female";
                        GenderIcon.Text = "Female";
                    }

                    if (!string.IsNullOrEmpty(local.Username) || local.Username != "Empty")
                    {
                        TxtUsername.Text = local.Username;
                    }
                 
                    if (!string.IsNullOrEmpty(local.Email) || local.Email != "Empty")
                    {
                        TxtEmail.Text = local.Email;
                    }

                    if (!string.IsNullOrEmpty(local.FirstName)|| local.FirstName != "Empty")
                    {
                        TxtFirstName.Text = local.FirstName;
                    }

                    if (!string.IsNullOrEmpty(local.LastName)|| local.LastName != "Empty")
                    {
                        TxtLastName.Text = local.LastName;
                    }

                    if (!string.IsNullOrEmpty(local.About) || local.About != "Empty")
                    {
                        TxtAboutChannal.Text = local.About;
                    }

                    if (!string.IsNullOrEmpty(local.Facebook) || local.Facebook != "Empty")
                    {
                        TxtFacebook.Text = local.Facebook;
                    }

                    if (!string.IsNullOrEmpty(local.Google) || local.Google != "Empty")
                    {
                        TxtGoogle.Text = local.Google;
                    }

                    if (!string.IsNullOrEmpty(local.Twitter)|| local.Twitter != "Empty")
                    {
                        TxtTwitter.Text = local.Twitter;
                    }
                     
                    if (local.FavCategory.Count > 0)
                    {
                        CategorySelect = local.FavCategory;
                        foreach (var t in local.FavCategory)
                        {
                            CategoryId += t + ",";
                            CategoryName += CategoriesController.ListCategories.FirstOrDefault(q => q.Id == t)?.Name + ",";
                        }

                        TxtFavCategory.Text = CategoryName.Remove(CategoryName.Length - 1, 1); 
                    } 
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #region Events

      
    
 
        private async void SaveDataButtonOnClick()
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    //Show a progress
                    AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));

                    var dictionary = new Dictionary<string, string>
                    {
                        {"settings_type", "general"},
                        {"username", TxtUsername.Text},
                        {"email", TxtEmail.Text},
                        {"first_name", TxtFirstName.Text},
                        {"last_name", TxtLastName.Text},
                        {"about", TxtAboutChannal.Text},
                        {"facebook", TxtFacebook.Text},
                        {"twitter", TxtTwitter.Text},
                        {"google", TxtGoogle.Text},
                        {"gender", GenderStatus},
                        {"fav_category", CategoryId}, 
                    };
                      
                    var (apiResult, respond) = await RequestsAsync.Global.Update_UserData_General_Http(dictionary);
                    if (apiResult == 200)
                    { 
                        if (respond is MessageObject result)
                        {
                            Console.WriteLine(result.Message);
                            var local = ListUtils.MyChannelList.FirstOrDefault();
                            if (local != null)
                            {
                                local.Username = UserDetails.Username = TxtUsername.Text;
                                local.Email = UserDetails.Email = TxtEmail.Text;
                                local.FirstName = TxtFirstName.Text;
                                local.LastName = TxtLastName.Text;
                                local.About = TxtAboutChannal.Text;
                                local.Gender = GenderStatus;
                                local.Facebook = TxtFacebook.Text;
                                local.Twitter = TxtTwitter.Text;
                                local.Google = TxtGoogle.Text;
                                local.FavCategory = CategorySelect;
                                 
                                var database = new SqLiteDatabase();
                                database.InsertOrUpdate_DataMyChannel(local);
                                database.Dispose();
                            }
                             
                            Toast.MakeText(this, GetText(Resource.String.Lbl_Done),ToastLength.Short).Show();
                            AndHUD.Shared.Dismiss(this);

                            Intent intent = new Intent();
                            SetResult(Result.Ok, intent);
                            Finish();
                        } 
                    } 
                    else
                    {
                        Methods.DisplayReportResult(this, respond);
                        //Show a Error image with a message
                        if (respond is ErrorObject error)
                        {
                            AndHUD.Shared.ShowError(this, error.errors.ErrorText, MaskType.Clear, TimeSpan.FromSeconds(2));
                        }
                    }
                }
                else
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
                AndHUD.Shared.Dismiss(this);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                AndHUD.Shared.Dismiss(this);
            }
        }

        //Change Image Cover
        private void ImageCoverLinerOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                OpenDialogGallery("Cover");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Change Image Avatar
        private void ImageAvatarLinerOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                OpenDialogGallery("Avatar");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Favorite category
        private void TxtFavCategoryOnTouch(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e.Event.Action != MotionEventActions.Down) return;

                var arrayIndexAdapter = new int[] { };
                var dialogList = new MaterialDialog.Builder(this).Theme(AppSettings.SetTabDarkTheme ? AFollestad.MaterialDialogs.Theme.Dark : AFollestad.MaterialDialogs.Theme.Light);

                var arrayAdapter = CategoriesController.ListCategories.Select(item => Methods.FunString.DecodeString(item.Name)).ToList();

                dialogList.Title(GetText(Resource.String.Lbl_ChooseFavCategory))
                    .Items(arrayAdapter)
                    .ItemsCallbackMultiChoice(arrayIndexAdapter, OnSelection)
                    .AlwaysCallMultiChoiceCallback()
                    .PositiveText(GetText(Resource.String.Lbl_Close)).OnPositive(this)
                    .Build().Show(); 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        #endregion

        #region Permissions && Result

        protected override async void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);

                //If its from Camera or Gallery  
                if (requestCode == CropImage.CropImageActivityRequestCode)
                {
                    var result = CropImage.GetActivityResult(data);

                    if (resultCode == Result.Ok && result.IsSuccessful)
                    {
                        var resultUri = result.Uri;

                        if (!string.IsNullOrEmpty(resultUri.Path))
                        {
                            var ch = ListUtils.MyChannelList.FirstOrDefault();
                            switch (ImageType)
                            {
                                case "Cover":
                                    {
                                        ImageCover = resultUri.Path;
                                        if (ch != null) ch.Cover = ImageCover;
                                        UserDetails.Cover = resultUri.Path; 
                                        break;
                                    }
                                case "Avatar":
                                    {
                                        ImageAvatar = resultUri.Path;
                                        if (ch != null) ch.Avatar = ImageAvatar;
                                        UserDetails.Avatar = resultUri.Path;
                                        break;
                                    }
                            }

                            if (ch != null)
                            {
                                var sqlEntity = new SqLiteDatabase();
                                sqlEntity.InsertOrUpdate_DataMyChannel(ch);
                                sqlEntity.Dispose();
                            }

                            //Send image function
                            if (Methods.CheckConnectivity())
                            {
                                if (ImageType == "Avatar" || ImageType == "Cover")
                                {
                                    var (code, uploadAvatar) = await RequestsAsync.Global.Update_UserData_Image_Http(resultUri.Path, ImageType.ToLower()).ConfigureAwait(false);
                                    if (code == 200)
                                    {
                                        Console.WriteLine(uploadAvatar); 
                                    }
                                    else Methods.DisplayReportResult(this, uploadAvatar);

                                    RunOnUiThread(() =>
                                    {
                                        try
                                        {
                                            Toast.MakeText(this, GetText(Resource.String.Lbl_Image_changed_successfully), ToastLength.Short).Show();

                                            Intent intent = new Intent();
                                            SetResult(Result.Ok, intent);
                                            Finish();
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine(e);
                                        }
                                    });
                                }
                            }
                            else
                            {
                                Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                            }
                        }
                        else
                        {
                            Toast.MakeText(this, GetText(Resource.String.Lbl_something_went_wrong), ToastLength.Short).Show();
                        }
                    } 
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                if (requestCode == 108) //Image Picker
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        //Open Image 
                        OpenDialogGallery(ImageType);
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long).Show();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void OpenDialogGallery(string type)
        {
            try
            {
                ImageType = type;
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
                else
                {
                    // Check if we're running on Android 5.0 or higher
                    if ((int)Build.VERSION.SdkInt < 23)
                    {
                        Methods.Path.Chack_MyFolder();

                        //Open Image 
                        var myUri = Uri.FromFile(new File(Methods.Path.FolderDiskImage, Methods.GetTimestamp(DateTime.Now) + ".jpeg"));
                        CropImage.Builder()
                            .SetInitialCropWindowPaddingRatio(0)
                            .SetAutoZoomEnabled(true)
                            .SetMaxZoom(4)
                            .SetGuidelines(CropImageView.Guidelines.On)
                            .SetCropMenuCropButtonTitle(GetText(Resource.String.Lbl_Done))
                            .SetOutputUri(myUri).Start(this);
                    }
                    else
                    {
                        if (!CropImage.IsExplicitCameraPermissionRequired(this) && CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted &&
                            CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted && CheckSelfPermission(Manifest.Permission.Camera) == Permission.Granted)
                        {
                            Methods.Path.Chack_MyFolder();

                            //Open Image 
                            var myUri = Uri.FromFile(new File(Methods.Path.FolderDiskImage, Methods.GetTimestamp(DateTime.Now) + ".jpeg"));
                            CropImage.Builder()
                                .SetInitialCropWindowPaddingRatio(0)
                                .SetAutoZoomEnabled(true)
                                .SetMaxZoom(4)
                                .SetGuidelines(CropImageView.Guidelines.On)
                                .SetCropMenuCropButtonTitle(GetText(Resource.String.Lbl_Done))
                                .SetOutputUri(myUri).Start(this);
                        }
                        else
                        {
                            new PermissionsController(this).RequestPermission(108);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Menu

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
                    
            }
            return base.OnOptionsItemSelected(item);
        }

        #endregion
         
        #region MaterialDialog

        private bool OnSelection(MaterialDialog dialog, int[] which, string[] text)
        {
            try
            {
                CategoryId = "";
                CategoryName = "";
                CategorySelect = new List<string>();
                 
                foreach (var t in which)
                {
                    CategoryId += CategoriesController.ListCategories[t].Id + ",";
                    CategoryName += CategoriesController.ListCategories[t].Name + ",";

                    CategorySelect.Add(CategoryId);
                }

                TxtFavCategory.Text = CategoryName.Remove(CategoryName.Length - 1, 1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return true;
            }
            return true;
        }
         
        public void OnClick(MaterialDialog p0, DialogAction p1)
        {
            try
            {
                if (p1 == DialogAction.Positive)
                {
                }
                else if (p1 == DialogAction.Negative)
                {
                    p0.Dismiss();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion
         
        public override void OnTrimMemory(TrimMemory level)
        {
            try
            { 
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                base.OnTrimMemory(level);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public override void OnLowMemory()
        {
            try
            {
                GC.Collect(GC.MaxGeneration);
                base.OnLowMemory();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        protected override void OnDestroy()
        {
            try
            {
                PublisherAdView?.Destroy();
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}