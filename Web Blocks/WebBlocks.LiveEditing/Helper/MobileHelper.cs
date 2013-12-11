using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBlocks.LiveEditing.Helper
{
    public static class MobileHelper
    {
        public static bool IsMobile(HttpRequest request)
        {
            return request.UserAgent.Contains("BlackBerry") || 
                (request.UserAgent.Contains("iPhone") || 
                (request.UserAgent.Contains("Android")));
        }
    }
}