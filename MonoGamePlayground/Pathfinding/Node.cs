using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGamePlayground.Pathfinding
{
    class Node : IComparable<Node>
    {
        public Vector2 Position;
        public List<Edge> Connections = new List<Edge>();

        public Node CameFrom;
        public float Cost;

        public Node(Vector2 position)
        {
            Position = position;
        }

        public int CompareTo(Node other)
        {
            return (int)(Cost - other.Cost);
        }
    }
}
