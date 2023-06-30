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
        private protected DotGraphBuilder() { }

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
            return new EdgedDotGraphBuilder(Graph);
        }

        public NodedDotGraphBuilder AddNode(string nodeName)
        {
            Graph.AddNode(nodeName);
            return new NodedDotGraphBuilder(Graph);
        }

        public string Build()
        {
            return Graph.ToDotFormat();
        }
    }

    public class NodedDotGraphBuilder : DotGraphBuilder
    {
        public NodedDotGraphBuilder(Graph graph)
        {
            if (graph == null)
                throw new ArgumentNullException($"{nameof(graph)} can not be null");
            if (graph.Nodes == null || graph.Nodes.Count() < 1)
                throw new ArgumentException($"{nameof(graph)} must contain at least one edge!");
            Graph = graph;
        }

        public DotGraphBuilder With(Action<LastNodeAttributesSetter> nodeAttributesChanges)
        {
            var nodeAttributesSetter = new LastNodeAttributesSetter(Graph);
            nodeAttributesChanges(nodeAttributesSetter);
            return this;
        }
    }

    public class EdgedDotGraphBuilder : DotGraphBuilder
    {
        public EdgedDotGraphBuilder(Graph graph)
        {
            if (graph == null)
                throw new ArgumentNullException($"{nameof(graph)} can not be null");
            if (graph.Edges == null || graph.Edges.Count() < 1)
                throw new ArgumentException($"{nameof(graph)} must contain at least one edge!");
            Graph = graph;
        }

        public DotGraphBuilder With(Action<LastEdgeAttributesSetter> edgeAttributesChanges)
        {
            var edgeAttributetesSetter = new LastEdgeAttributesSetter(Graph);
            edgeAttributesChanges(edgeAttributetesSetter);
            return this;
        }
    }

    public abstract class DotGraphAttributesSetter<TSetter>
        where TSetter : class
    {
        private protected Graph Graph { get; }
        private protected abstract Dictionary<string, string> Attributes { get; }
        private protected DotGraphAttributesSetter(Graph graph)
        {
            Graph = graph;
        }

        private protected TSetter AddAttribute(string name, string value)
        {
            Attributes.Add(name, value);
            return this as TSetter;
        }

        public TSetter Color(string color) =>
            AddAttribute("color", color);

        public TSetter FontSize(int fontSize) =>
            AddAttribute("fontsize", Convert.ToString(fontSize, CultureInfo.InvariantCulture));

        public TSetter Label(string label) =>
            AddAttribute("label", label);
    }

    public class LastNodeAttributesSetter
        : DotGraphAttributesSetter<LastNodeAttributesSetter>
    {
        public LastNodeAttributesSetter(Graph graph) : base(graph) 
        {
            if (graph == null)
                throw new ArgumentNullException($"{nameof(graph)} can not be null");
            if (graph.Nodes == null || graph.Nodes.Count() < 1)
                throw new ArgumentException($"{nameof(graph)} must contain at least one edge!");
        }

        private protected override Dictionary<string, string> Attributes
        {
            get => Graph.Nodes.Last().Attributes;
        }

        public LastNodeAttributesSetter Shape(NodeShape shape)
        {
            var shapeString = Convert
                .ToString(shape, CultureInfo.InvariantCulture)
                .ToLowerInvariant();
            AddAttribute("shape", shapeString);
            return this;
        }
    }

    public class LastEdgeAttributesSetter
        : DotGraphAttributesSetter<LastEdgeAttributesSetter>
    {
        public LastEdgeAttributesSetter(Graph graph) : base(graph) 
        {
            if (graph == null)
                throw new ArgumentNullException($"{nameof(graph)} can not be null");
            if (graph.Edges == null || graph.Edges.Count() < 1)
                throw new ArgumentException($"{nameof(graph)} must contain at least one edge!");
        }

        private protected override Dictionary<string, string> Attributes
        {
            get => Graph.Edges.Last().Attributes;
        }

        public LastEdgeAttributesSetter Weight(double weight) =>
            AddAttribute("weight", Convert.ToString(weight, CultureInfo.InvariantCulture));
    }

    public enum NodeShape
    {
        None,
        Box,
        Ellipse
    }
}