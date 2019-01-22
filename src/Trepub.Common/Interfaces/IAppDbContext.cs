using System;
using System.Collections.Generic;
using System.Text;

namespace Trepub.Common.Interfaces
{
    public interface IAppDbContext
    {
        void BeginTransaction();

        void CommitTransaction();

        void RollbackTransaction();

    }
}
