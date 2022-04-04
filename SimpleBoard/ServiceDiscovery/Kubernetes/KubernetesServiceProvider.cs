using System;
using System.Collections.Generic;
using System.Linq;
using DnsClient;

namespace Lisb.Utils.ServiceDiscovery.Kubernetes
{
    public class KubernetesServiceProvider : IServiceProvider
    {
        private readonly LookupClient _lookup;
        
        public KubernetesServiceProvider()
        {
            _lookup = new LookupClient(new LookupClientOptions {UseCache = true});
        }
        
        public Service GetService(string ns, string name, string port)
        {
            var services = GetServices(ns, name, port);
            return services.FirstOrDefault();
        }

        public List<Service> GetServices(string ns, string name, string port)
        {
            try
            {
                if (string.IsNullOrEmpty(port))
                {
                    return GetServicesARecord(ns, name, 0);
                }

                return int.TryParse(port, out var portInt)
                    ? GetServicesARecord(ns, name, portInt)
                    : GetServicesSrvRecord(ns, name, port);
            }
            catch (Exception e)
            {
                return new List<Service>();
            }
        }

        private List<Service> GetServicesARecord(string ns, string name, int port)
        {
            var domain = $"{name}.{ns}.svc.cluster.local";
            var dnsResponse = _lookup.Query(domain, QueryType.A);
            var records = dnsResponse.Answers.ARecords();
            return records.Select(record => new Service(record.Address.ToString(), port)).ToList();
        }

        private List<Service> GetServicesSrvRecord(string ns, string name, string port)
        {
            var domain = $"_{port}._tcp.{name}.{ns}.svc.cluster.local";
            var dnsResponse = _lookup.Query(domain, QueryType.SRV);
            var records = dnsResponse.Answers.SrvRecords();
            return records.Select(record => new Service(record.Target.Value, record.Port)).ToList();
        }
    }
}