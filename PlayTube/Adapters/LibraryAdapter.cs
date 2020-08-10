using System;
using System.Collections.ObjectModel;
using System.Linq;
using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using PlayTube.Helpers.CacheLoaders;
using PlayTube.Helpers.Fonts;
using PlayTube.Helpers.Models;
using PlayTube.SQLite;

namespace PlayTube.Adapters
{
    public class LibraryAdapter : RecyclerView.Adapter
    {
        public event EventHandler<LibraryAdapterClickEventArgs> ItemClick;
        public event EventHandler<LibraryAdapterClickEventArgs> ItemLongClick;
        private readonly Activity ActivityContext;
        public ObservableCollection<Classes.LibraryItem> LibraryList = new ObservableCollection<Classes.LibraryItem>();

        public LibraryAdapter(Activity context)
        {
            try
            {
                HasStableIds = true;
                ActivityContext = context;
                AddLibrarySectionViews();
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
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Library_view, parent, false);
                var vh = new LibraryAdapterViewHolder(itemView, OnClick, OnLongClick);

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
                if (position >= 0)
                {
                    if (viewHolder is LibraryAdapterViewHolder holder)
                    {

                        var item = LibraryList[position];
                        if (item != null)
                        {
                            if (item.SectionId == "1") // Subscriptions
                            {
                                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, holder.SectionIconView, IonIconsFonts.Checkmark);
                                holder.SectionTextView.Text = ActivityContext.GetText(Resource.String.Lbl_Subscriptions);

                                if (item.VideoCount == 0)
                                {
                                    holder.SectionVideosCounTextView.Visibility = ViewStates.Gone;
                                }
                                else
                                {
                                    holder.SectionVideosCounTextView.Visibility = ViewStates.Visible;
                                    holder.SectionVideosCounTextView.Text =
                                        item.VideoCount + " " + ActivityContext.GetText(Resource.String.Lbl_Videos);
                                }
                            }
                            else if (item.SectionId == "2") // Watch Later
                            {
                                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, holder.SectionIconView, IonIconsFonts.IosClock);
                                holder.SectionTextView.Text = ActivityContext.GetText(Resource.String.Lbl_WatchLater);
                                if (item.VideoCount == 0)
                                {
                                    holder.SectionVideosCounTextView.Visibility = ViewStates.Gone;
                                }
                                else
                                {
                                    holder.SectionVideosCounTextView.Visibility = ViewStates.Visible;
                                    holder.SectionVideosCounTextView.Text =item.VideoCount + " " + ActivityContext.GetText(Resource.String.Lbl_Videos);
                                }
                            }
                            else if (item.SectionId == "3") // Recently Watched 
                            {
                                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, holder.SectionIconView, IonIconsFonts.Calendar);
                                holder.SectionTextView.Text = ActivityContext.GetText(Resource.String.Lbl_RecentlyWatched);
                                if (item.VideoCount == 0)
                                {
                                    holder.SectionVideosCounTextView.Visibility = ViewStates.Gone;
                                }
                                else
                                {
                                    holder.SectionVideosCounTextView.Visibility = ViewStates.Visible;
                                    holder.SectionVideosCounTextView.Text =item.VideoCount + " " + ActivityContext.GetText(Resource.String.Lbl_Videos);
                                }
                            }
                            else if (item.SectionId == "4") // Watch Offline 
                            {
                                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, holder.SectionIconView, IonIconsFonts.IosCloudDownload);
                                holder.SectionTextView.Text = ActivityContext.GetText(Resource.String.Lbl_WatchOffline);
                                if (item.VideoCount == 0)
                                {
                                    holder.SectionVideosCounTextView.Visibility = ViewStates.Gone;
                                }
                                else
                                {
                                    holder.SectionVideosCounTextView.Visibility = ViewStates.Visible;
                                    holder.SectionVideosCounTextView.Text = item.VideoCount + " " + ActivityContext.GetText(Resource.String.Lbl_Videos);
                                }
                            }
                            else if (item.SectionId == "5") // PlayLists
                            {
                                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, holder.SectionIconView, IonIconsFonts.IosFilm);
                                holder.SectionTextView.Text = ActivityContext.GetText(Resource.String.Lbl_PlayLists);
                                if (item.VideoCount == 0)
                                {
                                    holder.SectionVideosCounTextView.Visibility = ViewStates.Gone;
                                }
                                else
                                {
                                    holder.SectionVideosCounTextView.Visibility = ViewStates.Visible;
                                    holder.SectionVideosCounTextView.Text =item.VideoCount + " " + ActivityContext.GetText(Resource.String.Lbl_Videos);
                                }
                            }
                            else if (item.SectionId == "6") // Liked
                            {
                                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, holder.SectionIconView, IonIconsFonts.IosHeartOutline);
                                holder.SectionTextView.Text = ActivityContext.GetText(Resource.String.Lbl_Liked);
                                if (item.VideoCount == 0)
                                {
                                    holder.SectionVideosCounTextView.Visibility = ViewStates.Gone;
                                }
                                else
                                {
                                    holder.SectionVideosCounTextView.Visibility = ViewStates.Visible;
                                    holder.SectionVideosCounTextView.Text =item.VideoCount + " " + ActivityContext.GetText(Resource.String.Lbl_Videos);
                                }
                            }
                            else if (item.SectionId == "7") // Shared
                            {
                                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, holder.SectionIconView, IonIconsFonts.Share);
                                holder.SectionTextView.Text = ActivityContext.GetText(Resource.String.Lbl_Shared);
                                if (item.VideoCount == 0)
                                {
                                    holder.SectionVideosCounTextView.Visibility = ViewStates.Gone;
                                }
                                else
                                {
                                    holder.SectionVideosCounTextView.Visibility = ViewStates.Visible;
                                    holder.SectionVideosCounTextView.Text =item.VideoCount + " " + ActivityContext.GetText(Resource.String.Lbl_Videos);
                                }
                            }
                            else if (item.SectionId == "8") // Paid
                            {
                                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, holder.SectionIconView, IonIconsFonts.SocialUsd);
                                holder.SectionTextView.Text = ActivityContext.GetText(Resource.String.Lbl_Paid);
                                if (item.VideoCount == 0)
                                {
                                    holder.SectionVideosCounTextView.Visibility = ViewStates.Gone;
                                }
                                else
                                {
                                    holder.SectionVideosCounTextView.Visibility = ViewStates.Visible;
                                    holder.SectionVideosCounTextView.Text =item.VideoCount + " " + ActivityContext.GetText(Resource.String.Lbl_Videos);
                                }
                            }

                            if(item.BackgroundImage ==null)
                                return;

                            GlideImageLoader.LoadImage(ActivityContext, item.BackgroundImage, holder.BacgroundImageview, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void Add(Classes.LibraryItem item)
        {
            try
            {
                var check = LibraryList.FirstOrDefault(a => a.SectionId == item.SectionId);
                if (check == null)
                {
                    LibraryList.Add(item);
                    NotifyItemInserted(LibraryList.IndexOf(LibraryList.Last()));
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        public void Update(Classes.LibraryItem item)
        {
            try
            {
                var data = LibraryList.FirstOrDefault(a => a.SectionId == item.SectionId);
                if (data != null)
                {
                    //if (data.BackgroundImage != item.BackgroundImage)
                    //{
                        data.VideoCount = item.VideoCount;
                        data.BackgroundImage = item.BackgroundImage;

                        NotifyItemChanged(LibraryList.IndexOf(data));
                    //}
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public Classes.LibraryItem GetItem(int position)
        {
            return LibraryList[position];
        }

         public override int ItemCount => LibraryList?.Count ?? 0;
 

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

        void OnClick(LibraryAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(LibraryAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

        public void AddLibrarySectionViews()
        {
            try
            {
                var sqlEntity = new SqLiteDatabase();
                var check = sqlEntity.Get_LibraryItem();
                sqlEntity.Dispose();

                if (check != null && check.Count > 0)
                {
                    foreach (var all in check)
                    {
                        Classes.LibraryItem item = new Classes.LibraryItem
                        {
                            SectionId = all.SectionId,
                            SectionText = all.SectionText,
                            VideoCount = all.VideoCount,
                            BackgroundImage = all.BackgroundImage
                        };

                        Add(item);
                        NotifyDataSetChanged();
                    }
                }
                else
                {
                    //translate text in the adapter
                    LibraryList.Add(new Classes.LibraryItem()
                    {
                        SectionId = "1",
                        SectionText = ActivityContext.GetText(Resource.String.Lbl_Subscriptions),
                        VideoCount = 0,
                        BackgroundImage = "blackdefault"
                    });
                    LibraryList.Add(new Classes.LibraryItem()
                    {
                        SectionId = "2",
                        SectionText = ActivityContext.GetText(Resource.String.Lbl_WatchLater),
                        VideoCount = 0,
                        BackgroundImage = "blackdefault"
                    });
                    LibraryList.Add(new Classes.LibraryItem()
                    {
                        SectionId = "3",
                        SectionText = ActivityContext.GetText(Resource.String.Lbl_RecentlyWatched),
                        VideoCount = 0,
                        BackgroundImage = "blackdefault"
                    });
                    if (AppSettings.AllowOfflineDownload)
                    {
                        LibraryList.Add(new Classes.LibraryItem()
                        {
                            SectionId = "4",
                            SectionText = ActivityContext.GetText(Resource.String.Lbl_WatchOffline),
                            VideoCount = 0,
                            BackgroundImage = "blackdefault"
                        });
                    }
                    LibraryList.Add(new Classes.LibraryItem()
                    {
                        SectionId = "5",
                        SectionText = ActivityContext.GetText(Resource.String.Lbl_PlayLists),
                        VideoCount = 0,
                        BackgroundImage = "blackdefault"
                    });
                    LibraryList.Add(new Classes.LibraryItem()
                    {
                        SectionId = "6",
                        SectionText = ActivityContext.GetText(Resource.String.Lbl_Liked),
                        VideoCount = 0,
                        BackgroundImage = "blackdefault"
                    });
                    LibraryList.Add(new Classes.LibraryItem()
                    {
                        SectionId = "7",
                        SectionText = ActivityContext.GetText(Resource.String.Lbl_Shared),
                        VideoCount = 0,
                        BackgroundImage = "blackdefault"
                    });
                    LibraryList.Add(new Classes.LibraryItem()
                    {
                        SectionId = "8",
                        SectionText = ActivityContext.GetText(Resource.String.Lbl_Paid),
                        VideoCount = 0,
                        BackgroundImage = "blackdefault"
                    });

                    NotifyDataSetChanged();
                    sqlEntity.InsertLibraryItem(LibraryList);
                    sqlEntity.Dispose();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        } 
    }

    public class LibraryAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; }

        public ImageView BacgroundImageview { get; private set; }
        public TextView SectionTextView { get; private set; }
        public TextView SectionVideosCounTextView { get; private set; }
        public TextView SectionIconView { get; private set; }

        #endregion
        
        public LibraryAdapterViewHolder(View itemView, Action<LibraryAdapterClickEventArgs> clickListener, Action<LibraryAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                BacgroundImageview = (ImageView)MainView.FindViewById(Resource.Id.Imagelibraryvideo);
                SectionTextView = MainView.FindViewById<TextView>(Resource.Id.libraryText);
                SectionIconView = MainView.FindViewById<TextView>(Resource.Id.libraryicon);
                SectionVideosCounTextView = MainView.FindViewById<TextView>(Resource.Id.LibraryVideosCount);

                //Create an Event
                itemView.Click += (sender, e) => clickListener(new LibraryAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new LibraryAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }

    public class LibraryAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}