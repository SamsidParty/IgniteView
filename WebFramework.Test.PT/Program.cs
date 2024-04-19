
using WebFramework.PT;


public class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        PTWindowProvider.Activate();
        Application.Main(args);
    }
}