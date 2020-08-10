using System;
using Android.Content;
using Android.Content.Res;
using Android.Gms.Ads;
using Android.Gms.Ads.Formats;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Support.Constraints;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace PlayTube.Helpers.Ads
{
    public class TemplateView : FrameLayout
    {
        private int TemplateType;
        private NativeTemplateStyle Styles;
        private UnifiedNativeAd NativeAd;
        private UnifiedNativeAdView NativeAdView;

        private TextView PrimaryView;
        private TextView SecondaryView;

        //private RatingBar RatingBar;
        private TextView TertiaryView;
        private ImageView IconView;

        private MediaView MediaView;

        //private Button CallToActionView;
        private new ConstraintLayout Background;

        public static readonly string MediumTemplate = "medium_template";
        public static readonly string SmallTemplate = "small_template";
        public static readonly string BigTemplate = "big_template";
        public static readonly string NativeContentAd = "NativeContentAd";
        public static readonly string NativeAppInstallAd = "NativeAppInstallAd";

        protected TemplateView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public TemplateView(Context context) : base(context)
        {

        }

        public TemplateView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            InitView(context, attrs);
        }

        public TemplateView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            InitView(context, attrs);
        }

        public TemplateView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context,
            attrs, defStyleAttr, defStyleRes)
        {
            InitView(context, attrs);
        }

        public void SetStyles(NativeTemplateStyle styles)
        {
            Styles = styles;
            ApplyStyles();
        }

        public UnifiedNativeAdView GetNativeAdView()
        {
            return NativeAdView;
        }

        private void ApplyStyles()
        {
            try
            {
                Drawable mainBackground = Styles.GetMainBackgroundColor();
                if (mainBackground != null)
                {
                    Background.Background = mainBackground;
                    if (PrimaryView != null)
                    {
                        PrimaryView.Background = mainBackground;
                    }

                    if (SecondaryView != null)
                    {
                        SecondaryView.Background = mainBackground;
                    }

                    if (TertiaryView != null)
                    {
                        TertiaryView.Background = mainBackground;
                    }
                }

                Typeface primary = Styles.GetPrimaryTextTypeface();
                if (primary != null)
                {
                    PrimaryView?.SetTypeface(primary, TypefaceStyle.Normal);
                }

                Typeface secondary = Styles.GetSecondaryTextTypeface();
                if (secondary != null)
                {
                    SecondaryView?.SetTypeface(secondary, TypefaceStyle.Normal);
                }

                Typeface tertiary = Styles.GetTertiaryTextTypeface();
                if (tertiary != null)
                {
                    TertiaryView?.SetTypeface(tertiary, TypefaceStyle.Normal);
                }

                //Typeface ctaTypeface = Styles.GetCallToActionTextTypeface();
                //if (ctaTypeface != null)
                //{
                //    CallToActionView?.SetTypeface(ctaTypeface, TypefaceStyle.Normal);
                //}

                Color primaryTypefaceColor = Styles.GetPrimaryTextTypefaceColor();
                if (primaryTypefaceColor > 0)
                {
                    PrimaryView?.SetTextColor(primaryTypefaceColor);
                }

                Color secondaryTypefaceColor = Styles.GetSecondaryTextTypefaceColor();
                if (secondaryTypefaceColor > 0)
                {
                    SecondaryView?.SetTextColor(secondaryTypefaceColor);
                }

                Color tertiaryTypefaceColor = Styles.GetTertiaryTextTypefaceColor();
                if (tertiaryTypefaceColor > 0)
                {
                    TertiaryView?.SetTextColor(tertiaryTypefaceColor);
                }

                //var ctaTypefaceColor = Styles.GetCallToActionTypefaceColor();
                //if (ctaTypefaceColor > 0)
                //{
                //    CallToActionView?.SetTextColor(ctaTypefaceColor);
                //}

                //float ctaTextSize = Styles.GetCallToActionTextSize();
                //if (ctaTextSize > 0)
                //{
                //    CallToActionView?.SetTextSize(ComplexUnitType.Sp, ctaTextSize);
                //}

                float primaryTextSize = Styles.GetPrimaryTextSize();
                if (primaryTextSize > 0)
                {
                    PrimaryView?.SetTextSize(ComplexUnitType.Sp, primaryTextSize);
                }

                float secondaryTextSize = Styles.GetSecondaryTextSize();
                if (secondaryTextSize > 0)
                {
                    SecondaryView?.SetTextSize(ComplexUnitType.Sp, secondaryTextSize);
                }

                float tertiaryTextSize = Styles.GetTertiaryTextSize();
                if (tertiaryTextSize > 0)
                {
                    TertiaryView?.SetTextSize(ComplexUnitType.Sp, tertiaryTextSize);
                }

                //Drawable ctaBackground = Styles.GetCallToActionBackgroundColor();
                //if (ctaBackground != null && CallToActionView != null)
                //{
                //    CallToActionView.Background = ctaBackground;
                //}

                Drawable primaryBackground = Styles.GetPrimaryTextBackgroundColor();
                if (primaryBackground != null && PrimaryView != null)
                {
                    PrimaryView.Background = primaryBackground;
                }

                Drawable secondaryBackground = Styles.GetSecondaryTextBackgroundColor();
                if (secondaryBackground != null && SecondaryView != null)
                {
                    SecondaryView.Background = secondaryBackground;
                }

                Drawable tertiaryBackground = Styles.GetTertiaryTextBackgroundColor();
                if (tertiaryBackground != null && TertiaryView != null)
                {
                    TertiaryView.Background = tertiaryBackground;
                }

                Invalidate();
                RequestLayout();
            }
            catch (Exception e)
            {
                Console.WriteLine(e); 
            }
        }

        private bool AdHasOnlyStore(UnifiedNativeAd nativeAd)
        {
            string store = nativeAd.Store;
            string advertiser = nativeAd.Advertiser;
            return !TextUtils.IsEmpty(store) && TextUtils.IsEmpty(advertiser);
        }

        public void SetNativeAd(UnifiedNativeAd nativeAd)
        {
            try
            {
                NativeAd = nativeAd;

                string store = nativeAd.Store;
                string advertiser = nativeAd.Advertiser;
                string headline = nativeAd.Headline;
                string body = nativeAd.Body;
                string cta = nativeAd.CallToAction;
                int starRating = Convert.ToInt32(nativeAd.StarRating);
                NativeAd.Image icon = nativeAd.Icon;

                string secondaryText;

                //NativeAdView.CallToActionView=CallToActionView;
                NativeAdView.HeadlineView = PrimaryView;
                NativeAdView.MediaView = MediaView;
                SecondaryView.Visibility = ViewStates.Visible;
                if (AdHasOnlyStore(nativeAd))
                {
                    NativeAdView.StoreView = SecondaryView;
                    secondaryText = store;
                }
                else if (!TextUtils.IsEmpty(advertiser))
                {
                    NativeAdView.AdvertiserView = SecondaryView;
                    secondaryText = advertiser;
                }
                else
                {
                    secondaryText = "";
                }

                PrimaryView.Text = headline;
                //CallToActionView.Text=cta;

                //  Set the secondary view to be the star rating if available.
                //if (starRating > 0)
                //{
                //    SecondaryView.Visibility=ViewStates.Gone;
                //    RatingBar.Visibility = ViewStates.Visible;
                //    RatingBar.Max=5;
                //    NativeAdView.StarRatingView=RatingBar;
                //}
                //else
                //{
                //    SecondaryView.Text=secondaryText;
                //    SecondaryView.Visibility = ViewStates.Visible;
                //    RatingBar.Visibility= ViewStates.Gone;
                //}

                if (string.IsNullOrEmpty(secondaryText))
                {
                    SecondaryView.Visibility = ViewStates.Gone;
                }
                else
                {
                    SecondaryView.Visibility = ViewStates.Visible;
                    SecondaryView.Text = secondaryText;
                }

                if (icon != null)
                {
                    IconView.Visibility = ViewStates.Visible;
                    IconView.SetImageDrawable(icon.Drawable);
                }
                else
                {
                    IconView.Visibility = ViewStates.Gone;
                }

                if (TertiaryView != null && !string.IsNullOrEmpty(body))
                {
                    TertiaryView.Text = body;
                    NativeAdView.BodyView = TertiaryView;
                }
                else if (TertiaryView != null)
                {
                    TertiaryView.Visibility = ViewStates.Gone;
                }

                NativeAdView.SetNativeAd(nativeAd);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// To prevent memory leaks, make sure to destroy your ad when you don't need it anymore.
        /// This method does not destroy the template view.
        /// </summary>
        public void DestroyNativeAd()
        {
            NativeAd.Destroy();
        }

        public string GetTemplateTypeName()
        {
            if (TemplateType == Resource.Layout.gnt_medium_template_view)
            {
                return MediumTemplate;
            }

            if (TemplateType == Resource.Layout.gnt_small_template_view)
            {
                return SmallTemplate;
            }

            if (TemplateType == Resource.Layout.gnt_big_template_view)
            {
                return BigTemplate;
            }

            if (TemplateType == Resource.Layout.gnt_NativeContentAd_view)
            {
                return NativeContentAd;
            }

            if (TemplateType == Resource.Layout.gnt_NativeAppInstallAd_view)
            {
                return NativeAppInstallAd;
            }

            return "";
        }

        private void InitView(Context context, IAttributeSet attributeSet)
        {

            try
            {
                TypedArray attributes = context.Theme.ObtainStyledAttributes(attributeSet, Resource.Styleable.TemplateView, 0, 0);

                try
                {
                    TemplateType = attributes.GetResourceId(Resource.Styleable.TemplateView_gnt_template_type, Resource.Layout.gnt_medium_template_view);
                }
                finally
                {
                    attributes.Recycle();
                }

                LayoutInflater inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
                inflater.Inflate(TemplateType, this);
            }
            catch (Exception e)
            {
                Console.WriteLine(e); 
            }
        }

        protected override void OnFinishInflate()
        {
            try
            {
                base.OnFinishInflate();

                NativeAdView = (UnifiedNativeAdView)FindViewById(Resource.Id.native_ad_view);

                if (!AppSettings.ShowAdMobNative)
                {
                    NativeAdView.Visibility = ViewStates.Gone;
                }
                else
                {
                    if (TemplateType != Resource.Layout.gnt_big_template_view)
                    {
                        PrimaryView = (TextView)FindViewById(Resource.Id.primary);
                        SecondaryView = (TextView)FindViewById(Resource.Id.secondary);
                        TertiaryView = (TextView)FindViewById(Resource.Id.body);

                        //RatingBar = (RatingBar)FindViewById(Resource.Id.rating_bar);
                        //RatingBar.Enabled=false;

                        //CallToActionView = (Button)FindViewById(Resource.Id.cta);
                        IconView = (ImageView)FindViewById(Resource.Id.icon);
                        MediaView = (MediaView)FindViewById(Resource.Id.media_view);
                        Background = (ConstraintLayout)FindViewById(Resource.Id.background);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e); 
            }
        }

        public void NativeContentAdView(UnifiedNativeAd nativeAd)
        {
            try
            {
                NativeAdView = (UnifiedNativeAdView)FindViewById(Resource.Id.nativeAdView);

                // Set other ad assets.
                NativeAdView.HeadlineView = NativeAdView.FindViewById(Resource.Id.contentad_headline);
                NativeAdView.BodyView = NativeAdView.FindViewById(Resource.Id.contentad_body);
                NativeAdView.CallToActionView = NativeAdView.FindViewById(Resource.Id.contentad_call_to_action);
                NativeAdView.IconView = NativeAdView.FindViewById(Resource.Id.contentad_logo);
                NativeAdView.AdvertiserView = NativeAdView.FindViewById(Resource.Id.contentad_advertiser);
                NativeAdView.ImageView = NativeAdView.FindViewById(Resource.Id.contentad_image);

                // The headline and mediaContent are guaranteed to be in every UnifiedNativeAd.
                ((TextView)NativeAdView.HeadlineView).Text = nativeAd.Headline;

                // These assets aren't guaranteed to be in every UnifiedNativeAd, so it's important to
                // check before trying to display them.
                if (string.IsNullOrEmpty(nativeAd.Body))
                {
                    NativeAdView.BodyView.Visibility = ViewStates.Gone;
                }
                else
                {
                    NativeAdView.BodyView.Visibility = ViewStates.Visible;
                    ((TextView)NativeAdView.BodyView).Text = nativeAd.Body;
                }

                if (string.IsNullOrEmpty(nativeAd.CallToAction))
                {
                    NativeAdView.CallToActionView.Visibility = ViewStates.Gone;
                }
                else
                {
                    NativeAdView.CallToActionView.Visibility = ViewStates.Visible;
                    ((Button)NativeAdView.CallToActionView).Text = nativeAd.CallToAction;
                }

                if (nativeAd.Icon == null)
                {
                    NativeAdView.IconView.Visibility = ViewStates.Gone;
                }
                else
                {
                    ((ImageView)NativeAdView.IconView).SetImageDrawable(nativeAd.Icon.Drawable);
                    NativeAdView.IconView.Visibility = ViewStates.Visible;
                }

                if (nativeAd.Images?.Count == 0)
                {
                    NativeAdView.IconView.Visibility = ViewStates.Gone;
                }
                else
                {
                    if (nativeAd.Images != null)
                        ((ImageView)NativeAdView.ImageView).SetImageDrawable(nativeAd.Images[0].Drawable);

                    NativeAdView.ImageView.Visibility = ViewStates.Visible;
                }

                if (string.IsNullOrEmpty(nativeAd.Advertiser))
                {
                    NativeAdView.AdvertiserView.Visibility = ViewStates.Gone;
                }
                else
                {
                    ((TextView)NativeAdView.AdvertiserView).Text = nativeAd.Advertiser;
                    NativeAdView.AdvertiserView.Visibility = ViewStates.Visible;
                }

                // This method tells the Google Mobile Ads SDK that you have finished populating your
                // native ad view with this native ad.
                NativeAdView.SetNativeAd(nativeAd);

                // Get the video controller for the ad. One will always be provided, even if the ad doesn't
                // have a video asset.
                VideoController vc = nativeAd.VideoController;

                // Updates the UI to say whether or not this ad has a video asset.
                if (vc.HasVideoContent)
                {
                    //"Video status: Ad contains a %.2f:1 video asset."

                    // Create a new VideoLifecycleCallbacks object and pass it to the VideoController. The
                    // VideoController will call methods on this object when events occur in the video
                    // lifecycle.
                    vc.SetVideoLifecycleCallbacks(new VideoController.VideoLifecycleCallbacks());

                }
                else
                {
                    //"Video status: Ad does not contain a video asset."
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void NativeAppInstallAdView(UnifiedNativeAd nativeAppInstallAd)
        {
            try
            {
                NativeAdView = (UnifiedNativeAdView)FindViewById(Resource.Id.nativeInstallAdView);

                VideoController videoController = nativeAppInstallAd.VideoController;
                videoController.SetVideoLifecycleCallbacks(new VideoController.VideoLifecycleCallbacks());
                NativeAdView.HeadlineView = NativeAdView.FindViewById(Resource.Id.appinstall_headline);
                NativeAdView.BodyView = NativeAdView.FindViewById(Resource.Id.appinstall_body);
                NativeAdView.CallToActionView = NativeAdView.FindViewById(Resource.Id.appinstall_call_to_action);
                NativeAdView.IconView = NativeAdView.FindViewById(Resource.Id.appinstall_app_icon);
                NativeAdView.PriceView = NativeAdView.FindViewById(Resource.Id.appinstall_price);
                NativeAdView.StarRatingView = NativeAdView.FindViewById(Resource.Id.appinstall_stars);
                NativeAdView.StoreView = NativeAdView.FindViewById(Resource.Id.appinstall_store);

                ((TextView)NativeAdView.HeadlineView).Text = nativeAppInstallAd.Headline;
                ((TextView)NativeAdView.BodyView).Text = nativeAppInstallAd.Body;
                ((Button)NativeAdView.CallToActionView).Text = nativeAppInstallAd.CallToAction;

                if (nativeAppInstallAd.Icon.Drawable != null)
                {
                    ((ImageView)NativeAdView.IconView).SetImageDrawable(nativeAppInstallAd.Icon.Drawable);
                }

                MediaView mediaView = (MediaView)NativeAdView.FindViewById(Resource.Id.appinstall_media);
                ImageView imageView = (ImageView)NativeAdView.FindViewById(Resource.Id.appinstall_image);
                if (videoController.HasVideoContent)
                {
                    NativeAdView.MediaView = (mediaView);
                    imageView.Visibility = ViewStates.Gone;
                }
                else
                {
                    NativeAdView.ImageView = (imageView);
                    mediaView.Visibility = ViewStates.Gone;
                    var images = nativeAppInstallAd.Images;
                    if (images != null && images.Count > 0)
                    {
                        imageView.SetImageDrawable(images[0].Drawable);
                    }
                }

                if (nativeAppInstallAd.Price == null)
                {
                    NativeAdView.PriceView.Visibility = ViewStates.Invisible;
                }
                else
                {
                    NativeAdView.PriceView.Visibility = ViewStates.Visible;
                    ((TextView)NativeAdView.PriceView).Text = (nativeAppInstallAd.Price);
                }

                if (nativeAppInstallAd.Store == null)
                {
                    NativeAdView.StoreView.Visibility = ViewStates.Invisible;
                }
                else
                {
                    NativeAdView.StoreView.Visibility = ViewStates.Visible;
                    ((TextView)NativeAdView.StoreView).Text = (nativeAppInstallAd.Store);
                }

                //if (nativeAppInstallAd.StarRating == null)
                //{
                //    NativeAdView.StarRatingView.Visibility = ViewStates.Gone;
                //}
                //else
                //{
                //    ((RatingBar) NativeAdView.StarRatingView).Rating = nativeAppInstallAd.StarRating.FloatValue;
                //    NativeAdView.StarRatingView.Visibility = ViewStates.Visible;
                //}

                NativeAdView.SetNativeAd(nativeAppInstallAd);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void PopulateUnifiedNativeAdView(UnifiedNativeAd nativeAd)
        {
            try
            {
                NativeAdView = (UnifiedNativeAdView)FindViewById(Resource.Id.native_ad_view);

                // Set the media view.
                NativeAdView.MediaView = NativeAdView.FindViewById<MediaView>(Resource.Id.ad_media);

                // Set other ad assets.
                NativeAdView.HeadlineView = NativeAdView.FindViewById(Resource.Id.ad_headline);
                NativeAdView.BodyView = NativeAdView.FindViewById(Resource.Id.ad_body);
                NativeAdView.CallToActionView = NativeAdView.FindViewById(Resource.Id.ad_call_to_action);
                NativeAdView.IconView = NativeAdView.FindViewById(Resource.Id.ad_app_icon);
                NativeAdView.PriceView = NativeAdView.FindViewById(Resource.Id.ad_price);
                //NativeAdView.StarRatingView=NativeAdView.FindViewById(Resource.Id.ad_stars);
                NativeAdView.StoreView = NativeAdView.FindViewById(Resource.Id.ad_store);
                NativeAdView.AdvertiserView = NativeAdView.FindViewById(Resource.Id.ad_advertiser);

                // The headline and mediaContent are guaranteed to be in every UnifiedNativeAd.
                ((TextView)NativeAdView.HeadlineView).Text = nativeAd.Headline;
                //NativeAdView.MediaView.MediaContent(nativeAd.MediaContent);

                // These assets aren't guaranteed to be in every UnifiedNativeAd, so it's important to
                // check before trying to display them.
                if (string.IsNullOrEmpty(nativeAd.Body))
                {
                    NativeAdView.BodyView.Visibility = ViewStates.Gone;
                }
                else
                {
                    NativeAdView.BodyView.Visibility = ViewStates.Visible;
                    ((TextView)NativeAdView.BodyView).Text = nativeAd.Body;
                }

                if (string.IsNullOrEmpty(nativeAd.CallToAction))
                {
                    NativeAdView.CallToActionView.Visibility = ViewStates.Gone;
                }
                else
                {
                    NativeAdView.CallToActionView.Visibility = ViewStates.Visible;
                    ((Button)NativeAdView.CallToActionView).Text = nativeAd.CallToAction;
                }

                if (nativeAd.Icon == null)
                {
                    NativeAdView.IconView.Visibility = ViewStates.Gone;
                }
                else
                {
                    ((ImageView)NativeAdView.IconView).SetImageDrawable(nativeAd.Icon.Drawable);
                    NativeAdView.IconView.Visibility = ViewStates.Visible;
                }

                if (string.IsNullOrEmpty(nativeAd.Price))
                {
                    NativeAdView.PriceView.Visibility = ViewStates.Gone;
                }
                else
                {
                    NativeAdView.PriceView.Visibility = ViewStates.Visible;
                    ((TextView)NativeAdView.PriceView).Text = nativeAd.Price;
                }

                if (string.IsNullOrEmpty(nativeAd.Store))
                {
                    NativeAdView.StoreView.Visibility = ViewStates.Gone;
                }
                else
                {
                    NativeAdView.StoreView.Visibility = ViewStates.Visible;
                    ((TextView)NativeAdView.StoreView).Text = nativeAd.Store;
                }

                //if (nativeAd.StarRating == null)
                //{
                //    NativeAdView.StarRatingView.Visibility = ViewStates.Gone;
                //}
                //else
                //{
                //    ((RatingBar)NativeAdView.StarRatingView).Rating=nativeAd.StarRating.FloatValue();
                //    NativeAdView.StarRatingView.Visibility = ViewStates.Gone;
                //}

                if (string.IsNullOrEmpty(nativeAd.Advertiser))
                {
                    NativeAdView.AdvertiserView.Visibility = ViewStates.Gone;
                }
                else
                {
                    ((TextView)NativeAdView.AdvertiserView).Text = nativeAd.Advertiser;
                    NativeAdView.AdvertiserView.Visibility = ViewStates.Visible;
                }

                // This method tells the Google Mobile Ads SDK that you have finished populating your
                // native ad view with this native ad.
                NativeAdView.SetNativeAd(nativeAd);

                // Get the video controller for the ad. One will always be provided, even if the ad doesn't
                // have a video asset.
                VideoController vc = nativeAd.VideoController;

                // Updates the UI to say whether or not this ad has a video asset.
                if (vc.HasVideoContent)
                {
                    //"Video status: Ad contains a %.2f:1 video asset."

                    // Create a new VideoLifecycleCallbacks object and pass it to the VideoController. The
                    // VideoController will call methods on this object when events occur in the video
                    // lifecycle.
                    vc.SetVideoLifecycleCallbacks(new VideoController.VideoLifecycleCallbacks());

                }
                else
                {
                    //"Video status: Ad does not contain a video asset."
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

    }
}