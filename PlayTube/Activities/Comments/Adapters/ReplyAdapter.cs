using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using Android.App;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Com.Luseen.Autolinklibrary;
using PlayTube.Helpers.CacheLoaders;
using PlayTube.Helpers.Controller;
using PlayTube.Helpers.Fonts;
using PlayTube.Helpers.Models;
using PlayTube.Helpers.Utils;
using PlayTubeClient.Classes.Global;
using PlayTubeClient.RestCalls;

namespace PlayTube.Activities.Comments.Adapters
{ 
    public class ReplyAdapter : RecyclerView.Adapter
    {
        public event EventHandler<ReplyAdapterClickEventArgs> ReplyClick;
        public event EventHandler<AvatarReplyAdapterClickEventArgs> AvatarClick;
        public event EventHandler<ReplyAdapterClickEventArgs> ItemClick;
        public event EventHandler<ReplyAdapterClickEventArgs> ItemLongClick;

        private readonly Activity ActivityContext;
        public ObservableCollection<ReplyObject> ReplyList = new ObservableCollection<ReplyObject>();

        public ReplyAdapter(Activity context)
        {
            try
            {
                ActivityContext = context;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_PageCircle_view
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Style_Comment, parent, false);
                var vh = new ReplyAdapterViewHolder(itemView, OnClick, OnLongClick);
                return vh;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            try
            {
                if (viewHolder is ReplyAdapterViewHolder holder)
                {
                    var item = ReplyList[position];
                    if (item != null)
                    {
                        GlideImageLoader.LoadImage(ActivityContext, item.ReplyUserData.Avatar, holder.Image, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);

                        TextSanitizer chnager = new TextSanitizer(holder.CommentText, ActivityContext);
                        chnager.Load(Methods.FunString.DecodeString(item.Text));
                        holder.TimeTextView.Text = item.TextTime;

                        holder.UsernameTextView.Text = AppTools.GetNameFinal(item.ReplyUserData);

                        holder.LikeNumber.Text = Methods.FunString.FormatPriceValue(Convert.ToInt32(item.ReplyLikes));
                        holder.UnLikeNumber.Text = Methods.FunString.FormatPriceValue(Convert.ToInt32(item.ReplyDislikes));
                        holder.RepliesCount.Visibility = ViewStates.Invisible;

                        if (item.IsLikedReply == 1)
                        {
                            holder.LikeiconView.SetColorFilter(Color.ParseColor(AppSettings.MainColor));
                            holder.LikeButton.Tag = "1";
                        }
                        else
                        {
                            holder.LikeButton.Tag = "0";
                        }

                        if (item.IsDislikedReply == 1)
                        {
                            holder.UnLikeiconView.SetColorFilter(Color.ParseColor(AppSettings.MainColor));
                            holder.UnLikeButton.Tag = "1";
                        }
                        else
                        {
                            holder.UnLikeButton.Tag = "0";
                        }

                        if (!holder.Image.HasOnClickListeners)
                            holder.Image.Click += (sender, e) => OnAvatarClick(new AvatarReplyAdapterClickEventArgs{Class = item, Position = position, View = holder.MainView});

                        if (!holder.LikeButton.HasOnClickListeners)
                            holder.LikeButton.Click += (sender, e) => OnLikeButtonClick(holder,new ReplyAdapterClickEventArgs{ Class = item, Position = position, View = holder.MainView});

                        if (!holder.UnLikeButton.HasOnClickListeners)
                            holder.UnLikeButton.Click += (sender, e) => OnUnLikeButtonClick(holder,new ReplyAdapterClickEventArgs{ Class = item, Position = position, View = holder.MainView});

                        if (!holder.ReplyButton.HasOnClickListeners)
                            holder.ReplyButton.Click += (sender, e) => OnReplyClick(new ReplyAdapterClickEventArgs { Class = item, Position = position, View = holder.MainView });

                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void OnLikeButtonClick(ReplyAdapterViewHolder holder, ReplyAdapterClickEventArgs e)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    if (UserDetails.IsLogin)
                    {
                        if (e.Class != null)
                        {
                            if (holder.LikeButton.Tag.ToString() == "1")
                            {
                                holder.LikeiconView.SetColorFilter(Color.ParseColor("#777777"));

                                holder.LikeButton.Tag = "0";
                                e.Class.IsLikedReply = 0;

                                if (!holder.LikeNumber.Text.Contains("K") && !holder.LikeNumber.Text.Contains("M"))
                                {
                                    double x = Convert.ToDouble(holder.LikeNumber.Text);
                                    if (x > 0)
                                        x--;
                                    else
                                        x = 0;
                                    holder.LikeNumber.Text = x.ToString(CultureInfo.InvariantCulture);
                                    e.Class.ReplyLikes = Convert.ToInt32(x);
                                }
                            }
                            else
                            {
                                holder.LikeiconView.SetColorFilter(Color.ParseColor(AppSettings.MainColor));
                                holder.LikeButton.Tag = "1";
                                e.Class.IsLikedReply = 1;

                                if (!holder.LikeNumber.Text.Contains("K") && !holder.LikeNumber.Text.Contains("M"))
                                {
                                    double x = Convert.ToDouble(holder.LikeNumber.Text);
                                    x++;
                                    holder.LikeNumber.Text = x.ToString(CultureInfo.InvariantCulture);
                                    e.Class.ReplyLikes = Convert.ToInt32(x);
                                }
                            }

                            if (holder.UnLikeButton.Tag.ToString() == "1")
                            {
                                holder.UnLikeiconView.SetColorFilter(Color.ParseColor("#777777"));

                                holder.UnLikeButton.Tag = "0";
                                e.Class.IsDislikedReply = 0;

                                if (!holder.UnLikeNumber.Text.Contains("K") && !holder.UnLikeNumber.Text.Contains("M"))
                                {
                                    double x = Convert.ToDouble(holder.UnLikeNumber.Text);
                                    if (x > 0)
                                        x--;
                                    else
                                        x = 0;
                                    holder.UnLikeNumber.Text = x.ToString(CultureInfo.InvariantCulture);
                                    e.Class.ReplyDislikes = Convert.ToInt32(x);
                                }
                            }
                            PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Comments.Add_likeOrDislike_Comment_Http(e.Class.Id.ToString(), true) });

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

        private void OnUnLikeButtonClick(ReplyAdapterViewHolder holder, ReplyAdapterClickEventArgs e)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    if (UserDetails.IsLogin)
                    {
                        if (e.Class != null)
                        {
                            if (holder.UnLikeButton.Tag.ToString() == "1")
                            {
                                holder.UnLikeiconView.SetColorFilter(Color.ParseColor("#777777"));

                                holder.UnLikeButton.Tag = "0";
                                e.Class.IsDislikedReply = 0;

                                if (!holder.UnLikeNumber.Text.Contains("K") && !holder.UnLikeNumber.Text.Contains("M"))
                                {
                                    double x = Convert.ToDouble(holder.UnLikeNumber.Text);
                                    if (x > 0)
                                        x--;
                                    else
                                        x = 0;
                                    holder.UnLikeNumber.Text = x.ToString(CultureInfo.InvariantCulture);
                                    e.Class.ReplyDislikes = Convert.ToInt32(x);
                                }
                            }
                            else
                            {
                                holder.UnLikeiconView.SetColorFilter(Color.ParseColor(AppSettings.MainColor));

                                holder.UnLikeButton.Tag = "1";
                                e.Class.IsDislikedReply = 1;

                                if (!holder.UnLikeNumber.Text.Contains("K") && !holder.UnLikeNumber.Text.Contains("M"))
                                {
                                    double x = Convert.ToDouble(holder.UnLikeNumber.Text);
                                    x++;
                                    holder.UnLikeNumber.Text = x.ToString(CultureInfo.InvariantCulture);
                                    e.Class.ReplyDislikes = Convert.ToInt32(x);
                                }
                            }

                            if (holder.LikeButton.Tag.ToString() == "1")
                            {
                                holder.LikeiconView.SetColorFilter(Color.ParseColor("#777777"));

                                holder.LikeButton.Tag = "0";
                                e.Class.IsLikedReply = 0;

                                if (!holder.LikeNumber.Text.Contains("K") && !holder.LikeNumber.Text.Contains("M"))
                                {
                                    double x = Convert.ToDouble(holder.LikeNumber.Text);
                                    if (x > 0)
                                        x--;
                                    else
                                        x = 0;

                                    holder.LikeNumber.Text = x.ToString(CultureInfo.InvariantCulture);
                                    e.Class.ReplyLikes = Convert.ToInt32(x);
                                }
                            }
                            PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Comments.Add_likeOrDislike_Comment_Http(e.Class.Id.ToString(), false) });

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

        public override int ItemCount => ReplyList?.Count ?? 0;

 
        public ReplyObject GetItem(int position)
        {
            return ReplyList[position];
        }

        public override long GetItemId(int position)
        {
            try
            {
                return position;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return 0;
            }
        }

        public override int GetItemViewType(int position)
        {
            try
            {
                return position;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return 0;
            }
        }

        void OnReplyClick(ReplyAdapterClickEventArgs args) => ReplyClick?.Invoke(this, args);
        void OnAvatarClick(AvatarReplyAdapterClickEventArgs args) => AvatarClick?.Invoke(this, args);
        void OnClick(ReplyAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(ReplyAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);
    }

    public class ReplyAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; }
        public ImageView Image { get; private set; }
        public AutoLinkTextView CommentText { get; private set; }
        public TextView TimeTextView { get; private set; }
        public TextView UsernameTextView { get; private set; }
        public ImageView LikeiconView { get; private set; }
        public ImageView UnLikeiconView { get; private set; }
        public TextView ReplyiconView { get; private set; }
        public TextView LikeNumber { get; private set; }
        public TextView UnLikeNumber { get; private set; }
        public TextView RepliesCount { get; private set; }
        public LinearLayout LikeButton { get; private set; }
        public LinearLayout UnLikeButton { get; private set; }
        public LinearLayout ReplyButton { get; private set; }

        #endregion

        public ReplyAdapterViewHolder(View itemView, Action<ReplyAdapterClickEventArgs> clickListener, Action<ReplyAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                Image = MainView.FindViewById<ImageView>(Resource.Id.card_pro_pic);
                CommentText = MainView.FindViewById<AutoLinkTextView>(Resource.Id.active);

                UsernameTextView = MainView.FindViewById<TextView>(Resource.Id.username);
                TimeTextView = MainView.FindViewById<TextView>(Resource.Id.time);

                LikeiconView = MainView.FindViewById<ImageView>(Resource.Id.Likeicon);
                UnLikeiconView = MainView.FindViewById<ImageView>(Resource.Id.UnLikeicon);
                ReplyiconView = MainView.FindViewById<TextView>(Resource.Id.ReplyIcon);

                LikeNumber = MainView.FindViewById<TextView>(Resource.Id.LikeNumber);
                UnLikeNumber = MainView.FindViewById<TextView>(Resource.Id.UnLikeNumber);
                RepliesCount = MainView.FindViewById<TextView>(Resource.Id.RepliesCount);

                LikeButton = MainView.FindViewById<LinearLayout>(Resource.Id.LikeButton);
                UnLikeButton = MainView.FindViewById<LinearLayout>(Resource.Id.UnLikeButton);
                ReplyButton = MainView.FindViewById<LinearLayout>(Resource.Id.ReplyButton);
                 
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ReplyiconView, IonIconsFonts.Reply);

                //Event
                itemView.Click += (sender, e) => clickListener(new ReplyAdapterClickEventArgs{View = itemView, Position = AdapterPosition});
                itemView.LongClick += (sender, e) => longClickListener(new ReplyAdapterClickEventArgs{View = itemView, Position = AdapterPosition});
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class ReplyAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
        public ReplyObject Class { get; set; }
    }

    public class AvatarReplyAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
        public ReplyObject Class { get; set; }
    }

}