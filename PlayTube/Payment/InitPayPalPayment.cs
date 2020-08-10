using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Java.Math;
using PlayTube.Helpers.Utils;
using PlayTubeClient;
using Xamarin.PayPal.Android;

namespace PlayTube.Payment
{
    public class InitPayPalPayment
    {
        private readonly Activity ActivityContext;
        private static PayPalConfiguration PayPalConfig;
        private PayPalPayment PayPalPayment;
        private Intent IntentService;
        public readonly int PayPalDataRequestCode = 7171;

        public InitPayPalPayment(Activity activity)
        {
            ActivityContext = activity;
        }

        //Paypal  
        public void BtnPaypalOnClick(string price, string payType)
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
                    return;
                }

                var init = InitPayPal(price, payType);
                if (!init)
                    return;

                Intent intent = new Intent(ActivityContext, typeof(PaymentActivity));
                intent.PutExtra(PayPalService.ExtraPaypalConfiguration, PayPalConfig);
                intent.PutExtra(PaymentActivity.ExtraPayment, PayPalPayment);
                ActivityContext.StartActivityForResult(intent, PayPalDataRequestCode);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        private bool InitPayPal(string price, string payType)
        {
            try
            {
                //PayerID
                string currency = "USD";
                string paypalClintId = "";
                string textPay;
                var option = ListUtils.MySettingsList;
                if (option != null)
                {
                    currency = option.PaypalCurrency ?? "USD";
                    paypalClintId = option.PaypalId;
                }

                PayPalConfig = new PayPalConfiguration()
                    .ClientId(paypalClintId)
                    .LanguageOrLocale(AppSettings.Lang)
                    .MerchantName(AppSettings.ApplicationName)
                    .MerchantPrivacyPolicyUri(Android.Net.Uri.Parse(Client.WebsiteUrl + "/terms/privacy-policy"));

                switch (option?.PaypalMode)
                {
                    case "sandbox":
                        PayPalConfig.Environment(PayPalConfiguration.EnvironmentSandbox);
                        break;
                    case "live":
                        PayPalConfig.Environment(PayPalConfiguration.EnvironmentProduction);
                        break;
                    default:
                        PayPalConfig.Environment(PayPalConfiguration.EnvironmentProduction);
                        break;
                }

                switch (payType)
                {
                    case "purchaseVideo":
                        textPay = "Pay the video";
                        break;
                    case "Subscriber":
                    case "SubscriberVideo":
                        textPay = "Pay to subscribe";
                        break;
                    case "GoPro":
                        textPay = "Pay to pro member";
                        break;
                    case "RentVideo":
                        textPay = "Pay to rent video";
                        break;
                    case "DonateVideo":
                        textPay = "Pay to donate video";
                        break;
                    default:
                        textPay = "Pay the card";
                        break;
                }

                PayPalPayment = new PayPalPayment(new BigDecimal(price), currency, textPay, PayPalPayment.PaymentIntentSale);

                IntentService = new Intent(ActivityContext, typeof(PayPalService));
                IntentService.PutExtra(PayPalService.ExtraPaypalConfiguration, PayPalConfig);
                ActivityContext.StartService(IntentService);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public void StopPayPalService()
        {
            try
            {
                if (PayPalConfig != null)
                {
                    ActivityContext.StopService(new Intent(ActivityContext, typeof(PayPalService)));
                    PayPalConfig = null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}