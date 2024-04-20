using WebFramework.MAUI;

namespace WebFramework.Test.MAUI
{
    public partial class App : Application
    {
        public App()
        {
            try
            {
                //Throws On iOS Sometimes, But The App Can Still Continue Regardless
                InitializeComponent();
            }
            catch { }

            MainPage = new WebFrameworkPage();
            IVApplication.Main(new string[0]);
        }
    }
}
