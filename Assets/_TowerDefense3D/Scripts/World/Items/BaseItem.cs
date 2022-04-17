using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense3D
{
    public abstract class BaseItem : MonoBehaviour, IPlaceable
    {
        private string isGhostMatParamString = "_IsGhost";
        private Renderer[] renderers;
        private List<Material> materials = new List<Material>();

        private void Start()
        {
            renderers = GetComponentsInChildren<Renderer>();
            GetAllMaterials();
            ToggleGhostMode(true);
        }

        public virtual PlaceableItemAttributes GetItemAttributes()
        {
            return null;
        }

        public virtual void Place(Vector2 coordinate)
        {
            ToggleGhostMode(false);
        }

        public void ToggleGhostMode(bool isActive)
        {
            int val = isActive ? 1 : 0;
            foreach (var mat in materials)
            {
                if (mat.HasProperty(isGhostMatParamString))
                {
                    mat.SetInt(isGhostMatParamString, val);
                }
            }
            Debug.Log($"{name} ghost mode: {isActive}");
        }

        private void GetAllMaterials()
        {
            foreach (var l_renderer in renderers)
            {
                foreach (var mat in l_renderer.materials)
                {
                    if(!materials.Contains(mat))   
                        materials.Add(mat);
                }
            }
        }


    }
}
