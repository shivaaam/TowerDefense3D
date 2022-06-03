using System.Security.Cryptography;
using UnityEngine;

namespace TowerDefense3D
{
    public abstract class WeaponItem : BaseItem, IDamageable
    {
        private WeaponItemAttributes Attributes => itemAttributes as WeaponItemAttributes;
        protected IDamageable target;
        protected int health;
        [SerializeField] protected Healthbar healthBar;

        protected override void OnEnable()
        {
            base.OnEnable();
            GameEvents.OnDamageableDie.AddListener(OnDamageableHealthZero);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            GameEvents.OnDamageableDie.RemoveListener(OnDamageableHealthZero);
        }

        protected override void Start()
        {
            base.Start();
            ToggleHealthBar(false);
        }

        protected virtual void OnDamageableHealthZero(IDamageable l_damageable)
        {
            if (target == l_damageable)
                target = null;
        }

        public virtual Transform GetDamageableTransform()
        {
            return transform;
        }

        public Vector3 GetDamageableVelocity()
        {
            return Vector3.zero;
        }

        public int GetCurrentDamageableHealth()
        {
            return health;
        }

        public virtual void TakeDamage(int damage, Vector3 hitPoint)
        {
            health = Mathf.Clamp(health - damage, 0, Attributes.maxHealth);
            if (healthBar != null)
                healthBar.UpdateHealth(health, Attributes.maxHealth);
            if (health <= 0)
            {
                SpawnDestroyParticles();
                DestroyItem();
            }

        }

        public virtual void Attack(IDamageable l_target)
        {

        }

        public virtual void DestroyItem()
        {
            GameEvents.OnDamageableDie?.Invoke(this);
            AddressableLoader.DestroyAndReleaseAddressable(gameObject);
        }

        protected void SpawnDestroyParticles()
        {
            if (!string.IsNullOrEmpty(Attributes.particlesOnDestroyPrefab.AssetGUID))
            {
                GameObject deadParticles = AddressableLoader.InstantiateAddressable(Attributes.particlesOnDestroyPrefab);
                //deadParticles.transform.SetParent(transform);
                deadParticles.transform.position = new Vector3(transform.position.x, transform.position.y + 0.6f, transform.position.z);
            }
        }

        public override void Place(Vector2 coordinate)
        {
            base.Place(coordinate);
            ToggleHealthBar(true);
        }

        protected void ToggleHealthBar(bool isActive)
        {
            healthBar.gameObject.SetActive(isActive);
        }
    }
}
