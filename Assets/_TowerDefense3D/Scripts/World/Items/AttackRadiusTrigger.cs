using UnityEngine;
using UnityEngine.Events;

namespace TowerDefense3D
{
    public class AttackRadiusTrigger : MonoBehaviour
    {
        public UnityEvent<GameObject> OnObjectEnterRadius = new UnityEvent<GameObject>();
        public UnityEvent<GameObject> OnObjectExitRadius = new UnityEvent<GameObject>();

        private void OnTriggerEnter(Collider coll)
        {
            OnObjectEnterRadius?.Invoke(coll.gameObject);
        }

        private void OnTriggerExit(Collider coll)
        {
            OnObjectExitRadius?.Invoke(coll.gameObject);
        }
    }
}
