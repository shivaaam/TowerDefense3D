using System.Linq;
using UnityEngine;

namespace TowerDefense3D
{
    public class EnemySpider : GroundMeleeEnemy
    {
        [SerializeField] private Animator animator;
        [SerializeField] private AnimationEventsHandler animationEventsHandler;

        private string animParamIsDead = "IsDead";
        private string animParamIsMoving = "IsMoving";
        private string animParamAttack = "Attack";
        private string animParamHit = "Hit";

        private bool withinAttackRange;

        protected override void OnEnable()
        {
            base.OnEnable();
            animationEventsHandler.animationUnityEvent.AddListener(OnAnimationEvent);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            animationEventsHandler.animationUnityEvent.RemoveListener(OnAnimationEvent);
        }

        protected override void Update()
        {
            base.Update();
            withinAttackRange = target != null && Vector3.Distance(transform.position, target.GetDamageableTransform().position) < Attributes.attackDistance;
            if (health > 0)
            {
                // STATE: Moving
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
                }

                // STATE: Attacking
                else
                {
                    // attack logic goes here
                    if (target != null)
                    {
                        // out if attack range
                        if (!withinAttackRange)
                        {
                            Pose seekPose = AutonomousAgent.Seek(target.GetDamageableTransform().position, transform.position, transform.rotation, currentPathFollowSettings.maxMoveSpeed, currentPathFollowSettings.maxSteerSpeed);
                            calculatedMoveDir = transform.TransformDirection(seekPose.position);
                            transform.rotation = seekPose.rotation;
                        }
                        else
                        {
                            calculatedMoveDir = Vector3.zero;
                            Attack(this, target);
                        }
                    }
                }


                // evade obstacle (separation)
                if (evadeSettings.evadeObstacles/* && !withinAttackRange*/)
                {
                    Physics.OverlapSphereNonAlloc(transform.position, evadeSettings.minEvadeDistance, overlappedColliders, evadeSettings.evadeLayer);
                    if (overlappedColliders[0] != null)
                    {
                        IObstacle[] obstacles = overlappedColliders.Where(t => t != null)
                            ?.Select(y => y.GetComponent<IObstacle>())
                            ?.Where(u => u != (IObstacle)this).ToArray();

                        if (target != null)
                        {
                            obstacles = obstacles.Where(t => t.GetObstacleTransform() != target.GetDamageableTransform()).ToArray();
                        }

                        if (obstacles.Length > 0)
                        {
                            obstacles = obstacles.OrderBy(w => Vector3.Distance(transform.position, w.GetObstacleTransform().position)).ToArray();
                            Vector3 resultantVector = AutonomousAgent.Evade(obstacles, transform.position, GetObstacleRadius());
                            calculatedMoveDir += resultantVector * evadeSettings.maxEvadeForce;
                        }
                    }
                }
                
                Debug.DrawRay(transform.position, calculatedMoveDir, Color.red);
                transform.Translate(calculatedMoveDir * Time.deltaTime, Space.World);

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

        public override void Attack(IDamageDealer attacker, IDamageable defender)
        {
            base.Attack(attacker, defender);
            if (Time.time - LastAttackTime > Attributes.attackRate)
            {
                LastAttackTime = Time.time;
                animator.SetTrigger(animParamAttack);
            }
        }

        public override void DealDamage(IDamageDealer damageDealer, IDamageable defender, int damage, Vector3 hitPoint)
        {
            base.DealDamage(damageDealer, defender, damage, hitPoint);
            defender.TakeDamage(damage, hitPoint);
        }

        private void OnAnimationEvent(string eventName)
        {
            switch (eventName)
            {
                case Constants.animationEvent_AttackHit:
                    if(target != null)
                        DealDamage(this, target, Attributes.attackDamage, transform.position + transform.forward * Attributes.attackDistance);
                    break;
            }
        }
    }
}
