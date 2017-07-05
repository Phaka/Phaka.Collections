// Copyright (c) Werner Strydom. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

namespace Phaka.Collections
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     Represents a dependency graph
    /// </summary>
    /// <typeparam name="T">The nodes</typeparam>
    public class DependencyGraph<T>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DependencyGraph{T}" /> class.
        /// </summary>
        public DependencyGraph()
        {
            AdjacencyList = new ConcurrentDictionary<T, Tuple<HashSet<T>, HashSet<T>>>();
        }

        /// <summary>
        ///     Gets the adjacency list.
        /// </summary>
        /// <value>The adjacency list.</value>
        private ConcurrentDictionary<T, Tuple<HashSet<T>, HashSet<T>>> AdjacencyList { get; }

        /// <summary>
        ///     Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count => AdjacencyList.Count;

        /// <summary>
        ///     Adds the dependency.
        /// </summary>
        /// <param name="antecedent">The antecedent.</param>
        /// <param name="dependent">The dependent.</param>
        public void AddDependency(T antecedent, T dependent)
        {
            Add(antecedent);
            Add(dependent);

            AdjacencyList[antecedent].Item1.Add(dependent);
            AdjacencyList[dependent].Item2.Add(antecedent);
        }

        /// <summary>
        ///     Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Add(T item)
        {
            AdjacencyList.GetOrAdd(item, new Tuple<HashSet<T>, HashSet<T>>(new HashSet<T>(), new HashSet<T>()));
        }

        /// <summary>
        ///     Gets the direct dependents.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public IEnumerable<T> GetDirectDependents(T item)
        {
            if (AdjacencyList.TryGetValue(item, out Tuple<HashSet<T>, HashSet<T>> tuple))
                return tuple.Item1;

            throw new InvalidOperationException($"The node `{item}` doesn't exist");
        }

        /// <summary>
        ///     Gets the antecedents.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public IEnumerable<T> GetAntecedents(T item)
        {
            if (AdjacencyList.TryGetValue(item, out Tuple<HashSet<T>, HashSet<T>> tuple))
                return tuple.Item2;

            throw new InvalidOperationException($"The node `{item}` doesn't exist");
        }

        /// <summary>
        ///     Gets the root nodes.
        /// </summary>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        public IEnumerable<T> GetRootNodes()
        {
            return AdjacencyList.Where(p => p.Value.Item2.Count == 0).Select(p => p.Key);
        }

        /// <summary>
        ///     Gets the leaf nodes.
        /// </summary>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        public IEnumerable<T> GetLeafNodes()
        {
            return AdjacencyList.Where(p => p.Value.Item1.Count == 0).Select(p => p.Key);
        }

        /// <summary>
        ///     Gets the nodes.
        /// </summary>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        public IEnumerable<T> GetNodes()
        {
            return AdjacencyList.Keys;
        }

        /// <summary>
        ///     Gets the subgraph.
        /// </summary>
        /// <param name="func">The function.</param>
        /// <returns>DependencyGraph&lt;T&gt;.</returns>
        public DependencyGraph<T> GetSubgraph(Func<T, bool> func)
        {
            return GetSubgraph(GetNodes().Where(func));
        }

        /// <summary>
        ///     Gets the subgraph.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <returns>DependencyGraph&lt;T&gt;.</returns>
        public DependencyGraph<T> GetSubgraph(IEnumerable<T> nodes)
        {
            var stack = new Stack<T>(nodes);
            var visited = new HashSet<T>();
            var result = new DependencyGraph<T>();
            while (stack.Count > 0)
            {
                var node = stack.Pop();
                if (visited.Contains(node))
                    continue;

                foreach (var antecedent in GetAntecedents(node))
                {
                    if (!visited.Contains(antecedent))
                        stack.Push(antecedent);
                    result.AddDependency(antecedent, node);
                }
                visited.Add(node);
            }

            return result;
        }
    }
}