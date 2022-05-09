using UnityEngine;

namespace TowerDefense3D
{
    public class AutonomousAgent
    {
        public static Vector3 Evade(IObstacle[] obstacles, Vector3 currentPosition, float vehicleRadius)
        {
            Vector3 resultantVector = Vector3.zero;
            if (obstacles.Length > 0)
            {
                int obstaclesCount = 0;
                foreach (var obstacle in obstacles)
                {
                    if (obstacle != null)
                    {
                        obstaclesCount += 1;
                        resultantVector += Evade(obstacle, currentPosition);
                    }
                }
                if (obstaclesCount > 0)
                    resultantVector /= obstaclesCount;
            }
            return resultantVector;
        }

        public static Vector3 Evade(IObstacle obstacle, Vector3 currentPosition)
        {
            Vector3 dirToMe = (currentPosition - (obstacle.GetObstacleTransform().position/* + obstacle.GetObstacleVelocity()*/));
            dirToMe = new Vector3(dirToMe.x, 0, dirToMe.z);
            Vector3 resultantVector = (dirToMe.normalized / (dirToMe.magnitude - (obstacle.GetObstacleRadius() + obstacle.GetObstacleRadius())));
            return resultantVector;
        }

        public static Pose Seek(Vector3 target, Vector3 currentPosition, Quaternion currentRotation, float moveSpeed, float seekFactor)
        {
            // seek look ahead point
            Vector3 desired = (target - currentPosition).normalized;
            return new Pose(Vector3.forward * moveSpeed, Quaternion.Lerp(currentRotation, Quaternion.LookRotation(desired), seekFactor));
        }

        public static Pose FollowPath(Path l_path, PathFollowSettings settings, Vector3 currentPosition, Quaternion currentRotation)
        {
            // get nearest point on path
            l_path.splinePath.FindNearestPointTo(currentPosition, out float normalT);
            float additionalNormalT = ((settings.maxMoveSpeed * settings.lookAheadTime) / l_path.splinePath.length);
            float lookAheadNormalT = normalT + additionalNormalT;

            Vector3 nearestPointOnPath = l_path.splinePath.GetPoint(normalT);
            nearestPointOnPath = new Vector3(nearestPointOnPath.x, currentPosition.y, nearestPointOnPath.z); // zeroing the y component

            Vector3 lookAheadPointOnPath = l_path.splinePath.GetPoint(lookAheadNormalT);
            lookAheadPointOnPath = new Vector3(lookAheadPointOnPath.x, currentPosition.y, lookAheadPointOnPath.z); // zeroing the y component

            Pose newPose = new Pose(Vector3.forward * settings.maxMoveSpeed, currentRotation);
            float distanceToNearestPath = Vector3.Distance(currentPosition, nearestPointOnPath);
            if (distanceToNearestPath > l_path.pathRadius)
            {
                // seek look ahead point
                float desiredDistance = Vector3.Distance(lookAheadPointOnPath, currentPosition);
                float lerpFactor = (settings.minSteerSpeed + (desiredDistance - l_path.pathRadius) / (4 - l_path.pathRadius) * (settings.maxSteerSpeed - settings.minSteerSpeed)) * Time.deltaTime;
                newPose = Seek(lookAheadPointOnPath, currentPosition, currentRotation, settings.maxMoveSpeed, lerpFactor);
            }
            return newPose;
        }
    }
}
