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

        public DotGraphBuilderEdged AddEdge(string firstNode, string secondNode)
        {
            Graph.AddEdge(firstNode, secondNode);
            return new DotGraphBuilderEdged { Graph = Graph };
        }

        public DotGraphBuilderNoded AddNode(string nodeName)
        {
            Graph.AddNode(nodeName);
            return new DotGraphBuilderNoded { Graph = Graph };
        }

        public string Build()
        {
            return Graph.ToDotFormat();
        }
    }

    public class DotGraphBuilderNoded : DotGraphBuilder
    {
        private string color;
        private int fontsize;
        private string label;
        private NodeShape shape;

        public DotGraphBuilderNoded Color(string color)
        {
            this.color = color;
            return this;
        }

        public DotGraphBuilderNoded FontSize(int fontSize)
        {
            if (fontSize < 0)
                throw new ArgumentException($"font size of {fontSize} is less than 0!");
            this.fontsize = fontSize;
            return this;
        }

        public DotGraphBuilderNoded Label(string label)
        {
            this.label = label; 
            return this;    
        }

        public DotGraphBuilderNoded Shape(NodeShape shape)
        {
            this.shape = shape;
            return this;
        }

        public DotGraphBuilder With(Func<DotGraphBuilderNoded, DotGraphBuilderNoded> nodeAttributesSetter)
        {
            var attributes = nodeAttributesSetter(new DotGraphBuilderNoded());
            var latestNode = Graph?.Nodes.LastOrDefault();
            if (latestNode == null)
                throw new InvalidOperationException("There are no graph or nodes in the graph!");
            var nodeAttributesFields = typeof(DotGraphBuilderNoded).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            var a = nodeAttributesFields.OrderBy(c => c.Name).ThenBy(b => b.Name);
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

    public class DotGraphBuilderEdged : DotGraphBuilder
    {
        private string color;
        private int fontsize;
        private string label;
        private double weight;
        public DotGraphBuilderEdged Color(string color)
        {
            this.color = color;
            return this;
        }

        public DotGraphBuilderEdged FontSize(int fontSize)
        {
            if (fontSize < 0)
                throw new ArgumentException($"font size of {fontSize} is less than 0!");
            this.fontsize = fontSize;
            return this;
        }

        public DotGraphBuilderEdged Label(string label)
        {
            this.label = label;
            return this;
        }

        public DotGraphBuilderEdged Weight(double weight)
        {
            this.weight = weight;
            return this;
        }

        public DotGraphBuilder With(Func<DotGraphBuilderEdged, DotGraphBuilderEdged> edgeAttributesSetter)
        {
            var attributes = edgeAttributesSetter(new DotGraphBuilderEdged());
            var latestEdge = Graph?.Edges.LastOrDefault();
            if (latestEdge == null)
                throw new InvalidOperationException("There are no graph or edges in the graph!");
            var edgeAttributesFields = typeof(DotGraphBuilderEdged).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            var a = edgeAttributesFields.OrderBy(c => c.Name).ThenBy(b => b.Name);
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
                latestEdge.Attributes.Add(key, value.ToString().ToLower());
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