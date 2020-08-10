using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Integration.RecyclerView;
using Bumptech.Glide.Request;
using Bumptech.Glide.Util;
using Java.Util;
using PlayTube.Activities.Channel.ChannelPopular;
using PlayTube.Activities.Models;
using PlayTube.Adapters;
using PlayTube.Helpers.CacheLoaders;
using PlayTube.Helpers.Fonts;
using PlayTube.Helpers.Models;
using PlayTube.Helpers.Utils;
using PlayTube.SQLite;
using PlayTubeClient.Classes.Global;
using PlayTubeClient.Classes.Video;
using IList = System.Collections.IList;
using PopupMenu = Android.Support.V7.Widget.PopupMenu;

namespace PlayTube.Activities.Tabbes.Adapters
{ 
    public class TrendingAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {
        public event EventHandler<TrendingAdapterClickEventArgs> ItemClick;
        public event EventHandler<TrendingAdapterClickEventArgs> ItemLongClick;

        public event EventHandler<VideoRowAdapterClickEventArgs> VideoItemClick;
        public event EventHandler<VideoRowAdapterClickEventArgs> VideoItemLongClick;


        private int Position;
        private readonly Activity ActivityContext;
        private readonly TabbedMainActivity GlobalContext;
        private PopularChannelAdapter PopularChannelAdapter;
        public ObservableCollection<Classes.TrendingClass> TrendingList = new ObservableCollection<Classes.TrendingClass>();
        private readonly LibrarySynchronizer LibrarySynchronizer;

        public TrendingAdapter(Activity context)
        {
            try
            {
                ActivityContext = context;
                GlobalContext = TabbedMainActivity.GetInstance();
                LibrarySynchronizer = new LibrarySynchronizer(context);
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
                if (viewType == (int)ItemType.Channel)
                {
                    //Setup your layout here >> TemplateRecyclerViewLayout
                    View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.TemplateRecyclerViewLayout, parent, false);
                    var vh = new TrendingAdapterViewHolder(itemView, OnClick, OnLongClick);
                    return vh;
                }

                if (viewType == (int)ItemType.Video)
                {
                    //Setup your layout here >> Video_Row_View
                    View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Video_Row_View, parent, false);
                    var vh = new VideoRowAdapterViewHolder(itemView, VideoOnClick, VideoOnLongClick);
                    return vh;
                }
                 
                if (viewType == (int)ItemType.EmptyPage)
                {
                    //Setup your layout here >> EmptyStateLayout
                    View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.EmptyStateLayout, parent, false);
                    var vh = new EmptyStateViewHolder(itemView);
                    return vh;
                }
                return null;
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
                Position = position;
                 
                var item = TrendingList[Position];
                if (item != null)
                {
                    if (item.Type == ItemType.Channel)
                    {
                        if (viewHolder is TrendingAdapterViewHolder holder)
                        {
                            if (PopularChannelAdapter == null)
                            {
                                PopularChannelAdapter = new PopularChannelAdapter(ActivityContext)
                                {
                                    ChannelList = new ObservableCollection<GetPopularChannelsObject.Channel>()
                                };

                                LinearLayoutManager layoutManager = new LinearLayoutManager(ActivityContext, LinearLayoutManager.Horizontal, false);
                                holder.MRecycler.SetLayoutManager(layoutManager);
                                holder.MRecycler.GetLayoutManager().ItemPrefetchEnabled = true;

                                var sizeProvider = new FixedPreloadSizeProvider(10, 10);
                                var preLoader = new RecyclerViewPreloader<GetPopularChannelsObject.Channel>(ActivityContext, PopularChannelAdapter, sizeProvider, 10);
                                holder.MRecycler.AddOnScrollListener(preLoader);
                                holder.MRecycler.SetAdapter(PopularChannelAdapter);
                                PopularChannelAdapter.OnItemClick += PopularChannelAdapterOnOnItemClick;
                            }

                            if (item.ChannelList.Count > 0)
                            {
                                if (PopularChannelAdapter.ChannelList.Count == 0)
                                {
                                    PopularChannelAdapter.ChannelList = new ObservableCollection<GetPopularChannelsObject.Channel>(item.ChannelList);
                                    PopularChannelAdapter.NotifyDataSetChanged();
                                }
                                else if (PopularChannelAdapter.ChannelList != null && PopularChannelAdapter.ChannelList.Count != item.ChannelList.Count)
                                {
                                    PopularChannelAdapter.ChannelList = new ObservableCollection<GetPopularChannelsObject.Channel>(item.ChannelList);
                                    PopularChannelAdapter.NotifyDataSetChanged();
                                }
                            }

                            holder.MainLinear.Visibility = ViewStates.Visible;
                            holder.MoreText.Visibility = PopularChannelAdapter.ChannelList?.Count >= 5 ? ViewStates.Visible : ViewStates.Invisible;
                            holder.TitleText.Text = ActivityContext.GetText(Resource.String.Lbl_PopularChannels);

                        }
                    }
                    else if (item.Type == ItemType.Video)
                    {
                        if (viewHolder is VideoRowAdapterViewHolder videoRow)
                        {
                            GlideImageLoader.LoadImage(ActivityContext, item.VideoData.Thumbnail, videoRow.VideoImage, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);

                            videoRow.TxtDuration.Text = Methods.FunString.SplitStringDuration(item.VideoData.Duration);
                            videoRow.TxtTitle.Text = Methods.FunString.DecodeString(item.VideoData.Title);

                            videoRow.TxtChannelName.Text = AppTools.GetNameFinal(item.VideoData.Owner);

                            videoRow.TxtViewsCount.Text = Methods.FunString.FormatPriceValue(Convert.ToInt32(item.VideoData.Views)) + " " + ActivityContext.GetText(Resource.String.Lbl_Views);

                            FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, videoRow.MenueView, IonIconsFonts.AndroidMoreVertical);

                            //Verified 
                            if (item.VideoData.Owner.Verified == "1")
                            {
                                videoRow.IconVerified.Visibility = ViewStates.Visible;
                                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, videoRow.IconVerified, IonIconsFonts.CheckmarkCircled);
                            }
                            else
                            {
                                videoRow.IconVerified.Visibility = ViewStates.Gone;
                            }

                            if (!videoRow.MenueView.HasOnClickListeners)
                                videoRow.MenueView.Click += (sender, args) =>
                                {
                                    ContextThemeWrapper ctw = new ContextThemeWrapper(ActivityContext, Resource.Style.PopupMenuStyle);
                                    PopupMenu popup = new PopupMenu(ctw, videoRow.MenueView);
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
                                                    LibrarySynchronizer.AddToWatchLater(item.VideoData);
                                                    break;

                                                case Resource.Id.menu_AddPlaylist:
                                                    LibrarySynchronizer.AddToPlaylist(item.VideoData);
                                                    break;

                                                case Resource.Id.menu_Remove:
                                                    OnMenuRemove_Click(item.VideoData);
                                                    break;

                                                case Resource.Id.menu_Share:
                                                    LibrarySynchronizer.ShareVideo(item.VideoData);
                                                    break;

                                                case Resource.Id.menu_Report:
                                                    LibrarySynchronizer.AddReportVideo(item.VideoData);
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
                            AppTools.ShowGlobalBadgeSystem(videoRow.VideoType, item.VideoData);
                        }
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
                var index = TrendingList.IndexOf(TrendingList.FirstOrDefault(a => a.VideoData?.VideoId == video.VideoId));
                if (index != -1)
                {
                    TrendingList.RemoveAt(index);
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

        private void PopularChannelAdapterOnOnItemClick(object sender, PopularChannelAdapterClickEventArgs e)
        {
            try
            {
                if(e.Position <= -1) return;

                var item = PopularChannelAdapter.GetItem(e.Position);
                if (item == null) return;

                GlobalContext.ShowUserChannelFragment(item.UserData, item.UserData.Id);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public override int ItemCount => TrendingList?.Count ?? 0;
     
        public Classes.TrendingClass GetItem(int position)
        {
            return TrendingList[position];
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
                var item = TrendingList[position];
                if (item != null)
                {
                    if (item.Type == ItemType.Video)
                        return (int)ItemType.Video;
                    
                    if (item.Type == ItemType.Channel)
                        return (int)ItemType.Channel;

                    if (item.Type == ItemType.EmptyPage)
                        return (int)ItemType.EmptyPage;
                }
               
                return position;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return 0;
            }
        }

        void OnClick(TrendingAdapterClickEventArgs args) => ItemClick?.Invoke(ActivityContext, args);
        void OnLongClick(TrendingAdapterClickEventArgs args) => ItemLongClick?.Invoke(ActivityContext, args);

        void VideoOnClick(VideoRowAdapterClickEventArgs args) => VideoItemClick?.Invoke(this, args);
        void VideoOnLongClick(VideoRowAdapterClickEventArgs args) => VideoItemLongClick?.Invoke(this, args);

        public IList GetPreloadItems(int p0)
        {
            try
            {
                var d = new List<string>();
                var item = TrendingList[p0];

                if (item?.VideoData == null)
                    return Collections.SingletonList(p0);

                if (!string.IsNullOrEmpty(item.VideoData?.Thumbnail))
                {
                    d.Add(item.VideoData.Thumbnail);
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
            return Glide.With(ActivityContext).Load(p0.ToString()).Apply(new RequestOptions().CenterCrop());
        }
         
    }

    public class TrendingAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; }
        public RelativeLayout MainLinear { get; private set; }
        public TextView TitleText { get; private set; } 
        public TextView MoreText { get; private set; }
        public RecyclerView MRecycler { get; private set; }

        #endregion

        public TrendingAdapterViewHolder(View itemView, Action<TrendingAdapterClickEventArgs> clickListener, Action<TrendingAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            { 
                MainView = itemView;

                MainLinear = MainView.FindViewById<RelativeLayout>(Resource.Id.mainLinear);
                TitleText = MainView.FindViewById<TextView>(Resource.Id.textTitle); 
                MoreText = MainView.FindViewById<TextView>(Resource.Id.textMore);
                MRecycler = MainView.FindViewById<RecyclerView>(Resource.Id.recyler);
                 
                MRecycler.HasFixedSize = true;
                MRecycler.SetItemViewCacheSize(10);
                 
                //Create an Event
                MainLinear.Click += MoreTextOnClick;
                //itemView.Click += (sender, e) => clickListener(new TrendingAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                //itemView.LongClick += (sender, e) => longClickListener(new TrendingAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void MoreTextOnClick(object sender, EventArgs e)
        {
            try
            {
               var globalContext = TabbedMainActivity.GetInstance();
               if (globalContext?.TrendingFragment != null)
               {
                   globalContext.TrendingFragment.AllChannelPopularFragment = new AllChannelPopularFragment();
                   globalContext.FragmentBottomNavigator.DisplayFragment(globalContext.TrendingFragment.AllChannelPopularFragment);
               } 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }

    public class EmptyStateViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; }
        public Button EmptyStateButton { get; private set; }
        public ImageView EmptyStateIcon { get; private set; }
        public TextView DescriptionText { get; private set; }
        public TextView TitleText { get; private set; }

        #endregion

        public EmptyStateViewHolder(View itemView) : base(itemView)
        {
            try
            {
                MainView = itemView;

                EmptyStateIcon = (ImageView)MainView.FindViewById(Resource.Id.emtyicon);
                TitleText = (TextView)MainView.FindViewById(Resource.Id.headText);
                DescriptionText = (TextView)MainView.FindViewById(Resource.Id.seconderyText);
                EmptyStateButton = (Button)MainView.FindViewById(Resource.Id.button);

                //FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.AndroidCamera);
                EmptyStateIcon.SetImageResource(Resource.Drawable.icon_fire_vector);

                TitleText.Text = Application.Context.GetText(Resource.String.Lbl_Emptyvideos);
                DescriptionText.Text = Application.Context.GetText(Resource.String.Lbl_Start_Trending);
                EmptyStateButton.Visibility = ViewStates.Gone;

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }

    public class TrendingAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}