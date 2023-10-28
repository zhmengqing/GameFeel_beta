using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace ChickenECS
{
    public partial struct SpawnPositionSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ChickenDieComponent>();
        }

        // This OnUpdate accesses managed objects, so it cannot be burst compiled.
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;
            var entity = SystemAPI.GetSingleton<ChickenDieComponent>().prefab;
            var posManaged = SystemAPI.ManagedAPI.GetSingleton<EnemyArrManaged>();
            var instances = state.EntityManager.Instantiate(entity, 2000, Allocator.Temp);

            var pos = new float3(posManaged.posArr[1].x, posManaged.posArr[1].y, posManaged.posArr[1].z);
            foreach (var ins in instances)
            {
                // Update the entity's LocalTransform component with the new  position.
                var transform = SystemAPI.GetComponentRW<LocalTransform>(ins);
                transform.ValueRW.Position = pos;
            }
        }
    }

    //public partial struct ChickenTransJob : IJobParallelFor
    //{
    //    [BurstCompile]
    //    public void Execute(int index)
    //    {
            
    //    }
    //}
}
