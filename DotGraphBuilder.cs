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

        public DotGraphNodeBuilder AddNode(string nodeName)
        {
            Graph.AddNode(nodeName);
            return new DotGraphNodeBuilder { Graph = Graph };
        }

        public string Build()
        {
            return Graph.ToDotFormat();
        }
    }

    public class DotGraphNodeBuilder : DotGraphBuilder
    {
        private Dictionary<string, string> GetLastNodeAttributes(Graph graph)
        {
            var attributes = Graph?.Nodes?.Last()?.Attributes;
            if (attributes == null)
                throw new InvalidOperationException("There are no graph or nodes in the graph!");
            return attributes;
        }

        private DotGraphNodeBuilder AddAttribute(string name, string value)
        {
            var attributes = GetLastNodeAttributes(Graph);
            attributes.Add(name, value);
            return this;
        }

        public DotGraphNodeBuilder Color(string color) =>
            AddAttribute("color", color);

        public DotGraphNodeBuilder FontSize(int fontSize) =>
            AddAttribute("fontsize", Convert.ToString(fontSize, CultureInfo.InvariantCulture));

        public DotGraphNodeBuilder Label(string label) =>
            AddAttribute("label", label);

        public DotGraphNodeBuilder Shape(NodeShape shape)
        {
            var shapeString = Convert
                .ToString(shape, CultureInfo.InvariantCulture)
                .ToLowerInvariant();
            AddAttribute("shape", shapeString);
            return this;
        }

        public DotGraphBuilder With(Action<DotGraphNodeBuilder> nodeAttributesSetter)
        {
            nodeAttributesSetter(this);
            return this;
        }
    }

    public class DotGraphEdgeBuilder : DotGraphBuilder
    {
        private string color;
        private int fontsize;
        private string label;
        private double weight;
        private static readonly FieldInfo[] edgeAttributesFields;
        static DotGraphEdgeBuilder()
        {
            edgeAttributesFields = typeof(DotGraphEdgeBuilder)
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

        }

        public DotGraphEdgeBuilder Color(string color)
        {
            this.color = color;
            return this;
        }

        public DotGraphEdgeBuilder FontSize(int fontSize)
        {
            if (fontSize < 0)
                throw new ArgumentException($"font size of {fontSize} is less than 0!");
            this.fontsize = fontSize;
            return this;
        }

        public DotGraphEdgeBuilder Label(string label)
        {
            this.label = label;
            return this;
        }

        public DotGraphEdgeBuilder Weight(double weight)
        {
            this.weight = weight;
            return this;
        }

        public DotGraphBuilder With(Action<DotGraphEdgeBuilder> edgeAttributesSetter)
        {
            var attributes = new DotGraphEdgeBuilder();
            edgeAttributesSetter(attributes);
            var latestEdge = Graph?.Edges.LastOrDefault();
            if (latestEdge == null)
                throw new InvalidOperationException("There are no graph or edges in the graph!");
            foreach (var edgeAttributesField in edgeAttributesFields)
            {
                var value = edgeAttributesField.GetValue(attributes);
                if (value == null)
                    continue;
                var valueType = value.GetType();
                if (valueType.IsValueType)
                {
                    var defaultValue = Activator.CreateInstance(valueType);
                    if (value.Equals(defaultValue))
                        continue;
                }
                var key = edgeAttributesField.Name.ToLower();
                var stringedValue = Convert.ToString(value, CultureInfo.InvariantCulture).ToLowerInvariant();
                latestEdge.Attributes.Add(key, stringedValue);
            }
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