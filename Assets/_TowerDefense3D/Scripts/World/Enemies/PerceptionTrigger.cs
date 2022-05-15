using UnityEngine;
using UnityEngine.Events;

namespace TowerDefense3D
{
    public class PerceptionTrigger : MonoBehaviour
    {
        private SphereCollider collider;
        public UnityEvent<BaseItem> OnItemEnterRadius = new UnityEvent<BaseItem>();
        public UnityEvent<BaseItem> OnItemExitRadius = new UnityEvent<BaseItem>();

        private void Awake()
        {
            collider = GetComponent<SphereCollider>();
        }

        private void OnTriggerEnter(Collider l_collider)
        {
            BaseItem item = l_collider.gameObject.GetComponent<BaseItem>();
            if (item != null && item.GetItemState() == PlaceableItemState.Ready)
            {
                OnItemEnterRadius?.Invoke(item);
            }
        }

        private void OnTriggerExit(Collider l_collider)
        {
            BaseItem item = l_collider.gameObject.GetComponent<BaseItem>();
            if (item != null && item.GetItemState() == PlaceableItemState.Ready)
            {
                OnItemExitRadius?.Invoke(item);
            }
        }

        public void SetRadius(float l_radius)
        {
            collider.radius = l_radius;
        }
    }
}
