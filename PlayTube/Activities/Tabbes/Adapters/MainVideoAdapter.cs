using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Integration.RecyclerView;
using Bumptech.Glide.Request;
using Bumptech.Glide.Util;
using Java.Util;
using PlayTube.Activities.Models;
using PlayTube.Activities.Search;
using PlayTube.Activities.Videos;
using PlayTube.Adapters;
using PlayTube.Helpers.CacheLoaders;
using PlayTube.Helpers.Fonts;
using PlayTube.Helpers.Models;
using PlayTube.Helpers.Utils;
using PlayTube.SQLite;
using PlayTubeClient.Classes.Global;
using PopupMenu = Android.Support.V7.Widget.PopupMenu;
using IList = System.Collections.IList;

namespace PlayTube.Activities.Tabbes.Adapters
{
    public class MainVideoAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {
        public event EventHandler<MainVideoAdapterClickEventArgs> ItemClick;
        public event EventHandler<MainVideoAdapterClickEventArgs> ItemLongClick;

        public event EventHandler<VideoAdapterClickEventArgs> OtherVideosItemClick;
        public event EventHandler<VideoAdapterClickEventArgs> OtherVideosItemLongClick;

        private int Position;
        private readonly Activity ActivityContext;
        private readonly TabbedMainActivity GlobalContext;
        private CategoryIconAdapter CategoryIconAdapter;
        private VideoHorizontalAdapter TopVideosAdapter, LatestVideosAdapter, FavVideosAdapter;

        public ObservableCollection<Classes.MainVideoClass> MainVideoList = new ObservableCollection<Classes.MainVideoClass>();

        private readonly LibrarySynchronizer LibrarySynchronizer;

        public MainVideoAdapter(Activity context)
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
                if (viewType == (int)ItemType.Category || viewType == (int)ItemType.TopVideos || viewType == (int)ItemType.LatestVideos || viewType == (int)ItemType.FavVideos)
                { 
                    //Setup your layout here >> TemplateRecyclerViewLayout 
                    View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.TemplateRecyclerViewLayout, parent, false);
                    var vh = new MainVideoAdapterViewHolder(itemView, OnClick, OnLongClick);
                    return vh;
                }

                if (viewType == (int)ItemType.OtherVideos)
                {
                    //Setup your layout here >> Video_Big_View
                    View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Video_Big_View, parent, false);
                    var vh = new VideoBigAdapterViewHolder(itemView, OtherVideosOnClick, OtherVideosOnLongClick);
                    return vh;
                }
                 
                if (viewType == (int) ItemType.EmptyPage)
                {
                    //Setup your layout here >> EmptyStateLayout
                    View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.EmptyStateLayout, parent, false);
                    var vh = new Library.Adapters.EmptyStateViewHolder(itemView);
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

                var item = MainVideoList[Position];
                if (item != null)
                {
                    if (item.Type == ItemType.Category || item.Type == ItemType.TopVideos || item.Type == ItemType.LatestVideos || item.Type == ItemType.FavVideos)
                    {
                        if (viewHolder is MainVideoAdapterViewHolder holder)
                        {
                            if (item.Type == ItemType.Category)
                            {
                                if (CategoryIconAdapter == null)
                                {
                                    CategoryIconAdapter = new CategoryIconAdapter(ActivityContext) { CategoryList = new ObservableCollection<Classes.Category>() };
                                    CategoryIconAdapter.ItemClick += CategoryIconAdapterOnItemClick;

                                    LinearLayoutManager layoutManager = new LinearLayoutManager(ActivityContext, LinearLayoutManager.Horizontal, false);
                                    holder.MRecycler.SetLayoutManager(layoutManager);
                                    holder.MRecycler.GetLayoutManager().ItemPrefetchEnabled = true;

                                    holder.MRecycler.SetAdapter(CategoryIconAdapter);
                                }

                                if (item.CategoryList.Count > 0)
                                {
                                    if (CategoryIconAdapter.CategoryList.Count == 0)
                                    {
                                        CategoryIconAdapter.CategoryList = new ObservableCollection<Classes.Category>(item.CategoryList);
                                        CategoryIconAdapter.NotifyDataSetChanged();
                                    }
                                    else if (CategoryIconAdapter.CategoryList != null && CategoryIconAdapter.CategoryList.Count != item.CategoryList.Count)
                                    {
                                        CategoryIconAdapter.CategoryList = new ObservableCollection<Classes.Category>(item.CategoryList);
                                        CategoryIconAdapter.NotifyDataSetChanged();
                                    }
                                }

                                holder.MainLinear.Visibility = ViewStates.Visible;

                                if (!holder.MainLinear.HasOnClickListeners)
                                    holder.MainLinear.Click += CategoryMainLinearOnClick;
                                 
                                holder.MoreText.Visibility = CategoryIconAdapter.CategoryList?.Count >= 5 ? ViewStates.Visible : ViewStates.Invisible;
                                holder.TitleText.Text = ActivityContext.GetText(Resource.String.Lbl_Category);
                                holder.TitleText.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.ic_cat_vector, 0, 0, 0);
                            }
                            else if (item.Type == ItemType.TopVideos)
                            {
                                if (TopVideosAdapter == null)
                                {
                                    TopVideosAdapter = new VideoHorizontalAdapter(ActivityContext, VideoAdapterClickEventArgs.VideoType.TopVideo) { VideoList = new ObservableCollection<VideoObject>() };
                                    TopVideosAdapter.ItemClick += TopVideosAdapterOnItemClick;

                                    LinearLayoutManager layoutManager = new LinearLayoutManager(ActivityContext, LinearLayoutManager.Horizontal, false);
                                    holder.MRecycler.SetLayoutManager(layoutManager);
                                    holder.MRecycler.GetLayoutManager().ItemPrefetchEnabled = true;

                                    var sizeProvider = new FixedPreloadSizeProvider(10, 10);
                                    var preLoader = new RecyclerViewPreloader<VideoObject>(ActivityContext, TopVideosAdapter, sizeProvider, 10);
                                    holder.MRecycler.AddOnScrollListener(preLoader);
                                    holder.MRecycler.SetAdapter(TopVideosAdapter);
                                }
                                 
                                if (item.TopVideoList.Count > 0)
                                {
                                    if (TopVideosAdapter.VideoList.Count == 0)
                                    {
                                        TopVideosAdapter.VideoList = new ObservableCollection<VideoObject>(item.TopVideoList);
                                        TopVideosAdapter.NotifyDataSetChanged();
                                    }
                                    else if (TopVideosAdapter.VideoList != null && TopVideosAdapter.VideoList.Count != item.TopVideoList.Count)
                                    {
                                        TopVideosAdapter.VideoList = new ObservableCollection<VideoObject>(item.TopVideoList);
                                        TopVideosAdapter.NotifyDataSetChanged();
                                    }
                                }

                                holder.MainLinear.Visibility = ViewStates.Visible;

                                if (!holder.MainLinear.HasOnClickListeners)
                                    holder.MainLinear.Click += TopVideosMainLinearOnClick;
                                     
                                holder.MoreText.Visibility = TopVideosAdapter.VideoList?.Count >= 5 ? ViewStates.Visible : ViewStates.Invisible;
                                holder.TitleText.Text = ActivityContext.GetText(Resource.String.Lbl_Top_videos);
                                holder.TitleText.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.ic_bar_chart_vector, 0, 0, 0);
                            }
                            else if (item.Type == ItemType.LatestVideos)
                            {
                                if (LatestVideosAdapter == null)
                                {
                                    LatestVideosAdapter = new VideoHorizontalAdapter(ActivityContext, VideoAdapterClickEventArgs.VideoType.TopVideo) { VideoList = new ObservableCollection<VideoObject>() };
                                    LatestVideosAdapter.ItemClick += LatestVideosAdapterOnItemClick;

                                    LinearLayoutManager layoutManager = new LinearLayoutManager(ActivityContext, LinearLayoutManager.Horizontal, false);
                                    holder.MRecycler.SetLayoutManager(layoutManager);
                                    holder.MRecycler.GetLayoutManager().ItemPrefetchEnabled = true;

                                    var sizeProvider = new FixedPreloadSizeProvider(10, 10);
                                    var preLoader = new RecyclerViewPreloader<VideoObject>(ActivityContext, LatestVideosAdapter, sizeProvider, 10);
                                    holder.MRecycler.AddOnScrollListener(preLoader);
                                    holder.MRecycler.SetAdapter(LatestVideosAdapter);
                                }

                                if (item.LatestVideoList.Count > 0)
                                {
                                    if (LatestVideosAdapter.VideoList.Count == 0)
                                    {
                                        LatestVideosAdapter.VideoList = new ObservableCollection<VideoObject>(item.LatestVideoList);
                                        LatestVideosAdapter.NotifyDataSetChanged();
                                    }
                                    else if (LatestVideosAdapter.VideoList != null && LatestVideosAdapter.VideoList.Count != item.LatestVideoList.Count)
                                    {
                                        LatestVideosAdapter.VideoList = new ObservableCollection<VideoObject>(item.LatestVideoList);
                                        LatestVideosAdapter.NotifyDataSetChanged();
                                    }
                                }

                                holder.MainLinear.Visibility = ViewStates.Visible;

                                if (!holder.MainLinear.HasOnClickListeners)
                                    holder.MainLinear.Click += LatestVideosMainLinearOnClick;
                                 
                                holder.MoreText.Visibility = LatestVideosAdapter.VideoList?.Count >= 5 ? ViewStates.Visible : ViewStates.Invisible;
                                holder.TitleText.Text = ActivityContext.GetText(Resource.String.Lbl_Latest_videos);
                                holder.TitleText.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.ic_video_camera_vector, 0, 0, 0);
                            }
                            else if (item.Type == ItemType.FavVideos)
                            {
                                if (FavVideosAdapter == null)
                                {
                                    FavVideosAdapter = new VideoHorizontalAdapter(ActivityContext, VideoAdapterClickEventArgs.VideoType.TopVideo) { VideoList = new ObservableCollection<VideoObject>() };
                                    FavVideosAdapter.ItemClick += FavVideosAdapterOnItemClick;

                                    LinearLayoutManager layoutManager = new LinearLayoutManager(ActivityContext, LinearLayoutManager.Horizontal, false);
                                    holder.MRecycler.SetLayoutManager(layoutManager);
                                    holder.MRecycler.GetLayoutManager().ItemPrefetchEnabled = true;

                                    var sizeProvider = new FixedPreloadSizeProvider(10, 10);
                                    var preLoader = new RecyclerViewPreloader<VideoObject>(ActivityContext, FavVideosAdapter, sizeProvider, 10);
                                    holder.MRecycler.AddOnScrollListener(preLoader);
                                    holder.MRecycler.SetAdapter(FavVideosAdapter);
                                }

                                if (item.FavVideoList.Count > 0)
                                {
                                    if (FavVideosAdapter.VideoList.Count == 0)
                                    {
                                        FavVideosAdapter.VideoList = new ObservableCollection<VideoObject>(item.FavVideoList);
                                        FavVideosAdapter.NotifyDataSetChanged();
                                    }
                                    else if (FavVideosAdapter.VideoList != null && FavVideosAdapter.VideoList.Count != item.FavVideoList.Count)
                                    {
                                        FavVideosAdapter.VideoList = new ObservableCollection<VideoObject>(item.FavVideoList);
                                        FavVideosAdapter.NotifyDataSetChanged();
                                    }
                                }

                                holder.MainLinear.Visibility = ViewStates.Visible;

                                if (!holder.MainLinear.HasOnClickListeners)
                                    holder.MainLinear.Click += FavVideosMainLinearOnClick;

                                holder.MoreText.Visibility = FavVideosAdapter.VideoList?.Count >= 5 ? ViewStates.Visible : ViewStates.Invisible;
                                holder.TitleText.Text = ActivityContext.GetText(Resource.String.Lbl_Fav_videos);
                                holder.TitleText.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.ic_star_vector, 0, 0, 0);
                            }
                        }
                    }
                    else if (item.Type == ItemType.OtherVideos)
                    {
                        if (viewHolder is VideoBigAdapterViewHolder videoRow)
                        {
                            GlideImageLoader.LoadImage(ActivityContext, item.VideoData.Thumbnail, videoRow.VideoImage, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                            GlideImageLoader.LoadImage(ActivityContext, item.VideoData.Owner.Avatar, videoRow.ChannelImage, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);

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

        private void FavVideosMainLinearOnClick(object sender, EventArgs e)
        {
            try
            {
                Bundle bundle = new Bundle();

                bundle.PutString("Type", "Fav");

                VideosByTypeFragment videoViewerFragment = new VideosByTypeFragment()
                {
                    Arguments = bundle
                };
                GlobalContext?.FragmentBottomNavigator.DisplayFragment(videoViewerFragment);
                GlobalContext?.VideoActionsController.SetStopvideo();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void LatestVideosMainLinearOnClick(object sender, EventArgs e)
        {
            try
            {
                Bundle bundle = new Bundle();

                bundle.PutString("Type", "Latest");

                VideosByTypeFragment videoViewerFragment = new VideosByTypeFragment()
                {
                    Arguments = bundle
                };
                GlobalContext?.FragmentBottomNavigator.DisplayFragment(videoViewerFragment);
                GlobalContext?.VideoActionsController.SetStopvideo();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void TopVideosMainLinearOnClick(object sender, EventArgs e)
        {
            try
            {
                Bundle bundle = new Bundle();

                bundle.PutString("Type", "Top");

                VideosByTypeFragment videoViewerFragment = new VideosByTypeFragment()
                {
                    Arguments = bundle
                };
                GlobalContext?.FragmentBottomNavigator.DisplayFragment(videoViewerFragment);
                GlobalContext?.VideoActionsController.SetStopvideo();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void CategoryMainLinearOnClick(object sender, EventArgs e)
        {
            try
            {
                SearchFragment searchFragment = new SearchFragment();
                GlobalContext?.FragmentBottomNavigator.DisplayFragment(searchFragment);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        //Open Video from Fav
        private void FavVideosAdapterOnItemClick(object sender, VideoAdapterClickEventArgs e)
        {
            try
            {
                if (e.Position <= -1) return;

                var item = FavVideosAdapter.GetItem(e.Position);
                if (item == null) return;

                GlobalContext.StartPlayVideo(item);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Open Video from Latest
        private void LatestVideosAdapterOnItemClick(object sender, VideoAdapterClickEventArgs e)
        {
            try
            {
                if (e.Position <= -1) return;

                var item = LatestVideosAdapter.GetItem(e.Position);
                if (item == null) return;

                GlobalContext.StartPlayVideo(item);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Open Video from Top
        private void TopVideosAdapterOnItemClick(object sender, VideoAdapterClickEventArgs e)
        {
            try
            {
                if (e.Position <= -1) return;

                var item = TopVideosAdapter.GetItem(e.Position);
                if (item == null) return;

                GlobalContext.StartPlayVideo(item);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //remove Video
        private void OnMenuRemove_Click(VideoObject video)
        {
            try
            {
                var index = MainVideoList.IndexOf(MainVideoList.FirstOrDefault(a => a.VideoData?.VideoId == video.VideoId));
                if (index != -1)
                {
                    MainVideoList.RemoveAt(index);
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
         
        //get video by Category
        private void CategoryIconAdapterOnItemClick(object sender, CategoryIconAdapterClickEventArgs e)
        {
            try
            {
                if (e.Position <= -1) return;

                var item = CategoryIconAdapter.GetItem(e.Position);
                if (item == null) return;

                Bundle bundle = new Bundle();
                bundle.PutString("CatId", item.Id);
                bundle.PutString("CatName", item.Name);

                VideosByCategoryFragment videoViewerFragment = new VideosByCategoryFragment()
                {
                    Arguments = bundle
                };

                GlobalContext.FragmentBottomNavigator.DisplayFragment(videoViewerFragment);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public override int ItemCount => MainVideoList?.Count ?? 0;

        public Classes.MainVideoClass GetItem(int position)
        {
            return MainVideoList[position];
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
                var item = MainVideoList[position];
                if (item != null)
                {
                    if (item.Type == ItemType.Category)
                        return (int) ItemType.Category;

                    if (item.Type == ItemType.TopVideos)
                        return (int) ItemType.TopVideos;

                    if (item.Type == ItemType.LatestVideos)
                        return (int) ItemType.LatestVideos;

                    if (item.Type == ItemType.FavVideos)
                        return (int) ItemType.FavVideos;

                    if (item.Type == ItemType.OtherVideos)
                        return (int) ItemType.OtherVideos;

                    if (item.Type == ItemType.EmptyPage)
                        return (int) ItemType.EmptyPage;
                }

                return position;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return 0;
            }
        }

        void OnClick(MainVideoAdapterClickEventArgs args) => ItemClick?.Invoke(ActivityContext, args);
        void OnLongClick(MainVideoAdapterClickEventArgs args) => ItemLongClick?.Invoke(ActivityContext, args);

        void OtherVideosOnClick(VideoAdapterClickEventArgs args) => OtherVideosItemClick?.Invoke(this, args);
        void OtherVideosOnLongClick(VideoAdapterClickEventArgs args) => OtherVideosItemLongClick?.Invoke(this, args);

        public IList GetPreloadItems(int p0)
        {
            try
            {
                var d = new List<string>();
                var item = MainVideoList[p0];

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

    public class MainVideoAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; }
        public RelativeLayout MainLinear { get; private set; }
        public TextView TitleText { get; private set; }
        public TextView MoreText { get; private set; }
        public RecyclerView MRecycler { get; private set; }

        #endregion

        public MainVideoAdapterViewHolder(View itemView, Action<MainVideoAdapterClickEventArgs> clickListener, Action<MainVideoAdapterClickEventArgs> longClickListener) : base(itemView)
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
                //itemView.Click += (sender, e) => clickListener(new MainVideoAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                //itemView.LongClick += (sender, e) => longClickListener(new MainVideoAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        } 
    }

    public class MainVideoAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}