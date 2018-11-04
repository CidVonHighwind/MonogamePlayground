using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGamePlayground.Pathfinding
{
    class PathfindingGame : IGame
    {
        private MouseState _mouseState, _lastMouseState;
        private Texture2D _sprWhite;

        private Vector2 _playerPosition = new Vector2(250, 250);

        private List<Vector2> _playerPath = new List<Vector2>();
        private float _pathState;

        private List<Node> _nodeList = new List<Node>();

        private List<Vector2> _debugFields = new List<Vector2>();
        private List<Vector2> _debugLines = new List<Vector2>();

        private Node _playerNode;
        private Node _goalNode;

        int[,] _map = new int[128, 72];

        private int _tileSize = 10;

        public void Load(GraphicsDeviceManager graphics, ContentManager content)
        {
            _sprWhite = new Texture2D(graphics.GraphicsDevice, 1, 1);
            _sprWhite.SetData(new[] { Color.White });

            _playerNode = new Node(_playerPosition);
            _goalNode = new Node(Vector2.Zero);
        }

        public void Update(GameTime gameTime)
        {
            _lastMouseState = _mouseState;
            _mouseState = Mouse.GetState();

            var mouseX = _mouseState.X / 10;
            var mouseY = _mouseState.Y / 10;

            if (_mouseState.LeftButton == ButtonState.Pressed)
                if (mouseX >= 0 && mouseX < _map.GetLength(0) &&
                    mouseY >= 0 && mouseY < _map.GetLength(1))
                    _map[mouseX, mouseY] = 1;

            // set player direction
            if (_mouseState.RightButton == ButtonState.Pressed)
            //&& lastMouseState.RightButton != ButtonState.Pressed)
            {
                SearchRoute(new Vector2(_mouseState.X, _mouseState.Y));
            }

            // move the player
            if (_playerPath.Count >= 2)
            {
                var path = _playerPath[1] - _playerPath[0];
                _pathState += 1 / path.Length() * 3;

                // finished path segment?
                if (_pathState >= 1)
                {
                    _pathState = 0;
                    _playerPosition = _playerPath[1];
                    _playerPath.RemoveAt(0);
                }
                else
                {
                    _playerPosition = _playerPath[0] + path * _pathState;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            // draw the map
            for (var y = 0; y < _map.GetLength(1); y++)
                for (var x = 0; x < _map.GetLength(0); x++)
                {
                    // draw the grid
                    spriteBatch.Draw(_sprWhite, new Rectangle(x * 10, y * 10, 10, 10),
                        ((x + y) % 2 == 0 ? Color.LightGray : Color.DarkGray) * 0.5f);

                    // draw the walls
                    if (_map[x, y] > 0)
                        spriteBatch.Draw(_sprWhite, new Rectangle(x * 10, y * 10, 10, 10), Color.White * 0.75f);
                }

            for (var i = 0; i < _nodeList.Count; i++)
            {
                // draw the nodes
                spriteBatch.Draw(_sprWhite, new Rectangle(
                    (int)_nodeList[i].Position.X - 1, (int)_nodeList[i].Position.Y - 1, 2, 2), Color.Blue);

                // draw the edges
                for (var j = 0; j < _nodeList[i].Connections.Count; j++)
                {
                    var source = _nodeList[i].Position;
                    var origin = _nodeList[i].Connections[j].Node.Position;
                    var direction = origin - source;
                    var rotation = (float)Math.Atan2(direction.Y, direction.X);

                    spriteBatch.Draw(_sprWhite, new Rectangle(
                        (int)_nodeList[i].Position.X,
                        (int)_nodeList[i].Position.Y,
                        (int)_nodeList[i].Connections[j].Cost, 1),
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
            spriteBatch.Draw(_sprWhite, new Rectangle(
                (int)_playerPosition.X - 5, (int)_playerPosition.Y - 5, 10, 10), Color.LightGreen);

            spriteBatch.End();
        }

        private void SearchRoute(Vector2 goalPosition)
        {
            _playerPath.Clear();
            _pathState = 0;

            _playerNode.Position = _playerPosition;
            _goalNode.Position = goalPosition;

            _nodeList.Clear();
            CreateGraph();

            GraphSearch(_playerNode, _goalNode);

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
        }

        private void GraphSearch(Node startNode, Node endNode)
        {
            // init nodes
            for (var i = 0; i < _nodeList.Count; i++)
            {
                _nodeList[i].CameFrom = null;
                _nodeList[i].Cost = float.PositiveInfinity;
            }

            var closedSet = new List<Node>();

            var openSet = new List<Node> { startNode };

            startNode.Cost = 0;

            while (openSet.Count > 0)
            {
                closedSet.Add(openSet[0]);

                // found a solution?
                if (openSet[0] == endNode)
                {
                    var currentNode = openSet[0];
                    _playerPath.Insert(0, currentNode.Position);
                    while (currentNode.CameFrom != null)
                    {
                        _playerPath.Insert(0, currentNode.CameFrom.Position);
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
            for (var y = 1; y < _map.GetLength(1) - 1; y++)
                for (var x = 1; x < _map.GetLength(0) - 1; x++)
                    if (_map[x, y] == 1)
                    {
                        if (_map[x - 1, y] == 0 && _map[x, y - 1] == 0 && _map[x - 1, y - 1] == 0)
                            _nodeList.Add(new Node(new Vector2(x * 10 - 1, y * 10 - 1)));
                        if (_map[x, y - 1] == 0 && _map[x + 1, y - 1] == 0 && _map[x + 1, y] == 0)
                            _nodeList.Add(new Node(new Vector2(x * 10 + 11, y * 10 - 1)));
                        if (_map[x - 1, y] == 0 && _map[x - 1, y + 1] == 0 && _map[x, y + 1] == 0)
                            _nodeList.Add(new Node(new Vector2(x * 10 - 1, y * 10 + 11)));
                        if (_map[x + 1, y] == 0 && _map[x + 1, y + 1] == 0 && _map[x, y + 1] == 0)
                            _nodeList.Add(new Node(new Vector2(x * 10 + 11, y * 10 + 11)));
                    }

            _playerNode.Connections.Clear();
            _goalNode.Connections.Clear();
            _nodeList.Add(_playerNode);
            _nodeList.Add(_goalNode);

            for (var i = 0; i < _nodeList.Count; i++)
                for (var j = i + 1; j < _nodeList.Count; j++)
                {
                    // add edge if nodes are in line of sight
                    if (LineOfSight(_nodeList[i], _nodeList[j]))
                    {
                        _nodeList[i].Connections.Add(new Edge(_nodeList[i], _nodeList[j]));
                        _nodeList[j].Connections.Add(new Edge(_nodeList[j], _nodeList[i]));
                    }
                }
        }

        //
        // https://theshoemaker.de/2016/02/ray-casting-in-2d-grids/
        //
        private bool LineOfSight(Node firstNode, Node secondNode)
        {
            var currentPosition = firstNode.Position;

            _debugFields.Clear();
            _debugLines.Clear();

            var posX = (int)(currentPosition.X / _tileSize);
            var posY = (int)(currentPosition.Y / _tileSize);

            var nextX = posX;
            var nextY = posY;

            var dir = secondNode.Position - currentPosition;

            if ((int)(currentPosition.X / _tileSize) != currentPosition.X / _tileSize && dir.X > 0)
                nextX++;

            if ((int)(currentPosition.Y / _tileSize) != currentPosition.Y / _tileSize && dir.Y > 0)
                nextY++;

            while (true)
            {
                var path = secondNode.Position - currentPosition;
                var normalized = path;
                normalized.Normalize();

                var dtX = (nextX * _tileSize - currentPosition.X) / path.X;
                var dtY = (nextY * _tileSize - currentPosition.Y) / path.Y;

                // finished without finding a wall?
                if ((float.IsNaN(dtX) || Math.Abs(dtX) >= 1) &&
                    (float.IsNaN(dtY) || Math.Abs(dtY) >= 1))
                {
                    return true;
                }

                if (Math.Abs(dtX) < Math.Abs(dtY))
                {
                    currentPosition.X = nextX * _tileSize;
                    currentPosition.Y += path.Y * Math.Abs(dtX);

                    posX = nextX;
                    if (dir.X < 0)
                        posX--;

                    _debugFields.Add(new Vector2(posX, posY));
                    // collision?
                    if (_map[posX, posY] == 1)
                    {
                        _debugLines.Add(currentPosition);
                        return false;
                    }

                    nextX += Math.Sign(dir.X);
                }
                else if (Math.Abs(dtX) > Math.Abs(dtY))
                {
                    currentPosition.X += path.X * Math.Abs(dtY);
                    currentPosition.Y = nextY * _tileSize;

                    posY = nextY;
                    if (dir.Y < 0)
                        posY--;

                    _debugFields.Add(new Vector2(posX, posY));
                    // collision?
                    if (_map[posX, posY] == 1)
                    {
                        _debugLines.Add(currentPosition);
                        return false;
                    }

                    nextY += Math.Sign(dir.Y);
                }
                else
                {
                    currentPosition.X = nextX * _tileSize;
                    currentPosition.Y = nextY * _tileSize;

                    posX = nextX;
                    if (dir.X < 0)
                        posX--;

                    // collision?
                    _debugFields.Add(new Vector2(posX, posY));
                    if (_map[posX, posY] == 1)
                    {
                        _debugLines.Add(currentPosition);
                        return false;
                    }

                    posY = nextY;
                    if (dir.Y < 0)
                        posY--;

                    // collision?
                    _debugFields.Add(new Vector2(posX, posY));
                    if (_map[posX, posY] == 1)
                    {
                        _debugLines.Add(currentPosition);
                        return false;
                    }

                    nextX += Math.Sign(dir.X);
                    nextY += Math.Sign(dir.Y);
                }

                _debugLines.Add(currentPosition);
            }
        }
    }
}
