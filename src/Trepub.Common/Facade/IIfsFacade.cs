using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Trepub.Common.Interfaces;
using System.Collections.Generic;

namespace Trepub.IFS
{
    public interface IIfsFacade
    {
        IRepository<T> GetRepository<T>() where T : class;
        T GetRepositoryByInterface<T>() where T : class;
        IAppDbContext GetDbContext();
        ILogger<T> GetLogger<T>();
        IConfiguration GetConfigurationRoot();
        IExternalServices GetExternalServices();
        IDirectoryStorage GetDirectoryStorage();
        IScheduler GetScheduler();
    }
}
