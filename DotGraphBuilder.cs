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

        public DotGraphEdgeBuilder AddEdge(string firstNode, string secondNode)
        {
            Graph.AddEdge(firstNode, secondNode);
            return new DotGraphEdgeBuilder { Graph = Graph };
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
        public NodedDotGraphBuilder With(Action<DotGraphNodeModifier> nodeAttributesSetter)
        {
            var graphModifier = new DotGraphNodeModifier { Graph = Graph };
            nodeAttributesSetter(graphModifier);
            return graphModifier;
        }
    }

    public class DotGraphNodeModifier : NodedDotGraphBuilder
    {
        private Dictionary<string, string> GetLastNodeAttributes(Graph graph)
        {
            var attributes = Graph?.Nodes?.Last()?.Attributes;
            if (attributes == null)
                throw new InvalidOperationException("There are no graph or nodes in the graph!");
            return attributes;
        }

        private DotGraphNodeModifier AddAttribute(string name, string value)
        {
            var attributes = GetLastNodeAttributes(Graph);
            attributes.Add(name, value);
            return this;
        }

        public DotGraphNodeModifier Color(string color) =>
            AddAttribute("color", color);

        public DotGraphNodeModifier FontSize(int fontSize) =>
            AddAttribute("fontsize", Convert.ToString(fontSize, CultureInfo.InvariantCulture));

        public DotGraphNodeModifier Label(string label) =>
            AddAttribute("label", label);

        public DotGraphNodeModifier Shape(NodeShape shape)
        {
            var shapeString = Convert
                .ToString(shape, CultureInfo.InvariantCulture)
                .ToLowerInvariant();
            AddAttribute("shape", shapeString);
            return this;
        }
    }

    public class DotGraphEdgeBuilder : DotGraphBuilder
    {
        private Dictionary<string, string> GetLastNodeAttributes(Graph graph)
        {
            var attributes = Graph?.Edges?.Last()?.Attributes;
            if (attributes == null)
                throw new InvalidOperationException("There are no graph or edges in the graph!");
            return attributes;
        }

        private DotGraphEdgeBuilder AddAttribute(string name, string value)
        {
            var attributes = GetLastNodeAttributes(Graph);
            attributes.Add(name, value);
            return this;
        }

        public DotGraphEdgeBuilder Color(string color) =>
             AddAttribute("color", color);

        public DotGraphEdgeBuilder FontSize(int fontSize) =>
            AddAttribute("fontsize", Convert.ToString(fontSize, CultureInfo.InvariantCulture));

        public DotGraphEdgeBuilder Label(string label) =>
            AddAttribute("label", label);

        public DotGraphEdgeBuilder Weight(double weight) =>
            AddAttribute("weight", Convert.ToString(weight, CultureInfo.InvariantCulture));

        public DotGraphBuilder With(Action<DotGraphEdgeBuilder> edgeAttributesSetter)
        {
            edgeAttributesSetter(this);
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