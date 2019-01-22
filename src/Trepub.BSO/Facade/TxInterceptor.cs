using Castle.DynamicProxy;
using Trepub.Common.Exceptions;
using Trepub.Common.Facade;
using Trepub.Common.Interfaces;
using System;
using System.Linq;
using System.Threading;

namespace Trepub.Common.Facade
{
    public class TxInterceptor : IInterceptor
    {
        protected IAppDbContext dbContext;

        public TxInterceptor()
        {
            this.dbContext = FacadeProvider.IfsFacade.GetDbContext();
        }

        public void Intercept(IInvocation invocation)
        {
            if (invocation.Method.GetCustomAttributes(typeof(RequiresNewAttribute), true).Count() > 0)
            {
                Exception exception = null;
                Thread t = new Thread(() =>
                {
                    try
                    {
                        this.InterceptInternal(invocation);
                    }
                    catch (Exception exc)
                    {
                        exception = exc;
                    }
                });
                t.Start();
                t.Join();
                if(exception != null)
                {
                    if(exception is AppException)
                    {
                        throw new AppException(((AppException)exception).ErrorCode, ((AppException)exception).Message, exception);
                    }
                    else
                    {
                        throw new Exception(exception.Message, exception);
                    }
                }
                //var t = Task.Run(() => InterceptInternal(invocation));
                //t.Wait();
            }
            else
            {
                InterceptInternal(invocation);
            }
        }

        public void InterceptInternal(object invocation)
        {
            try
            {
                this.dbContext.BeginTransaction();
                ((IInvocation)invocation).Proceed();
                this.dbContext.CommitTransaction();
            }
            catch (AppException exc)
            {
                this.dbContext.RollbackTransaction();
                throw new AppException(exc.ErrorCode, exc.Message, exc.Object, exc);
            }
            catch(Exception exc)
            {
                this.dbContext.RollbackTransaction();
                throw new Exception(exc.Message, exc);
            }
        }


    }
}
