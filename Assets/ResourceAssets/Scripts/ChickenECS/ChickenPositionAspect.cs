using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ChickenECS
{
    public readonly partial struct ChickenPositionAspect : IAspect
    {
        private readonly RefRW<LocalTransform> _localTransform;
        private readonly RefRW<ChickenPosComponent> _chickenPos;

        public void SetPosition()
        {
            _localTransform.ValueRW.Position = _chickenPos.ValueRW.pos;
            _localTransform.ValueRW.Rotation = _chickenPos.ValueRW.rot;
        }
    }

    public struct ChickenPosComponent : IComponentData
    {
        public float3 pos;
        public quaternion rot;
    }
}
