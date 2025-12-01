using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HrgWeb.Business.WorkflowEngine.Service
{
    public static class DependencyResolver
    {
        private static IUnityContainer _container;
        public static IUnityContainer Container
        {
            get
            {
                if (_container == null)
                {
                    _container = new UnityContainer();
                }
                return _container;
            }
        }
    }
}
