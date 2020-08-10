//###############################################################
// Author >> Elin Doughouz 
// Copyright (c) PlayTube 12/07/2018 All Right Reserved
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// Follow me on facebook >> https://www.facebook.com/Elindoughous
//=========================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Preferences;
using Android.Views;
using Android.Widget;
using Java.Lang;
using PlayTube.Activities.Channel;
using PlayTube.Activities.Models;
using PlayTube.Activities.Upgrade;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Models;
using PlayTube.Helpers.Utils;
using PlayTubeClient;
using PlayTubeClient.RestCalls;
using Boolean = System.Boolean;
using Exception = System.Exception;

namespace PlayTube.Activities.SettingsPreferences
{
    public class SettingsPrefsFragment : PreferenceFragmentCompat, ISharedPreferencesOnSharedPreferenceChangeListener, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    {
        #region Variables Preference

        private Preference EditChannel, EditPassword, Monetization, Verification, BlockedUsersPref, ClaerHistory, ClearCache, Help, Termsofuse, About, DeleteAccount, Logout, GoPro, TwoFactorPref, ManageSessionsPref, NightMode;
        private SwitchPreferenceCompat PictureInPicturePerf;
        //private ListPreference Lang;
        private readonly Activity ActivityContext;
        private string SNightModePref;
        private static bool SPictureInPicture;
        #endregion

        public SettingsPrefsFragment(Activity activity)
        {
            try
            {
                ActivityContext = activity;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #region General

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                // create ContextThemeWrapper from the original Activity Context with the custom theme
                Context contextThemeWrapper = AppSettings.SetTabDarkTheme ? new ContextThemeWrapper(ActivityContext, Resource.Style.SettingsThemeDark) : new ContextThemeWrapper(ActivityContext, Resource.Style.SettingsTheme);

                // clone the inflater using the ContextThemeWrapper
                LayoutInflater localInflater = inflater.CloneInContext(contextThemeWrapper);

                View view = base.OnCreateView(localInflater, container, savedInstanceState);

                return view;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }

        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            try
            {
                AddPreferencesFromResource(Resource.Xml.SettingsPrefs);

                MainSettings.SharedData = PreferenceManager.SharedPreferences;

                EditChannel = FindPreference("editChannal_key");
                EditPassword = FindPreference("editPassword_key");
                Monetization = FindPreference("monetization_key");
                Verification = FindPreference("verification_key");
                GoPro = FindPreference("goPro_key");
                TwoFactorPref = FindPreference("Twofactor_key");
                ManageSessionsPref = FindPreference("ManageSessions_key");
                BlockedUsersPref = FindPreference("blocked_key");

                PictureInPicturePerf = (SwitchPreferenceCompat)FindPreference("picture_in_picture_key");

                ClaerHistory = FindPreference("ClaerHistory_key");

                ClearCache = FindPreference("Clear_Cache_key");
                //Lang = (ListPreference)FindPreference("Lang_key");
                NightMode = FindPreference("Night_Mode_key");
                Help = FindPreference("help_key");
                Termsofuse = FindPreference("Termsofuse_key");
                About = FindPreference("About_key");
                DeleteAccount = FindPreference("deleteaccount_key");
                Logout = FindPreference("logout_key");

                OnSharedPreferenceChanged(MainSettings.SharedData, "Night_Mode_key");

                //Delete Preference
                //============== Account_Profile_key ===================
                var mCategoryAccount = (PreferenceCategory)FindPreference("Account_Profile_key");

                if (!AppSettings.ShowMonetization)
                    mCategoryAccount.RemovePreference(Monetization);

                var isPro = ListUtils.MyChannelList.FirstOrDefault()?.IsPro ?? "0";
                if (!AppSettings.ShowGoPro || isPro != "0")
                    mCategoryAccount.RemovePreference(GoPro);

                if (!AppSettings.ShowVerification)
                    mCategoryAccount.RemovePreference(Verification);

                if (!AppSettings.ShowBlockedUsers)
                    mCategoryAccount.RemovePreference(BlockedUsersPref);

                //============== SecurityAccount_key ===================
                var mCategorySecurity = (PreferenceCategory)FindPreference("SecurityAccount_key");
                if (!AppSettings.ShowEditPassword)
                    mCategorySecurity.RemovePreference(EditPassword);

                if (!AppSettings.ShowSettingsTwoFactor)
                    mCategorySecurity.RemovePreference(TwoFactorPref);

                if (!AppSettings.ShowSettingsManageSessions)
                    mCategorySecurity.RemovePreference(ManageSessionsPref);

                //============== CategoryGeneral_key ===================
                PreferenceCategory mCategoryGeneral = (PreferenceCategory)FindPreference("CategoryGeneral_key");
                if ((int)Build.VERSION.SdkInt <= 23)
                    mCategoryGeneral.RemovePreference(PictureInPicturePerf);

                //============== History_Privacy_key ===================
                PreferenceCategory mHistoryPrivacy = (PreferenceCategory)FindPreference("History_Privacy_key");

                if (!AppSettings.ShowClearHistory)
                    mHistoryPrivacy.RemovePreference(ClaerHistory);

                if (!AppSettings.ShowClearCache)
                    mHistoryPrivacy.RemovePreference(ClearCache);

                //============== Support_key ===================
                PreferenceCategory mCategorySupport = (PreferenceCategory)FindPreference("Support_key");

                if (!AppSettings.ShowHelp)
                    mCategorySupport.RemovePreference(Help);

                if (!AppSettings.ShowTermsOfUse)
                    mCategorySupport.RemovePreference(Termsofuse);

                if (!AppSettings.ShowAbout)
                    mCategorySupport.RemovePreference(About);

                if (!AppSettings.ShowDeleteAccount)
                    mCategorySupport.RemovePreference(DeleteAccount);

                NightMode.IconSpaceReserved = false;
                PictureInPicturePerf.IconSpaceReserved = false;

                PictureInPicturePerf.Checked = UserDetails.PipIsChecked;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public override void OnResume()
        {
            try
            {
                base.OnResume();
                PreferenceManager.SharedPreferences.RegisterOnSharedPreferenceChangeListener(this);
                AddOrRemoveEvent(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnPause()
        {
            try
            {
                base.OnPause();
                PreferenceManager.SharedPreferences.UnregisterOnSharedPreferenceChangeListener(this);
                AddOrRemoveEvent(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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

        private void AddOrRemoveEvent(bool addEvent)
        {
            try
            {
                // true +=  // false -=
                if (addEvent)
                {
                    EditChannel.PreferenceClick += EditChannelOnPreferenceClick;
                    EditPassword.PreferenceClick += EditPasswordOnPreferenceClick;
                    Monetization.PreferenceClick += MonetizationOnPreferenceClick;
                    Verification.PreferenceClick += VerificationOnPreferenceClick;
                    ClaerHistory.PreferenceClick += ClaerHistoryOnPreferenceClick;
                    BlockedUsersPref.PreferenceClick += BlockedUsersPrefOnPreferenceClick;
                    ClearCache.PreferenceClick += ClearCacheOnPreferenceClick;
                    //Lang.PreferenceChange += LangOnPreferenceChange;
                    Help.PreferenceClick += HelpOnPreferenceClick;
                    Termsofuse.PreferenceClick += TermsofuseOnPreferenceClick;
                    About.PreferenceClick += AboutOnPreferenceClick;
                    DeleteAccount.PreferenceClick += DeleteAccountOnPreferenceClick;
                    Logout.PreferenceClick += LogoutOnPreferenceClick;
                    GoPro.PreferenceClick += GoProOnPreferenceClick;
                    ManageSessionsPref.PreferenceClick += ManageSessionsPrefOnPreferenceClick;
                    TwoFactorPref.PreferenceClick += TwoFactorPrefOnPreferenceClick;
                    PictureInPicturePerf.PreferenceChange += PictureInPicturePerfOnPreferenceChange;
                }
                else
                {
                    EditChannel.PreferenceClick -= EditChannelOnPreferenceClick;
                    EditPassword.PreferenceClick -= EditPasswordOnPreferenceClick;
                    Monetization.PreferenceClick -= MonetizationOnPreferenceClick;
                    Verification.PreferenceClick -= VerificationOnPreferenceClick;
                    ClaerHistory.PreferenceClick -= ClaerHistoryOnPreferenceClick;
                    BlockedUsersPref.PreferenceClick -= BlockedUsersPrefOnPreferenceClick;
                    ClearCache.PreferenceClick -= ClearCacheOnPreferenceClick;
                    //Lang.PreferenceChange -= LangOnPreferenceChange;
                    Help.PreferenceClick -= HelpOnPreferenceClick;
                    Termsofuse.PreferenceClick -= TermsofuseOnPreferenceClick;
                    About.PreferenceClick -= AboutOnPreferenceClick;
                    DeleteAccount.PreferenceClick -= DeleteAccountOnPreferenceClick;
                    Logout.PreferenceClick -= LogoutOnPreferenceClick;
                    GoPro.PreferenceClick -= GoProOnPreferenceClick;
                    ManageSessionsPref.PreferenceClick -= ManageSessionsPrefOnPreferenceClick;
                    TwoFactorPref.PreferenceClick -= TwoFactorPrefOnPreferenceClick;
                    PictureInPicturePerf.PreferenceChange -= PictureInPicturePerfOnPreferenceChange;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion


        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
            try
            {
                if (key.Equals("Night_Mode_key"))
                {
                    // Set summary to be the user-description for the selected value
                    Preference etp = FindPreference("Night_Mode_key");

                    string getValue = MainSettings.SharedData.GetString("Night_Mode_key", string.Empty);
                    if (getValue == MainSettings.LightMode)
                    {
                        etp.Summary = ActivityContext.GetString(Resource.String.Lbl_Light);
                    }
                    else if (getValue == MainSettings.DarkMode)
                    {
                        etp.Summary = ActivityContext.GetString(Resource.String.Lbl_Dark);
                    }
                    else if (getValue == MainSettings.DefaultMode)
                    {
                        etp.Summary = ActivityContext.GetString(Resource.String.Lbl_SetByBattery);
                    }
                    else
                    {
                        etp.Summary = getValue;
                    }
                }
                else if (key.Equals("picture_in_picture_key"))
                {
                    bool getValue = MainSettings.SharedData.GetBoolean("picture_in_picture_key", false);
                    PictureInPicturePerf.Checked = getValue;
                    SPictureInPicture = getValue;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        //Account >> password , Edit info my Channal , Monetization , Verification , ManageSessions ,TwoFactor
        //===================================================
        private void EditPasswordOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs preferenceClickEventArgs)
        {
            try
            {
                var intent = new Intent(ActivityContext, typeof(PasswordActivity));
                ActivityContext.StartActivity(intent);

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void GoProOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                var intent = new Intent(ActivityContext, typeof(GoProActivity));
                ActivityContext.StartActivity(intent);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        private void EditChannelOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs preferenceClickEventArgs)
        {
            try
            {
                var intent = new Intent(ActivityContext, typeof(EditMyChannelActivity));
                ActivityContext.StartActivity(intent);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void MonetizationOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                var intent = new Intent(ActivityContext, typeof(MonetizationActivity));
                ActivityContext.StartActivity(intent);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void VerificationOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                var intent = new Intent(ActivityContext, typeof(VerificationActivity));
                ActivityContext.StartActivity(intent);

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        private void ManageSessionsPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                var intent = new Intent(ActivityContext, typeof(ManageSessionsActivity));
                ActivityContext.StartActivity(intent);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        private void TwoFactorPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                var intent = new Intent(ActivityContext, typeof(TwoFactorAuthActivity));
                ActivityContext.StartActivity(intent);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Privacy >> Clear 
        //===================================================
        private void ClearCacheOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs preferenceClickEventArgs)
        {
            try
            {
                var dialog = new MaterialDialog.Builder(ActivityContext).Theme(AppSettings.SetTabDarkTheme ? Theme.Dark : Theme.Light);

                dialog.Title(GetText(Resource.String.Lbl_Warning));
                dialog.Content(GetText(Resource.String.Lbl_TheFilesWillBeDeleted));
                dialog.PositiveText(GetText(Resource.String.Lbl_Yes)).OnPositive((materialDialog, action) =>
                {
                    Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_FilesAreNowDeleted), ToastLength.Long).Show();

                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            var dirPath = ActivityContext.CacheDir;
                            dirPath.Delete();

                            string path = Methods.Path.FolderDcimMyApp;
                            if (Directory.Exists(path))
                            {
                                Directory.Delete(path, true);
                            }

                            Methods.Path.Chack_MyFolder();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    });
                });
                dialog.NegativeText(GetText(Resource.String.Lbl_No)).OnNegative(this);
                dialog.AlwaysCallSingleChoiceCallback();
                dialog.Build().Show();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void ClaerHistoryOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs preferenceClickEventArgs)
        {
            try
            {
                new LibrarySynchronizer(ActivityContext).RemoveRecentlyWatched();

                if (Methods.CheckConnectivity())
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Video.Delete_History_Videos_Http() });
                else
                    Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();

                Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_Done), ToastLength.Long).Show();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Support >> Logout , DeleteAccount , Report , Help 
        //===================================================
        private void LogoutOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs preferenceClickEventArgs)
        {
            try
            {
                PopupDialogController dialog = new PopupDialogController(ActivityContext, null, "Logout");
                dialog.ShowNormalDialog(ActivityContext.GetText(Resource.String.Lbl_Warning), ActivityContext.GetText(Resource.String.Lbl_Are_you_sure_you_went_to_logout), ActivityContext.GetText(Resource.String.Lbl_Ok), ActivityContext.GetText(Resource.String.Lbl_Cancel));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void DeleteAccountOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs preferenceClickEventArgs)
        {
            try
            {
                PopupDialogController dialog = new PopupDialogController(ActivityContext, null, "DeleteAcount");
                dialog.ShowNormalDialog(ActivityContext.GetText(Resource.String.Lbl_Warning), ActivityContext.GetText(Resource.String.Lbl_Are_you_sure_you_went_to_delete_account) + " " + AppSettings.ApplicationName, ActivityContext.GetText(Resource.String.Lbl_Ok), ActivityContext.GetText(Resource.String.Lbl_Cancel));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void TermsofuseOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs preferenceClickEventArgs)
        {
            try
            {
                var intent = new Intent(ActivityContext, typeof(LocalWebViewActivity));
                intent.PutExtra("URL", Client.WebsiteUrl + "/terms/terms");
                intent.PutExtra("Type", ActivityContext.GetText(Resource.String.Lbl_Termsofuse));
                ActivityContext.StartActivity(intent);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void HelpOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs preferenceClickEventArgs)
        {
            try
            {
                var intent = new Intent(ActivityContext, typeof(LocalWebViewActivity));
                intent.PutExtra("URL", Client.WebsiteUrl + "/contact-us");
                intent.PutExtra("Type", ActivityContext.GetText(Resource.String.Lbl_Help));
                ActivityContext.StartActivity(intent);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void AboutOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs preferenceClickEventArgs)
        {
            try
            {
                var intent = new Intent(ActivityContext, typeof(LocalWebViewActivity));
                intent.PutExtra("URL", Client.WebsiteUrl + "/terms/about-us");
                intent.PutExtra("Type", ActivityContext.GetText(Resource.String.Lbl_about));
                ActivityContext.StartActivity(intent);

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //BlockedUsers
        private void BlockedUsersPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                ActivityContext.StartActivity(new Intent(ActivityContext, typeof(BlockedUsersActivity)));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //General >> PictureInPicture
        private void PictureInPicturePerfOnPreferenceChange(object sender, Preference.PreferenceChangeEventArgs e)
        {
            try
            {
                if (e.Handled)
                {
                    SwitchPreferenceCompat etp = (SwitchPreferenceCompat)sender;
                    var value = e.NewValue.ToString();
                    etp.Checked = Boolean.Parse(value);
                    SPictureInPicture = etp.Checked;
                    UserDetails.PipIsChecked = etp.Checked;

                    if (!AppTools.CheckPictureInPictureAllowed(ActivityContext) && SPictureInPicture)
                    {
                        var intent = new Intent("android.settings.PICTURE_IN_PICTURE_SETTINGS", Android.Net.Uri.Parse("package:" + ActivityContext.PackageName));
                        ActivityContext.StartActivityForResult(intent, 8520);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //General >> Lang  
        //================================
        //private void LangOnPreferenceChange(object sender, Preference.PreferenceChangeEventArgs e)
        //{
        //    try
        //    {
        //        if (e.Handled)
        //        {
        //            ListPreference etp = (ListPreference)sender;
        //            var value = e.NewValue;

        //            LangController.SetAppLanguage(Context,value.ToString());

        //            AppSettings.Lang = value.ToString();

        //            var userlang = ListUtils.DataUserLoginList.FirstOrDefault(a => a.UserId == UserDetails.UserId);
        //            if (userlang != null)
        //            {
        //                userlang.Lang = value.ToString();

        //                var sqlEntity = new SqLiteDatabase();
        //                sqlEntity.InsertOrUpdateLogin_Credentials(userlang);
        //                sqlEntity.Dispose();
        //            }

        //            Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_Closed_App), ToastLength.Long).Show();

        //            Intent intent = new Intent(ActivityContext, typeof(SplashScreenActivity));
        //            intent.AddCategory(Intent.CategoryHome);
        //            intent.SetAction(Intent.ActionMain);
        //            intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.ClearTask);
        //            ActivityContext.StartActivity(intent);
        //            ActivityContext.FinishAffinity();
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        Console.WriteLine(exception);
        //    }
        //}

        #region MaterialDialog

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

        public void OnSelection(MaterialDialog p0, View p1, int itemId, ICharSequence itemString)
        {
            try
            {
                string text = itemString.ToString();

                string getValue = MainSettings.SharedData.GetString("Night_Mode_key", string.Empty);

                if (text == GetString(Resource.String.Lbl_Light) && getValue != MainSettings.LightMode)
                {
                    //Set Light Mode   
                    NightMode.Summary = ActivityContext.GetString(Resource.String.Lbl_Light);

                    AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;
                    AppSettings.SetTabDarkTheme = false;
                    MainSettings.SharedData.Edit().PutString("Night_Mode_key", MainSettings.LightMode).Commit();

                    if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                    {
                        ActivityContext.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                        ActivityContext.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                    }

                    Intent intent = new Intent(ActivityContext, typeof(SplashScreenActivity));
                    intent.AddCategory(Intent.CategoryHome);
                    intent.SetAction(Intent.ActionMain);
                    intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.ClearTask);
                    ActivityContext.StartActivity(intent);
                    ActivityContext.FinishAffinity();
                }
                else if (text == GetString(Resource.String.Lbl_Dark) && getValue != MainSettings.DarkMode)
                {
                    NightMode.Summary = ActivityContext.GetString(Resource.String.Lbl_Dark);

                    AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightYes;
                    AppSettings.SetTabDarkTheme = true;
                    MainSettings.SharedData.Edit().PutString("Night_Mode_key", MainSettings.DarkMode).Commit();

                    if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                    {
                        ActivityContext.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                        ActivityContext.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                    }

                    Intent intent = new Intent(ActivityContext, typeof(SplashScreenActivity));
                    intent.AddCategory(Intent.CategoryHome);
                    intent.SetAction(Intent.ActionMain);
                    intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.ClearTask);
                    ActivityContext.StartActivity(intent);
                    ActivityContext.FinishAffinity();
                }
                else if (text == GetString(Resource.String.Lbl_SetByBattery) && getValue != MainSettings.DefaultMode)
                {
                    NightMode.Summary = ActivityContext.GetString(Resource.String.Lbl_SetByBattery);
                    MainSettings.SharedData.Edit().PutString("Night_Mode_key", MainSettings.DefaultMode).Commit();

                    if ((int)Build.VERSION.SdkInt >= 29)
                    {
                        AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightFollowSystem;

                        var currentNightMode = Resources.Configuration.UiMode & UiMode.NightMask;
                        switch (currentNightMode)
                        {
                            case UiMode.NightNo:
                                // Night mode is not active, we're using the light theme
                                AppSettings.SetTabDarkTheme = false;
                                break;
                            case UiMode.NightYes:
                                // Night mode is active, we're using dark theme
                                AppSettings.SetTabDarkTheme = true;
                                break;
                        }
                    }
                    else
                    {
                        AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightAuto;

                        var currentNightMode = Resources.Configuration.UiMode & UiMode.NightMask;
                        switch (currentNightMode)
                        {
                            case UiMode.NightNo:
                                // Night mode is not active, we're using the light theme
                                AppSettings.SetTabDarkTheme = false;
                                break;
                            case UiMode.NightYes:
                                // Night mode is active, we're using dark theme
                                AppSettings.SetTabDarkTheme = true;
                                break;
                        }

                        if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                        {
                            ActivityContext.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                            ActivityContext.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                        }

                        Intent intent = new Intent(ActivityContext, typeof(SplashScreenActivity));
                        intent.AddCategory(Intent.CategoryHome);
                        intent.SetAction(Intent.ActionMain);
                        intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.ClearTask);
                        ActivityContext.StartActivity(intent);
                        ActivityContext.FinishAffinity();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        public override bool OnPreferenceTreeClick(Preference preference)
        {
            try
            {
                if (preference.Key == "Night_Mode_key")
                {
                    var arrayAdapter = new List<string>();
                    var dialogList = new MaterialDialog.Builder(ActivityContext).Theme(AppSettings.SetTabDarkTheme ? Theme.Dark : Theme.Light);

                    dialogList.Title(Resource.String.Lbl_Night_Mode);

                    arrayAdapter.Add(GetText(Resource.String.Lbl_Light));
                    arrayAdapter.Add(GetText(Resource.String.Lbl_Dark));

                    if ((int)Build.VERSION.SdkInt >= 29)
                        arrayAdapter.Add(GetText(Resource.String.Lbl_SetByBattery));

                    dialogList.Items(arrayAdapter);
                    dialogList.PositiveText(GetText(Resource.String.Lbl_Close)).OnPositive(this);
                    dialogList.AlwaysCallSingleChoiceCallback();
                    dialogList.ItemsCallback(this).Build().Show();
                }

                return base.OnPreferenceTreeClick(preference);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return base.OnPreferenceTreeClick(preference);
            }
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);

                if (requestCode == 8520 && AppTools.CheckPictureInPictureAllowed(ActivityContext))
                {
                    SPictureInPicture = true;
                    UserDetails.PipIsChecked = true;
                    PictureInPicturePerf.Checked = true;
                    MainSettings.SharedData.Edit().PutBoolean("picture_in_picture_key", UserDetails.PipIsChecked).Commit();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}