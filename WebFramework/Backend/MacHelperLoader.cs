using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebFramework.Backend;

namespace WebFramework
{
    public class MacHelperLoader
    {
        public static dynamic Current;


        //Dynamically Finds And Loads The MacHelper Into "Current"
        public static void FindAndLoad()
        {
            Logger.LogInfo("Loading MacHelper");
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                var possibleTypes = asm.GetTypes().Where(t => t.IsClass && t.Name == "MacHelper");
                if (possibleTypes.Count() > 0)
                {
                    Current = Activator.CreateInstance(possibleTypes.First());
                    Current.OnLoad();
                }
            }
        }
    }
}
