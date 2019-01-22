using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Trepub.Common.Interfaces;
using Trepub.IFS.Database;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Trepub.IFS.Services;
using Trepub.IFS.FileSystem;
using Trepub.Common;

namespace Trepub.IFS
{
    public class IfsFacade : IIfsFacade
    {

        protected ILoggerFactory loggerFactory;
        protected IConfiguration configurationRoot;
        protected IExternalServices externalServices;
        protected IDirectoryStorage directoryStorage;
        protected static IfsFacade instance;
        protected IScheduler scheduler;

        public static IfsFacade Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new IfsFacade();
                }
                return instance;
            }
        }

        public IRepository<T> GetRepository<T>() where T : class
        {
            return AppRepositoryContext.GetRepo<T>();
        }

        public T GetRepositoryByInterface<T>() where T : class
        {
            var implType = this.GetType().Assembly.GetType("Trepub.IFS.Data." + typeof(T).Name.Substring(1));
            return (T)Activator.CreateInstance(implType);
        }

        public IAppDbContext GetDbContext()
        {
            return AppDbContext.Instance;
        }

        public ILogger<T> GetLogger<T>()
        {
            return this.loggerFactory.CreateLogger<T>();
        }

        public void SetILoggerFactory(ILoggerFactory f)
        {
            this.loggerFactory = f;
        }

        public IConfiguration GetConfigurationRoot()
        {
            return this.configurationRoot;
        }

        public void SetConfigurationRoot(IConfiguration cf)
        {
            this.configurationRoot = cf;
        }

        public IExternalServices GetExternalServices()
        {
            if (externalServices == null)
            {
                externalServices = new ExternalServices();
            }
            return externalServices;
        }

        public IDirectoryStorage GetDirectoryStorage()
        {
            if (this.directoryStorage == null)
            {
                this.directoryStorage = new DirectoryStorage();
            }
            return directoryStorage;
        }

        public IScheduler GetScheduler()
        {
            if (this.scheduler == null)
            {
                this.scheduler = new Trepub.IFS.Scheduler.Scheduler();
            }
            return this.scheduler;
        }
    }
}
