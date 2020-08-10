using System;
using System.Collections.Generic;
using AFollestad.MaterialDialogs;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Java.Lang;
using PlayTube.Activities.Tabbes;
using PlayTube.Helpers.CacheLoaders;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Models;
using PlayTube.Helpers.Utils;
using PlayTube.Payment;
using PlayTubeClient.Classes.Global;
using Exception = System.Exception;
using Fragment = Android.Support.V4.App.Fragment;

namespace PlayTube.Activities.PlayersView
{
    public class RestrictedVideoFragment : Fragment, MaterialDialog.ISingleButtonCallback, MaterialDialog.IListCallback
    {
        public TextView RestrictedTextView;
        public ImageView ImageVideo, RestrictedIcon;
        public Button PurchaseButton;
        private VideoObject VideoObject;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                View view = inflater.Inflate(Resource.Layout.RestrictedVideoLayout, container, false);
                return view;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            try
            {
                //Get Value And Set Toolbar
                InitComponent(view);
                base.OnViewCreated(view, savedInstanceState); 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void InitComponent(View view)
        {
            try
            {
                ImageVideo = (ImageView)view.FindViewById(Resource.Id.Imagevideo);
                RestrictedIcon = (ImageView)view.FindViewById(Resource.Id.restrictedIcon);
                RestrictedTextView = (TextView)view.FindViewById(Resource.Id.restrictedTextview);
                PurchaseButton = (Button)view.FindViewById(Resource.Id.purchaseButton);
                PurchaseButton.Visibility = ViewStates.Gone;

                PurchaseButton.Click += PurchaseButtonOnClick;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void PurchaseButtonOnClick(object sender, EventArgs e)
        {
            try
            { 
                if (UserDetails.IsLogin)
                {
                    var arrayAdapter = new List<string>();
                    var dialogList = new MaterialDialog.Builder(Activity).Theme(AppSettings.SetTabDarkTheme ? Theme.Dark : Theme.Light);

                    if (AppSettings.ShowPaypal)
                        arrayAdapter.Add(Activity.GetString(Resource.String.Btn_Paypal));

                    if (AppSettings.ShowCreditCard)
                        arrayAdapter.Add(Activity.GetString(Resource.String.Lbl_CreditCard));

                    dialogList.Items(arrayAdapter);
                    dialogList.NegativeText(Activity.GetString(Resource.String.Lbl_Close)).OnNegative(this);
                    dialogList.AlwaysCallSingleChoiceCallback();
                    dialogList.ItemsCallback(this).Build().Show(); 
                }
                else
                {
                    PopupDialogController dialog = new PopupDialogController(Activity , VideoObject, "Login");
                    dialog.ShowNormalDialog(Activity.GetText(Resource.String.Lbl_Warning), Activity.GetText(Resource.String.Lbl_Please_sign_in_Paid), Activity.GetText(Resource.String.Lbl_Yes), Activity.GetText(Resource.String.Lbl_No));
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void HideRestrictedInfo(bool hide)
        {
            RestrictedTextView.Visibility = hide ? ViewStates.Gone : ViewStates.Visible;
            RestrictedIcon.Visibility = hide ? ViewStates.Gone : ViewStates.Visible;
        }

        public void LoadRestriction(string strtext, string imageUrl, VideoObject videoObject)
        {
            try
            {
                HideRestrictedInfo(false);
                VideoObject = videoObject;
                if (strtext == "AgeRestriction")
                {
                    RestrictedIcon.SetImageResource(Resource.Drawable.icon_18plus);
                    RestrictedTextView.Text = GetText(Resource.String.Lbl_AgeRestricted);
                    if (!string.IsNullOrEmpty(imageUrl))
                        GlideImageLoader.LoadImage(Activity, imageUrl, ImageVideo, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                }
                else if (strtext == "GeoRestriction")
                {
                    RestrictedIcon.SetImageResource(Resource.Drawable.ic_GeoRestict);
                    RestrictedTextView.Text = GetText(Resource.String.Lbl_GEORestricted);
                    if (!string.IsNullOrEmpty(imageUrl))
                        GlideImageLoader.LoadImage(Activity, imageUrl, ImageVideo, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                }
                else if (strtext == "purchaseVideo")
                {
                    RestrictedIcon.SetImageResource(Resource.Drawable.ic_action_dollars);
                    RestrictedTextView.Text = GetText(Resource.String.Lbl_purchaseVideo);
                    if (!string.IsNullOrEmpty(imageUrl))
                        GlideImageLoader.LoadImage(Activity, imageUrl, ImageVideo, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);

                    var idCurrency = ListUtils.MySettingsList?.PaymentCurrency;
                    var (currency, currencyIcon) = AppTools.GetCurrency(idCurrency);
                    Console.WriteLine(currency);
                    PurchaseButton.Visibility = ViewStates.Visible;
                    PurchaseButton.Text = GetText(Resource.String.Lbl_Purchase) + " " + currencyIcon + videoObject.SellVideo;
                    PurchaseButton.Tag = "purchaseVideo";
                }
                else if (strtext == "RentVideo")
                {
                    RestrictedIcon.SetImageResource(Resource.Drawable.ic_action_dollars);
                    RestrictedTextView.Text = GetText(Resource.String.Lbl_RentVideo);
                    if (!string.IsNullOrEmpty(imageUrl))
                        GlideImageLoader.LoadImage(Activity, imageUrl, ImageVideo, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);

                    var idCurrency = ListUtils.MySettingsList?.PaymentCurrency;
                    var (currency, currencyIcon) = AppTools.GetCurrency(idCurrency);
                    Console.WriteLine(currency);
                    PurchaseButton.Visibility = ViewStates.Visible;
                    PurchaseButton.Text = GetText(Resource.String.Lbl_Rent) + " " + currencyIcon + videoObject.RentPrice;
                    PurchaseButton.Tag = "RentVideo";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

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
                if (text == Activity.GetString(Resource.String.Btn_Paypal))
                {
                    switch (PurchaseButton.Tag.ToString())
                    { 
                        case "purchaseVideo":
                        {
                            var cont = TabbedMainActivity.GetInstance();
                            if (cont != null)
                            {
                                cont.StopFragmentVideo();
                                cont.PayType = "purchaseVideo";
                                cont.Price = VideoObject.SellVideo;
                                cont.PaymentVideoObject = VideoObject;
                                cont.InitPayPalPayment.BtnPaypalOnClick(VideoObject.SellVideo, "purchaseVideo");
                            }

                            break;
                        }
                        case "RentVideo":
                        {
                            var cont = TabbedMainActivity.GetInstance();
                            if (cont != null)
                            {
                                cont.StopFragmentVideo();
                                cont.PayType = "RentVideo";
                                cont.Price = VideoObject.RentPrice;
                                cont.PaymentVideoObject = VideoObject;
                                cont.InitPayPalPayment.BtnPaypalOnClick(VideoObject.SellVideo, "RentVideo");
                            }

                            break;
                        }
                    }
                }
                else if (text == Activity.GetString(Resource.String.Lbl_CreditCard))
                {
                    OpenIntentCreditCard();
                }
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
                Intent intent = new Intent(Activity, typeof(PaymentCardDetailsActivity));

                switch (PurchaseButton.Tag.ToString())
                {
                    case "purchaseVideo":
                        intent.PutExtra("Price", VideoObject.SellVideo);
                        intent.PutExtra("payType", "purchaseVideo");
                        break; 
                    case "RentVideo":
                        intent.PutExtra("Price", VideoObject.RentPrice);
                        intent.PutExtra("payType", "RentVideo");
                        break;
                } 
                intent.PutExtra("Id", VideoObject.Id);
                Activity.StartActivity(intent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion
    }
}