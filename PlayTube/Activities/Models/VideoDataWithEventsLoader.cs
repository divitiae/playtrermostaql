using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using Com.Luseen.Autolinklibrary;
using Com.Sothree.Slidinguppanel;
using Java.Lang;
using Newtonsoft.Json;
using PlayTube.Activities.Tabbes;
using PlayTube.Activities.Videos;
using PlayTube.Helpers.CacheLoaders;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Fonts;
using PlayTube.Helpers.Models;
using PlayTube.Helpers.Utils;
using PlayTube.Payment;
using PlayTube.SQLite;
using PlayTubeClient;
using PlayTubeClient.Classes.Global;
using PlayTubeClient.Classes.Video;
using PlayTubeClient.RestCalls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Com.Github.Library.Bubbleview;
using PlayTube.Activities.SettingsPreferences;
using Exception = System.Exception;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using static PlayTube.Helpers.Utils.Methods;

namespace PlayTube.Activities.Models
{
    public class VideoDataWithEventsLoader : Java.Lang.Object, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback, MaterialDialog.IInputCallback
    {
        private readonly TabbedMainActivity ActivityContext;
        private TextView ShareIconView, AddToIconView, ShowMoreDescriptionIconView, TextChannelName, EditIconView, TxtSubscribeCount;
        private ImageView ImageChannelView, LikeIconView, UnLikeIconView, ViewMoreCommentSection;
        private Button SubscribeChannelButton;
        private LinearLayout VideoDescriptionLayout, LikeButton, UnLikeButton, ShareButton, AddToButton, EditButton;
        private TextView VideoTitle, VideoLikeCount, VideoUnLikeCount, VideoChannelViews, VideoPublishDate, VideoCategory, UpNextTextview;
        private AutoLinkTextView VideoDescription;
        private TextView SeeMore;
        private TextSanitizer TextSanitizerAutoLink;
        private BubbleLinearLayout SubscribeCountLayout;
        private VideoObject VideoDataHandler;
        private readonly Activity Context;
        public VideoEnumTypes VideoType;
        private LinearLayout PaymentLayout;
        private LinearLayout DonateButton, RentButton;
        private string TypeDialog;
        private Switch AutoNextswitch;
        private FrameLayout VideoButtomLayout;
        private RelativeLayout CommentButtomLayout, CircleLayout;
        private bool isVideoDescriptionExpended;

        public enum VideoEnumTypes
        {
            Youtube, Normal, DailyMotion, Vimeo, Ok, Twitch, Facebook, AgeRestricted, GeoBlocked
        }

        public VideoDataWithEventsLoader(Activity activityContext)
        {
            Context = activityContext;
            ActivityContext = (TabbedMainActivity)activityContext;
        }

        #region Functions

        public void SetViews()
        {
            try
            {
                isVideoDescriptionExpended = false;
                LikeIconView = ActivityContext.FindViewById<ImageView>(Resource.Id.Likeicon);
                UnLikeIconView = ActivityContext.FindViewById<ImageView>(Resource.Id.UnLikeicon);
                ShareIconView = ActivityContext.FindViewById<TextView>(Resource.Id.Shareicon);
                AddToIconView = ActivityContext.FindViewById<TextView>(Resource.Id.AddToicon);
                EditIconView = ActivityContext.FindViewById<TextView>(Resource.Id.editIcon);

                ShowMoreDescriptionIconView = ActivityContext.FindViewById<TextView>(Resource.Id.video_ShowDiscription);
                VideoDescriptionLayout = ActivityContext.FindViewById<LinearLayout>(Resource.Id.videoDescriptionLayout);
                ImageChannelView = ActivityContext.FindViewById<ImageView>(Resource.Id.Image_Channel);
                TextChannelName = ActivityContext.FindViewById<TextView>(Resource.Id.ChannelName);
                SubscribeChannelButton = ActivityContext.FindViewById<Button>(Resource.Id.SubcribeButton);

                SubscribeCountLayout = ActivityContext.FindViewById<BubbleLinearLayout>(Resource.Id.bubble_layout);
                TxtSubscribeCount = ActivityContext.FindViewById<TextView>(Resource.Id.subcriberscount);

                LikeButton = ActivityContext.FindViewById<LinearLayout>(Resource.Id.LikeButton);
                UnLikeButton = ActivityContext.FindViewById<LinearLayout>(Resource.Id.UnLikeButton);
                ShareButton = ActivityContext.FindViewById<LinearLayout>(Resource.Id.ShareButton);
                AddToButton = ActivityContext.FindViewById<LinearLayout>(Resource.Id.AddToButton);
                EditButton = ActivityContext.FindViewById<LinearLayout>(Resource.Id.editButton);

                LikeButton.Tag = "0";
                UnLikeButton.Tag = "0";

                PaymentLayout = ActivityContext.FindViewById<LinearLayout>(Resource.Id.PaymentLayout);
                RentButton = ActivityContext.FindViewById<LinearLayout>(Resource.Id.RentButton);
                DonateButton = ActivityContext.FindViewById<LinearLayout>(Resource.Id.DonateButton);

                RentButton.Visibility = ViewStates.Gone;
                DonateButton.Visibility = ViewStates.Gone;

                VideoTitle = ActivityContext.FindViewById<TextView>(Resource.Id.video_Titile);
                VideoLikeCount = ActivityContext.FindViewById<TextView>(Resource.Id.LikeNumber);
                VideoUnLikeCount = ActivityContext.FindViewById<TextView>(Resource.Id.UnLikeNumber);
                VideoChannelViews = ActivityContext.FindViewById<TextView>(Resource.Id.Channelviews);
                VideoPublishDate = ActivityContext.FindViewById<TextView>(Resource.Id.videoDate);
                VideoDescription = ActivityContext.FindViewById<AutoLinkTextView>(Resource.Id.videoDescriptionTextview);
                SeeMore = ActivityContext.FindViewById<TextView>(Resource.Id.seeMoreTextView);
                VideoCategory = ActivityContext.FindViewById<TextView>(Resource.Id.videoCategorytextview);

                VideoButtomLayout = ActivityContext.FindViewById<FrameLayout>(Resource.Id.videoButtomLayout);
                CommentButtomLayout = ActivityContext.FindViewById<RelativeLayout>(Resource.Id.CommentButtomLayout);
                UpNextTextview = ActivityContext.FindViewById<TextView>(Resource.Id.UpNextTextview);
                ViewMoreCommentSection = ActivityContext.FindViewById<ImageView>(Resource.Id.viewMoreCommentsection);
                AutoNextswitch = ActivityContext.FindViewById<Switch>(Resource.Id.AutoNextswitch);
                AutoNextswitch.Checked = UserDetails.AutoNext;

                CircleLayout = ActivityContext.FindViewById<RelativeLayout>(Resource.Id.circlelayout);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ShareIconView, IonIconsFonts.ReplyAll);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, AddToIconView, IonIconsFonts.PlusCircled);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ShowMoreDescriptionIconView, IonIconsFonts.ArrowDownB);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EditIconView, IonIconsFonts.AndroidCreate);

                LikeButton.Click += LikeButtonOnClick;
                UnLikeButton.Click += UnLikeButtonOnClick;
                ShareButton.Click += ShareButtonOnClick;
                AddToButton.Click += AddToButtonOnClick;
                SubscribeChannelButton.Click += SubscribeChannelButtonOnClick;
                TextChannelName.Click += ImageChannelViewOnClick;
                VideoCategory.Click += VideoCategoryOnClick;
                ImageChannelView.Click += ImageChannelViewOnClick;
                ShowMoreDescriptionIconView.Click += ShowMoreDescriptionIconViewOnClick;
                EditButton.Click += EditButtonOnClick;
                RentButton.Click += RentButtonOnClick;
                DonateButton.Click += DonateButtonOnClick;
                CommentButtomLayout.Click += CommentButtomLayout_Click;
                ViewMoreCommentSection.Click += ViewMoreCommentSectionClick;
                UpNextTextview.Click += ViewMoreCommentSectionClick;
                AutoNextswitch.CheckedChange += AutoNextswitchOnCheckedChange;
                SeeMore.Click += OnVideoDescriptionClick;
                TextSanitizerAutoLink = new TextSanitizer(VideoDescription, Context, ActivityContext);
                ToggleVideoDescription();
                FragmentTransaction ftvideo = ActivityContext.SupportFragmentManager.BeginTransaction();
                ftvideo.SetCustomAnimations(Resource.Animation.slide_up, Resource.Animation.slide_down);
                ftvideo.AddToBackStack(null);
                ftvideo.Add(VideoButtomLayout.Id, ActivityContext.CommentsFragment, null).Hide(ActivityContext.CommentsFragment).Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Event

        private void OnVideoDescriptionClick(object sender, EventArgs e)
        {
            isVideoDescriptionExpended = !isVideoDescriptionExpended;
            ToggleVideoDescription();
        }

        private void ToggleVideoDescription()
        {
            if (isVideoDescriptionExpended)
            {
                VideoDescription.SetSingleLine(false);
                VideoDescription.Ellipsize = null;
            }
            else
            {
                VideoDescription.SetLines(2);
                VideoDescription.Ellipsize = TextUtils.TruncateAt.End;
            }

            ChangeSeeMoreOption();
        }

        private void ChangeSeeMoreOption()
        {
            VideoDescription.Post(() =>
            {
                var lineCount = VideoDescription.LineCount;
                SeeMore.Visibility = lineCount > 2 ? ViewStates.Visible : ViewStates.Gone;

                var id = isVideoDescriptionExpended ? Resource.String.Lbl_See_Less : Resource.String.Lbl_See_More;
                SeeMore.Text = ActivityContext.GetText(id);
            });
        }

        private void AutoNextswitchOnCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            try
            {
                UserDetails.AutoNext = AutoNextswitch.Checked;
                MainSettings.AutoNext.Edit().PutBoolean(MainSettings.PrefKeyAutoNext, UserDetails.AutoNext).Commit();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

        }

        private void CommentButtomLayout_Click(object sender, EventArgs e)
        {
            try
            {
                UpNextTextview.Text = ActivityContext.GetString(Resource.String.Lbl_Comments);
                UpNextTextview.Tag = ActivityContext.GetString(Resource.String.Lbl_Comments);
                ViewMoreCommentSection.Visibility = ViewStates.Visible;
                AutoNextswitch.Visibility = ViewStates.Gone;
                CommentButtomLayout.Visibility = ViewStates.Gone;
                FragmentTransaction ftvideo = ActivityContext.SupportFragmentManager.BeginTransaction();

                if (!ActivityContext.CommentsFragment.IsAdded)
                {
                    ftvideo.SetCustomAnimations(Resource.Animation.slide_up, Resource.Animation.slide_down);
                    ftvideo.AddToBackStack(null);
                    ftvideo.Add(VideoButtomLayout.Id, ActivityContext.CommentsFragment, null).Commit();
                }
                else
                {
                    ftvideo.SetCustomAnimations(Resource.Animation.slide_up, Resource.Animation.slide_down);
                    ftvideo.Hide(ActivityContext.NextToFragment).Show(ActivityContext.CommentsFragment).Commit();
                }

                ActivityContext.CommentsFragment.StartApiService(VideoDataHandler.Id, "0");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void ViewMoreCommentSectionClick(object sender, EventArgs e)
        {
            HideCommentsAndShowNextTo();
        }

        public void HideCommentsAndShowNextTo()
        {
            try
            {
                UpNextTextview.Text = ActivityContext.GetString(Resource.String.Lbl_NextTo);
                UpNextTextview.Tag = ActivityContext.GetString(Resource.String.Lbl_NextTo);
                ViewMoreCommentSection.Visibility = ViewStates.Gone;
                AutoNextswitch.Visibility = ViewStates.Visible;
                CommentButtomLayout.Visibility = ViewStates.Visible;
                FragmentTransaction ftvideo = ActivityContext.SupportFragmentManager.BeginTransaction();
                ftvideo.AddToBackStack(null);
                ftvideo.SetCustomAnimations(Resource.Animation.slide_up, Resource.Animation.slide_down);
                ftvideo.Hide(ActivityContext.CommentsFragment).Show(ActivityContext.NextToFragment).Commit();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void DonateButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                new MaterialDialog.Builder(ActivityContext)
                    .Theme(AppSettings.SetTabDarkTheme ? Theme.Dark : Theme.Light)
                    .Title(Resource.String.Lbl_Donate)
                    .InputType(InputTypes.ClassNumber | InputTypes.NumberVariationNormal)
                    .PositiveText(ActivityContext.GetText(Resource.String.Btn_Send)).OnPositive(this)
                    .NegativeText(Resource.String.Lbl_Cancel).OnNegative(this)
                    .Input("0.00", "", this)
                    .Build().Show();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Rent
        private void RentButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                if (AppSettings.ShowInAppBilling && Client.IsExtended && AppSettings.VideoRentalPriceStatic)
                {
                    ActivityContext.PaymentVideoObject = VideoDataHandler;

                    ActivityContext.BillingPayment.SetConnInAppBilling();
                    ActivityContext.BillingPayment.InitInAppBilling("RentVideo");
                }
                else
                {
                    TypeDialog = "Payment_RentVideo";

                    var arrayAdapter = new List<string>();
                    var dialogList = new MaterialDialog.Builder(ActivityContext).Theme(AppSettings.SetTabDarkTheme ? AFollestad.MaterialDialogs.Theme.Dark : AFollestad.MaterialDialogs.Theme.Light);

                    if (AppSettings.ShowPaypal)
                        arrayAdapter.Add(ActivityContext.GetString(Resource.String.Btn_Paypal));

                    if (AppSettings.ShowCreditCard)
                        arrayAdapter.Add(ActivityContext.GetString(Resource.String.Lbl_CreditCard));

                    dialogList.Items(arrayAdapter);
                    dialogList.NegativeText(ActivityContext.GetString(Resource.String.Lbl_Close)).OnNegative(this);
                    dialogList.AlwaysCallSingleChoiceCallback();
                    dialogList.ItemsCallback(this).Build().Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void EditButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                Bundle bundle = new Bundle();
                bundle.PutString("ItemDataVideo", JsonConvert.SerializeObject(VideoDataHandler));

                var editVideoFragment = new EditVideoFragment()
                {
                    Arguments = bundle
                };

                ActivityContext.FragmentBottomNavigator.DisplayFragment(editVideoFragment);

                switch (VideoType)
                {
                    case VideoEnumTypes.Normal:
                        ActivityContext.VideoActionsController.SetStopvideo();
                        break;
                    case VideoEnumTypes.Youtube:
                        ActivityContext.YoutubePlayer.Pause();
                        break;
                }

                if (ActivityContext.SlidingUpPanel.GetPanelState() == SlidingUpPanelLayout.PanelState.Expanded)
                    ActivityContext.SlidingUpPanel.SetPanelState(SlidingUpPanelLayout.PanelState.Collapsed);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void AddToButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                TypeDialog = "AddTo";

                var dialogList = new MaterialDialog.Builder(ActivityContext).Theme(AppSettings.SetTabDarkTheme ? Theme.Dark : Theme.Light);

                var arrayAdapter = new List<string> { ActivityContext.GetString(Resource.String.Lbl_Addto_playlist) };

                var check = ListUtils.WatchLaterVideosList.FirstOrDefault(q => q.Videos?.VideoAdClass.Id == VideoDataHandler.Id);
                arrayAdapter.Add(check == null ? ActivityContext.GetString(Resource.String.Lbl_Addto_watchlater) : ActivityContext.GetString(Resource.String.Lbl_RemoveFromWatchLater));

                dialogList.Title(ActivityContext.GetString(Resource.String.Lbl_Add_To));
                dialogList.Items(arrayAdapter);
                dialogList.NegativeText(ActivityContext.GetString(Resource.String.Lbl_Close)).OnNegative(this);
                dialogList.AlwaysCallSingleChoiceCallback();
                dialogList.ItemsCallback(this).Build().Show();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void ShareButtonOnClick(object sender, EventArgs e)
        {
            ActivityContext.LibrarySynchronizer.ShareVideo(VideoDataHandler);
        }

        private void UnLikeButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    if (UserDetails.IsLogin)
                    {
                        if (UnLikeButton.Tag.ToString() == "0")
                        {
                            UnLikeButton.Tag = "1";
                            UnLikeIconView.SetColorFilter(Color.ParseColor(AppSettings.MainColor));
                            LikeIconView.SetColorFilter(Color.ParseColor("#8e8e8e"));

                            if (!VideoUnLikeCount.Text.Contains("K") && !VideoUnLikeCount.Text.Contains("M"))
                            {
                                var x = Convert.ToDouble(VideoUnLikeCount.Text);
                                x++;
                                VideoUnLikeCount.Text = x.ToString(CultureInfo.CurrentCulture);
                            }

                            if (LikeButton.Tag.ToString() == "1")
                            {
                                LikeButton.Tag = "0";
                                if (!VideoUnLikeCount.Text.Contains("K") && !VideoUnLikeCount.Text.Contains("M"))
                                {
                                    var x = Convert.ToDouble(VideoUnLikeCount.Text);
                                    if (x > 0)
                                    {
                                        x--;
                                    }
                                    else
                                    {
                                        x = 0;
                                    }

                                    VideoUnLikeCount.Text = x.ToString(CultureInfo.CurrentCulture);
                                }
                            }

                            Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_Video_UnLiked), ToastLength.Short).Show();

                            //Send API Request here for dislike
                            PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Video.Add_Like_Dislike_Videos_Http(VideoDataHandler.Id, "dislike") });
                        }
                        else
                        {
                            UnLikeButton.Tag = "0";


                            UnLikeIconView.SetColorFilter(Color.ParseColor("#8e8e8e"));
                            var x = Convert.ToDouble(VideoUnLikeCount.Text);
                            if (!VideoUnLikeCount.Text.Contains("K") && !VideoUnLikeCount.Text.Contains("M"))
                            {
                                if (x > 0)
                                {
                                    x--;
                                }
                                else
                                {
                                    x = 0;
                                }
                                VideoUnLikeCount.Text = x.ToString(CultureInfo.CurrentCulture);
                            }

                            //Send API Request here for Remove UNLike
                            PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Video.Add_Like_Dislike_Videos_Http(VideoDataHandler.Id, "dislike") });

                        }
                    }
                    else
                    {
                        PopupDialogController dialog = new PopupDialogController(ActivityContext, VideoDataHandler, "Login");
                        dialog.ShowNormalDialog(ActivityContext.GetText(Resource.String.Lbl_Warning), ActivityContext.GetText(Resource.String.Lbl_Please_sign_in_Dislike), ActivityContext.GetText(Resource.String.Lbl_Yes), ActivityContext.GetText(Resource.String.Lbl_No));
                    }
                }
                else
                {
                    Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void LikeButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    if (UserDetails.IsLogin)
                    {
                        try
                        {
                            //If User Liked
                            if (LikeButton.Tag.ToString() == "0")
                            {
                                LikeButton.Tag = "1";
                                LikeIconView.SetColorFilter(Color.ParseColor(AppSettings.MainColor));


                                UnLikeIconView.SetColorFilter(Color.ParseColor("#8e8e8e"));
                                if (!VideoLikeCount.Text.Contains("K") && !VideoLikeCount.Text.Contains("M"))
                                {
                                    var x = Convert.ToDouble(VideoLikeCount.Text);
                                    x++;
                                    VideoLikeCount.Text = x.ToString(CultureInfo.InvariantCulture);
                                }

                                if (UnLikeButton.Tag.ToString() == "1")
                                {
                                    UnLikeButton.Tag = "0";
                                    if (!VideoUnLikeCount.Text.Contains("K") && !VideoUnLikeCount.Text.Contains("M"))
                                    {
                                        var x = Convert.ToDouble(VideoUnLikeCount.Text);
                                        if (x > 0)
                                        {
                                            x--;
                                        }
                                        else
                                        {
                                            x = 0;
                                        }
                                        VideoUnLikeCount.Text = x.ToString(CultureInfo.CurrentCulture);
                                    }
                                }


                                //AddToLiked
                                ActivityContext.LibrarySynchronizer.AddToLiked(VideoDataHandler);

                                Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_Video_Liked), ToastLength.Short).Show();

                                //Send API Request here for Like
                                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Video.Add_Like_Dislike_Videos_Http(VideoDataHandler.Id, "like") });

                            }
                            else
                            {
                                LikeButton.Tag = "0";

                                LikeIconView.SetColorFilter(Color.ParseColor("#8e8e8e"));
                                UnLikeIconView.SetColorFilter(Color.ParseColor("#8e8e8e"));
                                if (!VideoLikeCount.Text.Contains("K") && !VideoLikeCount.Text.Contains("M"))
                                {
                                    var x = Convert.ToDouble(VideoLikeCount.Text);
                                    if (x > 0)
                                    {
                                        x--;
                                    }
                                    else
                                    {
                                        x = 0;
                                    }

                                    VideoLikeCount.Text = x.ToString(CultureInfo.CurrentCulture);
                                }

                                Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_Remove_Video_Liked), ToastLength.Short).Show();

                                //Send API Request here for Remove UNLike
                                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Video.Add_Like_Dislike_Videos_Http(VideoDataHandler.Id, "like") });
                            }
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception);
                        }
                    }
                    else
                    {
                        PopupDialogController dialog = new PopupDialogController(ActivityContext, VideoDataHandler, "Login");
                        dialog.ShowNormalDialog(ActivityContext.GetText(Resource.String.Lbl_Warning), ActivityContext.GetText(Resource.String.Lbl_Please_sign_in_Like), ActivityContext.GetText(Resource.String.Lbl_Yes), ActivityContext.GetText(Resource.String.Lbl_No));
                    }
                }
                else
                {
                    Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void VideoCategoryOnClick(object sender, EventArgs e)
        {
            try
            {
                Bundle bundle = new Bundle();
                bundle.PutString("CatId", VideoDataHandler.CategoryId);
                bundle.PutString("CatName", VideoDataHandler.CategoryName);

                var videoViewerFragment = new VideosByCategoryFragment()
                {
                    Arguments = bundle
                };

                ActivityContext.FragmentBottomNavigator.DisplayFragment(videoViewerFragment);


                switch (VideoType)
                {
                    case VideoEnumTypes.Normal:
                        ActivityContext.VideoActionsController.SetStopvideo();
                        break;
                    case VideoEnumTypes.Youtube:
                        ActivityContext.YoutubePlayer.Pause();
                        break;
                }

                if (ActivityContext.SlidingUpPanel.GetPanelState() == SlidingUpPanelLayout.PanelState.Expanded)
                    ActivityContext.SlidingUpPanel.SetPanelState(SlidingUpPanelLayout.PanelState.Collapsed);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void SubscribeChannelButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    if (UserDetails.IsLogin)
                    {
                        if (!string.IsNullOrEmpty(VideoDataHandler.Owner.SubscriberPrice) && VideoDataHandler.Owner.SubscriberPrice != "0")
                        {
                            if (SubscribeChannelButton.Tag.ToString() == "PaidSubscribe")
                            {
                                //This channel is paid, You must pay to subscribe
                                ActivityContext.OpenDialog(VideoDataHandler.Owner);
                            }
                            else
                            {
                                SubscribeChannelButton.Tag = "Subscribe";
                                SubscribeChannelButton.Text = ActivityContext.GetText(Resource.String.Btn_Subscribe);
                                //Color
                                SubscribeChannelButton.SetTextColor(Color.ParseColor("#efefef"));
                                //icon
                                Drawable icon = ActivityContext.GetDrawable(Resource.Drawable.SubcribeButton);
                                icon.Bounds = new Rect(10, 10, 10, 7);
                                SubscribeChannelButton.SetCompoundDrawablesWithIntrinsicBounds(icon, null, null, null);

                                //Remove The Video to Subcribed Videos Database
                                var sqlEntity = new SqLiteDatabase();
                                sqlEntity.RemoveSubscriptionsChannel(VideoDataHandler.Owner.Id);
                                sqlEntity.Dispose();

                                //Send API Request here for UnSubcribe 
                                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Global.Add_Subscribe_To_Channel_Http(VideoDataHandler.Owner.Id) });

                                // Toast.MakeText(this, this.GetText(Resource.String.Lbl_Channel_Removed_successfully, ToastLength.Short).Show();
                            }
                        }
                        else
                        {
                            if (SubscribeChannelButton.Tag.ToString() == "Subscribe")
                            {
                                SubscribeChannelButton.Tag = "Subscribed";
                                SubscribeChannelButton.Text = ActivityContext.GetText(Resource.String.Btn_Subscribed);

                                //Color
                                SubscribeChannelButton.SetTextColor(Color.ParseColor("#efefef"));
                                //icon
                                Drawable icon = ActivityContext.GetDrawable(Resource.Drawable.SubcribedButton);
                                icon.Bounds = new Rect(10, 10, 10, 7);
                                SubscribeChannelButton.SetCompoundDrawablesWithIntrinsicBounds(icon, null, null, null);

                                //Add The Video to  Subcribed Videos Database
                                Events_Insert_SubscriptionsChannel();

                                //Send API Request here for Subcribe
                                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Global.Add_Subscribe_To_Channel_Http(VideoDataHandler.Owner.Id) });


                                Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_Subscribed_successfully), ToastLength.Short).Show();
                            }
                            else
                            {
                                SubscribeChannelButton.Tag = "Subscribe";
                                SubscribeChannelButton.Text = ActivityContext.GetText(Resource.String.Btn_Subscribe);
                                //Color
                                SubscribeChannelButton.SetTextColor(Color.ParseColor("#efefef"));
                                //icon
                                Drawable icon = ActivityContext.GetDrawable(Resource.Drawable.SubcribeButton);
                                icon.Bounds = new Rect(10, 10, 10, 7);
                                SubscribeChannelButton.SetCompoundDrawablesWithIntrinsicBounds(icon, null, null, null);

                                //Remove The Video to Subcribed Videos Database
                                var sqlEntity = new SqLiteDatabase();
                                sqlEntity.RemoveSubscriptionsChannel(VideoDataHandler.Owner.Id);
                                sqlEntity.Dispose();

                                //Send API Request here for UnSubcribe
                                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Global.Add_Subscribe_To_Channel_Http(VideoDataHandler.Owner.Id) });

                                // Toast.MakeText(this, this.GetText(Resource.String.Lbl_Channel_Removed_successfully, ToastLength.Short).Show();
                            }
                        }
                    }
                    else
                    {
                        PopupDialogController dialog = new PopupDialogController(ActivityContext, VideoDataHandler, "Login");
                        dialog.ShowNormalDialog(ActivityContext.GetText(Resource.String.Lbl_Warning), ActivityContext.GetText(Resource.String.Lbl_Please_sign_in_Subcribed), ActivityContext.GetText(Resource.String.Lbl_Yes), ActivityContext.GetText(Resource.String.Lbl_No));
                    }
                }
                else
                {
                    Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public async void SetSubscribeChannelWithPaid()
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    SubscribeChannelButton.Tag = "Subscribed";
                    SubscribeChannelButton.Text = ActivityContext.GetText(Resource.String.Btn_Subscribed);

                    //Color
                    SubscribeChannelButton.SetTextColor(Color.ParseColor("#efefef"));
                    //icon
                    Drawable icon = ActivityContext.GetDrawable(Resource.Drawable.SubcribedButton);
                    icon.Bounds = new Rect(10, 10, 10, 7);
                    SubscribeChannelButton.SetCompoundDrawablesWithIntrinsicBounds(icon, null, null, null);

                    //Add The Video to  Subscribed Videos Database
                    Events_Insert_SubscriptionsChannel();

                    //Send API Request here for Subscribed
                    (int apiStatus, var respond) = await RequestsAsync.Global.Add_Subscribe_To_Channel_Http(VideoDataHandler.Owner.Id, "paid");
                    if (apiStatus == 200)
                    {
                        if (respond is MessageObject result)
                        {
                            Console.WriteLine(result.Message);
                            ActivityContext.RunOnUiThread(() =>
                            {
                                Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_Subscribed_successfully), ToastLength.Short).Show();
                            });
                        }
                    }
                    else Methods.DisplayReportResult(ActivityContext, respond);
                }
                else
                {
                    Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ImageChannelViewOnClick(object sender, EventArgs e)
        {
            ActivityContext.ShowUserChannelFragment(VideoDataHandler.Owner, VideoDataHandler.Owner.Id);
        }

        private void ShowMoreDescriptionIconViewOnClick(object sender, EventArgs e)
        {
            try
            {
                if (VideoDescriptionLayout.Tag.ToString() == "Open")
                {
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ShowMoreDescriptionIconView, IonIconsFonts.ArrowDownB);
                    //VideoDescriptionLayout.Visibility = ViewStates.Gone;
                    VideoDescriptionLayout.Tag = "closed";
                    VideoTitle.Text = Methods.FunString.DecodeString(VideoDataHandler.Title);
                    VideoDescriptionLayout.Animate().Alpha(1).SetDuration(400);
                    TextChannelName.Animate().Alpha(1).SetDuration(300);
                    VideoChannelViews.Animate().Alpha(1).SetDuration(300);
                    VideoTitle.SetMaxLines(1);

                    ViewGroup parent = (ViewGroup)VideoDescription.Parent;
                    ViewGroup.LayoutParams par = parent.LayoutParameters;
                    par.Height = 200;
                    VideoDescriptionLayout.LayoutParameters = par;

                }
                else
                {
                    // VideoDescriptionLayout.LayoutParameters = ViewGroup.LayoutParams.WrapContent;
                    //LinearLayout.LayoutParams par = (LinearLayout.LayoutParams)VideoDescriptionLayout.LayoutParameters;
                    ViewGroup parent = (ViewGroup)VideoDescription.Parent;
                    ViewGroup.LayoutParams par = parent.LayoutParameters;
                    par.Height = ViewGroup.LayoutParams.WrapContent;
                    VideoDescriptionLayout.LayoutParameters = par;

                    FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ShowMoreDescriptionIconView, IonIconsFonts.ArrowUpB);
                    //VideoDescriptionLayout.Visibility = ViewStates.Visible;
                    VideoDescriptionLayout.Tag = "Open";
                    VideoTitle.Text = Methods.FunString.DecodeString(VideoDataHandler.Title);
                    VideoDescriptionLayout.Animate().Alpha(1).SetDuration(500);
                    TextChannelName.Animate().Alpha(1).SetDuration(300);
                    VideoChannelViews.Animate().Alpha(1).SetDuration(300);
                    VideoTitle.SetMaxLines(4);


                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Event Menu >> WatchLater
        private async void OnMenuAddWatchLaterClick(VideoObject videoObject)
        {
            try
            {
                if (UserDetails.IsLogin)
                {
                    //Send API Request here for WatchLater
                    if (Methods.CheckConnectivity())
                    {
                        (int apiStatus, var respond) = await RequestsAsync.Video.AddToWatchLaterVideos_Http(VideoDataHandler.Id);
                        if (apiStatus == 200)
                        {
                            if (respond is MessageCodeObject result)
                            {
                                if (result.SuccessType.Contains("Removed"))
                                {
                                    ActivityContext.LibrarySynchronizer.RemovedFromWatchLater(videoObject);
                                    Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_RemovedFromWatchLater), ToastLength.Short).Show();
                                }
                                else if (result.SuccessType.Contains("Added"))
                                {
                                    ActivityContext.LibrarySynchronizer.AddToWatchLater(videoObject);
                                    Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_AddedToWatchLater), ToastLength.Short).Show();
                                }
                            }
                        }
                        else Methods.DisplayReportResult(ActivityContext, respond);
                    }
                    else
                    {
                        Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
                    }
                }
                else
                {
                    PopupDialogController dialog = new PopupDialogController(ActivityContext, videoObject, "Login");
                    dialog.ShowNormalDialog(ActivityContext.GetText(Resource.String.Lbl_Warning), ActivityContext.GetText(Resource.String.Lbl_Please_sign_in_WatchLater), ActivityContext.GetText(Resource.String.Lbl_Yes), ActivityContext.GetText(Resource.String.Lbl_No));
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Event Menu >> Playlist
        private void OnMenuAddPlaylistClick(VideoObject videoObject)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    if (UserDetails.IsLogin)
                    {
                        PopupDialogController dialog = new PopupDialogController(ActivityContext, videoObject, "PlayList");
                        dialog.ShowPlayListDialog();
                    }
                    else
                    {
                        PopupDialogController dialog = new PopupDialogController(ActivityContext, videoObject, "Login");
                        dialog.ShowNormalDialog(ActivityContext.GetText(Resource.String.Lbl_Warning), ActivityContext.GetText(Resource.String.Lbl_Please_sign_in_playlist), ActivityContext.GetText(Resource.String.Lbl_Yes), ActivityContext.GetText(Resource.String.Lbl_No));
                    }
                }
                else
                {
                    Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void Events_Insert_SubscriptionsChannel()
        {
            try
            {
                var sqlEntity = new SqLiteDatabase();

                if (VideoDataHandler.Owner != null)
                    sqlEntity.Insert_One_SubscriptionChannel(VideoDataHandler.Owner);

                sqlEntity.Dispose();

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Fun Video

        private void SetVideoType(VideoObject videoObject)
        {
            try
            {
                VideoType = VideoEnumTypes.Normal;

                if (videoObject.VideoLocation.Contains("Youtube") || videoObject.VideoLocation.Contains("youtu") || videoObject.VideoType == "VideoObject/youtube")
                    VideoType = VideoEnumTypes.Youtube;
                else if (!string.IsNullOrEmpty(videoObject.Vimeo))
                    VideoType = VideoEnumTypes.Vimeo;
                else if (!string.IsNullOrEmpty(videoObject.Daily))
                    VideoType = VideoEnumTypes.DailyMotion;
                else if (!string.IsNullOrEmpty(videoObject.Ok))
                    VideoType = VideoEnumTypes.Ok;
                else if (!string.IsNullOrEmpty(videoObject.Twitch))
                    VideoType = VideoEnumTypes.Twitch;
                else if (!string.IsNullOrEmpty(videoObject.Facebook))
                    VideoType = VideoEnumTypes.Facebook;
                else if (videoObject.IsOwner != null && (videoObject.AgeRestriction == "2" && videoObject.IsOwner.Value == false))
                    VideoType = VideoEnumTypes.AgeRestricted;
                else if (!string.IsNullOrEmpty(videoObject.GeoBlocking) && videoObject.IsOwner == false)
                    VideoType = VideoEnumTypes.GeoBlocked;

                videoObject.VideoType = VideoType.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void LoadVideoData(VideoObject videoObject)
        {
            try
            {
                if (videoObject == null)
                    return;

                VideoDataHandler = videoObject;
                SetVideoType(VideoDataHandler);

                //Run fast data fetch from the server 
                if (Methods.CheckConnectivity())
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => GetVideosInfoAsJson(videoObject.VideoId) });

                GlideImageLoader.LoadImage(Context, videoObject.Owner.Avatar, ImageChannelView, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);
                VideoTitle.Text = Methods.FunString.DecodeString(videoObject.Title);
                TextChannelName.Text = videoObject.Owner.Username;
                VideoLikeCount.Text = videoObject.LikesPercent;
                VideoUnLikeCount.Text = videoObject.DislikesPercent;
                VideoChannelViews.Text = videoObject.Views + " " + ActivityContext.GetText(Resource.String.Lbl_Views) + " | " + Methods.Time.ReplaceTime(videoObject.TimeAgo);
                VideoPublishDate.Text = ActivityContext.GetText(Resource.String.Lbl_Published_on) + " " + DateTimeExtension.ConvertToSpanishFormatIfNeeded(videoObject.TimeDate);
                VideoCategory.Text = CategoriesController.GetCategoryName(videoObject);

                ActivityContext.VideoChannelText.Text = TextChannelName.Text;
                ActivityContext.VideoTitleText.Text = VideoTitle.Text;

                if (videoObject.Owner.Verified == "1")
                {
                    ActivityContext.VideoChannelText.SetCompoundDrawablesWithIntrinsicBounds(0, 0, Resource.Drawable.icon_checkmark_small_vector, 0);
                    TextChannelName.SetCompoundDrawablesWithIntrinsicBounds(0, 0, Resource.Drawable.icon_checkmark_small_vector, 0);
                }
                else
                {
                    ActivityContext.VideoChannelText.SetCompoundDrawablesWithIntrinsicBounds(0, 0, 0, 0);
                    TextChannelName.SetCompoundDrawablesWithIntrinsicBounds(0, 0, 0, 0);
                }

                isVideoDescriptionExpended = false;
                TextSanitizerAutoLink.Load(Methods.FunString.DecodeString(videoObject.Description));
                ToggleVideoDescription();

                EditButton.Visibility = videoObject.IsOwner != null && videoObject.IsOwner.Value ? ViewStates.Visible : ViewStates.Gone;

                //Reset Views
                LikeIconView.SetColorFilter(Color.ParseColor("#8e8e8e"));
                LikeButton.Tag = "0";
                UnLikeIconView.SetColorFilter(Color.ParseColor("#8e8e8e"));
                UnLikeButton.Tag = "0";
                VideoLikeCount.Text = "0";
                VideoUnLikeCount.Text = "0";

                SubscribeChannelButton.Tag = "Subscribe";
                SubscribeChannelButton.Text = ActivityContext.GetText(Resource.String.Btn_Subscribe);
                SubscribeChannelButton.SetTextColor(Color.ParseColor("#efefef"));
                Drawable icon = ActivityContext.GetDrawable(Resource.Drawable.SubcribeButton);
                icon.Bounds = new Rect(10, 10, 10, 7);
                SubscribeChannelButton.SetCompoundDrawablesWithIntrinsicBounds(icon, null, null, null);

                //Close the description panel
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ShowMoreDescriptionIconView, IonIconsFonts.ArrowDownB);

                //VideoDescriptionLayout.Visibility = ViewStates.Gone;
                VideoDescriptionLayout.Tag = "closed";
                VideoTitle.Text = Methods.FunString.DecodeString(videoObject.Title);

                //Clear all data 
                if (ActivityContext.CommentsFragment != null && ActivityContext.CommentsFragment.MAdapter != null)
                {
                    ActivityContext.CommentsFragment.MAdapter.CommentList?.Clear();
                    ActivityContext.NextToFragment.MAdapter.VideoList?.Clear();
                    ActivityContext.CommentsFragment.MAdapter.NotifyDataSetChanged();
                    ActivityContext.NextToFragment.MAdapter.NotifyDataSetChanged();
                    ActivityContext.CommentsFragment.StartApiService(videoObject.Id, "0");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public async void SetNewDataVideo(VideoObject videoObject)
        {
            try
            {
                if (videoObject == null)
                    return;

                VideoDataHandler = videoObject;

                SetVideoType(VideoDataHandler);

                VideoChannelViews.Text = videoObject.Views + " " + ActivityContext.GetText(Resource.String.Lbl_Views) + " | " + Methods.Time.ReplaceTime(videoObject.TimeAgo);
                VideoTitle.Text = Methods.FunString.DecodeString(videoObject.Title);

                if (videoObject.IsLiked == "1") // true
                {
                    LikeIconView.SetColorFilter(Color.ParseColor(AppSettings.MainColor));
                    UnLikeIconView.SetColorFilter(Color.ParseColor("#8e8e8e"));
                    LikeButton.Tag = "1";
                }
                else
                {
                    LikeIconView.SetColorFilter(Color.ParseColor("#8e8e8e"));
                    LikeButton.Tag = "0";
                }

                if (videoObject.IsDisliked == "1") // true
                {
                    LikeIconView.SetColorFilter(Color.ParseColor("#8e8e8e"));
                    UnLikeIconView.SetColorFilter(Color.ParseColor(AppSettings.MainColor));
                    UnLikeButton.Tag = "1";
                }
                else
                {
                    UnLikeIconView.SetColorFilter(Color.ParseColor("#8e8e8e"));
                    UnLikeButton.Tag = "0";
                }

                var isOwner = videoObject.IsOwner != null && videoObject.IsOwner.Value;
                SubscribeChannelButton.Visibility = isOwner && AppSettings.HideSubscribeForOwner ? ViewStates.Invisible : ViewStates.Visible;
                SubscribeCountLayout.Visibility = isOwner && AppSettings.HideSubscribeForOwner ? ViewStates.Invisible : ViewStates.Visible;
                EditButton.Visibility = isOwner ? ViewStates.Visible : ViewStates.Gone;

                VideoLikeCount.Text = videoObject.Likes;
                VideoUnLikeCount.Text = videoObject.Dislikes;
                VideoPublishDate.Text = ActivityContext.GetText(Resource.String.Lbl_Published_on) + " " + DateTimeExtension.ConvertToSpanishFormatIfNeeded(videoObject.TimeDate);
                VideoCategory.Text = CategoriesController.GetCategoryName(videoObject);

                //Verified 
                TextChannelName.SetCompoundDrawablesWithIntrinsicBounds(0, 0, videoObject.Owner.Verified == "1" ? Resource.Drawable.icon_checkmark_small_vector : 0, 0);
                TextChannelName.Text = AppTools.GetNameFinal(videoObject.Owner);
                GlideImageLoader.LoadImage(ActivityContext, videoObject.Owner.Avatar, ImageChannelView, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);

                if (string.IsNullOrEmpty(videoObject.Owner.SubCount))
                    videoObject.Owner.SubCount = "0";

                TxtSubscribeCount.Text = videoObject.Owner.SubCount;

                isVideoDescriptionExpended = false;
                TextSanitizerAutoLink.Load(Methods.FunString.DecodeString(videoObject.Description));
                ToggleVideoDescription();

                //Rent
                if (!string.IsNullOrEmpty(videoObject.RentPrice) && videoObject.RentPrice != "0" && AppSettings.RentVideosSystem)
                {
                    RentButton.Visibility = ViewStates.Visible;
                }
                else
                {
                    RentButton.Visibility = ViewStates.Gone;
                }

                //Donate
                if (!string.IsNullOrEmpty(videoObject.Owner.DonationPaypalEmail) && AppSettings.DonateVideosSystem)
                {
                    DonateButton.Visibility = ViewStates.Visible;
                }
                else
                {
                    DonateButton.Visibility = ViewStates.Gone;
                }

                var file = VideoDownloadAsyncController.GetDownloadedDiskVideoUri(videoObject.VideoId);
                if (!string.IsNullOrEmpty(file))
                {
                    ActivityContext.VideoActionsController.DownloadIcon.SetImageResource(Resource.Drawable.ic_checked_red);
                    ActivityContext.VideoActionsController.DownloadIcon.Tag = "Downloaded";
                    ActivityContext.LibrarySynchronizer.AddToWatchOffline(videoObject);
                }

                if (videoObject.Owner.Id != UserDetails.UserId)
                {
                    UserDataObject channel = await ApiRequest.GetChannelData(ActivityContext, videoObject.Owner.Id);
                    if (channel != null)
                    {
                        videoObject.Owner = channel;

                        if (!string.IsNullOrEmpty(videoObject.Owner.SubscriberPrice) && videoObject.Owner.SubscriberPrice != "0")
                        {
                            if (videoObject.Owner.IsSubscribedToChannel == "0")
                            {
                                //This channel is paid, You must pay to subscribe
                                SubscribeChannelButton.Tag = "PaidSubscribe";

                                //Color
                                SubscribeChannelButton.SetTextColor(Color.ParseColor("#efefef"));
                                //icon
                                Drawable icon = ActivityContext.GetDrawable(Resource.Drawable.SubcribeButton);
                                icon.Bounds = new Rect(10, 10, 10, 7);
                                SubscribeChannelButton.SetCompoundDrawablesWithIntrinsicBounds(icon, null, null, null);

                                var idCurrency = ListUtils.MySettingsList?.PaymentCurrency;
                                var (currency, currencyIcon) = AppTools.GetCurrency(idCurrency);
                                Console.WriteLine(currency);
                                SubscribeChannelButton.Text = ActivityContext.GetText(Resource.String.Btn_Subscribe) + " " + currencyIcon + videoObject.Owner.SubscriberPrice;
                            }
                            else
                            {
                                SubscribeChannelButton.Tag = "Subscribed";

                                SubscribeChannelButton.Text = ActivityContext.GetText(Resource.String.Btn_Subscribed);

                                //Color
                                SubscribeChannelButton.SetTextColor(Color.ParseColor("#efefef"));
                                //icon
                                Drawable icon = ActivityContext.GetDrawable(Resource.Drawable.SubcribedButton);
                                icon.Bounds = new Rect(10, 10, 10, 7);
                                SubscribeChannelButton.SetCompoundDrawablesWithIntrinsicBounds(icon, null, null, null);
                            }
                        }
                        else
                        {
                            SubscribeChannelButton.Tag = videoObject.Owner.IsSubscribedToChannel == "0" ? "Subscribe" : "Subscribed";

                            if (SubscribeChannelButton.Tag.ToString() == "Subscribed")
                            {
                                SubscribeChannelButton.Text = ActivityContext.GetText(Resource.String.Btn_Subscribed);

                                //Color
                                SubscribeChannelButton.SetTextColor(Color.ParseColor("#efefef"));
                                //icon
                                Drawable icon = ActivityContext.GetDrawable(Resource.Drawable.SubcribedButton);
                                icon.Bounds = new Rect(10, 10, 10, 7);
                                SubscribeChannelButton.SetCompoundDrawablesWithIntrinsicBounds(icon, null, null, null);
                            }
                            else if (SubscribeChannelButton.Tag.ToString() == "Subscribe")
                            {
                                SubscribeChannelButton.Text = ActivityContext.GetText(Resource.String.Btn_Subscribe);

                                //Color
                                SubscribeChannelButton.SetTextColor(Color.ParseColor("#efefef"));
                                //icon
                                Drawable icon = ActivityContext.GetDrawable(Resource.Drawable.SubcribeButton);
                                icon.Bounds = new Rect(10, 10, 10, 7);
                                SubscribeChannelButton.SetCompoundDrawablesWithIntrinsicBounds(icon, null, null, null);
                            }
                        }

                        //Verified 
                        TextChannelName.SetCompoundDrawablesWithIntrinsicBounds(0, 0, videoObject.Owner.Verified == "1" ? Resource.Drawable.icon_checkmark_small_vector : 0, 0);
                    }
                }
                else
                {
                    UserDataObject channel = ListUtils.MyChannelList.FirstOrDefault();
                    if (channel == null) return;
                    videoObject.Owner = channel;
                    //Verified 
                    TextChannelName.SetCompoundDrawablesWithIntrinsicBounds(0, 0, videoObject.Owner.Verified == "1" ? Resource.Drawable.icon_checkmark_small_vector : 0, 0);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async Task GetVideosInfoAsJson(string videoId)
        {
            try
            {
                var (apiStatus, respond) = await RequestsAsync.Video.API_Get_Videos_Details_Http(videoId, UserDetails.AndroidId);
                if (apiStatus == 200)
                {
                    if (respond is GetVideosDetailsObject result)
                    {
                        var updater = ListUtils.GlobalViewsVideosList.FirstOrDefault(a => a.VideoId == videoId);
                        if (updater != null)
                        {
                            ListUtils.GlobalViewsVideosList.Add(updater);
                            SetNewDataVideo(updater);
                        }
                        else
                        {
                            ListUtils.GlobalViewsVideosList.Add(result.DataResult);
                            SetNewDataVideo(result.DataResult);
                        }

                        result.DataResult.SuggestedVideos = AppTools.ListFilter(result.DataResult.SuggestedVideos);
                        ListUtils.ArrayListPlay = new ObservableCollection<VideoObject>(result.DataResult.SuggestedVideos);
                        ActivityContext.NextToFragment.LoadDataAsync(new ObservableCollection<VideoObject>(result.DataResult.SuggestedVideos));

                        if (ListUtils.AdsVideoList.Count > 0)
                        {
                            if (result.DataResult.VideoAd.VideoAdClass != null)
                                ListUtils.AdsVideoList.Add(result.DataResult.VideoAd.VideoAdClass);
                        }
                        else
                        {
                            ListUtils.AdsVideoList = new ObservableCollection<VideoAdDataObject>();

                            if (result.DataResult.VideoAd.VideoAdClass != null)
                                ListUtils.AdsVideoList.Add(result.DataResult.VideoAd.VideoAdClass);
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

        #region MaterialDialog

        public void OnSelection(MaterialDialog p0, View p1, int itemId, ICharSequence itemString)
        {
            try
            {
                if (TypeDialog == "AddTo")
                {
                    if (itemString.ToString() == ActivityContext.GetString(Resource.String.Lbl_Addto_playlist))
                    {
                        OnMenuAddPlaylistClick(VideoDataHandler);
                    }
                    else if (itemString.ToString() == ActivityContext.GetString(Resource.String.Lbl_Addto_watchlater) || itemString.ToString() == ActivityContext.GetString(Resource.String.Lbl_RemoveFromWatchLater))
                    {
                        OnMenuAddWatchLaterClick(VideoDataHandler);
                    }
                }
                else if (TypeDialog == "Payment_RentVideo")
                {
                    string price = AppSettings.VideoRentalPriceStatic && AppSettings.VideoRentalPrice > 0
                        ? AppSettings.VideoRentalPrice.ToString()
                        : VideoDataHandler.RentPrice;

                    if (itemString.ToString() == ActivityContext.GetString(Resource.String.Btn_Paypal))
                    {
                        ActivityContext.Price = price;
                        ActivityContext.PayType = "RentVideo";
                        ActivityContext.PaymentVideoObject = VideoDataHandler;
                        ActivityContext.InitPayPalPayment.BtnPaypalOnClick(price, "RentVideo");
                    }
                    else if (itemString.ToString() == ActivityContext.GetString(Resource.String.Lbl_CreditCard))
                    {
                        Intent intent = new Intent(ActivityContext, typeof(PaymentCardDetailsActivity));
                        intent.PutExtra("Price", price);
                        intent.PutExtra("payType", "RentVideo");
                        intent.PutExtra("Id", VideoDataHandler.Id);
                        ActivityContext.StartActivity(intent);
                    }
                }
                else if (TypeDialog == "Payment_DonateVideo")
                {
                    if (itemString.ToString() == ActivityContext.GetString(Resource.String.Btn_Paypal))
                    {
                        ActivityContext.Price = stNUmber;
                        ActivityContext.PayType = "DonateVideo";
                        ActivityContext.InitPayPalPayment.BtnPaypalOnClick(stNUmber, "DonateVideo");
                    }
                    else if (itemString.ToString() == ActivityContext.GetString(Resource.String.Lbl_CreditCard))
                    {
                        Intent intent = new Intent(ActivityContext, typeof(PaymentCardDetailsActivity));
                        intent.PutExtra("Price", stNUmber);
                        intent.PutExtra("payType", "DonateVideo");
                        ActivityContext.StartActivity(intent);
                    }
                }
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

        private string stNUmber;
        public void OnInput(MaterialDialog p0, ICharSequence p1)
        {
            try
            {
                if (p1.Length() > 0)
                {
                    if (Methods.CheckConnectivity())
                    {
                        stNUmber = p1.ToString();

                        TypeDialog = "Payment_DonateVideo";

                        var arrayAdapter = new List<string>();
                        var dialogList = new MaterialDialog.Builder(ActivityContext).Theme(AppSettings.SetTabDarkTheme ? AFollestad.MaterialDialogs.Theme.Dark : AFollestad.MaterialDialogs.Theme.Light);

                        if (AppSettings.ShowPaypal)
                            arrayAdapter.Add(ActivityContext.GetString(Resource.String.Btn_Paypal));

                        if (AppSettings.ShowCreditCard)
                            arrayAdapter.Add(ActivityContext.GetString(Resource.String.Lbl_CreditCard));

                        dialogList.Items(arrayAdapter);
                        dialogList.NegativeText(ActivityContext.GetString(Resource.String.Lbl_Close)).OnNegative(this);
                        dialogList.AlwaysCallSingleChoiceCallback();
                        dialogList.ItemsCallback(this).Build().Show();
                    }
                    else
                    {
                        Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                    }
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