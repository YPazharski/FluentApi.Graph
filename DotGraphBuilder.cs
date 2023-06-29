using System;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

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
            throw new NotImplementedException();
        }

        internal string Build()
        {
            return Graph.ToDotFormat();
        }

        internal DotGraphBuilder With(Func<object, object> value)
        {
            throw new NotImplementedException();
        }
    }
}