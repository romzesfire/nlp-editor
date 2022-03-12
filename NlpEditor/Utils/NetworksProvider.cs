using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NlpEditor.Configuration;
using NlpEditor.Model;
using Smile;

namespace NlpEditor.Utils
{
    public class NetworksProvider : INetworkProvider
    {
        private GenieNetwork _yaml { get; set; }
        private IEnumerable<GenieNetwork> _networks { get; set; }
        private GenieConfiguration _configuration { get; set; }
        public NetworksProvider(IOptions<GenieConfiguration> config)
        {
            _configuration = config.Value;
            var license = JsonConvert.DeserializeObject<LicenseGenie>(File.ReadAllText(_configuration.LicenseFile));
            new Smile.License(license.Line, license.LicenseKey);
        }

        public void LoadYaml(string path)
        {
            var net = new Network();
            net.ReadFile(path);
            _yaml = new GenieNetwork(net);
        }

        public void LoadNetworks(IEnumerable<string> paths)
        {
            var nets = new List<GenieNetwork>();
            foreach (var path in paths)
            {
                var net = new Network();
                net.ReadFile(path);
                nets.Add(new GenieNetwork(net));
            }

            _networks = nets;
        }

        public GenieNetwork GetYaml() => _yaml;
        public IEnumerable<GenieNetwork> GetNetworks() => _networks;

        public IEnumerable<NodeGenie> GetPotentiallyActiveNodes()
        {
            if (_yaml != null && _networks != null)
            {
                var allNodes = _networks.SelectMany(n => n.Nodes);
                var observations = allNodes.Where(n => n.NodeType == Network.NodeDiagType.Observation);

                var potentialyActive = new List<NodeGenie>();
                foreach (var observation in observations)
                {
                    var observationInYaml = _yaml.Nodes.FirstOrDefault(o => o.Code == observation.Code);
                    
                    if (observationInYaml != null)
                    {
                        if(potentialyActive.Find(o=>o.Code == observationInYaml.Code) == null)
                            potentialyActive.Add(observationInYaml);

                        var parent = observationInYaml.Parents.FirstOrDefault();
                        while (parent != null)
                        {
                            if (potentialyActive.Find(o => o.Code == parent.Code) == null)
                                potentialyActive.Add(parent);
                            parent = parent.Parents.FirstOrDefault();
                        }
                    }
                }

                return potentialyActive;
            }
            return new List<NodeGenie>();
        }
    }

    public interface INetworkProvider
    {
        public IEnumerable<NodeGenie> GetPotentiallyActiveNodes();
        public void LoadYaml(string path);
        public void LoadNetworks(IEnumerable<string> paths);
        public GenieNetwork GetYaml();
        public IEnumerable<GenieNetwork> GetNetworks();
    }
}
