using System;
using LendingLibrary.Utils;
using Microsoft.Practices.Unity.Configuration;
using Unity;

namespace LendingLibrary
{
    public static class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
              RegisterTypes(container);
              return container;
          });

        public static IUnityContainer Container => container.Value;
        #endregion

        public static void RegisterTypes(IUnityContainer container)
        {
            container.LoadConfiguration();
        }
    }
}