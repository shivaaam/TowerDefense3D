using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerDefense3D
{
    public abstract class RangeWeapon : WeaponItem
    {
        public RangeWeaponAttributes itemAttributes;
        public AmmoAttributes ammo;
        public AttackRadiusTrigger attackRadiusObject;
        public LayerMask enemyLayer;

        private SphereCollider attackRadiusCollider;

        private void OnEnable()
        {
            attackRadiusObject.OnObjectEnterRadius.AddListener(OnObjectEnterAttackRadius);
            attackRadiusObject.OnObjectExitRadius.AddListener(OnObjectExitAttackRadius);
        }

        private void OnDisable()
        {
            attackRadiusObject.OnObjectEnterRadius.RemoveListener(OnObjectEnterAttackRadius);
            attackRadiusObject.OnObjectExitRadius.RemoveListener(OnObjectExitAttackRadius);
        }

        private void Start()
        {
            InitializeAttackRadiusCollider();
        }

        private void Update()
        {
            if (target == null)
                return;

            // chase target and fire with given fire rate
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
