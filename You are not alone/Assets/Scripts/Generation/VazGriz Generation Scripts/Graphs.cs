using System;
using System.Collections.Generic;
using UnityEngine;

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
            return Position == other.Position;
        }

        public override int GetHashCode() {
            return Position.GetHashCode();
        }
    }

    public class Vertex<T> : Vertex {
        public T Item { get; private set; }

        public Vertex(T item) {
            Item = item;
        }

        public Vertex(Vector3 position, T item) : base(position) {
            Item = item;
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
    
    public class Graph<T> {
        private List<Vertex<T>> Vertices { get; set; }
        private List<Edge<T>> Edges { get; set; }

        public Graph() {
            Vertices = new List<Vertex<T>>();
            Edges = new List<Edge<T>>();
        }

        public void AddEdge(T u, T v) {
            var vertexU = Vertices.Find(vertex => vertex.Item.Equals(u));
            if (vertexU == null) {
                vertexU = new Vertex<T>(u);
                Vertices.Add(vertexU);
            }
            
            var vertexV = Vertices.Find(vertex => vertex.Item.Equals(v));
            if (vertexV == null) {
                vertexV = new Vertex<T>(v);
                Vertices.Add(vertexV);
            }
            
            Edges.Add(new Edge<T>(vertexU, vertexV));
        }

        private Vertex<T> GetVertex(T item) {
            return Vertices.Find(v => v.Item.Equals(item));
        }

        public T[] Neighbours(T item) {
            var vertex = GetVertex(item);
            var neighbours = new List<T>();
            foreach (var edge in Edges) {
                if (edge.U.Equals(vertex)) {
                    neighbours.Add(edge.V.Item);
                } else if (edge.V.Equals(vertex)) {
                    neighbours.Add(edge.U.Item);
                }
            }
            return neighbours.ToArray();
        }
        
        public int VertexCount => Vertices.Count;
        public int EdgeCount => Edges.Count;
    }
}