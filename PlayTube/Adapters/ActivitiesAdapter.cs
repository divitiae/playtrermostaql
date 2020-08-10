using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Android.App;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Java.Util;
using PlayTube.Activities.Tabbes;
using PlayTube.Helpers.CacheLoaders;
using PlayTube.Helpers.Fonts;
using PlayTube.Helpers.Utils;
using PlayTubeClient.Classes.Activities;
using IList = System.Collections.IList;

namespace PlayTube.Adapters
{
    public class ActivitiesAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {
        public event EventHandler<ActivitiesAdapterClickEventArgs> ItemClick;
        public event EventHandler<ActivitiesAdapterClickEventArgs> LikeItemClick;
        public event EventHandler<ActivitiesAdapterClickEventArgs> DisLikeItemClick;
        public event EventHandler<ActivitiesAdapterClickEventArgs> ItemLongClick;
        private readonly Activity ActivityContext;
        public ObservableCollection<ActivitiesObject> ActivitiesList = new ObservableCollection<ActivitiesObject>();

        public ActivitiesAdapter(Activity context)
        {
            HasStableIds = true;
            ActivityContext = context;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Video_Row_View
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Style_Activities_View, parent, false);
                var vh = new ActivitiesAdapterViewHolder(itemView, OnClick, OnLongClick, LikeOnClick, DisLikeOnClick);
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
                if (viewHolder is ActivitiesAdapterViewHolder holder)
                {
                    var item = ActivitiesList[position];
                    if (item != null)
                    {
                        GlideImageLoader.LoadImage(ActivityContext, item.Image, holder.ImageAct, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);

                        if (TabbedMainActivity.GetInstance()?.UserChannelFragment == null)
                        {
                            var dataChannel = ListUtils.MyChannelList.FirstOrDefault();
                            if (dataChannel != null)
                            {
                                item.UserData = dataChannel;

                                GlideImageLoader.LoadImage(ActivityContext, dataChannel.Avatar, holder.ImageUser, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);
                                holder.NameUser.Text = AppTools.GetNameFinal(dataChannel);
                            }
                        }
                        else
                        {
                            var dataChannel = TabbedMainActivity.GetInstance()?.UserChannelFragment?.UserData;
                            if (dataChannel != null)
                            {
                                item.UserData = dataChannel;

                                GlideImageLoader.LoadImage(ActivityContext, dataChannel.Avatar, holder.ImageUser, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);
                                holder.NameUser.Text = AppTools.GetNameFinal(dataChannel);
                            } 
                        }
                         
                        holder.TimeUser.Text = Methods.Time.TimeAgo(Convert.ToInt32(item.Time), false);

                        holder.ContentAct.Text = Methods.FunString.DecodeString(item.Text);

                        switch (item.IsLiked)
                        {
                            case "0":
                                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, holder.IconLikeAct, FontAwesomeIcon.ThumbsUp);
                                holder.IconLikeAct.SetTextColor(Color.ParseColor("#444444"));
                                holder.LikeActLayout.Tag = "Like";
                                break;
                            case "1":
                                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeSolid, holder.IconLikeAct, FontAwesomeIcon.ThumbsUp);
                                holder.IconLikeAct.SetTextColor(Color.ParseColor("#4caf50"));
                                holder.LikeActLayout.Tag = "Liked";
                                break;
                            default:
                                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, holder.IconLikeAct, FontAwesomeIcon.ThumbsUp);
                                holder.IconLikeAct.SetTextColor(Color.ParseColor("#444444"));
                                holder.LikeActLayout.Tag = "Like";
                                break;
                        }

                        holder.CountLikeAct.Text = Methods.FunString.FormatPriceValue(Convert.ToInt32(item.Likes));

                        switch (item.IsDisliked)
                        {
                            case "0":
                                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, holder.IconDislikeAct, FontAwesomeIcon.ThumbsDown);
                                holder.IconDislikeAct.SetTextColor(Color.ParseColor("#444444"));
                                holder.DislikeActLayout.Tag = "Dislike";
                                break;
                            case "1":
                                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeSolid, holder.IconDislikeAct, FontAwesomeIcon.ThumbsDown);
                                holder.IconDislikeAct.SetTextColor(Color.ParseColor("#4caf50"));
                                holder.DislikeActLayout.Tag = "Disliked";
                                break;
                            default:
                                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, holder.IconDislikeAct, FontAwesomeIcon.ThumbsDown);
                                holder.IconDislikeAct.SetTextColor(Color.ParseColor("#444444"));
                                holder.DislikeActLayout.Tag = "Dislike";
                                break;
                        }

                        holder.CountDislikeAct.Text = Methods.FunString.FormatPriceValue(Convert.ToInt32(item.Dislikes));

                        FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, holder.IconCommentAct, FontAwesomeIcon.CommentDots); 
                        holder.CountCommentAct.Text = Methods.FunString.FormatPriceValue(Convert.ToInt32(item.CommentsCount)); 
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public override int ItemCount => ActivitiesList?.Count ?? 0;

        public ActivitiesObject GetItem(int position)
        {
            return ActivitiesList[position];
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
        void LikeOnClick(ActivitiesAdapterClickEventArgs args) => LikeItemClick?.Invoke(this, args);
        void DisLikeOnClick(ActivitiesAdapterClickEventArgs args) => DisLikeItemClick?.Invoke(this, args);
        void OnClick(ActivitiesAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(ActivitiesAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

        public IList GetPreloadItems(int p0)
        {
            try
            {
                var d = new List<string>();
                var item = ActivitiesList[p0];

                if (item == null)
                    return Collections.SingletonList(p0);

                if (item.Image != "")
                {
                    d.Add(item.Image);
                    return d;
                }

                return d;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Collections.SingletonList(p0);
            }
        }

        public RequestBuilder GetPreloadRequestBuilder(Java.Lang.Object p0)
        {
            return Glide.With(ActivityContext).Load(p0.ToString())
                .Apply(new RequestOptions().CenterCrop());
        }
    }

    public class ActivitiesAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; }

        public ImageView ImageUser { get; private set; }
        public ImageView ImageAct { get; private set; }
        public TextView NameUser { get; private set; }
        public TextView TimeUser { get; private set; }
        public TextView ContentAct { get; private set; }
        public LinearLayout LikeActLayout { get; private set; }
        public TextView IconLikeAct { get; private set; }
        public TextView CountLikeAct { get; private set; }
        public LinearLayout DislikeActLayout { get; private set; }
        public TextView IconDislikeAct { get; private set; }
        public TextView CountDislikeAct { get; private set; }
        public LinearLayout CommentActLayout { get; private set; }
        public TextView IconCommentAct { get; private set; }
        public TextView CountCommentAct { get; private set; }

        #endregion

        public ActivitiesAdapterViewHolder(View itemView, Action<ActivitiesAdapterClickEventArgs> clickListener, Action<ActivitiesAdapterClickEventArgs> longClickListener,
            Action<ActivitiesAdapterClickEventArgs> likeClickListener, Action<ActivitiesAdapterClickEventArgs> dislikeClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                ImageUser = (ImageView)MainView.FindViewById(Resource.Id.ImageUser);
                ImageAct = (ImageView)MainView.FindViewById(Resource.Id.ImageAct);
                NameUser = MainView.FindViewById<TextView>(Resource.Id.NameUser);
                TimeUser = MainView.FindViewById<TextView>(Resource.Id.TimeUser);
                ContentAct = MainView.FindViewById<TextView>(Resource.Id.ContentAct);
                LikeActLayout = MainView.FindViewById<LinearLayout>(Resource.Id.LikeActLayout);
                IconLikeAct = MainView.FindViewById<TextView>(Resource.Id.IconLikeAct);
                CountLikeAct = MainView.FindViewById<TextView>(Resource.Id.CountLikeAct);
                DislikeActLayout = MainView.FindViewById<LinearLayout>(Resource.Id.DislikeActLayout);
                IconDislikeAct = MainView.FindViewById<TextView>(Resource.Id.IconDislikeAct);
                CountDislikeAct = MainView.FindViewById<TextView>(Resource.Id.CountDislikeAct);
                CommentActLayout = MainView.FindViewById<LinearLayout>(Resource.Id.CommentActLayout);
                IconCommentAct = MainView.FindViewById<TextView>(Resource.Id.IconCommentAct);
                CountCommentAct = MainView.FindViewById<TextView>(Resource.Id.CountCommentAct);

                //Create an Event
                itemView.Click += (sender, e) => clickListener(new ActivitiesAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                //itemView.LongClick += (sender, e) => longClickListener(new ActivitiesAdapterClickEventArgs { View = itemView, Position = AdapterPosition });

                LikeActLayout.Click += (sender, e) => likeClickListener(new ActivitiesAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                DislikeActLayout.Click += (sender, e) => dislikeClickListener(new ActivitiesAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }

    public class ActivitiesAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}