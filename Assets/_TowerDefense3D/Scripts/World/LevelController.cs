using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense3D
{
    public class LevelController : MonoBehaviour
    {
        public Grid worldGrid;
        private BoxCollider boxCollider;

        public InitialCameraSetupSettings initialCameraSetup;

        void Start()
        {
            worldGrid = new Grid(worldGrid.rows, worldGrid.columns, worldGrid.cellSize, worldGrid.origin);

            boxCollider = GetComponent<BoxCollider>();
            UpdateBoxColliderSize();
            SetCameraInitialView();
        }

        public GridCell GetGridCellAtWorldPosition(Vector3 l_pos)
        {
            return worldGrid.GetGridCellAtWorldPoint(l_pos, transform);
        }

        public Vector3 GetGridCellWorldPosition(GridCell cell)
        {
            Vector3 worldPos = worldGrid.GetGridCellWorldPosition(cell, new Vector2(cell.size / 2f, cell.size / 2f), transform);
            worldPos = new Vector3(worldPos.x, transform.position.y, worldPos.z);
            return worldPos;
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
            CameraController.SetInitialCameraSettings(initialCameraSetup);
        }

        public void RecenterCamera()
        {
            SetCameraInitialView();
        }

        private void OnDrawGizmosSelected()
        {
            worldGrid.DrawGrid(transform);
        }
        
    }
}
