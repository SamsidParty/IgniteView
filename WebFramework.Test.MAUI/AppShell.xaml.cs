namespace WebFramework.Test.MAUI
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            IVApplication.Main(new string[0]);
        }
    }
}
