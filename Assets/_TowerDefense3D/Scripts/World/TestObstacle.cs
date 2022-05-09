using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense3D
{
    public class TestObstacle : MonoBehaviour, IObstacle
    {
        [SerializeField] private float radius;

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
            return radius;
        }
    }
}
