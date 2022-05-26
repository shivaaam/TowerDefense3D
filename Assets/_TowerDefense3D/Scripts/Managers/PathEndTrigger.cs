using UnityEngine;
using UnityEngine.Events;

namespace TowerDefense3D
{
    public class PathEndTrigger : MonoBehaviour
    {
        public LayerMask collisionLayer;
        public UnityEvent<GameObject> OnEnterPathEndTrigger = new UnityEvent<GameObject>();

        private void OnTriggerEnter(Collider coll)
        {
            if ((1 << coll.gameObject.layer | collisionLayer)== collisionLayer)
            {
                OnEnterPathEndTrigger?.Invoke(coll.gameObject);
            }
        }
    }
}
