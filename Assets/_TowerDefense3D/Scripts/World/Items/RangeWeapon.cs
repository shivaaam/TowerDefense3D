using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerDefense3D
{
    public abstract class RangeWeapon : WeaponItem
    {
        public RangeWeaponAttributes Attributes => itemAttributes as RangeWeaponAttributes;
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
            health = Attributes.maxHealth;
            
            fireInterval = Attributes.fireRate > 0 ? 1f / Attributes.fireRate : -1f;
        }

        private void Update()
        {
            if (target == null || state != PlaceableItemState.Ready)
                return;

            Vector3 predictedPos = target.GetDamageableTransform().position + target.GetDamageableVelocity() * (Attributes.lookAheadFactor > 0 ? Attributes.lookAheadFactor : 1);
            RotateTowardsTarget(predictedPos, Attributes.trackTargetSpeed, Attributes.rotationAxis);
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

        private void InitializeAttackRadiusCollider()
        {
            attackRadiusCollider = attackRadiusObject.GetComponent<SphereCollider>();

            if (attackRadiusCollider != null)
                attackRadiusCollider.radius = Attributes.attackRadius;
        }

        private void OnObjectEnterAttackRadius(GameObject obj)
        {
            BaseEnemy enemy = obj.GetComponent<BaseEnemy>();
            if (enemy == null || enemy.GetCurrentDamageableHealth() <= 0)
                return;
            
            if (target == null)
            {
                target = obj.GetComponent<IDamageable>();
            }
        }

        private void OnObjectExitAttackRadius(GameObject obj)
        {
            BaseEnemy enemy = obj.GetComponent<BaseEnemy>();
            if (enemy == null || enemy.GetCurrentDamageableHealth() <= 0)
                return;

            Collider[] colls = Physics.OverlapSphere(attackRadiusCollider.transform.position, attackRadiusCollider.radius, Attributes.ammo.damageLayer);
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

        private void RotateTowardsTarget(Vector3 l_targetPos, float speed, Vector2 l_rotationAxis)
        {
            if (health <= 0)
                return;

            // Yaw
            if (l_rotationAxis.y != 0)
            {
                Vector3 yawRootFwd = Vector3.ProjectOnPlane(weaponYawRoot.forward, Vector3.up);
                Vector3 dirToTargetYaw = Vector3.ProjectOnPlane(l_targetPos - weaponYawRoot.position, Vector3.up);
                float yawAngle = Vector3.SignedAngle(yawRootFwd, dirToTargetYaw, Vector3.up);
                weaponYawRoot.Rotate(Vector3.up, yawAngle * Time.deltaTime * speed);
            }

            // Pitch
            if (l_rotationAxis.x != 0)
            {
                Vector3 yawFwd = Vector3.ProjectOnPlane(weaponYawRoot.forward, Vector3.up);
                Vector3 pitchRootFwd = weaponPitchRoot.forward;
                Vector3 dirToTargetPitch = Vector3.ProjectOnPlane(l_targetPos - weaponPitchRoot.position, weaponYawRoot.right);
                
                float currentPitchAngle = Vector3.SignedAngle(yawFwd, pitchRootFwd, weaponYawRoot.right);
                float finalPitchAngle = Vector3.SignedAngle(yawFwd, dirToTargetPitch, weaponYawRoot.right);
                float pitchAngle = Mathf.Lerp(currentPitchAngle, finalPitchAngle, Time.deltaTime * Attributes.trackTargetSpeed);

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
            GameObject bulletObject = AddressableLoader.InstantiateAddressable(Attributes.ammo.prefab);
            bulletObject.transform.position = muzzles[currentMuzzleIndex].transform.position;
            bulletObject.transform.rotation = muzzles[currentMuzzleIndex].transform.rotation;
            bulletObject.transform.SetParent(BulletsParent);
            BaseAmmo ammo = bulletObject.GetComponent<BaseAmmo>();
            if (ammo != null)
            {
                ammo.SetAttributes(Attributes.ammo);
                ammo.Attack(ammo, target);
            }
            PlayOneShotAudioClip(Attributes.ammo.fireSound, true);
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

        protected override void OnDamageableHealthZero(IDamageable l_damageable)
        {
            base.OnDamageableHealthZero(l_damageable);
            OnObjectExitAttackRadius(l_damageable.GetDamageableTransform().gameObject);
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
