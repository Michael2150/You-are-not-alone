using System.Collections.Generic;
using Enums;
using Generation.VazGriz_Generation_Scripts;
using Graphs;
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
        
        [Header("Pathfinding Settings")]
        [Range(0,10)] [SerializeField] private int roomPathCost = 10;
        [Range(0,10)] [SerializeField] private int nonePathCost = 5;
        [Range(0,10)] [SerializeField] private int hallwayPathCost = 1;
        
        [Header("Debug")]
        [SerializeField] private bool _showGrid = false;
        [SerializeField] private bool _showPath = true;
        [SerializeField] private bool _showRooms = true;
        
        private Random _random;
        private Grid2D<CellState> _grid;
        private List<Generator2D.Room> _rooms;
        private Delaunay2D _delaunay;
        private HashSet<Prim.Edge> _selectedEdges;
        
        public Grid2D<CellState> Grid => _grid;
        public Vector3Int BlockSize => blockSize;
        
        public Grid2D<CellState> GenerateLevel()
        {
            _random = new Random(seed: (uint)seed);
            _grid = new Grid2D<CellState>(mapSize, Vector2Int.zero);
            _rooms = new List<Generator2D.Room>();
            
            PlaceRooms();
            Triangulate();
            CreateHallways();
            PathfindHallways();

            return _grid;
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
                    //TODO: PlaceRoom(newRoom.Bounds.position, newRoom.Bounds.size);
        
                    foreach (var pos in newRoom.Bounds.allPositionsWithin) {
                        _grid[pos] = CellState.Room;
                    }
                }
            }
        }

        private void Triangulate() {
            List<Vertex> vertices = new List<Vertex>();
        
            foreach (var room in _rooms) {
                vertices.Add(new Vertex<Generator2D.Room>((Vector2)room.Bounds.position + ((Vector2)room.Bounds.size) / 2, room));
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
                        pathCost.cost += roomPathCost;
                    } else if (_grid[b.Position] == CellState.None)
                    {
                        pathCost.cost += nonePathCost;
                    } else if (_grid[b.Position] == CellState.Hallway)
                    {
                        pathCost.cost += hallwayPathCost;
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
        
                            var delta = current - prev;
                        }
                    }
        
                    foreach (var pos in path) {
                        if (_grid[pos] == CellState.Hallway) {
                            //TODO: PlaceHallway(pos);
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
                        var pos = new Vector3(x*blockSize.x, 0, y*blockSize.z);
                        var cell = _grid[x, y];
                        switch (cell)
                        {
                            case CellState.Room:
                                if (!_showRooms) continue;
                                Gizmos.color = new Color(1, 0, 0, 0.5f);
                                break;
                            case CellState.Hallway:
                                if (!(_showPath)) continue;
                                Gizmos.color = new Color(0, 1, 0, 0.5f);
                                break;
                            default:
                                if (!_showGrid) continue;
                                Gizmos.color = new Color(1,1,1, 0.1f);
                                break;
                        }
                        Gizmos.DrawCube(pos, blockSize);
                    }
                }
            }
        }
    }
}
