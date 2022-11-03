using System;
using Enums;
using Generation.VazGriz_Generation_Scripts;
using UnityEngine;


namespace Generation
{
    public class BlockGenData : MonoBehaviour
    {
        public float Probability = 0.5f;
        public Grid2D<CellState> Neighbours = new(Vector2Int.one*3, Vector2Int.zero);

        public enum CellType {
            Any = 0,
            Inside = 1,
            Outside = 2,
        }

        public bool NeighbourCheck(Grid2D<CellState> grid, Vector2Int pos)
        {
            // Check validity of parameters
            if (grid == null) return false;
            if (grid.Size.x < 3 || grid.Size.y < 3) return false;
            if (pos.x < 0 || pos.y < 0) return false;
            if (pos.x >= grid.Size.x || pos.y >= grid.Size.y) return false;
            
            // Check current cell
            if (!EnumHelpers.CellIsOccupied(grid[pos])) return false;

            bool result = false;
            
            // For each rotation of the neighbours grid
            for (int rotation_itt = 0; rotation_itt < 4; rotation_itt++)
            {
                // Rotate the neighbours grid
                Rotate(ref Neighbours);
                
                if (result) continue;

                // For each cell in the neighbours grid
                var valid = true;
                for (int grid_x_offset = -1; grid_x_offset <= 1; grid_x_offset++)
                {
                    for (int grid_y_offset = -1; grid_y_offset <= 1; grid_y_offset++)
                    {
                        if (grid_x_offset == 0 && grid_y_offset == 0) continue;

                        var gridPos = pos + new Vector2Int(grid_x_offset, grid_y_offset);
                        var neighboursPos = new Vector2Int(grid_x_offset + 1, grid_y_offset + 1);
                        var gridPosInBounds = gridPos.x >= 0 && gridPos.y >= 0 && gridPos.x < grid.Size.x && gridPos.y < grid.Size.y;
                        var neighboursPosInBounds = neighboursPos.x >= 0 && neighboursPos.y >= 0 && neighboursPos.x < Neighbours.Size.x && neighboursPos.y < Neighbours.Size.y;
                        var gridVal = gridPosInBounds? grid[gridPos] : CellState.OutOfBounds;
                        var neighboursVal = neighboursPosInBounds ?  Neighbours[neighboursPos] : CellState.OutOfBounds;
                        
                        valid = EnumHelpers.CellsAreAllowed(neighboursVal, gridVal);
                        
                        if (!valid) break;
                    }
                    if (!valid) break;
                }
                if (valid) result = true;
            }
            return result;
        }

        // Rotates a grid 90 degrees clockwise
        public void Rotate(ref Grid2D<CellState> grid)
        {
            var newGrid = new Grid2D<CellState>(grid.Size, grid.Offset);
            for (var x = 0; x < grid.Size.x; x++)
            {
                for (var y = 0; y < grid.Size.y; y++)
                {
                    newGrid[new Vector2Int(x, y)] = grid[new Vector2Int(grid.Size.x - y - 1, x)];
                }
            }
            grid = newGrid;
        }
    }
}
