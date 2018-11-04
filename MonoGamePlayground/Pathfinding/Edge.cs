namespace MonoGamePlayground.Pathfinding
{
    class Edge
    {
        public Node Node;
        public float Cost;

        public Edge(Node origin, Node node)
        {
            Node = node;

            var vec = origin.Position - node.Position;
            Cost = vec.Length();
        }
    }
}
