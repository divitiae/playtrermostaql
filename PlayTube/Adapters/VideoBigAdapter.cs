using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Support.V7.Widget;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using PlayTube.Activities.Models;
using PlayTube.Helpers.CacheLoaders;
using PlayTube.Helpers.Fonts;
using PlayTube.Helpers.Utils;
using PlayTube.SQLite;
using PlayTubeClient.Classes.Global;
using PopupMenu = Android.Support.V7.Widget.PopupMenu;
using Java.Util;
using IList = System.Collections.IList;

namespace PlayTube.Adapters
{
    public class VideoBigAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {
        public event EventHandler<VideoAdapterClickEventArgs> ItemClick;
        public event EventHandler<VideoAdapterClickEventArgs> ItemLongClick;
        private readonly Activity ActivityContext;
        public ObservableCollection<VideoObject> VideoList = new ObservableCollection<VideoObject>();
        private readonly LibrarySynchronizer LibrarySynchronizer;
        public VideoBigAdapter(Activity context)
        {
            HasStableIds = true;
            ActivityContext = context;
            LibrarySynchronizer = new LibrarySynchronizer(context);
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Video_Big_View
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Video_Big_View, parent, false);

                var vh = new VideoBigAdapterViewHolder(itemView, OnClick, OnLongClick);

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
                if (viewHolder is VideoBigAdapterViewHolder holder)
                {
                    var item = VideoList[position];
                    if (item != null)
                    {
                        GlideImageLoader.LoadImage(ActivityContext, item.Thumbnail, holder.VideoImage, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                        GlideImageLoader.LoadImage(ActivityContext, item.Owner.Avatar, holder.ChannelImage, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);

                        holder.TxtDuration.Text = Methods.FunString.SplitStringDuration(item.Duration);
                        holder.TxtTitle.Text = Methods.FunString.DecodeString(item.Title);
                         
                        holder.TxtChannelName.Text = AppTools.GetNameFinal(item.Owner);

                        holder.TxtViewsCount.Text = Methods.FunString.FormatPriceValue(Convert.ToInt32(item.Views)) + " " + ActivityContext.GetText(Resource.String.Lbl_Views);

                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, holder.MenueView, IonIconsFonts.AndroidMoreVertical);                       

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


                        if (!holder.MenueView.HasOnClickListeners)
                            holder.MenueView.Click += (sender, args) =>
                            {
                                ContextThemeWrapper ctw =new ContextThemeWrapper(ActivityContext, Resource.Style.PopupMenuStyle);
                                PopupMenu popup = new PopupMenu(ctw, holder.MenueView);
                                popup.MenuInflater.Inflate(Resource.Menu.Video_More_Menue, popup.Menu);
                                popup.Show();
                                popup.MenuItemClick += (o, eventArgs) =>
                                {
                                    try
                                    {
                                        var id = eventArgs.Item.ItemId;
                                        switch (id)
                                        {
                                            case Resource.Id.menu_AddWatchLater:
                                                LibrarySynchronizer.AddToWatchLater(item);
                                                break;

                                            case Resource.Id.menu_AddPlaylist:
                                                LibrarySynchronizer.AddToPlaylist(item);
                                                break;

                                            case Resource.Id.menu_Remove:
                                                OnMenuRemove_Click(item);
                                                break;

                                            case Resource.Id.menu_Share:
                                                LibrarySynchronizer.ShareVideo(item);
                                                break;

                                            case Resource.Id.menu_Report:
                                                LibrarySynchronizer.AddReportVideo(item);
                                                break;
                                        }
                                    }
                                    catch (Exception exception)
                                    {
                                        Console.WriteLine(exception);
                                    }
                                };
                            };

                        //Set Badge on videos
                        AppTools.ShowGlobalBadgeSystem(holder.VideoType,item);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void OnMenuRemove_Click(VideoObject video)
        {
            try
            {
                var index = VideoList.IndexOf(VideoList.FirstOrDefault(a => a.VideoId == video.VideoId));
                if (index != -1)
                {
                    VideoList.Remove(video);
                    NotifyItemRemoved(index);

                    Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_Video_Removed), ToastLength.Short).Show();

                    var data = ListUtils.GlobalNotInterestedList.FirstOrDefault(a => a.VideoId == video.VideoId);
                    if (data == null)
                    {
                        ListUtils.GlobalNotInterestedList.Add(video);
                    }

                    var sqlEntity = new SqLiteDatabase();
                    sqlEntity.Insert_NotInterestedVideos(video);
                    sqlEntity.Dispose();
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

    public class VideoBigAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; set; }
        public ImageView VideoImage { get; private set; }
        public TextView TxtDuration { get; private set; }
        public ImageView ChannelImage { get; private set; }
        public TextView TxtTitle { get; private set; }
        public TextView TxtChannelName { get; private set; }
        public TextView TxtViewsCount { get; private set; }
        public TextView IconVerified { get; private set; }
        public TextView MenueView { get; private set; }
        public TextView VideoType { get; private set; }
        #endregion

        public VideoBigAdapterViewHolder(View itemView, Action<VideoAdapterClickEventArgs> clickListener,Action<VideoAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                //Get values
                VideoImage = MainView.FindViewById<ImageView>(Resource.Id.Imagevideo);
                TxtDuration = MainView.FindViewById<TextView>(Resource.Id.duration);
                ChannelImage = MainView.FindViewById<ImageView>(Resource.Id.Image_Channel);
                TxtTitle = MainView.FindViewById<TextView>(Resource.Id.Title);
                TxtChannelName = MainView.FindViewById<TextView>(Resource.Id.ChannelName);
                TxtViewsCount = MainView.FindViewById<TextView>(Resource.Id.Views_Count);
                IconVerified = MainView.FindViewById<TextView>(Resource.Id.IconVerified);

                MenueView = MainView.FindViewById<TextView>(Resource.Id.videoMenue);
                VideoType = MainView.FindViewById<TextView>(Resource.Id.videoType);
                //Create an Event
                itemView.Click += (sender, e) => clickListener(new VideoAdapterClickEventArgs { View = itemView, Position = AdapterPosition, VideoStyle = VideoAdapterClickEventArgs.VideoType.BigVideo });
                itemView.LongClick += (sender, e) => longClickListener(new VideoAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }

   
}