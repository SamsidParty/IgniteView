namespace WebFramework.MAUI;

public class WebFrameworkPage : ContentPage
{
	public static WebView Instance;
	public static WebFrameworkPage Page;

	public WebFrameworkPage()
	{
		Instance = new WebView();
        Content = Instance;
		Page = this;
    }
}