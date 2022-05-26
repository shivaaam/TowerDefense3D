using UnityEngine;

namespace TowerDefense3D
{
    public class Constants
    {
        public const string version = "0.0.1";

        public const int maxLevelsCount = 3;

        public const int startPlayerMoney = 2000;
        public const int maxPlayerMoney = 99999;
        public const int startPlayerLives = 3;
        public const int maxPlayerLives = 5;

        public const float minHealthBarDistance = 30f;
        public const float maxHealthBarDistance = 42f;

        public const float minEnemySteerSpeed = 0.1f;
        public const float minEnemyMoveSpeed = 0.65f;
        public const float maxEnemyMoveSpeed = 6f;
        public const float maxEnemyLookAheadOffset = 3f;

        public const float minMissileHeight = 0.5f;
        public const float maxMissileHeight = 100f;

        public const string animationEvent_AttackHit = "AttackHit";
    }
}
