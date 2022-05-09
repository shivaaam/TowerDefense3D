using System.Collections;
using System.Linq;
using BezierSolution;
using UnityEngine;

namespace TowerDefense3D
{
    public abstract class BaseEnemy : MonoBehaviour, IDamageable, IPathFollower, IObstacle
    {
        [SerializeField] protected EnemyAttributes enemyAttributes;
        private Vector3 previousPosition;
        protected bool isFollowingPath;
        [SerializeField] protected Healthbar healthBar;
        [SerializeField] protected Path currentPath;
        [SerializeField] protected float radius;
        protected int health;
        protected Vector3 velocity;

        [SerializeField] protected PathFollowSettings currentPathFollowSettings;
        [SerializeField] protected EvadeSettings evadeSettings;

        private Collider[] overlappedColliders = new Collider[15];
        private Coroutine moveCoroutine;
        private Vector3 calculatedMoveDir;

        protected virtual void Start()
        {
            health = enemyAttributes.maxHealth;
            SetFollowPath(currentPath);
            StartPathFollow(currentPath, currentPathFollowSettings, evadeSettings);
        }

        private void Update()
        {
            UpdateVelocity();
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
            if (evadeSettings.evadeObstacles/* && Time.time - lastEvadeTime >= evadeUpdateInterval*/)
            {
                //lastEvadeTime = Time.time;
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

        private IEnumerator MoveCoroutine(float updateInterval, float evadeUpdateInterval)
        {
            float lastEvadeTime = 0;
            while (true)
            {
                UpdateVelocity();
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
                if (evadeSettings.evadeObstacles && Time.time - lastEvadeTime >= evadeUpdateInterval)
                {
                    lastEvadeTime = Time.time;
                    Physics.OverlapSphereNonAlloc(transform.position, evadeSettings.minEvadeDistance, overlappedColliders, evadeSettings.evadeLayer);
                    if (overlappedColliders[0] != null)
                    {
                        IObstacle[] obstacles = overlappedColliders.Where(t => t != null)
                            ?.Select(y => y.GetComponent<IObstacle>())
                            ?.Where(u => u != (IObstacle)this).ToArray();

                        if (obstacles.Length > 0)
                        {
                            Vector3 resultantVector = AutonomousAgent.Evade(obstacles, transform.position, GetObstacleRadius());
                            calculatedMoveDir += resultantVector * evadeSettings.maxEvadeForce;
                        }
                    }
                }
                yield return new WaitForSeconds(updateInterval);
            }
        }

        private void UpdateVelocity()
        {
            velocity = (transform.position - previousPosition) * Time.deltaTime;
            previousPosition = transform.position;
        }

        public int GetCurrentDamageableHealth()
        {
            return health;
        }

        public virtual void TakeDamage(int damage)
        {
            health = Mathf.Clamp(health - damage, 0, enemyAttributes.maxHealth);
            if (healthBar != null)
                healthBar.UpdateHealth(health, enemyAttributes.maxHealth);

            if(health <= 0)
                GameEvents.OnDamageableDie?.Invoke(this);
        }

        public Transform GetDamageableTransform()
        {
            return transform;
        }

        public void SetFollowPath(Path l_path)
        {
            currentPath = l_path;
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

        private Path FindNearestPath()
        {
            return new Path();
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
            return radius;
        }
    }
}
