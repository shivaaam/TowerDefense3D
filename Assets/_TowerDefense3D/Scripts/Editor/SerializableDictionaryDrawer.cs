using UnityEditor;

namespace TowerDefense3D
{
    [CustomPropertyDrawer(typeof(WorldPrefabsDictionary))]
    public class WorldPrefabsSerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer { }

    [CustomPropertyDrawer(typeof(EnemyTypePrefabDictionary))]
    public class EnemyTypePrefabSerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer { }

    [CustomPropertyDrawer(typeof(EnemyCountDictionary))]
    public class EnemyCountSerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer { }

    [CustomPropertyDrawer(typeof(RangeWeapon.MuzzleParticleDictionary))]
    public class MuzzleParticleSerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer { }
}