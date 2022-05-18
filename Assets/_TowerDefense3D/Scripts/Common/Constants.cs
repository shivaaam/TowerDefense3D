using UnityEngine;

namespace TowerDefense3D
{
    public class Constants
    {
        public const string version = "0.0.1";

        public const float minHealthBarDistance = 30f;
        public const float maxHealthBarDistance = 42f;

        public const float minEnemySteerSpeed = 0.1f;
        public const float minEnemyMoveSpeed = 0.65f;
        public const float maxEnemyMoveSpeed = 6f;
        public const float maxEnemyLookAheadOffset = 3f;

        public const string animationEvent_AttackHit = "AttackHit";
    }
}
