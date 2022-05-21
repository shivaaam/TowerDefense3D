using System.Linq;
using UnityEngine;

namespace TowerDefense3D
{
    public class EnemyUFOMissile : AerialRangeEnemy
    {
        [SerializeField] private GameObject meshObject;
        [SerializeField] private GameObject muzzle;

        private Transform BulletsParent
        {
            get
            {
                string parentObjName = "Bullets";
                var obj = GameObject.Find(parentObjName);
                if (obj == null)
                {
                    obj = new GameObject(parentObjName);
                }
                return obj.transform;
            }
        }

        protected override void Update()
        {
            base.Update();
            if (health > 0)
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
                // align UFO normal with path normal
                if (currentPath.splinePath != null)
                {
                    currentPath.splinePath.FindNearestPointTo(transform.position, out float normalizedT);
                    meshObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, currentPath.splinePath.GetNormal(normalizedT));
                }

                // STATE: Attacking
                if (currentState == EnemyStates.Attacking)
                {
                    if (Time.time - LastAttackTime > Attributes.attackRate)
                    {
                        if(target != null)
                            Attack(this, target);
                    }
                }
            }
        }

        protected override void Die(Vector3 hitPoint)
        {
            // spawn explosion particles and sounds here
            AddressableLoader.DestroyAndReleaseAddressable(gameObject);
        }

        public override void Attack(IDamageDealer attacker, IDamageable defender)
        {
            base.Attack(attacker, defender);

            GameObject bulletObject = AddressableLoader.InstantiateAddressable(Attributes.ammo.prefab);
            bulletObject.transform.position = muzzle.transform.position;
            bulletObject.transform.rotation = muzzle.transform.rotation;
            bulletObject.transform.SetParent(BulletsParent);
            BaseAmmo ammo = bulletObject.GetComponent<BaseAmmo>();
            if (ammo != null)
            {
                ammo.SetAttributes(Attributes.ammo);
                ammo.Attack(ammo, target);
            }
        }
    }
}
