namespace WebFramework.MAUI;

public class WebFrameworkPage : ContentPage
{
	public static WebView Instance;

	public WebFrameworkPage()
	{
		Instance = new WebView();
        Content = Instance;


    }
}