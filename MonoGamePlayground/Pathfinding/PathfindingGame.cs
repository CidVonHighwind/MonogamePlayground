using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGamePlayground.GBShader
{
    class PathfindingGame : IGame
    {
        class Node : IComparable<Node>
        {
            public Vector2 Position;
            public List<Connection> Connections = new List<Connection>();

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

        class Connection
        {
            public Node Node;
            public float Cost;

            public Connection(Node origin, Node node)
            {
                Node = node;

                var vec = origin.Position - node.Position;
                Cost = vec.Length();
            }
        }

        private Texture2D sprWhite;

        int[,] Map = new int[128, 72];

        Vector2 PlayerPosition = new Vector2(250, 250);

        private List<Vector2> PlayerPath = new List<Vector2>();
        private float PathState;

        private List<Node> NodeList = new List<Node>();

        private List<Vector2> debugFields = new List<Vector2>();
        private List<Vector2> debugLines = new List<Vector2>();

        private int TileSize = 10;

        private MouseState mouseState, lastMouseState;

        private Node playerNode;
        private Node goalNode;

        public void Load(GraphicsDeviceManager graphics, ContentManager content)
        {
            sprWhite = new Texture2D(graphics.GraphicsDevice, 1, 1);
            sprWhite.SetData(new[] { Color.White });

            playerNode = new Node(PlayerPosition);
            goalNode = new Node(Vector2.Zero);
        }

        public void Update(GameTime gameTime)
        {
            lastMouseState = mouseState;
            mouseState = Mouse.GetState();

            var mouseX = mouseState.X / 10;
            var mouseY = mouseState.Y / 10;

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (mouseX >= 0 && mouseX < Map.GetLength(0) &&
                    mouseY >= 0 && mouseY < Map.GetLength(1))
                    Map[mouseX, mouseY] = 1;
            }

            // set player direction
            if (mouseState.RightButton == ButtonState.Pressed)
            //&& lastMouseState.RightButton != ButtonState.Pressed)
            {
                SearchRoute(new Vector2(mouseState.X, mouseState.Y));
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                NodeList.Clear();
                CreateGraph();
            }

            // move the player
            if (PlayerPath.Count >= 2)
            {
                var path = PlayerPath[1] - PlayerPath[0];
                PathState += 1 / path.Length() * 3;

                // finished path segment?
                if (PathState >= 1)
                {
                    PathState = 0;
                    PlayerPosition = PlayerPath[1];
                    PlayerPath.RemoveAt(0);
                }
                else
                {
                    PlayerPosition = PlayerPath[0] + path * PathState;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            
            // draw the map
            for (var y = 0; y < Map.GetLength(1); y++)
            {
                for (var x = 0; x < Map.GetLength(0); x++)
                {
                    spriteBatch.Draw(sprWhite, new Rectangle(x * 10, y * 10, 10, 10),
                        ((x + y) % 2 == 0 ? Color.LightGray : Color.DarkGray) * 0.5f);

                    if (Map[x, y] > 0)
                        spriteBatch.Draw(sprWhite, new Rectangle(x * 10, y * 10, 10, 10), Color.White * 0.75f);
                }
            }

            // draw the nodes
            for (var i = 0; i < NodeList.Count; i++)
            {
                spriteBatch.Draw(sprWhite, new Rectangle(
                    (int)NodeList[i].Position.X - 1, (int)NodeList[i].Position.Y - 1, 2, 2), Color.Blue);

                for (var j = 0; j < NodeList[i].Connections.Count; j++)
                {
                    var source = NodeList[i].Position;
                    var origin = NodeList[i].Connections[j].Node.Position;
                    var direction = origin - source;
                    var rotation = (float)Math.Atan2(direction.Y, direction.X);

                    spriteBatch.Draw(sprWhite, new Rectangle(
                        (int)NodeList[i].Position.X,
                        (int)NodeList[i].Position.Y,
                        (int)NodeList[i].Connections[j].Cost, 1),
                        new Rectangle(0, 0, 1, 1),
                        new Color(0, 1, 0) * 0.005f,
                        rotation, new Vector2(0, 0.5f), SpriteEffects.None, 0);
                }
            }

            //for (var i = 0; i < debugFields.Count; i++)
            //{
            //    spriteBatch.Draw(sprWhite, new Rectangle(
            //        (int)debugFields[i].X * 10, (int)debugFields[i].Y * 10, 10, 10), Color.Red * 0.5f);
            //}

            //for (var i = 0; i < debugLines.Count; i++)
            //{
            //    spriteBatch.Draw(sprWhite, new Rectangle(
            //        (int)debugLines[i].X - 1, (int)debugLines[i].Y - 1, 2, 2), Color.Pink * 0.5f);
            //}

            // draw the player
            spriteBatch.Draw(sprWhite, new Rectangle(
                (int)PlayerPosition.X - 5, (int)PlayerPosition.Y - 5, 10, 10), Color.LightGreen);

            spriteBatch.End();
        }

        private void SearchRoute(Vector2 goalPosition)
        {
            PlayerPath.Clear();
            PathState = 0;

            playerNode.Position = PlayerPosition;
            goalNode.Position = goalPosition;

            NodeList.Clear();
            CreateGraph();

            //// remove player edges
            //playerNode.Connections.Clear();

            //// remove goal edges
            //for (var i = 0; i < goalNode.Connections.Count; i++)
            //{
            //    for (var j = 0; j < goalNode.Connections[i].Node.Connections.Count; j++)
            //    {
            //        if (goalNode.Connections[i].Node.Connections[j].Node == goalNode)
            //        {
            //            goalNode.Connections[i].Node.Connections.RemoveAt(j);
            //            break;
            //        }
            //    }
            //}

            //goalNode.Connections.Clear();

            //// add edgest to the player and the goal node
            //for (var i = 0; i < NodeList.Count; i++)
            //{
            //    if (LineOfSight(playerNode, NodeList[i]))
            //        playerNode.Connections.Add(new Connection(playerNode, NodeList[i]));

            //    if (LineOfSight(goalNode, NodeList[i]))
            //    {
            //        NodeList[i].Connections.Add(new Connection(NodeList[i], goalNode));
            //        goalNode.Connections.Add(new Connection(goalNode, NodeList[i]));
            //    }
            //}

            //if (LineOfSight(playerNode, new Node(new Vector2(mouseState.X, mouseState.Y))))
            //{
            //    PlayerPath.Add(PlayerPosition);
            //    PlayerPath.Add(new Vector2(mouseState.X, mouseState.Y));
            //}

            GraphSearch(playerNode, goalNode);
        }

        private void GraphSearch(Node startNode, Node endNode)
        {
            // init nodes
            for (var i = 0; i < NodeList.Count; i++)
            {
                NodeList[i].CameFrom = null;
                NodeList[i].Cost = float.PositiveInfinity;
            }

            var closedSet = new List<Node>();

            var openSet = new List<Node>();
            openSet.Add(startNode);

            startNode.Cost = 0;

            while (openSet.Count > 0)
            {
                closedSet.Add(openSet[0]);

                if (openSet[0] == endNode)
                {
                    var currentNode = openSet[0];
                    PlayerPath.Insert(0, currentNode.Position);
                    while (currentNode.CameFrom != null)
                    {
                        PlayerPath.Insert(0, currentNode.CameFrom.Position);
                        currentNode = currentNode.CameFrom;
                    }
                    return;
                }

                for (var i = 0; i < openSet[0].Connections.Count; i++)
                {
                    if (closedSet.Contains(openSet[0].Connections[i].Node))
                        continue;

                    if (!openSet.Contains(openSet[0].Connections[i].Node))
                        openSet.Add(openSet[0].Connections[i].Node);
                    // found a better connection to the node?
                    else if (openSet[0].Cost + openSet[0].Connections[i].Cost >= openSet[0].Connections[i].Node.Cost)
                        continue;

                    openSet[0].Connections[i].Node.CameFrom = openSet[0];
                    openSet[0].Connections[i].Node.Cost = openSet[0].Cost + openSet[0].Connections[i].Cost;
                }

                openSet.RemoveAt(0);
                openSet.Sort();
            }

            startNode.Cost = 0;
        }

        private void CreateGraph()
        {
            // add the nodes
            for (var y = 1; y < Map.GetLength(1) - 1; y++)
                for (var x = 1; x < Map.GetLength(0) - 1; x++)
                    if (Map[x, y] == 0)
                    {
                        if (Map[x - 1, y] == 1 || Map[x, y - 1] == 1 || Map[x - 1, y - 1] == 1)
                            NodeList.Add(new Node(new Vector2(x * 10, y * 10)));
                        if (Map[x, y - 1] == 1 || Map[x + 1, y - 1] == 1 || Map[x + 1, y] == 1)
                            NodeList.Add(new Node(new Vector2(x * 10 + 10, y * 10)));
                        if (Map[x - 1, y] == 1 || Map[x - 1, y + 1] == 1 || Map[x, y + 1] == 1)
                            NodeList.Add(new Node(new Vector2(x * 10, y * 10 + 10)));
                        if (Map[x + 1, y] == 1 || Map[x + 1, y + 1] == 1 || Map[x, y + 1] == 1)
                            NodeList.Add(new Node(new Vector2(x * 10 + 10, y * 10 + 10)));
                    }

            playerNode.Connections.Clear();
            goalNode.Connections.Clear();
            NodeList.Add(playerNode);
            NodeList.Add(goalNode);

            for (var i = 0; i < NodeList.Count; i++)
                for (var j = 0; j < NodeList.Count; j++)
                {
                    if (i == j)
                        continue;

                    if (LineOfSight(NodeList[i], NodeList[j]))
                        NodeList[i].Connections.Add(new Connection(NodeList[i], NodeList[j]));
                }
        }

        //
        // https://theshoemaker.de/2016/02/ray-casting-in-2d-grids/
        //
        private bool LineOfSight(Node firstNode, Node secondNode)
        {
            var currentPosition = firstNode.Position;

            debugFields.Clear();
            debugLines.Clear();

            var posX = (int)(currentPosition.X / TileSize);
            var posY = (int)(currentPosition.Y / TileSize);

            var nextX = posX;
            var nextY = posY;

            var dir = secondNode.Position - currentPosition;

            //nextX += Math.Sign(dir.X);
            //nextY += Math.Sign(dir.Y);

            if ((int)(currentPosition.X / TileSize) == currentPosition.X / TileSize)
            {
                //if (dir.X < 0)
                //    nextX--;
            }
            else if (dir.X > 0)
                nextX++;

            if ((int)(currentPosition.Y / TileSize) == currentPosition.Y / TileSize)
            {
                //if (dir.Y < 0)
                //    nextY--;
            }
            else if (dir.Y > 0)
                nextY++;

            while (true)
            {
                var path = secondNode.Position - currentPosition;
                var normalized = path;
                normalized.Normalize();

                //if (path.X > 0)
                //    nextX++;
                //if (path.X < 0)
                //    nextX--;

                //if (path.Y > 0)
                //    nextY++;
                //if (path.Y < 0)
                //    nextY--;


                //if (path.X == 0)
                //{
                //    return false;
                //}
                //else if (path.Y == 0)
                //{
                //    return false;
                //}
                //else
                //{
                var dtX = (nextX * TileSize - currentPosition.X) / path.X;
                var dtY = (nextY * TileSize - currentPosition.Y) / path.Y;

                // finished?
                if ((float.IsNaN(dtX) || Math.Abs(dtX) >= 1) &&
                    (float.IsNaN(dtY) || Math.Abs(dtY) >= 1))
                {
                    currentPosition = secondNode.Position;
                    return true;
                }

                if (Math.Abs(dtX) < Math.Abs(dtY))
                {
                    currentPosition.X = nextX * TileSize;
                    currentPosition.Y += path.Y * Math.Abs(dtX);

                    posX = nextX;
                    if (dir.X < 0)
                        posX--;

                    debugFields.Add(new Vector2(posX, posY));
                    // collision?
                    if (Map[posX, posY] == 1)
                    {
                        debugLines.Add(currentPosition);
                        return false;
                    }

                    nextX += Math.Sign(dir.X);
                }
                else if (Math.Abs(dtX) > Math.Abs(dtY))
                {
                    currentPosition.X += path.X * Math.Abs(dtY);
                    currentPosition.Y = nextY * TileSize;

                    posY = nextY;
                    if (dir.Y < 0)
                        posY--;

                    debugFields.Add(new Vector2(posX, posY));
                    // collision?
                    if (Map[posX, posY] == 1)
                    {
                        debugLines.Add(currentPosition);
                        return false;
                    }

                    nextY += Math.Sign(dir.Y);
                }
                else
                {
                    currentPosition.X = nextX * TileSize;
                    currentPosition.Y = nextY * TileSize;

                    posX = nextX;
                    if (dir.X < 0)
                        posX--;

                    // collision?
                    debugFields.Add(new Vector2(posX, posY));
                    if (Map[posX, posY] == 1)
                    {
                        debugLines.Add(currentPosition);
                        return false;
                    }

                    posY = nextY;
                    if (dir.Y < 0)
                        posY--;

                    // collision?
                    debugFields.Add(new Vector2(posX, posY));
                    if (Map[posX, posY] == 1)
                    {
                        debugLines.Add(currentPosition);
                        return false;
                    }

                    nextX += Math.Sign(dir.X);
                    nextY += Math.Sign(dir.Y);
                }

                debugLines.Add(currentPosition);
                //}
            }

            return false;
        }
    }
}
