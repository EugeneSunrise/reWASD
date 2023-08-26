using System;
using System.Net;
using reWASDUI;

namespace reWASDDownloader
{
    public class DSWebClient : WebClient
    {
        public static int DEFAULT_TIMEOUT = 600000;

        protected override WebRequest GetWebRequest(Uri address)
        {
            //IL_0006: Unknown result type (might be due to invalid IL or missing references)
            //IL_0010: Unknown result type (might be due to invalid IL or missing references)
            try
            {
                ServicePointManager.set_Expect100Continue(true);
                ServicePointManager.set_SecurityProtocol((SecurityProtocolType)(ServicePointManager.get_SecurityProtocol() | 0xFC0));
            }
            catch (Exception)
            {
            }
            HttpWebRequest obj = base.GetWebRequest(address) as HttpWebRequest;
            obj.Proxy = Program.DefaultWebProxy;
            obj.UserAgent = Program.PRODUCT_USERAGENT;
            obj.Timeout = DEFAULT_TIMEOUT;
            return obj;
        }
    }
}
