using System.Collections;
using System.Collections.Generic;
using BezierSolution;
using UnityEngine;

namespace TowerDefense3D
{
    public interface IPathFollower
    {
        public void SetFollowPath(Path l_path);

        public Path GetFollowPath();

        public void StartPathFollow(Path path, PathFollowSettings followSettings, EvadeSettings evadeSettings);

        public void StopPathFollow(Path path);


    }

    [System.Serializable]
    public struct PathFollowSettings
    {
        public float maxMoveSpeed;
        public float minSteerSpeed;
        public float maxSteerSpeed;
        public float lookAheadTime;
    }

    [System.Serializable]
    public struct EvadeSettings
    {
        public bool evadeObstacles;
        public LayerMask evadeLayer;
        public float minEvadeDistance;
        public float maxEvadeForce;
    }

    [System.Serializable]
    public struct Path
    {
        public BezierSpline splinePath;
        public float pathRadius;
    }
}
