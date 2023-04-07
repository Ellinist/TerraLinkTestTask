using Ninject.Modules;
using TerraLinkTestTask.Implementations;
using TerraLinkTestTask.Interfaces;
using TerraLinkTestTask.Models;

namespace TerraLinkTestTask.Infrastructure
{
    public class NinjectRegistration : NinjectModule
    {
        public override void Load()
        {
            _ = Bind<DocumentInQueue>().ToSelf().InSingletonScope();

            _ = Bind<IDocumentsQueue>().To<DocumentsQueue>().InTransientScope();
            _ = Bind<IExternalSystemConnector>().To<ExternalSystemConnector>().InTransientScope();
        }
    }
}
