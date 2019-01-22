using System;
using System.Collections.Generic;
using System.Text;

namespace Trepub.Common.Interfaces
{
    public interface IProgressHandler
    {
        void OnProgress(int progressPercent, string progressMessage);
    }
}
