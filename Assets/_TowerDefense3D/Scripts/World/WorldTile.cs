using NaughtyAttributes;
using UnityEngine;

namespace TowerDefense3D
{
    public class WorldTile : MonoBehaviour
    {
        [EnumFlags] public PlaceableItemType validForPlacement;
        public WorldCellType cellType;
    }
}
