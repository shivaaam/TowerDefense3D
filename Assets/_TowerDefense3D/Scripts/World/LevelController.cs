using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense3D
{
    public class LevelController : MonoBehaviour
    {
        public bool showGrid;
        public Color gridColor;
        public Grid worldGrid;

        private BoxCollider boxCollider;

        void Start()
        {
            worldGrid = new Grid(worldGrid.rows, worldGrid.columns, worldGrid.cellSize);

            boxCollider = GetComponent<BoxCollider>();
            UpdateBoxColliderSize();
        }

        public GridCell GetGridCellAtWorldPosition(Vector3 l_pos)
        {
            Vector3 localPos = transform.InverseTransformPoint(l_pos);
            int row = Mathf.FloorToInt(localPos.z / worldGrid.cellSize);
            int col = Mathf.FloorToInt(localPos.x / worldGrid.cellSize);
            
            return worldGrid.cells[row, col];
        }

        public Vector3 GetGridCellWorldPosition(GridCell cell)
        {
            float x = (cell.coordinate.y * cell.size) + cell.size/2f;
            float z = (cell.coordinate.x * cell.size) + cell.size/2f;

            return new Vector3(x, transform.position.y, z);
        }

        private void UpdateBoxColliderSize()
        {
            if (GetComponent<MeshCombiner2>() == null)
            {
                boxCollider.enabled = false;
                return;
            }

            if (boxCollider == null)
                return;

            float x = worldGrid.columns * worldGrid.cellSize;
            float z = worldGrid.rows * worldGrid.cellSize;

            boxCollider.size = new Vector3(x, 0.2f, z);
            boxCollider.center = boxCollider.size / 2f;
        }

        private void SetCameraInitialView()
        {

        }

        private void OnDrawGizmosSelected()
        {
            Grid grid = worldGrid;

            // draw grid
            if (showGrid)
            {
                Gizmos.color = gridColor;
                for (int i = 0; i < grid.rows; i++)
                {
                    for (int j = 0; j < grid.columns; j++)
                    {
                        Gizmos.DrawLine(transform.TransformPoint(new Vector3(j, 0,i) * grid.cellSize), transform.TransformPoint(new Vector3(j, 0, i+1) * grid.cellSize)); // vertical
                        Gizmos.DrawLine(transform.TransformPoint(new Vector3(j, 0,i) * grid.cellSize), transform.TransformPoint(new Vector3(j+1, 0, i) * grid.cellSize)); // horizontal
                        if (j == grid.columns - 1) // for last column
                        {
                            Gizmos.DrawLine(transform.TransformPoint(new Vector3(j+1, 0,i) * grid.cellSize), transform.TransformPoint(new Vector3(j+1, 0, i+1) * grid.cellSize)); // vertical
                        }
                        if (i == grid.rows - 1) // for last column
                        {
                            Gizmos.DrawLine(transform.TransformPoint(new Vector3(j, 0, i+1) * grid.cellSize), transform.TransformPoint(new Vector3(j + 1, 0, i + 1) * grid.cellSize)); // vertical
                        }
                    }
                }
            }

        }
        
    }
}
