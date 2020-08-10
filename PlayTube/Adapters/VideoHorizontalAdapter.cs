using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Android.App;
using Android.Views;
using Android.Support.V7.Widget;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Java.Util;
using PlayTube.Helpers.CacheLoaders;
using PlayTube.Helpers.Fonts;
using PlayTube.Helpers.Utils;
using PlayTubeClient.Classes.Global;
using IList = System.Collections.IList;

namespace PlayTube.Adapters
{
    public class VideoHorizontalAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {
        public event EventHandler<VideoAdapterClickEventArgs> ItemClick;
        public event EventHandler<VideoAdapterClickEventArgs> ItemLongClick;
        private readonly VideoAdapterClickEventArgs.VideoType VideoStyle;
        private readonly Activity ActivityContext;
        public ObservableCollection<VideoObject> VideoList = new ObservableCollection<VideoObject>();

        public VideoHorizontalAdapter(Activity context, VideoAdapterClickEventArgs.VideoType videoStyle)
        {
            HasStableIds = true;
            ActivityContext = context;
            VideoStyle = videoStyle;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Video_Horizontal_View
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Video_Horizontal_View, parent, false);

                var vh = new VideoHorizontalAdapterViewHolder(itemView, VideoStyle, OnClick, OnLongClick);

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
                if (viewHolder is VideoHorizontalAdapterViewHolder holder)
                {
                    var item = VideoList[position];
                    if (item != null)
                    { 
                        GlideImageLoader.LoadImage(ActivityContext, item.Thumbnail, holder.VideoImage, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);

                        holder.TxtDuration.Text = Methods.FunString.SplitStringDuration(item.Duration);
                        holder.TxtTitle.Text = Methods.FunString.DecodeString(item.Title);

                        holder.TxtChannelName.Text = AppTools.GetNameFinal(item.Owner);
                         
                        holder.TxtViewsCount.Text = Methods.FunString.FormatPriceValue(Convert.ToInt32(item.Views)) + " " + ActivityContext.GetText(Resource.String.Lbl_Views);

                        //Verified 
                        if (item.Owner.Verified == "1")
                        {
                            holder.IconVerified.Visibility = ViewStates.Visible;
                            FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, holder.IconVerified, IonIconsFonts.CheckmarkCircled);
                        }
                        else
                        {
                            holder.IconVerified.Visibility = ViewStates.Gone;
                        }

                        //Set Badge on videos
                        AppTools.ShowGlobalBadgeSystem(holder.VideoType, item);

                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        public override int ItemCount => VideoList?.Count ?? 0;

        public VideoObject GetItem(int position)
        {
            return VideoList[position];
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

        void OnClick(VideoAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(VideoAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

        public IList GetPreloadItems(int p0)
        {
            try
            {
                var d = new List<string>();
                var item = VideoList[p0];

                if (item == null)
                    return Collections.SingletonList(p0);

                if (item.Thumbnail != "")
                {
                    d.Add(item.Thumbnail);
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

    public class VideoHorizontalAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        private View MainView { get; set; }

        public ImageView VideoImage { get; private set; }
        public TextView TxtDuration { get; private set; }
        public TextView TxtTitle { get; private set; }
        public TextView TxtChannelName { get; private set; }
        public TextView TxtViewsCount { get; private set; }
        public TextView IconVerified { get; private set; }
        public TextView VideoType { get; private set; }

        #endregion
        
        public VideoHorizontalAdapterViewHolder(View itemView, VideoAdapterClickEventArgs.VideoType videoStyle, Action<VideoAdapterClickEventArgs> clickListener, Action<VideoAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                //Get values
                VideoImage = (ImageView)MainView.FindViewById(Resource.Id.Imagevideo);
                TxtDuration = MainView.FindViewById<TextView>(Resource.Id.duration);
                TxtTitle = MainView.FindViewById<TextView>(Resource.Id.Title);
                TxtChannelName = MainView.FindViewById<TextView>(Resource.Id.ChannelName);
                TxtViewsCount = MainView.FindViewById<TextView>(Resource.Id.Views_Count);
                IconVerified = MainView.FindViewById<TextView>(Resource.Id.IconVerified);

                VideoType = MainView.FindViewById<TextView>(Resource.Id.videoType);
                
                //Create an Event
                itemView.Click += (sender, e) => clickListener(new VideoAdapterClickEventArgs { View = itemView, Position = AdapterPosition,VideoStyle = videoStyle });
                itemView.LongClick += (sender, e) => longClickListener(new VideoAdapterClickEventArgs { View = itemView, Position = AdapterPosition });

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }

    public class VideoAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
        public VideoType VideoStyle { get; set; }

        public enum VideoType
        {
            TopVideo, BigVideo, LatestVideo , FavVideo
        }
    }
}