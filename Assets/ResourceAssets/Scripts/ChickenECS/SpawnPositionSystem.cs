using System.Numerics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace ChickenECS
{
    public partial struct SpawnPositionSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ChickenDieComponent>();

        }

        public void OnUpdate(ref SystemState state)
        {
            EnemyTransManaged posManaged = SystemAPI.ManagedAPI.GetSingleton<EnemyTransManaged>();

            if (posManaged.triger.isDestroy)
            {
                EntityQueryDesc desc = new EntityQueryDesc()
                {
                    All = new ComponentType[] { ComponentType.ReadOnly<ChickenPosComponent>() },
                };
                EntityQuery query = state.EntityManager.CreateEntityQuery(desc);
                NativeArray<Entity> entityArray = query.ToEntityArray(Allocator.TempJob);
                state.EntityManager.DestroyEntity(entityArray);
                entityArray.Dispose();

                posManaged.triger.isDestroy = false;
                return;
            }

            if (!posManaged.triger.isTriger)
            {
                return;
            }
            posManaged.triger.isTriger = false;


            Entity entity = SystemAPI.GetSingleton<ChickenDieComponent>().prefab;
            EntityCommandBuffer entityCommandBuf = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            //int spawnAmount = posManaged.maxDieNum;
            //if (playerEntityQuery.CalculateEntityCount() < spawnAmount)
            {
                Entity spawnedEntity = entityCommandBuf.Instantiate(entity);
                entityCommandBuf.SetComponent(spawnedEntity, new ChickenPosComponent
                {
                    pos = new float3(posManaged.trans.pos.x, posManaged.trans.pos.y, posManaged.trans.pos.z),
                    rot = new quaternion(posManaged.trans.rot.x, posManaged.trans.rot.y, posManaged.trans.rot.z, posManaged.trans.rot.w)
                });
            }
        }

    }

}
