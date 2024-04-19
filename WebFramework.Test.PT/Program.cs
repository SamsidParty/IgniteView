
using WebFramework.PT;


public class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        PTWindowProvider.Activate();
        IVApplication.Main(args);
    }
}