using System;
using System.Collections.Generic;
using System.Text;

namespace Trepub.Common.Interfaces
{
   public  interface ISmsSender
    {
        void SendSMS(string number, string message);
    }
}
