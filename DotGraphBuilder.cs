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
        private string color;
        private int fontsize;
        private string label;
        private NodeShape shape;
        private static readonly FieldInfo[] nodeAttributesFields;
        static DotGraphNodeBuilder()
        {
            nodeAttributesFields = typeof(DotGraphNodeBuilder)
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public DotGraphNodeBuilder Color(string color)
        {
            this.color = color;
            return this;
        }

        public DotGraphNodeBuilder FontSize(int fontSize)
        {
            if (fontSize < 0)
                throw new ArgumentException($"font size of {fontSize} is less than 0!");
            this.fontsize = fontSize;
            return this;
        }

        public DotGraphNodeBuilder Label(string label)
        {
            this.label = label; 
            return this;    
        }

        public DotGraphNodeBuilder Shape(NodeShape shape)
        {
            this.shape = shape;
            return this;
        }

        public DotGraphBuilder With(Action<DotGraphNodeBuilder> nodeAttributesSetter)
        {
            var attributes = new DotGraphNodeBuilder();
            nodeAttributesSetter(attributes);
            var latestNode = Graph?.Nodes.LastOrDefault();
            if (latestNode == null)
                throw new InvalidOperationException("There are no graph or nodes in the graph!");
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
                var stringedValue = Convert.ToString(value, CultureInfo.InvariantCulture).ToLowerInvariant();
                latestNode.Attributes.Add(key, stringedValue);
            }
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