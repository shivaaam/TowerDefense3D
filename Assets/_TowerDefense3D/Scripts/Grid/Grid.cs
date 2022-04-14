using UnityEngine;

namespace TowerDefense3D
{
    [System.Serializable]
    public class Grid
    {
        public int rows;
        public int columns;
        public int cellSize;
        public GridCell[,] cells;

        public Grid(int l_rows, int l_columns, int l_size)
        {
            rows = l_rows;
            columns = l_columns;
            cellSize = l_size;
            cells = new GridCell[rows, columns];

            for (int i = 0; i < l_rows; i++)
            {
                for (int j = 0; j < l_columns; j++)
                {
                    cells[i,j] = new GridCell(new Vector2(i, j), l_size);
                }
            }
        }
    }
}
