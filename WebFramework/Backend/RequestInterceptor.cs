using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebFramework
{
    public class RequestInterceptor
    {
        public static List<Func<HttpListenerContext, bool>> Interceptors = new List<Func<HttpListenerContext, bool>>();

        public static void RegisterInterceptor(Func<HttpListenerContext, bool> interceptor)
        {
            Interceptors.Add(interceptor);
        }
    }
}
