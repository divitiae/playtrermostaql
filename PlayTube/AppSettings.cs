//###############################################################
// Author >> Elin Doughouz
// Copyright (c) PlayTube 12/07/2018 All Right Reserved
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// Follow me on facebook >> https://www.facebook.com/Elindoughous
//=========================================================

using Android.Graphics;

namespace PlayTube
{
    public static class AppSettings
    {
        //Main Settings >>>>> 
        public static string TripleDesAppServiceProvider = "XzpkswtsEcWvKZJFBYbe5fTylXy6QFbaxacr6zqgVCWUyVHAA18bcOcerZmfaEfwlExiswDfRpYp1lhmyBMXMXrxXLxoVetRuGMyuUOlvXNWCYE5/nZIngEsuklDoo+D4KYiXAaYNkuiO2XoYO3rk6dzWpzvlPnA84u2WU+/SzFwxKvGKxhM7LnUoYHHiZhk3997DA6fYL3aEQTkkzG0L9QrJhNA6tv1Tmye/99yxGk8C7L3ZX8e+TNkfJZHnE1x";
        
        //********************************************************* 
        public static string ApplicationName = "VOX Kick";
        public static string Version = "1.9";

        //Main Colors >>
        //*********************************************************
        public static string MainColor = "#4ca5ff";

        //Language Settings >> http://www.lingoes.net/en/translator/langcode.htm
        //*********************************************************
        public static bool FlowDirectionRightToLeft = false;
        public static string Lang = ""; //Default language ar_AE

        //true = Show Username ,  false = Show Full name
        //*********************************************************
        public static bool ShowUserPlayListVideoObject = true;

        //Notification Settings >>
        //*********************************************************
        public static bool ShowNotification = true;
        public static string OneSignalAppId = "2c337953-d0b1-4230-85bc-1de93d573714";

        //AdMob >> Please add the code ads in the Here and analytic.xml 
        //*********************************************************
        public static bool ShowAdMobBanner = true;
        public static bool ShowAdMobInterstitial = false;
        public static bool ShowAdMobRewardVideo = false;
        public static bool ShowAdMobNative = false; 

        public static string AdInterstitialKey = "ca-app-pub-5135691635931982/6168068662";
        public static string AdRewardVideoKey = "ca-app-pub-5135691635931982/4663415300";
        public static string AdAdMobNativeKey = "ca-app-pub-5135691635931982/2619721801";

        //Three times after entering the ad is displayed
        public static int ShowAdMobInterstitialCount = 3;
        public static int ShowAdMobRewardedVideoCount = 3;

        //Please add the key for Youtube Player
        public static string YoutubePlayerKey = "AIzaSyA-JSf9CU1cdMpgzROCCUpl4wOve9S94ZU";

        //*********************************************************

        //Social Logins >>
        //If you want login with facebook or google you should change id key in the analytic.xml file or AndroidManifest.xml
        //Facebook >> ../values/analytic.xml  
        //Google >> ../Properties/AndroidManifest.xml .. line 27
        //*********************************************************
        public static bool ShowFacebookLogin = false;
        public static bool ShowGoogleLogin = true; 

        public static readonly string ClientId = "713449623527-o18evvr89cdh4v7rsrl790qp8l3fg2dl.apps.googleusercontent.com";

        //First Page
        //*********************************************************
        public static bool ShowSkipButton = true;//#New

        //Set Theme Full Screen App
        //*********************************************************
        public static bool EnableFullScreenApp = false;
        public static bool EnablePictureToPictureMode = true; 

        //Data Channal Users >> About
        //*********************************************************
        public static bool ShowEmailAccount = true;

        //Tab >> 
        //*********************************************************
        public static bool ShowArticle = true;

        //Offline Watched Videos >>  
        //*********************************************************
        public static bool AllowOfflineDownload = true;
        public static bool AllowDownloadProUser = true;

        //Import && Upload Videos >>  
        //*********************************************************
        public static bool ShowButtonImport { get; set; } = true;
        public static bool ShowButtonUpload { get; set; } = true;

        //Last_Messages Page >>
        ///********************************************************* 
        public static bool RunSoundControl = true;
        public static int RefreshChatActivitiesSeconds = 6000; // 6 Seconds
        public static int MessageRequestSpeed = 3000; // 3 Seconds

        public static int ShowButtonSkip = 6; // 6 Seconds 

        //CategoriesVideoList
        //*********************************************************
        public static bool CategoriesVideoStyleImage = false; // Style 1 
        public static bool CategoriesVideoStyleText = true; // Style 2 

        //Set Theme App >> Color - Tab
        //*********************************************************
        public static bool SetTabDarkTheme = true;

        public static bool SetYoutubeTypeBadgeIcon = true;
        public static bool SetVimeoTypeBadgeIcon = true;
        public static bool SetDailyMotionTypeBadgeIcon = true;
        public static bool SetTwichTypeBadgeIcon = true;
        public static bool SetOkTypeBadgeIcon = true;
        public static bool SetFacebookTypeBadgeIcon = true;

        //Bypass Web Erros 
        ///*********************************************************
        public static bool TurnTrustFailureOnWebException = true;
        public static bool TurnSecurityProtocolType3072On = true;

        //*********************************************************
        public static bool RenderPriorityFastPostLoad = true;

        //Error Report Mode
        //*********************************************************
        public static bool SetApisReportMode = true;

        public static bool CompressImage = false;
        public static int AvatarSize = 60; 
        public static int ImageSize = 400;   

        public static int CountVideosTop = 13;  
        public static int CountVideosLatest = 13;  
        public static int CountVideosFav = 13;  
         
        //Settings 
        //*********************************************************
        public static bool ShowEditPassword = true; 
        public static bool ShowMonetization = true; //(Withdrawals)
        public static bool ShowVerification = true; 
        public static bool ShowBlockedUsers = true; 
        public static bool ShowSettingsTwoFactor = true;  // #New
        public static bool ShowSettingsManageSessions = true;  // #New

        public static bool ShowGoPro = true; 
        public static int PricePro = 10;  

        public static bool ShowClearHistory = true; 
        public static bool ShowClearCache = true; 
         
        public static bool ShowHelp = true; 
        public static bool ShowTermsOfUse = true; 
        public static bool ShowAbout = true; 
        public static bool ShowDeleteAccount = true;

        //********************************************************* 
        public static bool ImageCropping = true; //#New

        //*********************************************************
        /// <summary>
        /// Currency
        /// CurrencyStatic = true : get currency from app not api 
        /// CurrencyStatic = false : get currency from api (default)
        /// </summary>
        public static readonly bool CurrencyStatic = false; //#New
        public static readonly string CurrencyIconStatic = "$"; //#New
        public static readonly string CurrencyCodeStatic = "USD"; //#New 

        //********************************************************* 
        public static bool RentVideosSystem = true; //#New 
        /// <summary>
        /// RentVideos
        /// VideoRentalPriceStatic = true : Video rent becomes a fixed rate in the app >> #Compatible with InAppBilling 
        /// VideoRentalPriceStatic = false : Video rent price from api (default)       >> #Not compatible with InAppBilling just Paypal and CreditCard
        /// 
        /// VideoRentalPrice = 0.0 USD : The fixed value of the video rental price can be determined
        /// </summary> 
        public static bool VideoRentalPriceStatic = false; //#New
        public static int VideoRentalPrice = 50; //#New

        //*********************************************************  
        public static bool DonateVideosSystem = true; //#New

        public static bool ShowCategoriesInHome = true; //#New

        public static bool ShowPaypal = true; //#New
        public static bool ShowCreditCard = true; //#New

        /// <summary>
        /// if you want this feature enabled go to Properties -> AndroidManefist.xml and remove comments from below code
        /// <uses-permission android:name="com.android.vending.BILLING" />
        /// </summary>
        public static bool ShowInAppBilling = false; //#New

        public static bool HideSubscribeForOwner = false; 
        public static bool DisableYouTubeInitializationFailureMessages = true;  //#New
        public static bool UseSpanishDateFormat = true;  //#New
        public static bool ShowAppLogoInToolbar = true;  //#New
        public static bool ShowVideoWithDynamicHeight = true; //#New

        //FaceBook Ads >> Please add the code ad in the Here and analytic.xml 
        //*********************************************************
        public static bool ShowFbBannerAds = false; //#New
        public static bool ShowFbInterstitialAds = false; //#New
        public static bool ShowFbRewardVideoAds = false; //#New
        public static bool ShowFbNativeAds = false; //#New

        //YOUR_PLACEMENT_ID
        public static string AdsFbBannerKey = "250485588986218_554026418632132"; //#New
        public static string AdsFbInterstitialKey = "250485588986218_554026125298828"; //#New
        public static string AdsFbRewardVideoKey = "250485588986218_554072818627492"; //#New
        public static string AdsFbNativeKey = "250485588986218_554706301897477"; //#New
    }
} 
