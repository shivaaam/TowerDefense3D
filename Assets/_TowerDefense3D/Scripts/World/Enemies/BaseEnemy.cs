using System.Collections;
using System.Linq;
using BezierSolution;
using UnityEngine;
using UnityEngine.Video;

namespace TowerDefense3D
{
    public abstract class BaseEnemy : MonoBehaviour, IDamageable, IPathFollower, IObstacle
    {
        [SerializeField] protected EnemyAttributes enemyAttributes;
        private Vector3 previousPosition;
        protected bool isFollowingPath;
        protected Collider collider;
        [SerializeField] protected GameObject meshObject;
        [SerializeField] protected PerceptionTrigger perceptionTrigger;
        [SerializeField] protected Healthbar healthBar;
        [SerializeField] protected Path currentPath;
        protected int health;
        protected Vector3 velocity;
        protected float minMoveThreshold = 0.3f;

        [SerializeField] protected PathFollowSettings currentPathFollowSettings;
        [SerializeField] protected EvadeSettings evadeSettings;

        protected Collider[] overlappedColliders = new Collider[15];
        protected Vector3 calculatedMoveDir;

        private Coroutine buryCoroutine;
        protected IDamageable target;
        protected EnemyStates currentState;

        private Transform rewardsParent;
        protected Transform RewardsParent 
        {
            get
            {
                if (rewardsParent == null)
                {
                    GameObject obj = GameObject.Find("Rewards");
                    if (obj == null)
                    {
                        obj = new GameObject("Rewards");
                    }
                    rewardsParent = obj.transform;
                }
                return rewardsParent;
            }
        }

        protected virtual void OnEnable()
        {
            if (perceptionTrigger)
            {
                perceptionTrigger.OnItemEnterRadius.AddListener(OnItemEnterRadius);
                perceptionTrigger.OnItemExitRadius.AddListener(OnItemExitRadius);
            }
            GameEvents.OnDamageableDie.AddListener(OnTargetDead);
            GameEvents.OnEnemyReachedPathEnd.AddListener(OnReachedPathEnd);
        }

        protected virtual void OnDisable()
        {
            if (perceptionTrigger)
            {
                perceptionTrigger.OnItemEnterRadius.RemoveListener(OnItemEnterRadius);
                perceptionTrigger.OnItemExitRadius.RemoveListener(OnItemExitRadius);
            }
            GameEvents.OnDamageableDie.RemoveListener(OnTargetDead);
            GameEvents.OnEnemyReachedPathEnd.RemoveListener(OnReachedPathEnd);
        }

        protected virtual void Start()
        {
            collider = GetComponent<Collider>();
            previousPosition = transform.position;
            if(perceptionTrigger && enemyAttributes.perceptionRadius > 0)
                perceptionTrigger.SetRadius(enemyAttributes.perceptionRadius);
        }

        protected virtual void Update()
        {
            UpdateVelocity();
        }

        private void UpdateVelocity()
        {
            velocity = (transform.position - previousPosition) / Time.deltaTime;
            previousPosition = transform.position;
        }

        public int GetCurrentDamageableHealth()
        {
            return health;
        }

        public virtual void TakeDamage(int damage, Vector3 hitPoint)
        {
            health = Mathf.Clamp(health - damage, 0, enemyAttributes.maxHealth);
            if (healthBar != null)
                healthBar.UpdateHealth(health, enemyAttributes.maxHealth);

            if (health <= 0)
            {
                GameEvents.OnDamageableDie?.Invoke(this);
                Die(hitPoint);
            }

        }

        public Transform GetDamageableTransform()
        {
            return transform;
        }

        public Vector3 GetDamageableVelocity()
        {
            return velocity;
        }

        public void SetFollowPath(Path l_path)
        {
            currentPath = l_path;
        }

        public void SetPathFollowSettings(PathFollowSettings followSettings)
        {
            currentPathFollowSettings = followSettings;
        }

        public void SetEvadeSettings(EvadeSettings l_settings)
        {
            evadeSettings = l_settings;
        }

        public Path GetFollowPath()
        {
            return currentPath;
        }

        public void StartPathFollow(Path path, PathFollowSettings followSettings, EvadeSettings l_evadeSettings)
        {
            if(currentPath.splinePath != path.splinePath)
                SetFollowPath(path);

            isFollowingPath = true;
            currentPathFollowSettings = followSettings;
        }

        public void StopPathFollow(Path path)
        {
            isFollowingPath = false;
        }

        public void SetHealth(int l_health)
        {
            health = l_health;
        }

        public Transform GetObstacleTransform()
        {
            return transform;
        }

        public Vector3 GetObstacleVelocity()
        {
            return velocity;
        }

        public float GetObstacleRadius()
        {
            return enemyAttributes.pathFollowSettings.characterRadius;
        }

        protected virtual void Die(Vector3 hitPoint)
        {
            // disable all the components
            collider.enabled = false;
            SpawnDeadParticles();

            // spawn special Rewards
            SpawnSpecialRewards();
        }

        protected void SpawnDeadParticles()
        {
            if (!string.IsNullOrEmpty(enemyAttributes.particlesOnDeadPrefab.AssetGUID))
            {
                GameObject deadParticles = AddressableLoader.InstantiateAddressable(enemyAttributes.particlesOnDeadPrefab);
                deadParticles.transform.SetParent(transform);
                deadParticles.transform.position = transform.position;
            }
        }

        private void SpawnSpecialRewards()
        {
            bool shouldSpawn = Random.Range(0f, 1f) < enemyAttributes.specialRewardProbability;
            if (shouldSpawn)
            {
                var obj = AddressableLoader.InstantiateAddressable(enemyAttributes.specialRewardPrefab);
                obj.transform.SetParent(RewardsParent);
                obj.transform.position = transform.position;

                Collectable objCollectable = obj.GetComponent<Collectable>();
                if (objCollectable != null)
                    objCollectable.collectableAmount = enemyAttributes.specialReward;
            }
        }


        public void BuryInGround(float delayTime)
        {
            if(buryCoroutine != null)
                StopCoroutine(buryCoroutine);
            buryCoroutine = StartCoroutine(BuryCorouitine(delayTime));
        }

        private IEnumerator BuryCorouitine(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            float animationTime = 1f;
            float timeElapsed = 0f;
            float initY = meshObject.transform.position.y;
            float yOffset = 2f;
            while (timeElapsed < animationTime)
            {
                float y = Mathf.Lerp(initY, initY-yOffset, timeElapsed / animationTime);
                meshObject.transform.position = new Vector3(meshObject.transform.position.x, y, meshObject.transform.position.z);
                timeElapsed += Time.deltaTime;
                yield return null; 
            }

            yield return new WaitForSeconds(animationTime);
            AddressableLoader.DestroyAndReleaseAddressable(gameObject);
        }

        public void SetState(EnemyStates l_state)
        {
            currentState = l_state;
        }

        public void SetTarget(IDamageable l_target)
        {
            target = l_target;
            OnSetTarget(l_target);
        }

        private void OnSetTarget(IDamageable l_target)
        {
            if (l_target == null)
                SetState(EnemyStates.Moving);
            else
                SetState(EnemyStates.Attacking);
        }

        private void OnItemEnterRadius(BaseItem item)
        {
            if (item is IDamageable damageable)
            {
                GameEvents.OnItemEnterEnemyRadius?.Invoke(damageable, this);
            }
        }

        private void OnItemExitRadius(BaseItem item)
        {
            if (item is IDamageable damageable)
            {
                GameEvents.OnItemExitEnemyRadius?.Invoke(damageable, this);
            }
        }

        private void OnTargetDead(IDamageable l_target)
        {
            if (target == l_target)
                target = null;
        }

        private void OnReachedPathEnd(BaseEnemy l_enemy)
        {
            if (l_enemy == this)
            {
                // spawn vanish particles (whoosh)
                GameEvents.OnDamageableDie?.Invoke(this);
                AddressableLoader.DestroyAndReleaseAddressable(gameObject);
            }
        }

        public int GetKillReward()
        {
            return enemyAttributes.killReward;
        }
    }

    public enum EnemyStates
    {
        Moving,
        Attacking
    }
}
