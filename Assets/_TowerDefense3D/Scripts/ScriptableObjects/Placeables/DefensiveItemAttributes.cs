using UnityEngine;

namespace TowerDefense3D
{
    [CreateAssetMenu(fileName = "NewDefensiveItem", menuName = "Placeable Items/Defensive Item")]
    public class DefensiveItemAttributes : PlaceableItemAttributes
    {
        public int maxHealth;
    }
}
