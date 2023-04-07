
namespace TerraLinkTestTask.Interfaces
{
    public interface IObservable
    {
        void AddObserver(IObserver o);
        void RemoveObserver(IObserver o);
    }
}
