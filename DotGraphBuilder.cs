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
        public DotGraphBuilder With(Action<DotGraphNodeAttributesBuilder> nodeAttributesSetter)
        {
            var lastNode = Graph?.Nodes?.Last();
            var nodeAttributesBuilder = new DotGraphNodeAttributesBuilder(lastNode);
            nodeAttributesSetter(nodeAttributesBuilder);
            return this;
        }
    }

    public class DotGraphNodeAttributesBuilder
    {
        private Dictionary<string, string> Attributes { get; }
        public DotGraphNodeAttributesBuilder(GraphNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));
            Attributes = node.Attributes;
        }

        private DotGraphNodeAttributesBuilder AddAttribute(string name, string value)
        {
            Attributes.Add(name, value);
            return this;
        }

        public DotGraphNodeAttributesBuilder Color(string color) =>
            AddAttribute("color", color);

        public DotGraphNodeAttributesBuilder FontSize(int fontSize) =>
            AddAttribute("fontsize", Convert.ToString(fontSize, CultureInfo.InvariantCulture));

        public DotGraphNodeAttributesBuilder Label(string label) =>
            AddAttribute("label", label);

        public DotGraphNodeAttributesBuilder Shape(NodeShape shape)
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
        public DotGraphBuilder With(Action<DotGraphEdgeAttributesBuilder> edgeAttributesSetter)
        {
            var lastEdge = Graph?.Edges?.Last();
            var graphEdgeAttributesBuilder = new DotGraphEdgeAttributesBuilder(lastEdge);
            edgeAttributesSetter(graphEdgeAttributesBuilder);
            return this;
        }
    }

    public class DotGraphEdgeAttributesBuilder
    {
        private Dictionary<string, string> Attributes { get; }
        public DotGraphEdgeAttributesBuilder(GraphEdge graphEdge)
        {
            if (graphEdge == null)
                throw new ArgumentNullException(nameof(graphEdge));
            Attributes = graphEdge.Attributes;
        }

        private DotGraphEdgeAttributesBuilder AddAttribute(string name, string value)
        {
            Attributes.Add(name, value);
            return this;
        }

        public DotGraphEdgeAttributesBuilder Color(string color) =>
             AddAttribute("color", color);

        public DotGraphEdgeAttributesBuilder FontSize(int fontSize) =>
            AddAttribute("fontsize", Convert.ToString(fontSize, CultureInfo.InvariantCulture));

        public DotGraphEdgeAttributesBuilder Label(string label) =>
            AddAttribute("label", label);

        public DotGraphEdgeAttributesBuilder Weight(double weight) =>
            AddAttribute("weight", Convert.ToString(weight, CultureInfo.InvariantCulture));
    }

    public enum NodeShape
    {
        None,
        Box,
        Ellipse
    }
}