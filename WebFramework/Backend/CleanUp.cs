using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebFramework.Backend
{
    public class CleanUp
    {
        internal static List<Action> Actions = new List<Action>();

        public static void RegisterCleanUpAction(Action action)
        {
            Actions.Add(action);
        }

        public static async Task RunCleanUpActions()
        {
            Logger.LogInfo("Running Clean Up");
            foreach (var action in Actions)
            {
                await Task.Run(action);
            }
        }
    }
}
