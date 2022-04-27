using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense3D
{
    public abstract class BaseItem : MonoBehaviour, IPlaceable
    {
        public PlaceableItemState state;

        protected AudioSource[] audioSources;
        private string isGhostMatParamString = "_IsGhost";
        private Renderer[] renderers;
        private List<Material> materials = new List<Material>();

        private int currentAudioSourceIndex;

        protected virtual void OnEnable()
        {

        }

        protected virtual void OnDisable()
        {

        }

        protected virtual void Start()
        {
            audioSources = GetComponents<AudioSource>();
            renderers = GetComponentsInChildren<Renderer>();
            GetAllMaterials();
            ToggleGhostMode(true);
            SetItemState(PlaceableItemState.GettingPlaced);
        }

        public virtual PlaceableItemAttributes GetItemAttributes()
        {
            return null;
        }

        public virtual PlaceableItemType GetPlaceableItemType()
        {
            return PlaceableItemType.None;
        }

        public virtual void Place(Vector2 coordinate)
        {
            SetItemState(PlaceableItemState.Ready);
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
        }

        public PlaceableItemState GetItemState()
        {
            return state;
        }

        public void SetItemState(PlaceableItemState l_state)
        {
            state = l_state;
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

        protected void PlayOneShotAudioClip(AudioClip clip, bool makeRandomChanges = false)
        {
            if (audioSources[currentAudioSourceIndex] != null)
            {
                audioSources[currentAudioSourceIndex].volume = makeRandomChanges ? Random.Range(0.85f, 1f) : 1;
                audioSources[currentAudioSourceIndex].pitch = makeRandomChanges ? Random.Range(0.8f, 1.2f) : 1;
                audioSources[currentAudioSourceIndex].PlayOneShot(clip);
                currentAudioSourceIndex = (currentAudioSourceIndex + 1) % audioSources.Length;
            }
        }
    }

    public enum PlaceableItemType
    {
        None = 0,
        Weapon = 1 << 0,
        Defense = 1 << 1,
        Cosmetic = 1 << 2
    }

    public enum PlaceableItemState
    {
        GettingPlaced,
        UnderConstruction,
        Ready,
    }
}
