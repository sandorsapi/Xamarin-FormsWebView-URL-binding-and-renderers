namespace XY_App.Views
{
	public delegate void VideoPageDelegateEventHandler(bool commandString);

	public partial class VideoPage : KeyboardSensitivePage
	{
		public static event VideoPageDelegateEventHandler FullScreenEvent;

		private double width = 0;
		private double height = 0;
		private object savedHeader;
		private VideoPageViewModel bindingContext = null;

		private static Grid hiddenFooter = new Grid
		{
			HeightRequest = 0
		};

		public VideoPage()
		{
			NavigationPage.SetHasNavigationBar(this, false);
			InitializeComponent();
			ConfigureCallbacks();
			VideoPageViewModel.CommentsChangedEventHandler += new VideoPageCommentsChangedEventHandler(CommentsChanged);
		}

		public void CommentsChanged()
		{
			if (bindingContext.commentsItemsSource != null)
			{
				Device.BeginInvokeOnMainThread(() =>
				{
					commentsListView.ItemsSource = bindingContext.commentsItemsSource;
				});
			}
		}

		void HandleInject(string str)
		{
			FullScreenEvent(Convert.ToBoolean(str));
		}

		private void ConfigureCallbacks()
		{
			formsWebView.RegisterLocalCallback("JavascriptFunction", (str) => HandleInject(str));
		}

		public void SetLayout(bool isLandscape)
		{
			bindingContext = BindingContext as VideoPageViewModel;
			
			if (!bindingContext.CombinatingIsVisible)
			{
				bindingContext.IsFullScreen = isLandscape;
				bindingContext.IsMenuVisible = !isLandscape;
				bindingContext.MenuHeight = isLandscape ? 0 : 40;

				if (isLandscape)
				{
					if (savedHeader == null)
					{
						savedHeader = commentsListView.Header;
					}
					commentsListView.Footer = hiddenFooter;
					commentsListView.ItemsSource = null;
					commentsListView.IsPullToRefreshEnabled = false;

					Padding = 0;

					SetLayoutHeights(Height);
					AbsoluteLayout.SetLayoutBounds(Header, new Rectangle(0, 230, 1, 0));
				}
				else
				{
					if (savedHeader != null)
					{
						commentsListView.Header = savedHeader;
					}
					if (null != bindingContext.commentsItemsSource && bindingContext.commentsItemsSource.Count > 0)
					{
						commentsListView.ItemsSource = bindingContext.commentsItemsSource;
					}
					commentsListView.IsPullToRefreshEnabled = true;

					var padding = 0;
					switch (Device.RuntimePlatform)
					{
						case "iOS":
							padding = 20;
							break;

						default:
							break;
					}
					Padding = new Thickness(0, padding, 0, 0);

					var videoPlayerHeight = width / 16 * 9;
					SetLayoutHeights(videoPlayerHeight);
					AbsoluteLayout.SetLayoutBounds(Header, new Rectangle(0, videoPlayerHeight, 1, -1));
				}
			}
		}

		public void SetLayoutHeights(double videoHeight)
		{
			AbsoluteLayout.SetLayoutBounds(formsWebView, new Rectangle(0, 0, 1, videoHeight));
		}

		private void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
		{
			var bindingContext = BindingContext as VideoPageViewModel;
			bindingContext.CombinatingIsVisible = false;
			VideoPageViewModel.isReplyCommentField = false;
			VideoPageViewModel.isEditCommentVisible = false;
		}

		protected override void OnSizeAllocated(double width, double height)
		{
			base.OnSizeAllocated(width, height);
			this.width = width;
			this.height = height;
		}
	}
}