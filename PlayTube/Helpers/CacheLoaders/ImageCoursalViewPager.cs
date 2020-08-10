using System;
using System.Collections.ObjectModel;
using Android.App;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using PlayTube.Activities.Tabbes;
using PlayTube.Helpers.Utils;
using PlayTubeClient.Classes.Global;
using Object = Java.Lang.Object;

namespace PlayTube.Helpers.CacheLoaders
{
    public class ImageCoursalViewPager : PagerAdapter
    {

        public Activity Context;
        public ObservableCollection<VideoObject> VideoList;
        public LayoutInflater Inflater;

        public ImageCoursalViewPager(Activity context, ObservableCollection<VideoObject> videoList)
        {
            Context = context;
            VideoList = videoList;
            Inflater = LayoutInflater.From(Context);
        }

        private TextView FeaturedVideoTitle;
        public override Object InstantiateItem(ViewGroup view, int position)
        {
            try
            {
                View layout = Inflater.Inflate(Resource.Layout.ImageCoursalLayout, view, false);
                var mainFeaturedVideo = layout.FindViewById<ImageView>(Resource.Id.Imagevideo);
                var channelImage = layout.FindViewById<ImageView>(Resource.Id.Image_Channel);
               var channelName = layout.FindViewById<TextView>(Resource.Id.Channel_Name);
                FeaturedVideoTitle = layout.FindViewById<TextView>(Resource.Id.TitleFeaturedVideo);

                channelName.Text = AppTools.GetNameFinal(VideoList[position].Owner);
                FeaturedVideoTitle.Text = Methods.FunString.DecodeString(VideoList[position].Title);

                GlideImageLoader.LoadImage(Context, VideoList[position].Thumbnail, mainFeaturedVideo, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                GlideImageLoader.LoadImage(Context, VideoList[position].Owner.Avatar, channelImage, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);

                mainFeaturedVideo.Click += (sender, args) =>
                {
                   TabbedMainActivity.GetInstance()?.StartPlayVideo(VideoList[position]);
                };

                channelImage.Click += (sender, args) =>
                {
                    TabbedMainActivity.GetInstance()?.ShowUserChannelFragment(VideoList[position].Owner, VideoList[position].Owner.Id);
                };

                FeaturedVideoTitle.Click += (sender, args) =>
                {
                    TabbedMainActivity.GetInstance()?.StartPlayVideo(VideoList[position]);
                };

                view.AddView(layout);

                return layout;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }

        }

        public override bool IsViewFromObject(View view, Object @object)
        {
            return view.Equals(@object);
        }

        public override int Count
        {
            get
            {
                if (VideoList != null)
                {
                    return VideoList.Count;
                }
                else
                {
                    return 0;
                }
            }
        }

        public override void DestroyItem(ViewGroup container, int position, Object @object)
        {
            try
            {
                View view = (View)@object;
                container.RemoveView(view);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            } 
        } 
    } 
}