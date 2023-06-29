using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace FluentApi.Graph
{
	public class DotGraphBuilder
	{
		public static DotGraphBuilder DirectedGraph(string graphName)
		{
			throw new NotImplementedException();
		}

        internal static DotGraphBuilder UndirectedGraph(string graphName)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        internal DotGraphBuilder With(Func<object, object> value)
        {
            throw new NotImplementedException();
        }
    }
}