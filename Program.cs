using Ninject;
using TerraLinkTestTask.Infrastructure;

namespace TerraLinkTestTask
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IKernel kernel = new StandardKernel(new NinjectRegistration());
        }
    }
}