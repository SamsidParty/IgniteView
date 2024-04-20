using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebFramework.Backend;

namespace WebFramework
{
    public class WinHelperLoader
    {
        public static dynamic Current;


        //Dynamically Finds And Loads The WinHelper Into "Current"
        public static void FindAndLoad()
        {
            Logger.LogInfo("Loading WinHelper");
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                var possibleTypes = asm.GetTypes().Where(t => t.IsClass && t.Name == "WinHelper");
                if (possibleTypes.Count() > 0)
                {
                    Current = Activator.CreateInstance(possibleTypes.First());
                    Current.OnLoad();
                }
            }
        }
    }
}
