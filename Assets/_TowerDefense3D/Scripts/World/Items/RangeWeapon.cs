using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerDefense3D
{
    public abstract class RangeWeapon : WeaponItem
    {
        public RangeWeaponAttributes itemAttributes;
        public AttackRadiusTrigger attackRadiusObject;
        public LayerMask enemyLayer;
        
        public Transform weaponYawRoot;
        public Transform weaponPitchRoot;
        public Transform[] muzzles;

        private SphereCollider attackRadiusCollider;

        protected override void OnEnable()
        {
            base.OnEnable();
            attackRadiusObject.OnObjectEnterRadius.AddListener(OnObjectEnterAttackRadius);
            attackRadiusObject.OnObjectExitRadius.AddListener(OnObjectExitAttackRadius);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            attackRadiusObject.OnObjectEnterRadius.RemoveListener(OnObjectEnterAttackRadius);
            attackRadiusObject.OnObjectExitRadius.RemoveListener(OnObjectExitAttackRadius);
        }

        protected override void Start()
        {
            base.Start();
            InitializeAttackRadiusCollider();
            health = itemAttributes.maxHealth;
        }

        private void Update()
        {
            if (target == null || state != PlaceableItemState.Ready)
                return;

            RotateTowardsTarget(target.GetDamageableTransform(), itemAttributes.trackTargetSpeed, itemAttributes.rotationAxis);
        }

        public override PlaceableItemAttributes GetItemAttributes()
        {
            return itemAttributes;
        }

        public override PlaceableItemType GetPlaceableItemType()
        {
            return itemAttributes.type;
        }

        public override void TakeDamage(int damage)
        {
            health = Mathf.Clamp(health - damage, 0, itemAttributes.maxHealth);
        }

        private void InitializeAttackRadiusCollider()
        {
            attackRadiusCollider = attackRadiusObject.GetComponent<SphereCollider>();

            if (attackRadiusCollider != null)
                attackRadiusCollider.radius = itemAttributes.attackRadius;
        }

        private void OnObjectEnterAttackRadius(GameObject obj)
        {
            BaseEnemy enemy = obj.GetComponent<BaseEnemy>();
            if (enemy == null)
                return;
            
            if (target == null)
            {
                target = obj.GetComponent<IDamageable>();
            }
        }

        private void OnObjectExitAttackRadius(GameObject obj)
        {
            BaseEnemy enemy = obj.GetComponent<BaseEnemy>();
            if (enemy == null)
                return;

            Collider[] colls = Physics.OverlapSphere(attackRadiusCollider.transform.position, attackRadiusCollider.radius, enemyLayer);
            if (colls.Length > 0)
            {
                var nearestEnemy = colls.OrderBy(t => Vector3.Distance(t.transform.position, transform.position)).FirstOrDefault();
                if (nearestEnemy != null)
                    target = nearestEnemy.GetComponent<IDamageable>();
                else
                    target = null;
            }
            else
            {
                target = null;
            }
        }

        private void RotateTowardsTarget(Transform l_target, float speed, Vector2 l_rotationAxis)
        {
            if (l_target == null || health <= 0)
                return;

            // Yaw
            if (l_rotationAxis.y != 0)
            {
                Vector3 yawRootFwd = Vector3.ProjectOnPlane(weaponYawRoot.forward, Vector3.up);
                Vector3 dirToTargetYaw = Vector3.ProjectOnPlane(l_target.position - weaponYawRoot.position, Vector3.up);
                float yawAngle = Vector3.SignedAngle(yawRootFwd, dirToTargetYaw, Vector3.up);
                weaponYawRoot.Rotate(Vector3.up, yawAngle * Time.deltaTime * speed);
            }

            // Pitch
            if (l_rotationAxis.x != 0)
            {
                Vector3 yawFwd = Vector3.ProjectOnPlane(weaponYawRoot.forward, Vector3.up);
                Vector3 pitchRootFwd = weaponPitchRoot.forward;
                Vector3 dirToTargetPitch = Vector3.ProjectOnPlane(l_target.position - weaponPitchRoot.position, weaponYawRoot.right); //Vector3.ProjectOnPlane(l_target.position - weaponPitchRoot.position, Vector3.up);
                
                float currentPitchAngle = Vector3.SignedAngle(yawFwd, pitchRootFwd, weaponYawRoot.right);
                float finalPitchAngle = Vector3.SignedAngle(yawFwd, dirToTargetPitch, weaponYawRoot.right);
                float pitchAngle = Mathf.Lerp(currentPitchAngle, finalPitchAngle, Time.deltaTime * itemAttributes.trackTargetSpeed);

                weaponPitchRoot.localRotation = Quaternion.Euler(pitchAngle, weaponPitchRoot.localEulerAngles.y, 0);
            }
        }

        private void OnDrawGizmos()
        {
            // draw attack radius
            Gizmos.color = new Color(0, 1, 1, 0.25f);
            Gizmos.DrawSphere(transform.position, itemAttributes.attackRadius);


            // draw gizmo on target
            Gizmos.color = Color.red;
            if (target != null)
            {
                Gizmos.DrawSphere(target.GetDamageableTransform().position, 1f);
            }
        }
    }
}
