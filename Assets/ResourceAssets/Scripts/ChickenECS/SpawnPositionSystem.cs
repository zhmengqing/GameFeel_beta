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
        EntityQuery playerEntityQuery;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ChickenDieComponent>();

        }

        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer entityCommandBuf = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            EnemyTransManaged posManaged = SystemAPI.ManagedAPI.GetSingleton<EnemyTransManaged>();

            if (posManaged.triger.isDestroy)
            {
                //SystemAPI.Query <ChickenDieComponent> ().WithAll<Obstacle>() >


                //playerEntityQuery = state.EntityManager.CreateEntityQuery(typeof(ChickenDieComponent));
                //playerEntityQuery = state.GetEntityQuery(typeof(ChickenDieComponent));
                //entityCommandBuf.DestroyEntity(playerEntityQuery, EntityQueryCaptureMode.AtRecord);
                //state.EntityManager.DestroyEntity(playerEntityQuery);
                //entityCommandBuf.RemoveComponent(playerEntityQuery, typeof(ChickenDieComponent), EntityQueryCaptureMode.AtRecord);
                posManaged.triger.isDestroy = false;
                return;
            }

            if (!posManaged.triger.isTriger)
            {
                return;
            }
            posManaged.triger.isTriger = false;


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
