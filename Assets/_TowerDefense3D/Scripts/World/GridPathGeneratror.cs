using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace TowerDefense3D
{
    public class GridPathGenerator
    {
        public List<PathCell> GetRandomPath(Grid grid, GridCell start, GridCell end, int complexity = 30,
            int coverArea = 100)
        {
            List<PathCell> path = new List<PathCell>();
            PathCellType cellType = PathCellType.Source;

            int lastRow = 0;
            int lastColumn = 0;

            for (int i = (int)start.coordinate.x; i <= (int)end.coordinate.x; i++)
            {
                bool selectThisRow = (i == (int)start.coordinate.x) || (i == (int)end.coordinate.x) || (Random.Range(0, 100) < complexity && (i != lastRow + 1));
                if (!selectThisRow)
                {
                    cellType = PathCellType.Straight;
                    path.Add(new PathCell(cellType, grid.cells[i, lastColumn])); // fill for same column
                    continue;
                }

                lastRow = i;
                List<PathCell> cellsToAdd = new List<PathCell>();

                if (i == (int)start.coordinate.x || i == (int)end.coordinate.x) // if the row is same as starting cell
                {
                    cellType = (i == (int)start.coordinate.x) ? PathCellType.Source : PathCellType.Destination;
                    GridCell cell = i == (int)start.coordinate.x ? start : end;

                    cellsToAdd.Add(new PathCell(cellType, cell));
                    lastColumn = (int)cell.coordinate.y;
                }
                else
                {
                    cellType = PathCellType.Corner;
                    cellsToAdd.Add(new PathCell(cellType, grid.cells[i, lastColumn]));
                }

                if (i > 0 && i < grid.rows - 1) // if it's not the first or last row
                {
                    int columnIndex = 0;
                    if (i == (int)end.coordinate.x)
                    {
                        columnIndex = (int)path[path.Count - 1].gridCell.coordinate.y;
                    }
                    else
                    {
                        int rngStartIndex =
                            (int)((float)(grid.columns / 2f) - (float)(grid.columns / 2f * coverArea / 100f));
                        int rngEndIndex =
                            (int)((float)(grid.columns / 2f) + (float)(grid.columns / 2f * coverArea / 100f));

                        //Debug.Log($"start random: {rngStartIndex}");
                        //Debug.Log($"end random: {rngEndIndex}");

                        columnIndex = Random.Range(rngStartIndex, rngEndIndex);
                        columnIndex = Mathf.Clamp(columnIndex, 0, grid.columns - 1);
                        while (columnIndex == lastColumn)
                        {
                            columnIndex = Random.Range(rngStartIndex, rngEndIndex);
                            columnIndex = Mathf.Clamp(columnIndex, 0, grid.columns - 1);
                        }
                    }


                    // fill the points in same row
                    int multiplier = columnIndex < lastColumn ? -1 : 1;
                    int current = lastColumn + multiplier;
                    while (current != columnIndex)
                    {
                        cellType = PathCellType.Straight;
                        cellsToAdd.Add(new PathCell(cellType, grid.cells[i, current]));
                        current += multiplier;
                    }

                    cellType = PathCellType.Corner;
                    cellsToAdd.Add(new PathCell(cellType, grid.cells[i, columnIndex])); // add every cell till this
                    lastColumn = columnIndex;
                }

                path.AddRange(cellsToAdd);
            }

            //path[path.Count - 2].coordinate = new Vector2(path[path.Count - 2].coordinate.x, path[path.Count - 1].coordinate.y); // second last item

            return path.ToList();
        }
    }

    [System.Serializable]
    public class PathCell
    {
        public PathCellType type;
        public GridCell gridCell;

        public PathCell(PathCellType l_type, GridCell l_cell)
        {
            type = l_type;
            gridCell = l_cell;
        }
    }
    
    public enum PathCellType
    {
        Corner,
        Straight,
        Source,
        Destination
    }
}