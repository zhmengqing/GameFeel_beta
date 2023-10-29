using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

namespace ChickenECS
{

    public partial struct ChickenPositionSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            JobHandle jobHandle = new PositionJob().ScheduleParallel(state.Dependency);
            jobHandle.Complete();
        }
    }

    [BurstCompile]
    public partial struct PositionJob : IJobEntity
    {

        [BurstCompile]
        public void Execute(ChickenPositionAspect aspect)
        {
            aspect.SetPosition();
        }
    }

}
