using WebFramework.MAUI;

namespace WebFramework.Test.MAUI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new WebFrameworkPage();
            IVApplication.Main(new string[0]);
        }
    }
}
