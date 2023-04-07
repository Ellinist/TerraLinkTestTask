using TerraLinkTestTask.Models;

namespace TerraLinkTestTask.Interfaces
{
    public interface IObserver
    {
        void Update(List<DocumentInQueue> o, IExternalSystemConnector iface, CancellationToken token);
    }
}
