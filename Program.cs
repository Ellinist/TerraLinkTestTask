using Ninject;
using TerraLinkTestTask.Infrastructure;

namespace TerraLinkTestTask
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Инициализация ядра DI-контейнера.
            IKernel kernel = new StandardKernel(new NinjectRegistration());
        }
    }
}