using System.Linq;
using UnityEngine;

namespace TowerDefense3D
{
    public class EnemySpider : GroundMeleeEnemy
    {
        [SerializeField] private Animator animator;

        private string animParamIsDead = "IsDead";
        private string animParamIsMoving = "IsMoving";
        private string animParamAttack = "Attack";
        private string animParamHit = "Hit";

        protected override void Update()
        {
            base.Update();
            if (health > 0)
            {
                if (currentState == EnemyStates.Moving)
                {
                    Pose orientation = new Pose(transform.forward, Quaternion.identity);
                    calculatedMoveDir = orientation.position;

                    // path following
                    if (isFollowingPath && currentPath.splinePath != null)
                    {
                        orientation = AutonomousAgent.FollowPath(currentPath, currentPathFollowSettings, transform.position, transform.rotation);
                        calculatedMoveDir = transform.TransformDirection(orientation.position);
                        transform.rotation = orientation.rotation;
                    }

                    // evade obstacle (separation)
                    if (evadeSettings.evadeObstacles)
                    {
                        Physics.OverlapSphereNonAlloc(transform.position, evadeSettings.minEvadeDistance, overlappedColliders, evadeSettings.evadeLayer);
                        if (overlappedColliders[0] != null)
                        {
                            IObstacle[] obstacles = overlappedColliders.Where(t => t != null)
                                ?.Select(y => y.GetComponent<IObstacle>())
                                ?.Where(u => u != (IObstacle)this).ToArray();

                            if (obstacles.Length > 0)
                            {
                                obstacles = obstacles.OrderBy(w => Vector3.Distance(transform.position, w.GetObstacleTransform().position)).ToArray();
                                Vector3 resultantVector = AutonomousAgent.Evade(obstacles, transform.position, GetObstacleRadius());
                                calculatedMoveDir += resultantVector * evadeSettings.maxEvadeForce;
                            }
                        }
                    }
                    transform.Translate(calculatedMoveDir * Time.deltaTime, Space.World);
                }
                else
                {
                    // attack logic goes here
                }



                // Play Move animation
                if (velocity.magnitude > minMoveThreshold)
                {
                    if (!animator.GetBool(animParamIsMoving))
                    {
                        animator.SetBool(animParamIsMoving, true);
                    }
                }
                else
                {
                    if (animator.GetBool(animParamIsMoving))
                    {
                        animator.SetBool(animParamIsMoving, false);
                    }
                }

            }
        }

        protected override void Die(Vector3 hitPoint)
        {
            base.Die(hitPoint);
            animator.SetBool(animParamIsDead, true);
        }
    }
}
