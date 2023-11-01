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
            if (!posManaged.triger.isTriger)
            {
                return;
            }
            posManaged.triger.isTriger = false;
            //EntityQuery playerEntityQuery = state.EntityManager.CreateEntityQuery(typeof(ChickenPosComponent));

            EntityCommandBuffer entityCommandBuf = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);


            Entity entity = SystemAPI.GetSingleton<ChickenDieComponent>().prefab;

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
