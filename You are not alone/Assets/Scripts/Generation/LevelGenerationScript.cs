using System;
using System.Collections.Generic;
using Enums;
using Generation.VazGriz_Generation_Scripts;
using Graphs;
using Unity.VisualScripting;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Generation
{
    public class LevelGenerationScript : MonoBehaviour
    {
        [Header("Generation Settings")]
        [SerializeField] private uint seed = 1;
        [SerializeField] private Vector3Int blockSize = Vector3Int.one*5;
        [SerializeField] private Vector2Int mapSize = Vector2Int.one*10;
        [SerializeField] private Vector2Int minRoomSize = Vector2Int.one;
        [SerializeField] private Vector2Int maxRoomSize = Vector2Int.one;
        [SerializeField] private int roomCount = 10;

        [Header("Debug")]
        [SerializeField] private bool showGrid = false;
        [SerializeField] private bool showPath = true;
        [SerializeField] private bool showRooms = true;
        [SerializeField] private bool _showMidpoints = false;

        private const int RoomPathCost = 10;
        private const int NonePathCost = 5;
        private const int HallwayPathCost = 1;

        private Random _random;
        private Grid2D<CellState> _grid;
        private List<Generator2D.Room> _rooms;
        private Delaunay2D _delaunay;
        private HashSet<Prim.Edge> _selectedEdges;
        private Graph<Vector2Int> _roomsRoomGraph;

        public Grid2D<CellState> Grid => _grid;
        public Vector3Int BlockSize => blockSize;
        public Graph<Vector2Int> RoomGraph => _roomsRoomGraph;


        private void Start()
        {
            GenerateLevel();
        }

        public Grid2D<CellState> GenerateLevel()
        {
            _random = new Random(seed: (uint)seed);
            _grid = new Grid2D<CellState>(mapSize, Vector2Int.zero);
            _rooms = new List<Generator2D.Room>();
            
            PlaceRooms();
            Triangulate();
            CreateHallways();
            PathfindHallways();
            PrintRoomAndHallwayCount();
            CreateGraph();

            return _grid;
        }

        private void PrintRoomAndHallwayCount()
        {
            //Log the amount of rooms and hallways in the grid2d
            int roomCount = 0;
            int hallwayCount = 0;
            for (int x = 0; x < _grid.Size.x; x++)
            {
                for (int y = 0; y < _grid.Size.y; y++)
                {
                    if (_grid[x, y] == CellState.Room)
                    {
                        roomCount++;
                    }
                    else if (_grid[x, y] == CellState.Hallway)
                    {
                        hallwayCount++;
                    }
                }
            }
            Debug.Log($"Rooms: {roomCount}, Hallways: {hallwayCount}, Active: {roomCount + hallwayCount}");
        }

        private void CreateGraph()
        {
            _roomsRoomGraph = new Graph<Vector2Int>();
            for (int x = 0; x < _grid.Size.x; x++)
            {
                for (int y = 0; y < _grid.Size.y; y++)
                {
                    var cell = _grid[x, y];
                    if (EnumHelpers.CellIsOccupied(cell))
                    {
                        //Check the cells around it
                        for (int i = -1; i <= 1; i++)
                        {
                            for (int j = -1; j <= 1; j++)
                            {
                                if (i == 0 && j == 0) continue;
                                if (x + i < 0 || x + i >= _grid.Size.x) continue;
                                if (y + j < 0 || y + j >= _grid.Size.y) continue;
                                if (EnumHelpers.CellIsOccupied(_grid[x + i, y + j]))
                                {
                                    var vector2Int = new Vector2Int(x, y);
                                    var vector2Int1 = new Vector2Int(x + i, y + j);
                                    RoomGraph.AddEdge(vector2Int, vector2Int1);
                                }
                            }
                        }
                    }
                }
            }
            Debug.Log($"Vertices: {_roomsRoomGraph.VertexCount}, Edges: {_roomsRoomGraph.EdgeCount}");
        }

        private void PlaceRooms()
        {
            for (int i = 0; i < roomCount; i++) {
                Vector2Int location = new Vector2Int(
                    _random.NextInt(1, mapSize.x+1),
                    _random.NextInt(1, mapSize.y+1)
                );
        
                Vector2Int roomSize = new Vector2Int(
                    _random.NextInt(minRoomSize.x, maxRoomSize.x + 1),
                    _random.NextInt(minRoomSize.y, maxRoomSize.y + 1)
                );
        
                bool add = true;
                Generator2D.Room newRoom = new Generator2D.Room(location, roomSize);
                Generator2D.Room buffer = new Generator2D.Room(location + new Vector2Int(-1, -1), roomSize + new Vector2Int(2, 2));
        
                foreach (var room in _rooms) {
                    if (Generator2D.Room.Intersect(room, buffer)) {
                        add = false;
                        break;
                    }
                }
        
                add = !(newRoom.Bounds.xMin < 0 || newRoom.Bounds.xMax >= mapSize.x 
                        || newRoom.Bounds.yMin < 0 || newRoom.Bounds.yMax >= mapSize.y);
        
                if (add) {
                    _rooms.Add(newRoom);
                    foreach (var pos in newRoom.Bounds.allPositionsWithin) {
                        _grid[pos] = CellState.Room;
                    }
                }
            }
        }

        private void Triangulate() {
            List<Vertex> vertices = new List<Vertex>();
        
            foreach (var room in _rooms) {
                vertices.Add(new Vertex<Generator2D.Room>((Vector2)room.Bounds.position + 
                                                          ((Vector2)room.Bounds.size) / 2, room));
            }
        
            _delaunay = Delaunay2D.Triangulate(vertices);
        }
        
        private void CreateHallways() {
            var edges = new List<Prim.Edge>();
        
            foreach (var edge in _delaunay.Edges) {
                edges.Add(new Prim.Edge(edge.U, edge.V));
            }
        
            var mst = Prim.MinimumSpanningTree(edges, edges[0].U);
        
            _selectedEdges = new HashSet<Prim.Edge>(mst);
            var remainingEdges = new HashSet<Prim.Edge>(edges);
            remainingEdges.ExceptWith(_selectedEdges);
        
            foreach (var edge in remainingEdges) {
                if (_random.NextDouble() < 0.125) {
                    _selectedEdges.Add(edge);
                }
            }
        }

        private void PathfindHallways() {
            DungeonPathfinder2D aStar = new DungeonPathfinder2D(mapSize);
        
            foreach (var edge in _selectedEdges) {
                var startRoom = (edge.U as Vertex<Generator2D.Room>).Item;
                var endRoom = (edge.V as Vertex<Generator2D.Room>).Item;
        
                var startPosf = startRoom.Bounds.center;
                var endPosf = endRoom.Bounds.center;
                var startPos = new Vector2Int((int)startPosf.x, (int)startPosf.y);
                var endPos = new Vector2Int((int)endPosf.x, (int)endPosf.y);
        
                var path = aStar.FindPath(startPos, endPos, (DungeonPathfinder2D.Node a, DungeonPathfinder2D.Node b) => {
                    var pathCost = new DungeonPathfinder2D.PathCost();
                
                    pathCost.cost = Vector2Int.Distance(b.Position, endPos);    //heuristic
        
                    if (_grid[b.Position] == CellState.Room)
                    {
                        pathCost.cost += RoomPathCost;
                    } else if (_grid[b.Position] == CellState.None)
                    {
                        pathCost.cost += NonePathCost;
                    } else if (_grid[b.Position] == CellState.Hallway)
                    {
                        pathCost.cost += HallwayPathCost;
                    }
        
                    pathCost.traversable = true;
        
                    return pathCost;
                });
        
                if (path != null) {
                    for (int i = 0; i < path.Count; i++) {
                        var current = path[i];
        
                        if (_grid[current] == CellState.None) {
                            _grid[current] = CellState.Hallway;
                        }
        
                        if (i > 0) {
                            var prev = path[i - 1];
                        }
                    }
                }
            }
        }
    
        public void ClearLevel()
        {
            //Clear the grid
            _grid = null;
            _rooms = null;
            _selectedEdges = null;
        }

        private void OnDrawGizmos()
        {
            //Draw the grid
            if (_grid != null)
            {
                for (int x = 0; x < _grid.Size.x; x++)
                {
                    for (int y = 0; y < _grid.Size.y; y++)
                    {
                        var grid_pos = new Vector2Int(x, y);
                        var cornerPositionInGrid = GetPositionInGrid(grid_pos);
                        var middlePositionInGrid = GetPositionInGrid(grid_pos);
                        var cell = _grid[x, y];
                        switch (cell)
                        {
                            case CellState.Room:
                                if (!showRooms) continue;
                                Gizmos.color = new Color(1, 0, 0, 0.5f);
                                break;
                            case CellState.Hallway:
                                if (!(showPath)) continue;
                                Gizmos.color = new Color(0, 1, 0, 0.5f);
                                break;
                            default:
                                if (!showGrid) continue;
                                Gizmos.color = new Color(1,1,1, 0.1f);
                                break;
                        }
                        Gizmos.DrawCube(cornerPositionInGrid, blockSize);
                        
                        if (!_showMidpoints) continue;
                        Gizmos.color = Color.blue;
                        Gizmos.DrawSphere(middlePositionInGrid, 1f);
                    }
                }
            }
        }

        public Vector2Int GetGridPosition(Vector3 transformPosition)
        {
            return new Vector2Int(Mathf.FloorToInt(transformPosition.x / blockSize.x), 
                                    Mathf.FloorToInt(transformPosition.z / blockSize.z));
        }
        
        public Vector3 GetPositionInGrid(Vector2Int gridPosition)
        {
            return new Vector3(gridPosition.x * blockSize.x, 0, gridPosition.y * blockSize.z);
        }
    }
}
