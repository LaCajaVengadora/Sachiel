using Microsoft.Extensions.DependencyInjection;

namespace Sachiel.Services { 
    public class ViewFactory(IServiceProvider serviceProvider) : IViewFactory 
    { 
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        public T Create<T>() => _serviceProvider.GetRequiredService<T>();
    }
}
