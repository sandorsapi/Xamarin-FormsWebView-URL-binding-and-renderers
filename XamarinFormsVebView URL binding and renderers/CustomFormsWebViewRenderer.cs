[assembly: ExportRenderer(typeof(CustomFormsWebView), typeof(CustomFormsWebViewRenderer))]

namespace XY_App.Droid.Renderers
{
	public class CustomFormsWebViewRenderer : FormsWebViewRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<FormsWebView> e)
		{
			base.OnElementChanged(e);

			if (Control != null)
			{
				Control.Settings.MediaPlaybackRequiresUserGesture = false;
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName.Equals("Source"))
			{
				var webView = Control as Android.Webkit.WebView;
				var customFormsSender = sender as CustomFormsWebView;
				webView.SetLayerType(LayerType.Hardware, new Paint());
				webView.Settings.LoadWithOverviewMode = true;
				webView.Settings.UseWideViewPort = true;
				webView.LoadUrl(customFormsSender.Source);
			}
		}
	}
}