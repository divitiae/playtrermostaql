using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Java.Lang;
using PlayTube.Activities.Upgrade.Adapters;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Utils;
using PlayTube.Payment;
using PlayTube.PaymentGoogle;
using PlayTube.SQLite;
using PlayTubeClient;
using PlayTubeClient.RestCalls;
using Xamarin.PayPal.Android;
using Exception = System.Exception;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace PlayTube.Activities.Upgrade
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Keyboard | ConfigChanges.Orientation | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenLayout | ConfigChanges.ScreenSize | ConfigChanges.SmallestScreenSize | ConfigChanges.UiMode | ConfigChanges.Locale)]
    public class GoProActivity : AppCompatActivity, MaterialDialog.ISingleButtonCallback, MaterialDialog.IListCallback
    {
        #region Variables Basic

        private InitPayPalPayment InitPayPalPayment;
        private RecyclerView MainPlansRecyclerView;
        private LinearLayoutManager PlansLayoutManagerView;
        private UpgradeGoProAdapter PlansAdapter;
        private TextView HeadText;
        private InitInAppBillingPayment BillingPayment;

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetTheme(AppSettings.SetTabDarkTheme ? Resource.Style.MyTheme_Dark_Base : Resource.Style.MyTheme_Base);

                Methods.App.FullScreenApp(this);

                // Create your application here
                SetContentView(Resource.Layout.Go_Pro_Layout);

                if (AppSettings.ShowInAppBilling && Client.IsExtended)
                {
                    BillingPayment = new InitInAppBillingPayment(this);
                }

                if (AppSettings.ShowPaypal)
                {
                    InitPayPalPayment = new InitPayPalPayment(this);
                }

                //Get Value And Set Toolbar
                InitComponent();
                InitToolbar();
                SetRecyclerViewAdapters();
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

        protected override void OnDestroy()
        {
            try
            {
                InitPayPalPayment?.StopPayPalService();
                BillingPayment?.DisconnectInAppBilling();
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
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

        #region Functions

        private void InitComponent()
        {
            try
            {
                MainPlansRecyclerView = FindViewById<RecyclerView>(Resource.Id.recycler2);
                HeadText = FindViewById<TextView>(Resource.Id.headText);

                HeadText.Text = GetText(Resource.String.Lbl_Title_Pro1) + " " + AppSettings.ApplicationName + " " + GetText(Resource.String.Lbl_Title_Pro2);
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
                var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (toolbar != null)
                {
                    toolbar.Title = GetText(Resource.String.Lbl_Go_Pro);
                    toolbar.SetTitleTextColor(Color.White);
                    SetSupportActionBar(toolbar);
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

        private void SetRecyclerViewAdapters()
        {
            try
            {
                PlansAdapter = new UpgradeGoProAdapter(this);
                PlansLayoutManagerView = new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false);
                MainPlansRecyclerView.SetLayoutManager(PlansLayoutManagerView);
                MainPlansRecyclerView.HasFixedSize = true;
                MainPlansRecyclerView.SetAdapter(PlansAdapter);
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
                    PlansAdapter.UpgradeButtonItemClick += UpgradeButtonOnClick;
                }
                else
                {
                    PlansAdapter.UpgradeButtonItemClick -= UpgradeButtonOnClick;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Events

        private void UpgradeButtonOnClick(object sender, UpgradeGoProAdapterClickEventArgs e)
        {
            try
            {
                if (e.Position > -1)
                {
                    var item = PlansAdapter.GetItem(e.Position);
                    if (item != null)
                    {
                        if (item.PlanPrice == "0")
                        {
                            Finish();
                        }
                        else
                        {
                            if (AppSettings.ShowInAppBilling && Client.IsExtended)
                            {
                                BillingPayment?.SetConnInAppBilling();
                                BillingPayment?.InitInAppBilling("membership");
                            }
                            else
                            {
                                var arrayAdapter = new List<string>();
                                var dialogList = new MaterialDialog.Builder(this).Theme(AppSettings.SetTabDarkTheme ? AFollestad.MaterialDialogs.Theme.Dark : AFollestad.MaterialDialogs.Theme.Light);

                                if (AppSettings.ShowPaypal)
                                    arrayAdapter.Add(GetString(Resource.String.Btn_Paypal));

                                if (AppSettings.ShowCreditCard)
                                    arrayAdapter.Add(GetString(Resource.String.Lbl_CreditCard));

                                //if (AppSettings.ShowBankTransfer)
                                //    arrayAdapter.Add(GetString(Resource.String.Lbl_BankTransfer));

                                dialogList.Items(arrayAdapter);
                                dialogList.NegativeText(GetString(Resource.String.Lbl_Close)).OnNegative(this);
                                dialogList.AlwaysCallSingleChoiceCallback();
                                dialogList.ItemsCallback(this).Build().Show();
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Result

        //Result
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);
                BillingPayment?.Handler?.HandleActivityResult(requestCode, resultCode, data);

                if (requestCode == InitPayPalPayment?.PayPalDataRequestCode)
                {
                    switch (resultCode)
                    {
                        case Result.Ok:
                            var confirmObj = data.GetParcelableExtra(PaymentActivity.ExtraResultConfirmation);
                            PaymentConfirmation configuration = Android.Runtime.Extensions.JavaCast<PaymentConfirmation>(confirmObj);
                            if (configuration != null)
                            {
                                //string createTime = configuration.ProofOfPayment.CreateTime;
                                //string intent = configuration.ProofOfPayment.Intent;
                                //string paymentId = configuration.ProofOfPayment.PaymentId;
                                //string state = configuration.ProofOfPayment.State;
                                //string transactionId = configuration.ProofOfPayment.TransactionId;

                                if (!Methods.CheckConnectivity())
                                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                                else
                                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { SetProApi });
                            }
                            break;
                        case Result.Canceled:
                            Toast.MakeText(this, GetText(Resource.String.Lbl_Canceled), ToastLength.Long).Show();
                            break;
                    }
                }
                else if (requestCode == PaymentActivity.ResultExtrasInvalid)
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_Invalid), ToastLength.Long).Show();
                }
                else if (requestCode == 1001 && resultCode == Result.Ok)
                {
                    if (!Methods.CheckConnectivity())
                        Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                    else
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { SetProApi });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

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
                if (text == GetString(Resource.String.Btn_Paypal))
                {
                    InitPayPalPayment.BtnPaypalOnClick(AppSettings.PricePro.ToString(), "GoPro");
                }
                else if (text == GetString(Resource.String.Lbl_CreditCard))
                {
                    OpenIntentCreditCard();
                }
                //else if (text == GetString(Resource.String.Lbl_BankTransfer))
                //{
                //    OpenIntentBankTransfer();
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void OpenIntentCreditCard()
        {
            try
            {
                Intent intent = new Intent(this, typeof(PaymentCardDetailsActivity));
                intent.PutExtra("Price", AppSettings.PricePro.ToString());
                intent.PutExtra("payType", "GoPro");
                StartActivity(intent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //private void OpenIntentBankTransfer()
        //{
        //    try
        //    {
        //        Intent intent = new Intent(this, typeof(PaymentLocalActivity));
        //        intent.PutExtra("Price", AppSettings.PricePro.ToString());
        //        intent.PutExtra("payType", "GoPro");
        //        StartActivity(intent);
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //    }
        //}

        #endregion

        private async Task SetProApi()
        {
            if (Methods.CheckConnectivity())
            {
                (int apiStatus, var respond) = await RequestsAsync.Global.UpgradeAsync();
                if (apiStatus == 200)
                {
                    var dataUser = ListUtils.MyChannelList.FirstOrDefault();
                    if (dataUser != null)
                    {
                        dataUser.IsPro = "1";

                        var sqlEntity = new SqLiteDatabase();
                        sqlEntity.InsertOrUpdate_DataMyChannel(dataUser);
                        sqlEntity.Dispose();
                    }

                    Toast.MakeText(this, GetText(Resource.String.Lbl_Done), ToastLength.Long).Show();
                    Finish();
                }
                else Methods.DisplayReportResult(this, respond);
            }
            else
            {
                Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
            }
        }
    }
}