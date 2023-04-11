using Ninject.Modules;
using TerraLinkTestTask.Implementations;
using TerraLinkTestTask.Interfaces;

namespace TerraLinkTestTask.Infrastructure
{
    /// <summary>
    /// Класс регистрации зависимостей.
    /// </summary>
    public class NinjectRegistration : NinjectModule
    {
        public override void Load()
        {
            _ = Bind<IDocumentsQueue>().To<DocumentsQueue>().InSingletonScope();
            _ = Bind<IExternalSystemConnector>().To<ExternalSystemConnector>().InTransientScope();
        }
    }
}
