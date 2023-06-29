using System;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Reflection;
using System.CodeDom;

namespace FluentApi.Graph
{
	public class DotGraphBuilder
	{
        public Graph Graph { get; set; }
		public static DotGraphBuilder DirectedGraph(string graphName)
		{
            var graphBuilder = new DotGraphBuilder();
            graphBuilder.Graph = new Graph(graphName, true, false);
            return graphBuilder;
		}

        public static DotGraphBuilder UndirectedGraph(string graphName)
        {
            var graphBuilder = new DotGraphBuilder();
            graphBuilder.Graph = new Graph(graphName, false, true);
            return graphBuilder;
        }

        internal DotGraphBuilder AddEdge(string firstNode, string secondNode)
        {
            throw new NotImplementedException();
        }

        internal DotGraphBuilder AddNode(string nodeName)
        {
            Graph.AddNode(nodeName);
            return this;
        }

        internal string Build()
        {
            return Graph.ToDotFormat();
        }

        public DotGraphBuilder With(Func<NodeAttributes, NodeAttributes> nodeAttributesSetter)
        {
            var attributes = nodeAttributesSetter(new NodeAttributes());
            var latestNode = Graph?.Nodes.LastOrDefault();
            if (latestNode == null)
                throw new InvalidOperationException("There are no graph or nodes in the graph!");
            var nodeAttributesFields = typeof(NodeAttributes).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var nodeAttributesField in nodeAttributesFields)
            {
                var value = nodeAttributesField.GetValue(attributes);
                if (value == null)
                    continue;
                var valueType = value.GetType();
                if (valueType.IsValueType)
                {
                    var defaultValue = Activator.CreateInstance(valueType);
                    if (value.Equals(defaultValue))
                        continue;
                }
                var key = nodeAttributesField.Name.ToLower();
                latestNode.Attributes.Add(key, value.ToString().ToLower());
            }
            return this;
        }
    }

    public class NodeAttributes
    {
        private string color;
        private int fontsize;
        private string label;
        private NodeShape shape;

        public NodeAttributes Color(string color)
        {
            this.color = color;
            return this;
        }

        public NodeAttributes FontSize(int fontSize)
        {
            if (fontSize < 0)
                throw new ArgumentException($"font size of {fontSize} is less than 0!");
            this.fontsize = fontSize;
            return this;
        }

        public NodeAttributes Label(string label)
        {
            this.label = label; 
            return this;    
        }

        public NodeAttributes Shape(NodeShape shape)
        {
            this.shape = shape;
            return this;
        }
    }

    public enum NodeShape
    {
        None,
        Box,
        Ellipse
    }
}