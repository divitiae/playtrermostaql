using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Java.Lang;
using PlayTube.Activities.Tabbes;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Models;
using PlayTube.Helpers.Utils;
using PlayTube.SQLite;
using PlayTubeClient;
using PlayTubeClient.Classes.Auth;
using PlayTubeClient.Classes.Global;
using PlayTubeClient.RestCalls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Exception = System.Exception;


namespace PlayTube.Activities.Default
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class RegisterActivity : AppCompatActivity, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    {
        #region Variables Basic

        private Button RegisterButton;
        private EditText EmailEditext, UsernameEditext, PasswordEditext, PasswordRepeatEditext, GenderEditext;
        private LinearLayout MainLinearLayout;
        private ProgressBar ProgressBar;
        private TextView SecTermTextView, SecPrivacyTextView;
        private CheckBox ChkAgree;
        private string IdGender;
        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                Methods.App.FullScreenApp(this);

                SetTheme(AppSettings.SetTabDarkTheme ? Resource.Style.MyTheme_Dark_Base : Resource.Style.MyTheme_Base);

                // Create your application here 
                SetContentView(Resource.Layout.Register_Layout);

                //Get Value And Set Toolbar
                InitComponent();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                AddOrRemoveEvent(true);
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
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            try
            {
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                base.OnTrimMemory(level);
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
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Functions

        private void InitComponent()
        {
            try
            {
                EmailEditext = FindViewById<EditText>(Resource.Id.emailfield);
                UsernameEditext = FindViewById<EditText>(Resource.Id.usernamefield);
                GenderEditext = FindViewById<EditText>(Resource.Id.genderfield);
                PasswordEditext = FindViewById<EditText>(Resource.Id.passwordfield);
                PasswordRepeatEditext = FindViewById<EditText>(Resource.Id.ConfirmPasswordfield);
                MainLinearLayout = FindViewById<LinearLayout>(Resource.Id.mainLinearLayout);
                RegisterButton = FindViewById<Button>(Resource.Id.signUpButton);
                SecTermTextView = FindViewById<TextView>(Resource.Id.secTermTextView);
                SecPrivacyTextView = FindViewById<TextView>(Resource.Id.secPrivacyTextView);
                ChkAgree = FindViewById<CheckBox>(Resource.Id.termCheckBox);

                ProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
                ProgressBar.Visibility = ViewStates.Invisible;

                Methods.SetFocusable(GenderEditext);
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
                    MainLinearLayout.Click += MainLinearLayoutOnClick;
                    RegisterButton.Click += RegisterButtonOnClick;
                    SecTermTextView.Click += SecTermTextView_Click;
                    SecPrivacyTextView.Click += SecPrivacyTextView_Click;
                    GenderEditext.Touch += GenderditextOnTouch;
                }
                else
                {
                    MainLinearLayout.Click -= MainLinearLayoutOnClick;
                    RegisterButton.Click -= RegisterButtonOnClick;
                    SecTermTextView.Click -= SecTermTextView_Click;
                    SecPrivacyTextView.Click -= SecPrivacyTextView_Click;
                    GenderEditext.Touch -= GenderditextOnTouch;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Events

        private async void RegisterButtonOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!ChkAgree.Checked)
                {
                    ProgressBar.Visibility = ViewStates.Invisible;
                    Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Warning), GetText(Resource.String.Lbl_You_can_not_access_your_disapproval),GetText(Resource.String.Lbl_Ok));
                }
                else
                {
                    if (!Methods.CheckConnectivity())
                    {
                        ProgressBar.Visibility = ViewStates.Invisible;
                        Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security),GetText(Resource.String.Lbl_CheckYourInternetConnection), GetText(Resource.String.Lbl_Ok));
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(UsernameEditext.Text.Replace(" ", "")) || !string.IsNullOrEmpty(PasswordEditext.Text.Replace(" ", ""))
                           || !string.IsNullOrEmpty(GenderEditext.Text.Replace(" ", "")) || !string.IsNullOrEmpty(PasswordRepeatEditext.Text.Replace(" ", "")) 
                           ||!string.IsNullOrEmpty(EmailEditext.Text.Replace(" ", "")))
                        {
                            bool check = Methods.FunString.IsEmailValid(EmailEditext.Text);
                            if (!check)
                            {
                                Methods.DialogPopup.InvokeAndShowDialog(this,GetText(Resource.String.Lbl_VerificationFailed),GetText(Resource.String.Lbl_IsEmailValid), GetText(Resource.String.Lbl_Ok));
                            }
                            else
                            {
                                if (PasswordRepeatEditext.Text == PasswordEditext.Text)
                                {
                                    ProgressBar.Visibility = ViewStates.Visible;

                                    (int apiStatus, dynamic respond) = await RequestsAsync.Global.Registration_Http(EmailEditext.Text.Replace(" ", ""), UsernameEditext.Text.Replace(" ", ""),PasswordEditext.Text, PasswordRepeatEditext.Text, IdGender,UserDetails.DeviceId);
                                    if (apiStatus == 200)
                                    {
                                        if (respond is RegisterObject result)
                                        {
                                            SetDataLogin(result);

                                            UserDetails.IsLogin = true;

                                            StartActivity(new Intent(this, typeof(TabbedMainActivity)));

                                            ProgressBar.Visibility = ViewStates.Invisible;
                                            FinishAffinity();
                                        }
                                        else if (respond is MessageObject message)
                                        {
                                            ProgressBar.Visibility = ViewStates.Invisible;
                                            Methods.DialogPopup.InvokeAndShowDialog(this,GetText(Resource.String.Lbl_Security),message.Message.Contains("We have sent you an email")? GetString(Resource.String.Lbl_VerifyRegistration): message.Message, GetText(Resource.String.Lbl_Ok));
                                        }
                                    }
                                    else if (apiStatus == 400)
                                    {
                                        if (respond is ErrorObject error)
                                        {
                                            ProgressBar.Visibility = ViewStates.Invisible;
                                            Methods.DialogPopup.InvokeAndShowDialog(this,GetText(Resource.String.Lbl_Security), error.errors.ErrorText,GetText(Resource.String.Lbl_Ok));
                                        }
                                    }
                                    else
                                    {
                                        ProgressBar.Visibility = ViewStates.Invisible;
                                        Methods.DialogPopup.InvokeAndShowDialog(this,GetText(Resource.String.Lbl_Security), respond.ToString(),GetText(Resource.String.Lbl_Ok));
                                    }
                                }
                                else
                                {
                                    ProgressBar.Visibility = ViewStates.Invisible;
                                    Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security),GetText(Resource.String.Lbl_Error_Register_password),GetText(Resource.String.Lbl_Ok));
                                }
                            }
                        }
                        else
                        {
                            ProgressBar.Visibility = ViewStates.Invisible;
                            Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security),GetText(Resource.String.Lbl_Please_enter_your_data), GetText(Resource.String.Lbl_Ok));
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                ProgressBar.Visibility = ViewStates.Invisible;
                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), exception.Message, GetText(Resource.String.Lbl_Ok));
            }
        }

        private void GenderditextOnTouch(object sender, View.TouchEventArgs e)
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


        private void MainLinearLayoutOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                InputMethodManager inputManager = (InputMethodManager)GetSystemService(InputMethodService);
                inputManager.HideSoftInputFromWindow(CurrentFocus.WindowToken, HideSoftInputFlags.None);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void SecPrivacyTextView_Click(object sender, EventArgs e)
        {
            try
            {
                string url = Client.WebsiteUrl + "/terms/privacy-policy";
                Methods.App.OpenbrowserUrl(this, url);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void SecTermTextView_Click(object sender, EventArgs e)
        {
            try
            {
                string url = Client.WebsiteUrl + "/terms/terms";
                Methods.App.OpenbrowserUrl(this, url);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        private void SetDataLogin(RegisterObject auth)
        {
            try
            {
                UserDetails.Username = UsernameEditext.Text;
                UserDetails.FullName = UsernameEditext.Text;
                UserDetails.Password = PasswordEditext.Text;
                UserDetails.AccessToken = Current.AccessToken = auth.User.S;
                UserDetails.UserId = Client.UserId = auth.User.UserId.ToString();
                UserDetails.Status = "Active";
                UserDetails.Cookie = auth.User.Cookie;
                UserDetails.Email = EmailEditext.Text;

                //Insert user data to database
                DataTables.LoginTb user = new DataTables.LoginTb
                {
                    UserId = UserDetails.UserId,
                    AccessToken = UserDetails.AccessToken,
                    Cookie = UserDetails.Cookie,
                    Username = UsernameEditext.Text,
                    Password = PasswordEditext.Text,
                    Status = "Active",
                    Lang = "",
                    Email = EmailEditext.Text,
                    DeviceId = UserDetails.DeviceId,
                };
                ListUtils.DataUserLoginList.Clear();
                ListUtils.DataUserLoginList.Add(user);

                SqLiteDatabase dbDatabase = new SqLiteDatabase();
                dbDatabase.InsertOrUpdateLogin_Credentials(user);
                dbDatabase.Dispose();

                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => ApiRequest.GetChannelData(this, UserDetails.UserId) });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #region MaterialDialog

        public void OnSelection(MaterialDialog p0, View p1, int itemId, ICharSequence itemstring)
        {
            try
            {
                GenderEditext.Text = itemstring.ToString();
                IdGender = itemId == 0 ? "male" : "female";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
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

    }
}