using System;
using System.Collections.Generic;
using System.Linq;
using Lisb.Utils.ServiceDiscovery.Kubernetes;

namespace Lisb.Utils.ServiceDiscovery
{
    public class Service
    {
        public static string ServiceDiscoveryType => Environment.GetEnvironmentVariable("SERVICE_DISCOVERY") ?? "kubernetes";
        public static IServiceProvider Provider = NewServiceProvider();
        
        public string Host { get; private set; }
        public int Port { get; private set; }
        
        public Service(string host, int port)
        {
            Host = host;
            Port = port;
        }

        public static Service GetService(string ns, string name, string port)
        {
            return Provider.GetService(ns, name, port);
        }

        public static List<Service> GetServices(string ns, string name, string port)
        {
            return Provider.GetServices(ns, name, port);
        }
        
        public static string GetServiceUrl(string prefix, string ns, string name, string port, string endpoint = null)
        {
            var path = GetService(ns, name, port).ToString();
            
            if (path == null) return null;


            if (prefix != null && !path.ToLower().StartsWith(prefix.ToLower()))
                path = prefix + path;

            if (endpoint != null)
            {
                if (path.Last() == '/')
                    path = path.Substring(0, path.Length - 1);

                if (endpoint[0] != '/')
                    path += "/";

                path += endpoint;
            }

            return path;
        }
        
        private static IServiceProvider NewServiceProvider()
        {
            if (ServiceDiscoveryType.Equals("kubernetes", StringComparison.InvariantCultureIgnoreCase))
            {
                return new KubernetesServiceProvider();
            }

            return null;
        }

        public override string ToString()
        {
            return $"{Host}:{Port}";
        }
    }
}