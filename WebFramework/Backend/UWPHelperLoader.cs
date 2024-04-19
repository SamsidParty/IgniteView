using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebFramework.Backend;

namespace WebFramework
{
    public class UWPHelperLoader
    {
        public static dynamic Current;


        //Dynamically Finds And Loads The UWPHelper Into "Current"
        public static void FindAndLoad()
        {
            Logger.LogInfo("Loading UWPHelper");
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                var possibleTypes = asm.GetTypes().Where(t => t.IsClass && t.Name == "UWPHelper");
                if (possibleTypes.Count() > 0)
                {
                    Current = Activator.CreateInstance(possibleTypes.First());
                    Current.OnLoad();
                }
            }
        }
    }
}
