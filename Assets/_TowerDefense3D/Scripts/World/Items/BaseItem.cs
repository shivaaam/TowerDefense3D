using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense3D
{
    public abstract class BaseItem : MonoBehaviour, IPlaceable, IObstacle
    {
        [SerializeField] protected PlaceableItemAttributes itemAttributes;
        [SerializeField] protected GameObject placeButton;
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
#if UNITY_ANDROID || UNITY_IOS
            TogglePlaceButton(true);
#endif
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
            TogglePlaceButton(false);
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
            if (clip == null)
                return;

            if (audioSources[currentAudioSourceIndex] != null)
            {
                audioSources[currentAudioSourceIndex].volume = makeRandomChanges ? Random.Range(0.85f, 1f) : 1;
                audioSources[currentAudioSourceIndex].pitch = makeRandomChanges ? Random.Range(0.8f, 1.2f) : 1;
                audioSources[currentAudioSourceIndex].PlayOneShot(clip);
                currentAudioSourceIndex = (currentAudioSourceIndex + 1) % audioSources.Length;
            }
        }

        public Transform GetObstacleTransform()
        {
            return transform;
        }

        public Vector3 GetObstacleVelocity()
        {
            return Vector3.zero;
        }

        public float GetObstacleRadius()
        {
            return Mathf.Sqrt(Mathf.Pow(GetItemAttributes().size.x/2, 2) + Mathf.Pow(GetItemAttributes().size.y/2, 2));
        }

        protected void TogglePlaceButton(bool isActive)
        {
            placeButton.SetActive(isActive);
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
