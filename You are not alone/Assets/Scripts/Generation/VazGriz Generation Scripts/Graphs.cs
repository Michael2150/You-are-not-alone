using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

namespace Graphs {
    [Obsolete(" This class is obsolete. Use Vertex<T> instead.", false)]
    public class Vertex : IEquatable<Vertex> {
        public Vector3 Position { get; private set; }

        public Vertex() {

        }

        public Vertex(Vector3 position) {
            Position = position;
        }

        public override bool Equals(object obj) {
            if (obj is Vertex v) {
                return Position == v.Position;
            }
            return false;
        }

        public bool Equals(Vertex other) {
            if (other == null) return false;
            return Position == other.Position;
        }

        public override int GetHashCode() {
            return Position.GetHashCode();
        }
    }

    public class Vertex<T> : Vertex {
        public T Item { get; }

        public Vertex(T item) {
            Item = item;
        }

        public Vertex(Vector3 position, T item) : base(position) {
            Item = item;
        }
        
        public override bool Equals(object obj) {
            if (obj is Vertex<T> v) {
                return Item.Equals(v.Item);
            }
            return false;
        }

        protected bool Equals(Vertex<T> other)
        {
            return base.Equals(other) && EqualityComparer<T>.Default.Equals(Item, other.Item);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Item);
        }
    }

    [Obsolete(" This class is obsolete. Use Edge<T> instead.", false)]
    public class Edge : IEquatable<Edge> {
        public Vertex U { get; set; }
        public Vertex V { get; set; }

        public Edge() {

        }

        public Edge(Vertex u, Vertex v) {
            U = u;
            V = v;
        }

        public static bool operator ==(Edge left, Edge right) {
            return (left.U == right.U || left.U == right.V)
                && (left.V == right.U || left.V == right.V);
        }

        public static bool operator !=(Edge left, Edge right) {
            return !(left == right);
        }

        public override bool Equals(object obj) {
            if (obj is Edge e) {
                return this == e;
            }

            return false;
        }

        public bool Equals(Edge e) {
            return this == e;
        }

        public override int GetHashCode() {
            return U.GetHashCode() ^ V.GetHashCode();
        }
    }
    
    public class Edge<T> : IEquatable<Edge<T>> {
        public Vertex<T> U { get; set; }
        public Vertex<T> V { get; set; }

        public Edge() {

        }

        public Edge(Vertex<T> u, Vertex<T> v) {
            U = u;
            V = v;
        }

        public static bool operator ==(Edge<T> left, Edge<T> right) {
            return (left.U == right.U || left.U == right.V)
                   && (left.V == right.U || left.V == right.V);
        }

        public static bool operator !=(Edge<T> left, Edge<T> right) {
            return !(left == right);
        }

        public override bool Equals(object obj) {
            if (obj is Edge<T> e) {
                return this == e;
            }

            return false;
        }

        public bool Equals(Edge<T> e) {
            return this == e;
        }

        public override int GetHashCode() {
            return U.GetHashCode() ^ V.GetHashCode();
        }
    }
    
    /// <summary>
    ///    A graph of vertices objects. Each vertex can have a list all the neighbor vertices it is connected to.
    ///    This class needs to be able to add values, remove values, and find values in the graph.
    ///    This class also needs to find the neighbors of a vertex based on the vertex's value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Graph<T> {
        private readonly List<GraphVertex<T>> _vertices = new List<GraphVertex<T>>();

        public void AddEdge(T u, T v)
        {
            var vertexU = AddVertex(u);
            var vertexV = AddVertex(v);
            vertexU.AddNeighbors(vertexV);
            vertexV.AddNeighbors(vertexU);
        }
        
        private GraphVertex<T> AddVertex(T value) {
            var vertex = GetVertex(value);
            if (vertex == null) {
                vertex = new GraphVertex<T>(new Vertex<T>(value));
                _vertices.Add(vertex);
            }
            return vertex;
        }

        private GraphVertex<T> GetVertex(T value)
        {
            return _vertices.Find(v => v.Vertex.Item.Equals(value));
        }

        private List<Edge<T>> GetEdges() {
            var edges = new List<Edge<T>>();
            foreach (var vertex in _vertices) {
                foreach (var neighbor in vertex.Neighbors) {
                    var edge = new Edge<T>(vertex.Vertex, neighbor);
                    if (!edges.Contains(edge)) {
                        edges.Add(edge);
                    }
                }
            }
            return edges;
        }
        
        public List<T> Neighbours(T value) {
            var vertex = GetVertex(value);
            if (vertex == null) return new List<T>();
            return vertex.Neighbors.Select(v => v.Item).ToList();
        }

        public int VertexCount => _vertices.Count;
        public int EdgeCount => GetEdges().Count;

        private class GraphVertex<TG> {
            public Vertex<TG> Vertex { get; set; }
            public List<Vertex<TG>> Neighbors { get; set; } = new List<Vertex<TG>>();

            public GraphVertex(Vertex<TG> vertex) {
                Vertex = vertex;
            }

            public void AddNeighbors(GraphVertex<TG> vertexV)
            {
                if (Neighbors.Contains(vertexV.Vertex)) return;
                Neighbors.Add(vertexV.Vertex);
            }
        }
    }
}