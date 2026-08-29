using Microsoft.Extensions.DependencyInjection;

namespace Sachiel.Services { 
    public class ViewFactory(IServiceProvider serviceProvider) : IViewFactory 
    { 
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        public T Create<T>() where T : class => _serviceProvider.GetRequiredService<T>();
        public T Create<T>(params object[] parameters) where T : class 
            => ActivatorUtilities.CreateInstance<T>(_serviceProvider, parameters);
    }
}
