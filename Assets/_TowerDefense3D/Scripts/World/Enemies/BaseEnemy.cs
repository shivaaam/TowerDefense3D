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
        protected int health;
        protected Vector3 velocity;

        [SerializeField] protected PathFollowSettings currentPathFollowSettings;
        [SerializeField] protected EvadeSettings evadeSettings;

        protected Collider[] overlappedColliders = new Collider[15];
        protected Vector3 calculatedMoveDir;

        protected virtual void Start()
        {
            health = enemyAttributes.maxHealth;
            SetFollowPath(currentPath);
            StartPathFollow(currentPath, currentPathFollowSettings, evadeSettings);
        }

        protected virtual void Update()
        {
            UpdateVelocity();
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
        }
    }
}
