using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;
using Ninject;
using GlogowskiBus.DAL.Abstract;
using GlogowskiBus.DAL.Concrete;
using GlogowskiBus.BLL.Abstract;
using GlogowskiBus.BLL.Concrete;
using Ninject.Web.Common;

namespace GlogowskiBus.UI.Infrastructure
{
    public class NinjectResolver : System.Web.Http.Dependencies.IDependencyResolver, System.Web.Mvc.IDependencyResolver
    {
        private IKernel kernel;

        public NinjectResolver() : this(new StandardKernel())
        {
        }

        public NinjectResolver(IKernel ninjectKernel)
        {
            kernel = ninjectKernel;
            AddBindings(kernel);
        }
        public IDependencyScope BeginScope()
        {
            return this;
        }
        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }
        public void Dispose()
        {
            // do nothing
        }
        private void AddBindings(IKernel kernel)
        {
            kernel.Bind<IUnitOfWork>().To<UnitOfWork>().InRequestScope();
            kernel.Bind<IBusService>().To<BusService>().InRequestScope();
            kernel.Bind<IBusStopService>().To<BusStopService>().InRequestScope();
            kernel.Bind<IBusLineService>().To<BusLineService>().InRequestScope();
        }
    }
}