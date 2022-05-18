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

        protected virtual void OnEnable()
        {
            if (perceptionTrigger)
            {
                perceptionTrigger.OnItemEnterRadius.AddListener(OnItemEnterRadius);
                perceptionTrigger.OnItemExitRadius.AddListener(OnItemExitRadius);
            }
        }

        protected virtual void OnDisable()
        {
            if (perceptionTrigger)
            {
                perceptionTrigger.OnItemEnterRadius.RemoveListener(OnItemEnterRadius);
                perceptionTrigger.OnItemExitRadius.RemoveListener(OnItemExitRadius);
            }
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
                Die(hitPoint);
                GameEvents.OnDamageableDie?.Invoke(this);
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
            BuryInGround(5f);
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
            float initY = transform.position.y;
            float yOffset = 2f;
            while (timeElapsed < animationTime)
            {
                float y = Mathf.Lerp(initY, initY-yOffset, timeElapsed / animationTime);
                transform.position = new Vector3(transform.position.x, y, transform.position.z);
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
    }

    public enum EnemyStates
    {
        Moving,
        Attacking
    }
}
