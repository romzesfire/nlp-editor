using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Medzoom.CDSS.DTO;
using NlpEditor.Utils;
using Smile;

namespace NlpEditor.Model
{
    public class GenieNetwork
    {
        public IEnumerable<NodeGenie> Nodes { get; set; }

        public GenieNetwork(Network net)
        {
            Nodes = GetAllGenieNodes(net);
        }

        private IEnumerable<NodeGenie>? GetAllGenieNodes(Network net)
        {
            var nodes = new List<NodeGenie>();
            foreach (var node in net.GetAllNodeIds())
            {
                if(CodesConverter.IsCode(node))
                    nodes.Add(new NodeGenie(node, net));
            }

            foreach (var node in nodes)
            {
                var nodeCode = CodesConverter.CodingToShort(node.Code);
                var parentsCode = net.GetParentIds(nodeCode);
                var parents = parentsCode == null || !parentsCode.Any() ? new List<NodeGenie>() : nodes
                    .Where(n=>n.Code == CodesConverter.ShortToCoding(parentsCode.First()));
                node.SetParents(parents);

                var childrenCode = net.GetCostChildIds(nodeCode);
                var children = childrenCode == null || !childrenCode.Any() ? new List<NodeGenie>() : nodes
                    .Where(n => n.Code == CodesConverter.ShortToCoding(childrenCode.First()));
                node.SetChildren(children);
            }

            return nodes;
        }
    }
    public class NodeGenie
    {
        public string Name { get; set; }
        public Coding Code { get; set; }
        public IEnumerable<State> States { get; set; }
        public IEnumerable<NodeGenie> Parents { get; private set; }
        public IEnumerable<NodeGenie> Children { get; private set; }
        public Network.NodeDiagType NodeType { get; set; }
        public NodeGenie(string code, Network net)
        {
            Code = CodesConverter.ShortToCoding(code);
            Name = net.GetNodeName(code);
            States = net.GetOutcomeIds(code).Select(s => new State(s));
            NodeType = net.GetNodeDiagType(code);
        }

        public void SetParents(IEnumerable<NodeGenie> parents)
        {
            Parents = parents;
        }
        public void SetChildren(IEnumerable<NodeGenie> children)
        {
            Children = children;
        }
    }

    public class State
    {
        public string FullName { get; set; }
        public Coding Code { get; set; }

        public State(string outcome)
        {
            FullName = outcome;
            Code = CodesConverter.ShortToCoding(outcome.Substring(outcome.LastIndexOf("_") + 1));
        }
    }
}
