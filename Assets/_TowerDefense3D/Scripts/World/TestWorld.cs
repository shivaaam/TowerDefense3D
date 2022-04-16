using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace TowerDefense3D
{
    public class TestWorld : MonoBehaviour
    {
        public bool showGrid;
        public bool drawPath;
        public Color gridColor;
        public Color pathColor;
        public Grid worldGrid;
        public List<PathCell> path = new List<PathCell>();

        public Vector2 testStartCell;
        public Vector2 testEndCell;
        public int testPathComplexity = 30;
        public int testCoverArea = 100;

        public WorldPrefabsDictionary worldTilesDictionary = new WorldPrefabsDictionary();

        void Start()
        {
            worldGrid = new Grid(16, 20, 1);
        }

        public List<PathCell> GetRandomPath(Grid grid, GridCell start, GridCell end, int complexity = 30, int coverArea = 100)
        {
            GridPathGenerator pathGenerator = new GridPathGenerator();
            return pathGenerator.GetRandomPath(grid, start, end, complexity, coverArea);
        }

        public void GenerateWorldTiles()
        {
            if (path.Count <= 0)
                return;

            List<GameObject> children = new List<GameObject>();
            for (int i = 0; i < transform.childCount; i++)
            {
                children.Add(transform.GetChild(i).gameObject);
            }

            children.ForEach(t=>DestroyImmediate(t));
            children.Clear();

            for (int i = 0; i < path.Count; i++)
            {
                var obj = Instantiate(worldTilesDictionary[path[i].type], transform);
                obj.transform.position = transform.TransformPoint(new Vector3(path[i].gridCell.coordinate.y * worldGrid.cellSize + 0.5f, 0,
                    path[i].gridCell.coordinate.x * worldGrid.cellSize + 0.5f));

                if(i == 0)
                    SetPathTileRotation(obj.transform, path[i], path[i+1], null);
                else if (i < path.Count - 1)
                    SetPathTileRotation(obj.transform, path[i], path[i+1], path[i-1]);
                else 
                    SetPathTileRotation(obj.transform, path[i], null, path[i-1]);
            }
        }

        private void SetPathTileRotation(Transform cellTransform, PathCell currentCellData, PathCell nextCellData, PathCell prevCellData)
        {
            int rowDiffWithNext = 0;
            int colDiffWithNext = 0;
            int rowDiffWithPrev = 0;
            int colDiffWithPrev = 0;

            if (nextCellData!=null)
            {
                rowDiffWithNext = (int)(nextCellData.gridCell.coordinate.x - currentCellData.gridCell.coordinate.x);
                colDiffWithNext = (int)(nextCellData.gridCell.coordinate.y - currentCellData.gridCell.coordinate.y);
            }

            if (prevCellData != null)
            {
                rowDiffWithPrev = (int)(prevCellData.gridCell.coordinate.x - currentCellData.gridCell.coordinate.x);
                colDiffWithPrev = (int)(prevCellData.gridCell.coordinate.y - currentCellData.gridCell.coordinate.y);
            }

            float angleDiff = Vector2.SignedAngle(Vector2.up, new Vector2(nextCellData.gridCell.coordinate.y, nextCellData.gridCell.coordinate.x) - new Vector2(currentCellData.gridCell.coordinate.y, currentCellData.gridCell.coordinate.x));
            //if (currentCellData.type is PathCellType.Source or PathCellType.Destination or PathCellType.Straight)
            //{
            //    // get next cell data direction
            //}
            if (currentCellData.type is PathCellType.Corner)
            {
                if (colDiffWithPrev < 0 && rowDiffWithNext > 0)
                {
                    angleDiff = 0;
                }
                else if (colDiffWithPrev > 0 && rowDiffWithNext > 0)
                {
                    angleDiff = 90;
                }
                else if (rowDiffWithPrev < 0 && colDiffWithNext > 0)
                {
                    angleDiff = 180;
                }
                else
                {
                    angleDiff = 270;
                }
            }
            cellTransform.Rotate(new Vector3(0, angleDiff, 0));
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

            // draw path points
            if (drawPath)
            {
                if (path.Count > 0)
                {
                    Gizmos.color = pathColor;
                    for (int i = 0; i < path.Count; i++)
                    {
                        Gizmos.DrawCube(transform.TransformPoint(new Vector3(path[i].gridCell.coordinate.y * grid.cellSize + 0.5f, 0, path[i].gridCell.coordinate.x * grid.cellSize + 0.5f)), new Vector3(grid.cellSize, grid.cellSize, grid.cellSize));
                    }
                }
            }

        }

#if UNITY_EDITOR
        [ContextMenu("Make new path")]
        public void GetNewPath()
        {
            path = GetRandomPath(worldGrid, worldGrid.cells[(int)testStartCell.x, (int)testStartCell.y], worldGrid.cells[(int)testEndCell.x, (int)testEndCell.y], testPathComplexity, testCoverArea);
            GenerateWorldTiles();
        }
#endif
    }

    [System.Serializable] public class WorldPrefabsDictionary : SerializableDictionary<PathCellType, GameObject> { }

    public enum WorldCellType
    {
        Environment,
        Path,
        ItemPlaceable,
    }
}
