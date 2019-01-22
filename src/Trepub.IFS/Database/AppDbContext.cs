using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Trepub.Common.Facade;
using Trepub.Common.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Data;
//using System.Data.SqlClient;
using System.Threading;
using MySql.Data.MySqlClient;

namespace Trepub.IFS.Database
{
    public class AppDbContext : IAppDbContext
    {
        protected ConcurrentDictionary<int, IDbConnection> dbConnections;
        protected ConcurrentDictionary<int, IDbTransaction> txs;
        protected ConcurrentDictionary<int, int> txCount;
        protected string connectionString;
        protected IConfiguration configuration;
        protected static AppDbContext instace;
        protected ILogger<AppDbContext> logger;

        public AppDbContext()
        {
            this.configuration = FacadeProvider.IfsFacade.GetConfigurationRoot();
            //this.connectionString = configuration.GetConnectionString("TrepubConnection");
            this.connectionString = configuration.GetConnectionString("mysql");
            this.dbConnections = new ConcurrentDictionary<int, IDbConnection>();
            this.txs = new ConcurrentDictionary<int, IDbTransaction>();
            this.txCount = new ConcurrentDictionary<int, int>();
            this.logger = FacadeProvider.IfsFacade.GetLogger<AppDbContext>();
        }

        public static AppDbContext Instance
        {
            get
            {
                if(instace == null)
                {
                    instace = new AppDbContext();
                }
                return instace;
            }
        }

        public void BeginTransaction()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            if (!dbConnections.ContainsKey(threadId))
            {
                //var dbConnection = new SqlConnection(connectionString);
                var dbConnection = new MySqlConnection(connectionString);
                dbConnection.Open();
                var tx = dbConnection.BeginTransaction(IsolationLevel.ReadUncommitted);
                if(!dbConnections.TryAdd(threadId, dbConnection))
                {
                    throw new Exception($"failed to add connection during begin transaction operation; threadId={threadId}");
                }

                if(!txs.TryAdd(threadId, tx))
                {
                    throw new Exception($"failed to add transaction during begin transaction operation; threadId={threadId}");
                }

                if (!txCount.TryAdd(threadId, 1))
                {
                    throw new Exception($"failed to add count index during begin transaction operation; threadId={threadId}" );
                }
                if (logger.IsEnabled(LogLevel.Trace)){
                    logger.LogTrace($"ApDBContext: **** TRANSACTION STARTED **** THREAD ID: {threadId} ,  ConnectionId: {dbConnection.GetHashCode()} **** ");
                }
            }
            else
            {
                txCount[threadId] += 1;
                if (logger.IsEnabled(LogLevel.Trace)){
                    logger.LogTrace($"ApDBContext: ++++ TRANSACTION COUNT UP THREAD ID: {threadId}, COUNT: {txCount[threadId]} ++++");
                }
            }
        }

        public void CommitTransaction()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            if (txCount[threadId] <= 1)
            {
                //transaction 
                var tx = txs[threadId];
                tx.Commit();
                if(!txs.TryRemove(threadId, out tx))
                {
                    throw new Exception($"failed to remove transaction during commit operation; threadId={threadId}");
                }

                //connection 
                var c = dbConnections[threadId];
                c.Close();
                if(!dbConnections.TryRemove(threadId, out c))
                {
                    throw new Exception($"failed to remove connection during commit operation; threadId={threadId}");
                }

                //count 
                int count;
                if(!txCount.TryRemove(threadId, out count))
                {
                    throw new Exception($"failed to remove count index during commit operation; threadId={threadId}");
                }
                if (logger.IsEnabled(LogLevel.Trace)){
                    logger.LogTrace($"ApDBContext: **** TRANSACTION COMMITTED **** THREAD ID: {threadId},  ConnectionId: {c.GetHashCode()} **** ");
                }
            }
            else
            {
                txCount[threadId] -= 1;
                if (logger.IsEnabled(LogLevel.Trace)){
                    logger.LogTrace($"ApDBContext: ---- TRANSACTION COUNT DOWN THREAD ID: {threadId}, COUNT: {txCount[threadId]} ----");
                }
            }

        }

        public void RollbackTransaction()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            if (txCount[threadId] <= 1)
            {
                //transaction 
                var tx = txs[threadId];
                tx.Rollback();
                if(!txs.TryRemove(threadId, out tx))
                {
                    throw new Exception($"failed to remove transaction during rollback operation; threadId={threadId}");
                }

                //connection 
                var c = dbConnections[threadId];
                c.Close();
                if(!dbConnections.TryRemove(threadId, out c))
                {
                    throw new Exception($"failed to remove connection during rollback operation; threadId={threadId}");
                }

                //count 
                int count;
                if(!txCount.TryRemove(threadId, out count))
                {
                    throw new Exception($"failed to remove count index during rollback operation; threadId={threadId}");
                }

                if (logger.IsEnabled(LogLevel.Trace)){
                    logger.LogTrace($"ApDBContext: **** TRANSACTION ROLLEDBACK **** THREAD ID: {threadId},  ConnectionId: {c.GetHashCode()} **** ");
                }

            }
            else
            {
                txCount[threadId] -= 1;
            }
        }

        public IDbTransaction Transaction
        {
            get
            {
                return txs[Thread.CurrentThread.ManagedThreadId];
            }
        }

        public IDbConnection Connection
        {
            get
            {
                return dbConnections[Thread.CurrentThread.ManagedThreadId];
            }
        }

    }
}
