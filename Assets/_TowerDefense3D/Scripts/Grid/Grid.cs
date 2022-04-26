using UnityEngine;

namespace TowerDefense3D
{
    [System.Serializable]
    public class Grid
    {
        public Vector2 origin;
        public int rows;
        public int columns;
        public int cellSize;
        public GridCell[,] cells;

        [SerializeField] private bool drawGrid = true;
        [SerializeField] private Color gridColor = new Color(0, 1, 1, 0.25f);

        public Grid(int l_rows, int l_columns, int l_size, Vector2 l_origin)
        {
            origin = l_origin;
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

        public GridCell GetGridCellAtWorldPoint(Vector3 l_position, Transform referenceTransform)
        {
            Vector3 localPosition = referenceTransform.InverseTransformPoint(l_position) - new Vector3(origin.x, 0, origin.y);

            int column = Mathf.FloorToInt(localPosition.x / cellSize);
            int row = Mathf.FloorToInt(localPosition.z / cellSize);
            
            if (column < columns && row < rows)
                return cells[row, column];

            return null;
        }

        public Vector3 GetCellLocalPosition(GridCell cell, Vector2 offset)
        {
            float x = cell.coordinate.y * cell.size + offset.x;
            float z = cell.coordinate.x * cell.size + offset.y;
            return new Vector3(origin.x, 0, origin.y) + new Vector3(x, 0, z);

        }

        public Vector3 GetGridCellWorldPosition(GridCell cell, Vector2 offset, Transform referenceTransform)
        {
            Vector3 worldPos = referenceTransform.TransformPoint(GetCellLocalPosition(cell, offset));
            return worldPos;
        }


        public void DrawGrid(Transform referenceTransform)
        {
            if (!drawGrid)
                return;

            Gizmos.color = gridColor;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    Gizmos.DrawLine(
                        referenceTransform.TransformPoint(new Vector3(origin.x, 0, origin.y) + new Vector3(j, 0, i) * cellSize),
                        referenceTransform.TransformPoint(new Vector3(origin.x, 0, origin.y) + new Vector3(j, 0, i + 1) * cellSize)); // vertical
                    Gizmos.DrawLine(
                        referenceTransform.TransformPoint(new Vector3(origin.x, 0, origin.y) + new Vector3(j, 0, i) * cellSize),
                        referenceTransform.TransformPoint(new Vector3(origin.x, 0, origin.y) + new Vector3(j + 1, 0, i) * cellSize)); // horizontal

                    if (j == columns - 1) // for last column
                    {
                        Gizmos.DrawLine(
                            referenceTransform.TransformPoint(new Vector3(origin.x, 0, origin.y) + new Vector3(j + 1, 0, i) * cellSize),
                            referenceTransform.TransformPoint(new Vector3(origin.x, 0, origin.y) + new Vector3(j + 1, 0, i + 1) * cellSize)); // vertical
                    }

                    if (i == rows - 1) // for last row
                    {
                        Gizmos.DrawLine(
                            referenceTransform.TransformPoint(new Vector3(origin.x, 0, origin.y) + new Vector3(j, 0, i + 1) * cellSize),
                            referenceTransform.TransformPoint(new Vector3(origin.x, 0, origin.y) + new Vector3(j + 1, 0, i + 1) * cellSize)); // horizontal
                    }
                }
            }
        }
    }
}
