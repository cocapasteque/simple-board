using System.Collections.Generic;

namespace Lisb.Utils.ServiceDiscovery
{
    public interface IServiceProvider
    {
        Service GetService(string ns, string name, string port);
        List<Service> GetServices(string ns, string name, string port);
    }
}