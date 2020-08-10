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
    public class CommentsAdapter : RecyclerView.Adapter
    {
        public event EventHandler<CommentAdapterClickEventArgs> ReplyClick;
        public event EventHandler<AvatarCommentAdapterClickEventArgs> AvatarClick;
        public event EventHandler<CommentAdapterClickEventArgs> ItemClick;
        public event EventHandler<CommentAdapterClickEventArgs> ItemLongClick;

        private readonly Activity ActivityContext;
        public ObservableCollection<CommentObject> CommentList = new ObservableCollection<CommentObject>();

        public CommentsAdapter(Activity context)
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


                var vh = new CommentAdapterViewHolder(itemView, OnClick, OnLongClick);
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
                if (viewHolder is CommentAdapterViewHolder holder)
                {
                    var item = CommentList[position];
                    if (item != null)
                    {
                        GlideImageLoader.LoadImage(ActivityContext,item.CommentUserData.Avatar, holder.Image, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);

                        TextSanitizer chnager = new TextSanitizer(holder.CommentText, ActivityContext);
                        chnager.Load(Methods.FunString.DecodeString(item.Text));
                        holder.TimeTextView.Text = item.TextTime;

                        holder.UsernameTextView.Text = AppTools.GetNameFinal(item.CommentUserData);

                        holder.LikeNumber.Text = Methods.FunString.FormatPriceValue(Convert.ToInt32(item.Likes));
                        holder.UnLikeNumber.Text = Methods.FunString.FormatPriceValue(Convert.ToInt32(item.Likes));
                        holder.RepliesCount.Text = item.RepliesCount.ToString();

                        if (item.IsLikedComment == 1)
                        {
                            holder.LikeiconView.SetColorFilter(Color.ParseColor(AppSettings.MainColor));
                            holder.LikeButton.Tag = "1";
                        }
                        else
                        {
                            holder.LikeButton.Tag = "0";
                        }

                        if (item.IsDislikedComment == 1)
                        {
                            holder.UnLikeiconView.SetColorFilter(Color.ParseColor(AppSettings.MainColor));
                            holder.UnLikeButton.Tag = "1";
                        }
                        else
                        {
                            holder.UnLikeButton.Tag = "0";
                        }

                        if (!holder.Image.HasOnClickListeners)
                            holder.Image.Click += (sender, e) => OnAvatarClick(new AvatarCommentAdapterClickEventArgs { Class = item, Position = position, View = holder.MainView });

                        if (!holder.ReplyButton.HasOnClickListeners)
                            holder.ReplyButton.Click += (sender, e) => OnReplyClick(new CommentAdapterClickEventArgs { Class = item, Position = position, View = holder.MainView });

                        if (!holder.LikeButton.HasOnClickListeners)
                            holder.LikeButton.Click += (sender, e) => OnLikeButtonClick(holder, new CommentAdapterClickEventArgs { Class = item, Position = position, View = holder.MainView });

                        if (!holder.UnLikeButton.HasOnClickListeners)
                            holder.UnLikeButton.Click += (sender, e) => OnUnLikeButtonClick(holder, new CommentAdapterClickEventArgs { Class = item, Position = position, View = holder.MainView });
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        private void OnLikeButtonClick(CommentAdapterViewHolder holder , CommentAdapterClickEventArgs e)
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
                                e.Class.IsLikedComment = 0;

                                if (!holder.LikeNumber.Text.Contains("K") && !holder.LikeNumber.Text.Contains("M"))
                                {
                                    double x = Convert.ToDouble(holder.LikeNumber.Text);
                                    if (x > 0)
                                        x--;
                                    else
                                        x = 0;
                                    holder.LikeNumber.Text = x.ToString(CultureInfo.InvariantCulture);
                                    e.Class.Likes = Convert.ToInt32(x);
                                }
                            }
                            else
                            {
                                holder.LikeiconView.SetColorFilter(Color.ParseColor(AppSettings.MainColor)); 
                                holder.LikeButton.Tag = "1";
                                e.Class.IsLikedComment = 1;
                               
                                if (!holder.LikeNumber.Text.Contains("K") && !holder.LikeNumber.Text.Contains("M"))
                                {
                                    double x = Convert.ToDouble(holder.LikeNumber.Text);
                                    x++;
                                    holder.LikeNumber.Text = x.ToString(CultureInfo.InvariantCulture);
                                    e.Class.Likes = Convert.ToInt32(x);
                                }
                            }

                            if (holder.UnLikeButton.Tag.ToString() == "1")
                            {
                                holder.UnLikeiconView.SetColorFilter(Color.ParseColor("#777777"));
                              
                                holder.UnLikeButton.Tag = "0";
                                e.Class.IsDislikedComment = 0;

                                if (!holder.UnLikeNumber.Text.Contains("K") && !holder.UnLikeNumber.Text.Contains("M"))
                                {
                                    double x = Convert.ToDouble(holder.UnLikeNumber.Text);
                                    if (x > 0)
                                        x--;
                                    else
                                        x = 0;
                                    holder.UnLikeNumber.Text = x.ToString(CultureInfo.InvariantCulture);
                                    e.Class.DisLikes = Convert.ToInt32(x);
                                }
                            }
                            PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Comments.Add_likeOrDislike_Comment_Http(e.Class.Id.ToString(), true) });
                             
                        }
                    }
                    else
                    {
                        PopupDialogController dialog = new PopupDialogController(ActivityContext, null, "Login");
                        dialog.ShowNormalDialog(ActivityContext.GetText(Resource.String.Lbl_Warning),ActivityContext.GetText(Resource.String.Lbl_Please_sign_in_Like),ActivityContext.GetText(Resource.String.Lbl_Yes),
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

        private void OnUnLikeButtonClick(CommentAdapterViewHolder holder , CommentAdapterClickEventArgs e)
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
                                e.Class.IsDislikedComment = 0;

                                if (!holder.UnLikeNumber.Text.Contains("K") && !holder.UnLikeNumber.Text.Contains("M"))
                                {
                                    double x = Convert.ToDouble(holder.UnLikeNumber.Text);
                                    if (x > 0)
                                        x--;
                                    else
                                        x = 0;
                                    holder.UnLikeNumber.Text = x.ToString(CultureInfo.InvariantCulture);
                                    e.Class.DisLikes = Convert.ToInt32(x);
                                }
                            }
                            else
                            {
                                holder.UnLikeiconView.SetColorFilter(Color.ParseColor(AppSettings.MainColor));
                               
                                holder.UnLikeButton.Tag = "1";
                                e.Class.IsDislikedComment = 1;

                                if (!holder.UnLikeNumber.Text.Contains("K") && !holder.UnLikeNumber.Text.Contains("M"))
                                {
                                    double x = Convert.ToDouble(holder.UnLikeNumber.Text);
                                    x++;
                                    holder.UnLikeNumber.Text = x.ToString(CultureInfo.InvariantCulture);
                                    e.Class.DisLikes = Convert.ToInt32(x);
                                }
                            }

                            if (holder.LikeButton.Tag.ToString() == "1")
                            {
                                holder.LikeiconView.SetColorFilter(Color.ParseColor("#777777"));
                               
                                holder.LikeButton.Tag = "0";
                                e.Class.IsLikedComment = 0;

                                if (!holder.LikeNumber.Text.Contains("K") && !holder.LikeNumber.Text.Contains("M"))
                                {
                                    double x = Convert.ToDouble(holder.LikeNumber.Text);
                                    if (x > 0)
                                        x--;
                                    else
                                        x = 0;

                                    holder.LikeNumber.Text = x.ToString(CultureInfo.InvariantCulture);
                                    e.Class.Likes = Convert.ToInt32(x);
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
         
         public override int ItemCount => CommentList?.Count ?? 0;

        public CommentObject GetItem(int position)
        {
            return CommentList[position];
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
        void OnReplyClick(CommentAdapterClickEventArgs args) => ReplyClick?.Invoke(this, args);
        void OnAvatarClick(AvatarCommentAdapterClickEventArgs args) => AvatarClick?.Invoke(this, args);
        void OnClick(CommentAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(CommentAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class CommentAdapterViewHolder : RecyclerView.ViewHolder
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

        public CommentAdapterViewHolder(View itemView, Action<CommentAdapterClickEventArgs> clickListener, Action<CommentAdapterClickEventArgs> longClickListener) : base(itemView)
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
                itemView.Click += (sender, e) => clickListener(new CommentAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new CommentAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class CommentAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
        public CommentObject Class { get; set; }
    }

    public class AvatarCommentAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
        public CommentObject Class { get; set; }
    }


}