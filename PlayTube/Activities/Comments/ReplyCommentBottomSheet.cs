using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AT.Markushi.UI;
using Com.Luseen.Autolinklibrary;
using Developer.SEmojis.Actions;
using Developer.SEmojis.Helper;
using Newtonsoft.Json;
using PlayTube.Activities.Article;
using PlayTube.Activities.Comments.Adapters;
using PlayTube.Activities.Default;
using PlayTube.Activities.Tabbes;
using PlayTube.Helpers.CacheLoaders;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Fonts;
using PlayTube.Helpers.Models;
using PlayTube.Helpers.Utils;
using PlayTubeClient.Classes.Comment;
using PlayTubeClient.Classes.Global;
using PlayTubeClient.RestCalls;
using static Android.Support.Design.Widget.BottomSheetBehavior;

namespace PlayTube.Activities.Comments
{
    public  class ReplyCommentBottomSheet : BottomSheetDialogFragment
    {
        #region Variables Basic

        private LinearLayout RootView;
        private View Inflated;
        private ViewStub EmptyStateLayout;
        private EmojiconEditText EmojiconEditTextView;
        private AppCompatImageView Emojiicon;
        private CircleButton SendButton;
        private RecyclerView ReplyRecyclerView;
        private LinearLayoutManager MLayoutManager;
        public ReplyAdapter ReplyAdapter;
        private ImageView Image;
        private AutoLinkTextView CommentText;
        private TextView TimeTextView;
        private TextView UsernameTextView;
        private ImageView LikeiconView;
        private ImageView UnLikeiconView;
        private TextView ReplyiconView;
        private TextView LikeNumber;
        private TextView UnLikeNumber;
        private TextView RepliesCount;
        private LinearLayout LikeButton;
        private LinearLayout UnLikeButton;
        private LinearLayout ReplyButton;

        private readonly BottomSheetCallback MBottomSheetBehaviorCallback = new MyBottomSheetCallBack();
        private CommentObject Comment = new CommentObject();
        private TabbedMainActivity ActivityContext;
        private string Type;
        private RecyclerViewOnScrollListener MainScrollEvent;
        private CommentClickListener CommentClickListener;
        private static ReplyCommentBottomSheet Instance;
        #endregion

        #region General

        public override void SetupDialog(Dialog dialog, int style)
        {
            try
            {
                base.SetupDialog(dialog, style);
                View contentView = View.Inflate(Context, Resource.Layout.Style_Bottom_Sheet_Reply, null);
                dialog.SetContentView(contentView);
                var layoutParams = (CoordinatorLayout.LayoutParams)((View)contentView.Parent).LayoutParameters;
                var behavior = layoutParams.Behavior;

                if (behavior != null && behavior.GetType() == typeof(BottomSheetBehavior))
                    ((BottomSheetBehavior)behavior).SetBottomSheetCallback(MBottomSheetBehaviorCallback);

                Instance = this;

                Type = Arguments.GetString("Type");

                if (Arguments.ContainsKey("Object"))
                    Comment = JsonConvert.DeserializeObject<CommentObject>(Arguments.GetString("Object"));

                if (Type == "video") ActivityContext = TabbedMainActivity.GetInstance();

                InitComponent(contentView);
                SetRecyclerViewAdapters();

                CommentClickListener = new CommentClickListener(ActivityContext , "Reply");
                SendButton.Click += SendButton_Click;
                LikeButton.Click += OnLikeButtonClick;
                UnLikeButton.Click += OnUnLikeButtonClick;

                LoadReplies();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        public override void OnStart()
        {
            try
            {
                base.OnStart();
                var dialog = Dialog;
                //Make dialog full screen with transparent background
                if (dialog != null)
                {
                    var width = ViewGroup.LayoutParams.MatchParent;
                    var height = ViewGroup.LayoutParams.MatchParent;
                    dialog.Window.SetLayout(width, height);
                    dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion
          
        #region Functions

        private void InitComponent(View contentView)
        {
            try
            {
                Image = contentView.FindViewById<ImageView>(Resource.Id.card_pro_pic);
                CommentText = contentView.FindViewById<AutoLinkTextView>(Resource.Id.active);
                UsernameTextView = contentView.FindViewById<TextView>(Resource.Id.username);
                TimeTextView = contentView.FindViewById<TextView>(Resource.Id.time);
                LikeiconView = contentView.FindViewById<ImageView>(Resource.Id.Likeicon);
                UnLikeiconView = contentView.FindViewById<ImageView>(Resource.Id.UnLikeicon);
                ReplyiconView = contentView.FindViewById<TextView>(Resource.Id.ReplyIcon);
                LikeNumber = contentView.FindViewById<TextView>(Resource.Id.LikeNumber);
                UnLikeNumber = contentView.FindViewById<TextView>(Resource.Id.UnLikeNumber);
                RepliesCount = contentView.FindViewById<TextView>(Resource.Id.RepliesCount);
                LikeButton = contentView.FindViewById<LinearLayout>(Resource.Id.LikeButton);
                UnLikeButton = contentView.FindViewById<LinearLayout>(Resource.Id.UnLikeButton);
                ReplyButton = contentView.FindViewById<LinearLayout>(Resource.Id.ReplyButton);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ReplyiconView, IonIconsFonts.Reply);

                RootView = contentView.FindViewById<LinearLayout>(Resource.Id.root);
                EmojiconEditTextView = contentView.FindViewById<EmojiconEditText>(Resource.Id.EmojiconEditText5);
                Emojiicon = contentView.FindViewById<AppCompatImageView>(Resource.Id.emojiicon);
                SendButton = contentView.FindViewById<CircleButton>(Resource.Id.sendButton);
                ReplyRecyclerView = contentView.FindViewById<RecyclerView>(Resource.Id.recyler);
                EmptyStateLayout = contentView.FindViewById<ViewStub>(Resource.Id.viewStub);
                   
                var emojisIcon = new EmojIconActions(Activity, RootView, EmojiconEditTextView, Emojiicon);
                emojisIcon.ShowEmojIcon(); 
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
                ReplyAdapter = new ReplyAdapter(Activity);
                MLayoutManager = new LinearLayoutManager(Activity);
               ReplyRecyclerView.SetLayoutManager(MLayoutManager);
               ReplyRecyclerView.SetAdapter(ReplyAdapter);
               ReplyAdapter.AvatarClick += ReplyAdapterOnAvatarClick;
               ReplyAdapter.ReplyClick += ReplyAdapterOnReplyClick;
               ReplyAdapter.ItemLongClick += ReplyAdapterOnItemLongClick;

                RecyclerViewOnScrollListener recyclerViewOnScrollListener = new RecyclerViewOnScrollListener(MLayoutManager);
                MainScrollEvent = recyclerViewOnScrollListener;
                MainScrollEvent.LoadMoreEvent += OnScroll_OnLoadMoreEvent;
                ReplyRecyclerView.AddOnScrollListener(recyclerViewOnScrollListener);
                MainScrollEvent.IsLoading = false; 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static ReplyCommentBottomSheet GetInstance()
        {
            try
            {
                return Instance;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
        private void ReplyAdapterOnItemLongClick(object sender, ReplyAdapterClickEventArgs e)
        {
            try
            {
                if (e.Position <= -1) return;

                var item = ReplyAdapter.GetItem(e.Position);
                if (item == null) return;

                CommentClickListener.MoreReplyPostClick(item); 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion
         
        #region Get Replies

        private void LoadReplies()
        {
            try
            {
                if (Comment == null) return;

                GlideImageLoader.LoadImage(ActivityContext, Comment.CommentUserData.Avatar, Image, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);

                TextSanitizer chnager = new TextSanitizer(CommentText, ActivityContext);
                chnager.Load(Methods.FunString.DecodeString(Comment.Text));
                TimeTextView.Text = Comment.TextTime;

                UsernameTextView.Text = AppTools.GetNameFinal(Comment.CommentUserData);

                LikeNumber.Text = Methods.FunString.FormatPriceValue(Convert.ToInt32(Comment.Likes));
                UnLikeNumber.Text = Methods.FunString.FormatPriceValue(Convert.ToInt32(Comment.Likes));
                RepliesCount.Text = Comment.RepliesCount.ToString();

                if (Comment.IsLikedComment == 1)
                {
                    LikeiconView.SetColorFilter(Color.ParseColor(AppSettings.MainColor));
                    LikeButton.Tag = "1";
                }
                else
                {
                    LikeButton.Tag = "0";
                }

                if (Comment.IsDislikedComment == 1)
                {
                    UnLikeiconView.SetColorFilter(Color.ParseColor(AppSettings.MainColor));
                    UnLikeButton.Tag = "1";
                }
                else
                {
                    UnLikeButton.Tag = "0";
                }
                 
                if (Comment.CommentReplies?.Count > 0)
                {
                    ReplyAdapter.ReplyList = new ObservableCollection<ReplyObject>(Comment.CommentReplies);
                    ReplyAdapter.NotifyDataSetChanged();
                    EmptyStateLayout.Visibility = ViewStates.Gone;
                }

                string offset = ReplyAdapter.ReplyList.LastOrDefault()?.Id.ToString() ?? "0";
                StartApiService(offset);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion
     
        #region Load Data Api

        private void StartApiService(string offset = "0")
        {
            if (!Methods.CheckConnectivity())
                Toast.MakeText(Context, Context.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
            else
                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => LoadDataAsync(offset) });
        }

        private async Task LoadDataAsync(string offset = "0")
        {
            if (Methods.CheckConnectivity())
            {
                if (UserDetails.IsLogin)
                {
                    if (MainScrollEvent.IsLoading)
                        return;

                    MainScrollEvent.IsLoading = true;

                    int countList = ReplyAdapter.ReplyList.Count;

                    var (apiStatus, respond) = await RequestsAsync.Comments.Get_Replies_Http(Comment.Id.ToString(), "20", offset);
                    if (apiStatus != 200 || !(respond is GetRepliesObject result) || result.Data == null)
                    {
                        MainScrollEvent.IsLoading = false;
                        Methods.DisplayReportResult(Activity, respond);
                    }
                    else
                    {
                        var respondList = result.Data.Count;
                        if (respondList > 0)
                        {
                            if (countList > 0)
                            {
                                foreach (var item in from item in result.Data let check = ReplyAdapter.ReplyList.FirstOrDefault(a => a.Id == item.Id) where check == null select item)
                                {
                                    ReplyAdapter.ReplyList.Insert(0, item);
                                }

                                Activity.RunOnUiThread(() => { ReplyAdapter.NotifyItemRangeInserted(countList, ReplyAdapter.ReplyList.Count - countList); });
                            }
                            else
                            {
                                ReplyAdapter.ReplyList = new ObservableCollection<ReplyObject>(result.Data);
                                Activity.RunOnUiThread(() => { ReplyAdapter.NotifyDataSetChanged(); });
                            }
                        }
                        else if (ReplyAdapter.ReplyList.Count > 10 && !ReplyRecyclerView.CanScrollVertically(1))
                        {
                            Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_NoMoreComment), ToastLength.Short).Show();
                        }
                    }

                    Activity.RunOnUiThread(ShowEmptyPage);
                }
                else
                {
                    Activity.RunOnUiThread(() =>
                    {
                        try
                        {
                            ReplyRecyclerView.Visibility = ViewStates.Gone;

                            Inflated = EmptyStateLayout.Inflate();
                            EmptyStateInflater x = new EmptyStateInflater();
                            x.InflateLayout(Inflated, EmptyStateInflater.Type.Login);
                            if (!x.EmptyStateButton.HasOnClickListeners)
                            {
                                x.EmptyStateButton.Click += null;
                                x.EmptyStateButton.Click += LoginButtonOnClick;
                            }

                            EmptyStateLayout.Visibility = ViewStates.Visible;
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
                Inflated = EmptyStateLayout.Inflate();
                EmptyStateInflater x = new EmptyStateInflater();
                x.InflateLayout(Inflated, EmptyStateInflater.Type.NoConnection);
                if (!x.EmptyStateButton.HasOnClickListeners)
                {
                    x.EmptyStateButton.Click += null;
                    x.EmptyStateButton.Click += EmptyStateButtonOnClick;
                }

                Toast.MakeText(Context, Context.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
            }
            MainScrollEvent.IsLoading = false;
        }

        private void LoginButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                Activity.StartActivity(new Intent(Activity, typeof(LoginActivity)));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void ShowEmptyPage()
        {
            try
            {
                MainScrollEvent.IsLoading = false;

                if (ReplyAdapter.ReplyList.Count > 0)
                {
                    ReplyRecyclerView.Visibility = ViewStates.Visible;
                    EmptyStateLayout.Visibility = ViewStates.Gone;
                }
                else
                {
                    ReplyRecyclerView.Visibility = ViewStates.Gone;

                    if (Inflated == null)
                        Inflated = EmptyStateLayout.Inflate();

                    EmptyStateInflater x = new EmptyStateInflater();
                    x.InflateLayout(Inflated, EmptyStateInflater.Type.NoReplies);
                    if (!x.EmptyStateButton.HasOnClickListeners)
                    {
                        x.EmptyStateButton.Click += null;
                    }
                    EmptyStateLayout.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception e)
            {
                MainScrollEvent.IsLoading = false;

                Console.WriteLine(e);
            }
        }

        //No Internet Connection 
        private void EmptyStateButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                StartApiService();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Events

        private void SendButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(EmojiconEditTextView.Text))
                {
                    if (UserDetails.IsLogin)
                    {
                        if (Methods.CheckConnectivity())
                        {                   
                            //Comment Code 
                            string time = Methods.Time.TimeAgo(DateTime.Now, false);
                            EmojiconEditTextView.ClearFocus();

                            int unixTimestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                            string time2 = unixTimestamp.ToString();
                            string message = EmojiconEditTextView.Text;

                            ReplyObject comment = new ReplyObject
                            {
                                Text = message,
                                TextTime = time,
                                UserId = Convert.ToInt32(UserDetails.UserId),
                                Id = Convert.ToInt32(time2),
                                IsReplyOwner = true,
                                PostId = Convert.ToInt32(Comment.PostId),
                                ReplyUserData = new UserDataObject
                                {
                                    Avatar = UserDetails.Avatar,
                                    Username = UserDetails.Username,
                                    Name = UserDetails.FullName,
                                }
                            };

                            EmptyStateLayout.Visibility = ViewStates.Gone;
                            ReplyRecyclerView.Visibility = ViewStates.Visible;
                            ReplyAdapter.ReplyList.Add(comment);
                            ReplyAdapter.NotifyItemInserted(ReplyAdapter.ReplyList.Count - 1);
                            ReplyRecyclerView.ScrollToPosition(ReplyAdapter.ReplyList.Count - 1);
                            var x = Convert.ToInt32(Comment.RepliesCount);
                            RepliesCount.Text = Methods.FunString.FormatPriceValue(++x);
                             
                            if (Type == "video")
                            { 
                                var dataComments = ActivityContext?.CommentsFragment?.MAdapter?.CommentList?.FirstOrDefault(a => a.Id == Comment.Id);
                                if (dataComments != null)
                                {
                                    dataComments.RepliesCount++;

                                    if (dataComments.CommentReplies?.Count > 0)
                                    {
                                        dataComments.CommentReplies.Add(comment);
                                    }
                                    else
                                    {
                                        dataComments.CommentReplies = new List<ReplyObject> {comment};
                                    }

                                    int index = ActivityContext.CommentsFragment.MAdapter.CommentList.IndexOf(dataComments);
                                    ActivityContext?.CommentsFragment?.MAdapter.NotifyItemChanged(index);
                                }

                                //Api request  
                                Task.Run(async () =>
                                {
                                    var (respondCode, respond) = await RequestsAsync.Comments.Reply_Video_comments_Http(Comment.VideoId.ToString(), Comment.Id.ToString(), message);
                                    if (respondCode.Equals(200))
                                    {
                                        if (respond is AddReplyObject result)
                                        {
                                            var dataComment = ReplyAdapter.ReplyList.FirstOrDefault(a => a.Id == int.Parse(time2));
                                            if (dataComment != null)
                                                dataComment.Id = result.ReplyId;
                                        }
                                    }
                                    else Methods.DisplayReportResult(Activity, respond);
                                });
                            }
                            else if (Type == "Article")
                            {
                                var dataComments = ShowArticleActivity.MAdapter?.CommentList?.FirstOrDefault(a => a.Id == Comment.Id);
                                if (dataComments != null)
                                {
                                    dataComments.RepliesCount++;

                                    if (dataComments.CommentReplies?.Count > 0)
                                    {
                                        dataComments.CommentReplies.Add(comment);
                                    }
                                    else
                                    {
                                        dataComments.CommentReplies = new List<ReplyObject> {comment};
                                    }

                                    int index = ShowArticleActivity.MAdapter.CommentList.IndexOf(dataComments);
                                    ShowArticleActivity.MAdapter.NotifyItemChanged(index);
                                }
                                
                                //Api request  
                                Task.Run(async () =>
                                {
                                    var (respondCode, respond) = await RequestsAsync.Articles.reply_Articles_comments_Http(Comment.PostId.ToString(), Comment.Id.ToString(), message);
                                    if (respondCode.Equals(200))
                                    {
                                        if (respond is AddReplyObject result)
                                        {
                                            var dataComment = ReplyAdapter.ReplyList.FirstOrDefault(a => a.Id == int.Parse(time2));
                                            if (dataComment != null)
                                                dataComment.Id = result.ReplyId;
                                        }
                                    }
                                    else Methods.DisplayReportResult(Activity, respond);
                                });
                            }

                            //Hide keyboard
                            EmojiconEditTextView.Text = "";
                            EmojiconEditTextView.ClearFocus(); 
                        }
                        else
                        {
                            Toast.MakeText(Activity, GetText(Resource.String.Lbl_CheckYourInternetConnection),ToastLength.Short).Show();
                        }
                    }
                    else
                    {
                        PopupDialogController dialog = new PopupDialogController(Activity, null, "Login");
                        dialog.ShowNormalDialog(Activity.GetText(Resource.String.Lbl_Warning),
                            Activity.GetText(Resource.String.Lbl_Please_sign_in_comment),
                            Activity.GetText(Resource.String.Lbl_Yes),
                            Activity.GetText(Resource.String.Lbl_No));
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void OnUnLikeButtonClick(object sender, EventArgs e)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    if (UserDetails.IsLogin)
                    {
                        dynamic dataComments = Type == "video" ? ActivityContext?.CommentsFragment?.MAdapter?.CommentList?.FirstOrDefault(a => a.Id == Comment.Id) : ShowArticleActivity.MAdapter?.CommentList?.FirstOrDefault(a => a.Id == Comment.Id);

                        if (dataComments != null)
                        {
                            if (UnLikeButton.Tag.ToString() == "1")
                            {
                                UnLikeiconView.SetColorFilter(Color.ParseColor("#777777"));

                                UnLikeButton.Tag = "0";
                                dataComments.IsDislikedComment = 0;

                                if (!UnLikeNumber.Text.Contains("K") && !UnLikeNumber.Text.Contains("M"))
                                {
                                    double x = Convert.ToDouble(UnLikeNumber.Text);
                                    if (x > 0)
                                        x--;
                                    else
                                        x = 0;
                                    UnLikeNumber.Text = x.ToString(CultureInfo.InvariantCulture);
                                    dataComments.DisLikes = Convert.ToInt32(x);
                                }
                            }
                            else
                            {
                                UnLikeiconView.SetColorFilter(Color.ParseColor(AppSettings.MainColor));

                                UnLikeButton.Tag = "1";
                                dataComments.IsDislikedComment = 1;

                                if (!UnLikeNumber.Text.Contains("K") && !UnLikeNumber.Text.Contains("M"))
                                {
                                    double x = Convert.ToDouble(UnLikeNumber.Text);
                                    x++;
                                    UnLikeNumber.Text = x.ToString(CultureInfo.InvariantCulture);
                                    dataComments.DisLikes = Convert.ToInt32(x);
                                }
                            }

                            if (LikeButton.Tag.ToString() == "1")
                            {
                                LikeiconView.SetColorFilter(Color.ParseColor("#777777"));

                                LikeButton.Tag = "0";
                                dataComments.IsLikedComment = 0;

                                if (!LikeNumber.Text.Contains("K") && !LikeNumber.Text.Contains("M"))
                                {
                                    double x = Convert.ToDouble(LikeNumber.Text);
                                    if (x > 0)
                                        x--;
                                    else
                                        x = 0;

                                    LikeNumber.Text = x.ToString(CultureInfo.InvariantCulture);
                                    dataComments.Likes = Convert.ToInt32(x);
                                }
                            }
                            PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Comments.Add_likeOrDislike_Comment_Http(dataComments.Id.ToString(), false) });

                        }
                    }
                    else
                    {
                        PopupDialogController dialog = new PopupDialogController(ActivityContext, null, "Login");
                        dialog.ShowNormalDialog(ActivityContext.GetText(Resource.String.Lbl_Warning),
                            ActivityContext.GetText(Resource.String.Lbl_Please_sign_in_Dislike),
                            ActivityContext.GetText(Resource.String.Lbl_Yes),
                            ActivityContext.GetText(Resource.String.Lbl_No));
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

        private void OnLikeButtonClick(object sender, EventArgs e)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    if (UserDetails.IsLogin)
                    {
                        dynamic dataComments = Type == "video" ? ActivityContext?.CommentsFragment?.MAdapter?.CommentList?.FirstOrDefault(a => a.Id == Comment.Id) : ShowArticleActivity.MAdapter?.CommentList?.FirstOrDefault(a => a.Id == Comment.Id);

                        if (dataComments != null)
                        {
                            if (LikeButton.Tag.ToString() == "1")
                            {
                                LikeiconView.SetColorFilter(Color.ParseColor("#777777"));

                                LikeButton.Tag = "0";
                                dataComments.IsLikedComment = 0;

                                if (!LikeNumber.Text.Contains("K") && !LikeNumber.Text.Contains("M"))
                                {
                                    double x = Convert.ToDouble(LikeNumber.Text);
                                    if (x > 0)
                                        x--;
                                    else
                                        x = 0;
                                    LikeNumber.Text = x.ToString(CultureInfo.InvariantCulture);
                                    dataComments.Likes = Convert.ToInt32(x);
                                }
                            }
                            else
                            {
                                LikeiconView.SetColorFilter(Color.ParseColor(AppSettings.MainColor));
                                LikeButton.Tag = "1";
                                dataComments.IsLikedComment = 1;

                                if (!LikeNumber.Text.Contains("K") && !LikeNumber.Text.Contains("M"))
                                {
                                    double x = Convert.ToDouble(LikeNumber.Text);
                                    x++;
                                    LikeNumber.Text = x.ToString(CultureInfo.InvariantCulture);
                                    dataComments.Likes = Convert.ToInt32(x);
                                }
                            }

                            if (UnLikeButton.Tag.ToString() == "1")
                            {
                                UnLikeiconView.SetColorFilter(Color.ParseColor("#777777"));

                                UnLikeButton.Tag = "0";
                                dataComments.IsDislikedComment = 0;

                                if (!UnLikeNumber.Text.Contains("K") && !UnLikeNumber.Text.Contains("M"))
                                {
                                    double x = Convert.ToDouble(UnLikeNumber.Text);
                                    if (x > 0)
                                        x--;
                                    else
                                        x = 0;
                                    UnLikeNumber.Text = x.ToString(CultureInfo.InvariantCulture);
                                    dataComments.DisLikes = Convert.ToInt32(x);
                                }
                            }
                            PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Comments.Add_likeOrDislike_Comment_Http(dataComments.Id.ToString(), true) });

                        }
                    }
                    else
                    {
                        PopupDialogController dialog = new PopupDialogController(ActivityContext, null, "Login");
                        dialog.ShowNormalDialog(ActivityContext.GetText(Resource.String.Lbl_Warning), ActivityContext.GetText(Resource.String.Lbl_Please_sign_in_Like), ActivityContext.GetText(Resource.String.Lbl_Yes),
                            ActivityContext.GetText(Resource.String.Lbl_No));
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

        private void ReplyAdapterOnAvatarClick(object sender, AvatarReplyAdapterClickEventArgs e)
        {
            try
            {
                ActivityContext.ShowUserChannelFragment(e.Class.ReplyUserData, e.Class.UserId.ToString()); 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        private void ReplyAdapterOnReplyClick(object sender, ReplyAdapterClickEventArgs e)
        {
            try
            {
                EmojiconEditTextView.Text = "";
                EmojiconEditTextView.Text = "@" + e.Class.ReplyUserData.Username + " ";
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        #endregion

        #region BottomSheetCallBack

        public class MyBottomSheetCallBack : BottomSheetCallback
        {
            public override void OnSlide(View bottomSheet, float slideOffset)
            {
                try
                {
                    //Sliding
                    if (slideOffset == StateHidden) Dispose();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            public override void OnStateChanged(View bottomSheet, int newState)
            {
                //State changed
            }
        }

        #endregion

        #region Scroll

        private void OnScroll_OnLoadMoreEvent(object sender, EventArgs eventArgs)
        {
            try
            {
                //Code get last id where LoadMore >>
                var item = ReplyAdapter.ReplyList.LastOrDefault();
                if (item != null && !string.IsNullOrEmpty(item.Id.ToString()) && !MainScrollEvent.IsLoading)
                    StartApiService(item.Id.ToString());
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion
    }
}