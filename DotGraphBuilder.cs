using System;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Reflection;
using System.CodeDom;
using System.Collections.Generic;

namespace FluentApi.Graph
{
	public class DotGraphBuilder
	{
        private protected Graph Graph { get; set; }
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

        public EdgedDotGraphBuilder AddEdge(string firstNode, string secondNode)
        {
            Graph.AddEdge(firstNode, secondNode);
            return new EdgedDotGraphBuilder { Graph = Graph };
        }

        public NodedDotGraphBuilder AddNode(string nodeName)
        {
            Graph.AddNode(nodeName);
            return new NodedDotGraphBuilder { Graph = Graph };
        }

        public string Build()
        {
            return Graph.ToDotFormat();
        }
    }

    public class NodedDotGraphBuilder : DotGraphBuilder
    {
        public DotGraphBuilder With(Action<DotGraphNodeAttributesSetter> nodeAttributesChanges)
        {
            var lastNode = Graph?.Nodes?.Last();
            var nodeAttributesSetter = new DotGraphNodeAttributesSetter(lastNode);
            nodeAttributesChanges(nodeAttributesSetter);
            return this;
        }
    }

    public class DotGraphNodeAttributesSetter
    {
        private Dictionary<string, string> Attributes { get; }
        public DotGraphNodeAttributesSetter(GraphNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));
            Attributes = node.Attributes;
        }

        private DotGraphNodeAttributesSetter AddAttribute(string name, string value)
        {
            Attributes.Add(name, value);
            return this;
        }

        public DotGraphNodeAttributesSetter Color(string color) =>
            AddAttribute("color", color);

        public DotGraphNodeAttributesSetter FontSize(int fontSize) =>
            AddAttribute("fontsize", Convert.ToString(fontSize, CultureInfo.InvariantCulture));

        public DotGraphNodeAttributesSetter Label(string label) =>
            AddAttribute("label", label);

        public DotGraphNodeAttributesSetter Shape(NodeShape shape)
        {
            var shapeString = Convert
                .ToString(shape, CultureInfo.InvariantCulture)
                .ToLowerInvariant();
            AddAttribute("shape", shapeString);
            return this;
        }
    }

    public class EdgedDotGraphBuilder : DotGraphBuilder
    {
        public DotGraphBuilder With(Action<DotGraphEdgeAttributesSetter> edgeAttributesChanges)
        {
            var lastEdge = Graph?.Edges?.Last();
            var edgeAttributetesSetter = new DotGraphEdgeAttributesSetter(lastEdge);
            edgeAttributesChanges(edgeAttributetesSetter);
            return this;
        }
    }

    public class DotGraphEdgeAttributesSetter
    {
        private Dictionary<string, string> Attributes { get; }
        public DotGraphEdgeAttributesSetter(GraphEdge graphEdge)
        {
            if (graphEdge == null)
                throw new ArgumentNullException(nameof(graphEdge));
            Attributes = graphEdge.Attributes;
        }

        private DotGraphEdgeAttributesSetter AddAttribute(string name, string value)
        {
            Attributes.Add(name, value);
            return this;
        }

        public DotGraphEdgeAttributesSetter Color(string color) =>
             AddAttribute("color", color);

        public DotGraphEdgeAttributesSetter FontSize(int fontSize) =>
            AddAttribute("fontsize", Convert.ToString(fontSize, CultureInfo.InvariantCulture));

        public DotGraphEdgeAttributesSetter Label(string label) =>
            AddAttribute("label", label);

        public DotGraphEdgeAttributesSetter Weight(double weight) =>
            AddAttribute("weight", Convert.ToString(weight, CultureInfo.InvariantCulture));
    }

    public enum NodeShape
    {
        None,
        Box,
        Ellipse
    }
}