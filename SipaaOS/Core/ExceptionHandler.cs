using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SipaaOS.Core
{
    public class ExceptionHandler
    {
        public static Action<Exception> SKBugCheckRaised = null;

        /// <summary>
        /// Ask SipaaKernel to show bug check screen
        /// </summary>
        /// <param name="ex"></param>
        public static void SKBugCheck(Exception ex)
        {
            if (SKBugCheckRaised != null)
                SKBugCheckRaised.Invoke(ex);
        }
    }
}
