
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace ChickenECS
{
    public partial struct SpawnInitSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        // This OnUpdate accesses managed objects, so it cannot be burst compiled.
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;
            var go = GameObject.Find("EnemyManager");
            var obj = go.GetComponent<EnemyPosObject>();

            var enemyArrManaged = new EnemyArrManaged();
            enemyArrManaged.posArr = obj.posArr;
            var entity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponentData(entity, enemyArrManaged);
        }
    }

    public class EnemyArrManaged : IComponentData
    {
        public Vector3[] posArr;
        // Every IComponentData class must have a no-arg constructor.
        public EnemyArrManaged()
        {
        }
    }
}
