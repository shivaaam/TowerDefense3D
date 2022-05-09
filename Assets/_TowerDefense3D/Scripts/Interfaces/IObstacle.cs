using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense3D
{
    public interface IObstacle
    {
        public Transform GetObstacleTransform();
        public Vector3 GetObstacleVelocity();
        public float GetObstacleRadius();
    }
}
