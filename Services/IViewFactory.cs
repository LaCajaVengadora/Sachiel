
namespace Sachiel.Services
{
    public interface IViewFactory 
    { 
        T Create<T>() where T : class;
        T Create<T>(params object[] parameters) where T : class;
    }
}
