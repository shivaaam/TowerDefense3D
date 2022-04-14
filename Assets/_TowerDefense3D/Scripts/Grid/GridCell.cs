using UnityEngine;

namespace TowerDefense3D
{
    [System.Serializable]
    public class GridCell
    {
        public int size;
        public Vector2 coordinate;

        public GridCell(Vector2 l_coord, int l_size)
        {
            coordinate = l_coord;
            size = l_size;
        }
    }
}
