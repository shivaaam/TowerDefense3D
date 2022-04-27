using System.Collections;
using System.Collections.Generic;
using System.Linq;
using nStation;
using UnityEngine;

namespace TowerDefense3D
{
    public abstract class RangeWeapon : WeaponItem
    {
        public RangeWeaponAttributes itemAttributes;
        public AttackRadiusTrigger attackRadiusObject;
        
        public Transform weaponYawRoot;
        public Transform weaponPitchRoot;
        public Transform[] muzzles;

        private SphereCollider attackRadiusCollider;
        private float fireInterval;
        private float lastFireTime;

        private int currentMuzzleIndex;

        private Transform BulletsParent {
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

            SetupMultipleAudioSources(muzzles.Length);
            InitializeAttackRadiusCollider();
            health = itemAttributes.maxHealth;
            
            fireInterval = itemAttributes.fireRate > 0 ? 1f / itemAttributes.fireRate : -1f;
        }

        private void Update()
        {
            if (target == null || state != PlaceableItemState.Ready)
                return;

            RotateTowardsTarget(target.GetDamageableTransform(), itemAttributes.trackTargetSpeed, itemAttributes.rotationAxis);
            FireWithInterval(fireInterval);
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

            Collider[] colls = Physics.OverlapSphere(attackRadiusCollider.transform.position, attackRadiusCollider.radius, itemAttributes.ammo.damageLayer);
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
                Vector3 dirToTargetPitch = Vector3.ProjectOnPlane(l_target.position - weaponPitchRoot.position, weaponYawRoot.right);
                
                float currentPitchAngle = Vector3.SignedAngle(yawFwd, pitchRootFwd, weaponYawRoot.right);
                float finalPitchAngle = Vector3.SignedAngle(yawFwd, dirToTargetPitch, weaponYawRoot.right);
                float pitchAngle = Mathf.Lerp(currentPitchAngle, finalPitchAngle, Time.deltaTime * itemAttributes.trackTargetSpeed);

                weaponPitchRoot.localRotation = Quaternion.Euler(pitchAngle, weaponPitchRoot.localEulerAngles.y, 0);
            }
        }

        private void FireWithInterval(float l_interval)
        {
            if (target == null || state != PlaceableItemState.Ready || health <= 0 || l_interval <= 0)
                return;

            if (Time.time - lastFireTime >= l_interval)
            {
                // fire weapon 
                Attack(target);
                lastFireTime = Time.time;
            }
        }

        public override void Attack(IDamageable l_target)
        {
            base.Attack(l_target);
            GameObject bulletObject = AddressableLoader.InstantiateAddressable(itemAttributes.ammo.prefab);
            bulletObject.transform.position = muzzles[currentMuzzleIndex].transform.position;
            bulletObject.transform.rotation = muzzles[currentMuzzleIndex].transform.rotation;
            bulletObject.transform.SetParent(BulletsParent);
            BaseAmmo ammo = bulletObject.GetComponent<BaseAmmo>();
            if (ammo != null)
            {
                ammo.SetAttributes(itemAttributes.ammo);
                ammo.Attack(ammo, target);
            }
            PlayOneShotAudioClip(itemAttributes.ammo.fireSound, true);
            currentMuzzleIndex = (currentMuzzleIndex + 1) % muzzles.Length;
        }

        private void SetAudioSourceSettings(AudioSource src)
        {
            if (audioSources.Length <= 0)
                return;

            src.maxDistance = audioSources[0].maxDistance;
            src.spatialBlend = audioSources[0].spatialBlend;
        }

        private void SetupMultipleAudioSources(int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (audioSources.Length > 0)
                {
                    var src = gameObject.AddComponent<AudioSource>();
                    SetAudioSourceSettings(src);
                }
            }
            audioSources = GetComponents<AudioSource>();
        }

        private void OnDrawGizmos()
        {
            //// draw attack radius
            //Gizmos.color = new Color(0, 1, 1, 0.25f);
            //Gizmos.DrawSphere(transform.position, itemAttributes.attackRadius);


            //// draw gizmo on target
            //Gizmos.color = Color.red;
            //if (target != null)
            //{
            //    Gizmos.DrawSphere(target.GetDamageableTransform().position, 1f);
            //}
        }
    }
}
