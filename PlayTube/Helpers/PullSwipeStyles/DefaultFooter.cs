using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using Liaoinstan.SpringViewLib.Containers;

namespace PlayTube.Helpers.PullSwipeStyles
{
    public class DefaultFooter : BaseFooter
    {
        private Context MainContext;
        private AnimationDrawable animationLoading;

        private int[] loadingAnimSrcs = new int[] { Resource.Drawable.mt_loading01, Resource.Drawable.mt_loading02 };

        private TextView Title;
        //public ProgressBar ProgressBarView;
        private ImageView ImageIcon;

        public DefaultFooter(Activity context)
        {
            MainContext = context;
            animationLoading = new AnimationDrawable();
            foreach (var src in loadingAnimSrcs)
            {
                animationLoading.AddFrame(ContextCompat.GetDrawable(context, src), 150);
                animationLoading.OneShot = (false);
            }
        }

        public override View GetView(LayoutInflater inflater, ViewGroup viewGroup)
        {
            View view = inflater.Inflate(Resource.Layout.PullRefreshFooter, viewGroup, true);
            //ProgressBarView = view.FindViewById<ProgressBar>(Resource.Id.default_header_progressbar);
            Title = view.FindViewById<TextView>(Resource.Id.default_header_title);
            ImageIcon = view.FindViewById<ImageView>(Resource.Id.default_header_arrow);
            if (animationLoading != null)
            {
                ImageIcon.SetImageDrawable(animationLoading);
            }

            return view;
        }

        public override void OnDropAnim(View rootView, int dy)
        {

        }

        public override void OnLimitDes(View rootView, bool upORdown)
        {
            if (!upORdown)
            {
                Title.Text = MainContext.GetText(Resource.String.Lbl_PullToRefresh);
            }
            else
            {
                Title.Text = MainContext.GetText(Resource.String.Lbl_RefreshMyNewsFeed);
            }
        }

        public override void OnPreDrag(View rootView)
        {
            animationLoading.Stop();
            if (animationLoading != null && animationLoading.NumberOfFrames > 0)
            {
                ImageIcon.SetImageDrawable(animationLoading.GetFrame(0));
            }
        }

        public override void OnStartAnim()
        {
            Title.Text = MainContext.GetText(Resource.String.Lbl_Refreshing);
            if (animationLoading != null)
            {
                ImageIcon.SetImageDrawable(animationLoading);
                animationLoading.Start();
            }
        }

        public override void OnFinishAnim()
        {
            animationLoading.Stop();
            if (animationLoading != null && animationLoading.NumberOfFrames > 0)
            {
                ImageIcon.SetImageDrawable(animationLoading.GetFrame(0));
            }
        }
    }
}