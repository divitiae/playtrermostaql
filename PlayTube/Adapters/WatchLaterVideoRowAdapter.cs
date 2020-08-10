using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using PlayTube.Activities.Models;
using PlayTube.Helpers.CacheLoaders;
using PlayTube.Helpers.Fonts;
using PlayTube.Helpers.Utils;
using PlayTube.SQLite;
using Exception = System.Exception;
using PopupMenu = Android.Support.V7.Widget.PopupMenu;
using Java.Util;
using PlayTubeClient.Classes.Video;
using IList = System.Collections.IList;

namespace PlayTube.Adapters
{
    public class WatchLaterVideoRowAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {
        public event EventHandler<WatchLaterVideoRowAdapterClickEventArgs> ItemClick;
        public event EventHandler<WatchLaterVideoRowAdapterClickEventArgs> ItemLongClick;
        private readonly Activity ActivityContext;
        public ObservableCollection<DataWatchLaterVideos> VideoList = new ObservableCollection<DataWatchLaterVideos>();
        private readonly LibrarySynchronizer LibrarySynchronizer;
        private string Type;

        public WatchLaterVideoRowAdapter(Activity context , string type = "Normal")
        {
            HasStableIds = true;
            ActivityContext = context;
            LibrarySynchronizer = new LibrarySynchronizer(context);
            Type = type;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Video_Row_View
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Video_Row_View, parent, false);
                var vh = new WatchLaterVideoRowAdapterViewHolder(itemView, OnClick, OnLongClick);

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
                if (viewHolder is WatchLaterVideoRowAdapterViewHolder holder)
                {
                    var item = VideoList[position];
                    if (item != null)
                    {
                        if (item.Videos != null)
                        {
                            GlideImageLoader.LoadImage(ActivityContext, item.Videos?.VideoAdClass.Thumbnail, holder.VideoImage, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);


                            holder.TxtDuration.Text = Methods.FunString.SplitStringDuration(item.Videos?.VideoAdClass.Duration);
                            holder.TxtTitle.Text = Methods.FunString.DecodeString(item.Videos?.VideoAdClass.Title);

                            holder.TxtChannelName.Text = AppTools.GetNameFinal(item.Videos?.VideoAdClass.Owner);

                            holder.TxtViewsCount.Text = Methods.FunString.FormatPriceValue(Convert.ToInt32(item.Videos?.VideoAdClass.Views)) + " " + ActivityContext.GetText(Resource.String.Lbl_Views);

                            FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, holder.MenueView, IonIconsFonts.AndroidMoreVertical);
                             
                            //Verified 
                            if (item.Videos?.VideoAdClass.Owner.Verified == "1")
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
                                    ContextThemeWrapper ctw = new ContextThemeWrapper(ActivityContext, Resource.Style.PopupMenuStyle);
                                    PopupMenu popup = new PopupMenu(ctw, holder.MenueView);
                                    if (Type == "MyChannel")
                                    {
                                        popup.MenuInflater.Inflate(Resource.Menu.MyVideoMore_Menu, popup.Menu);
                                    }
                                    else
                                    {
                                        popup.MenuInflater.Inflate(Resource.Menu.Video_More_Menue, popup.Menu);
                                    }

                                    popup.Show();
                                    popup.MenuItemClick += (o, eventArgs) =>
                                    {
                                        try
                                        {
                                            var id = eventArgs.Item.ItemId;
                                            switch (id)
                                            {
                                                case Resource.Id.menu_AddWatchLater:
                                                    LibrarySynchronizer.AddToWatchLater(item.Videos?.VideoAdClass);
                                                    break;

                                                case Resource.Id.menu_AddPlaylist:
                                                    LibrarySynchronizer.AddToPlaylist(item.Videos?.VideoAdClass);
                                                    break;

                                                case Resource.Id.menu_Remove:
                                                    OnMenuRemove_Click(item);
                                                    break;

                                                case Resource.Id.menu_Share:
                                                    LibrarySynchronizer.ShareVideo(item.Videos?.VideoAdClass);
                                                    break;

                                                case Resource.Id.menu_Report:
                                                    LibrarySynchronizer.AddReportVideo(item.Videos?.VideoAdClass);
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
                            AppTools.ShowGlobalBadgeSystem(holder.VideoType, item.Videos?.VideoAdClass);

                        } 
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        private void OnMenuRemove_Click(DataWatchLaterVideos video)
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
                        ListUtils.GlobalNotInterestedList.Add(video.Videos?.VideoAdClass);
                    }

                    var sqlEntity = new SqLiteDatabase();
                    sqlEntity.Insert_NotInterestedVideos(video.Videos?.VideoAdClass);
                    sqlEntity.Dispose();
                } 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

       
       public override int ItemCount => VideoList?.Count ?? 0;
        
        public DataWatchLaterVideos GetItem(int position)
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
        void OnClick(WatchLaterVideoRowAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(WatchLaterVideoRowAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

        public IList GetPreloadItems(int p0)
        {
            try
            {
                var d = new List<string>();
                var item = VideoList[p0];

                if (item == null)
                    return Collections.SingletonList(p0);

                if (item.Videos?.VideoAdClass.Thumbnail != "")
                {
                    d.Add(item.Videos?.VideoAdClass.Thumbnail);
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

    public class WatchLaterVideoRowAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; }

        public ImageView VideoImage { get; private set; }
        public TextView TxtDuration { get; private set; }
        public TextView TxtTitle { get; private set; }
        public TextView TxtChannelName { get; private set; }
        public TextView TxtViewsCount { get; private set; }
        public TextView IconVerified { get; private set; }
        public TextView MenueView { get; private set; }

        public TextView VideoType { get; private set; }

        #endregion

        public WatchLaterVideoRowAdapterViewHolder(View itemView, Action<WatchLaterVideoRowAdapterClickEventArgs> clickListener,Action<WatchLaterVideoRowAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                VideoImage = (ImageView)MainView.FindViewById(Resource.Id.Imagevideo);
                TxtDuration = MainView.FindViewById<TextView>(Resource.Id.duration);
                TxtTitle = MainView.FindViewById<TextView>(Resource.Id.Title);
                TxtChannelName = MainView.FindViewById<TextView>(Resource.Id.ChannelName);
                TxtViewsCount = MainView.FindViewById<TextView>(Resource.Id.Views_Count);
                IconVerified = MainView.FindViewById<TextView>(Resource.Id.IconVerified);
                MenueView = MainView.FindViewById<TextView>(Resource.Id.videoMenue);

                VideoType = MainView.FindViewById<TextView>(Resource.Id.videoType);
                //Create an Event
                itemView.Click += (sender, e) => clickListener(new WatchLaterVideoRowAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new WatchLaterVideoRowAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }

    public class WatchLaterVideoRowAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}