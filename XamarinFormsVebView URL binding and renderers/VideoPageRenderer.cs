[assembly: ExportRenderer(typeof(VideoPage), typeof(VideoPageRenderer))]

namespace XY_App.Droid.Renderers
{
	public class VideoPageRenderer : PageRenderer
	{
		private double previousWidth;
		private double previousHeight;
		private bool inited;

		private int _originalUIOptions = int.MinValue;

		protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
		{
			base.OnElementChanged(e);

			VideoPage.FullScreenEvent += new VideoPageDelegateEventHandler(FullScreen);

			if (Element != null && !inited)
			{
				Element.SizeChanged += Element_SizeChanged;

				Element.Appearing += (object sender, System.EventArgs args) => ((MainActivity)Context).RequestedOrientation = ScreenOrientation.FullSensor;
				Element.Disappearing += (object sender, System.EventArgs args) => ((MainActivity)Context).RequestedOrientation = ScreenOrientation.Portrait;

				inited = true;
			}
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == VisualElement.HeightProperty.PropertyName && !Element.Height.Equals(previousHeight))
			{
				var element = Element as VideoPage;
				var bc = element?.BindingContext as VideoPageViewModel;
				if (bc != null)
				{
					var height = ((element.Height < element.Width) && bc.IsFullScreen) ? element.Height : element.Width / 16 * 9;
					element.SetLayoutHeights(height);
				}
			}
		}
		
		private void Element_SizeChanged(object sender, System.EventArgs e)
		{
			bool isLandscape = false;

			if (!previousWidth.Equals(Element.Width) || !previousHeight.Equals(Element.Height))
			{
				previousWidth = Element.Width;
				previousHeight = Element.Height;

				isLandscape = Element.Width > Element.Height;

				var activity = (Activity)Forms.Context;

				var decorView = activity.Window.DecorView;
				var uiOptions = (int)decorView.SystemUiVisibility;
				if (_originalUIOptions == int.MinValue)
				{
					_originalUIOptions = uiOptions;
				}

				int newUiOptions;
				if (isLandscape)
				{
					newUiOptions = _originalUIOptions;

					newUiOptions |= (int)SystemUiFlags.LowProfile;
					newUiOptions |= (int)SystemUiFlags.Fullscreen;
					newUiOptions |= (int)SystemUiFlags.HideNavigation;
					newUiOptions |= (int)SystemUiFlags.Immersive;
				}
				else
				{
					newUiOptions = _originalUIOptions;
				}

				decorView.SystemUiVisibility = (StatusBarVisibility)newUiOptions;

				var element = Element as VideoPage;
				if (element != null)
				{
					element.SetLayout(isLandscape);
				}
			}
		}

		private void FullScreen(bool commandString)
		{
			try
			{
				if (commandString)
				{
					((MainActivity)Context).RequestedOrientation = ScreenOrientation.Landscape;
				}
				else
				{
					((MainActivity)Context).RequestedOrientation = ScreenOrientation.Portrait;
				}
			}
			catch (Exception ex)
			{
				return;
			}
			
		}
	}
}