using IgniteView.Dispatcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFramework
{
    public class IgniteDispatcher : ExecFunction
    {

        /// <summary>
        /// Fire & Forget, Starts A Method In A Seperate Process
        /// </summary>
        public static void RunWithoutReturn(Action action, Action<ExecFunctionOptions> configure = null) {

            Task.Run(() => RunAsync(action, configure: configure));
        }
    }
}
