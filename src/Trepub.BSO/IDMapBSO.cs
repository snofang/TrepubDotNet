using Microsoft.Extensions.Logging;
using Trepub.Common.Facade;
using Trepub.Common.Entities;
using Trepub.Common.Interfaces;
using Trepub.Common.Utils;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Trepub.BSO
{
    public class IDMapBSO
    {
        private readonly IRepository<Idmap> repo;
        private Dictionary<string, int> currentIds;
        private Dictionary<string, Idmap> idmaps;
        private const int idOffset = 100;
        protected readonly string DATETIME_FORMAT = "yyMMdd";
        protected ILogger<IDMapBSO> logger;

        public IDMapBSO()
        {
            this.repo = FacadeProvider.IfsFacade.GetRepository<Idmap>();
            this.logger = FacadeProvider.IfsFacade.GetLogger<IDMapBSO>();
        }

        /// <summary>
        /// Gets next sequential id for the specified key.
        /// </summary>
        /// <param name="IdKey"></param>
        /// <returns></returns>
        public int GetNextID(string idKey)
        {
            lock (this)
            {
                //fetching cache if not done already
                if (currentIds == null)
                {
                    FillCache();
                }

                if (idmaps.ContainsKey(idKey))
                {
                    if (currentIds[idKey] >= idmaps[idKey].Idvalue)
                    {
                        //getting next id range
                        idmaps[idKey].Idvalue += idOffset;
                        UpdateIdmap(idmaps[idKey]);
                    }
                }
                else
                {
                    //adding new entry if not exist any
                    var newIdmap = new Idmap();
                    newIdmap.Idkey = idKey;
                    newIdmap.Idvalue = idOffset*2;
                    AddIdmap(newIdmap);
                    idmaps.Add(idKey, newIdmap);
                    currentIds.Add(idKey, idOffset);
                }
                currentIds[idKey]++;
                return currentIds[idKey];
            }
        }

        public string GetNextIDStr(string idKey)
        {
            return DateTime.Now.ToString(DATETIME_FORMAT) + GetNextID(idKey);
        }

        [RequiresNew]
        public virtual void AddIdmap(Idmap idmap)
        {
            this.repo.Add(idmap);
            if (logger.IsEnabled(LogLevel.Trace))
            {
                logger.LogTrace($"Idmap Added;  ThreadId: {Thread.CurrentThread.ManagedThreadId}, IdMap:\n {ObjectDumper.Dump(idmap)}");
            }
        }

        [RequiresNew]
        public virtual void UpdateIdmap(Idmap idmap)
        {
            this.repo.Update(idmap);
            if (logger.IsEnabled(LogLevel.Trace))
            {
                logger.LogTrace($"Idmap Updated;  ThreadId: {Thread.CurrentThread.ManagedThreadId}, IdMap:\n {ObjectDumper.Dump(idmap)}");
            }
        }
        private void FillCache()
        {
            this.currentIds = new Dictionary<string, int>();
            this.idmaps = new Dictionary<string, Idmap>();
            var list = this.repo.List();
            foreach (var i in list)
            {
                idmaps.Add(i.Idkey, i);
                currentIds.Add(i.Idkey, i.Idvalue);
            }
        }
    }
}
