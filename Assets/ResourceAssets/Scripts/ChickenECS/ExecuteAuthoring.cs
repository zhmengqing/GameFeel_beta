using Unity.Entities;
using UnityEngine;

namespace ChickenECS
{
    public class ExecuteAuthoring : MonoBehaviour
    {
        class Baker : Baker<ExecuteAuthoring>
        {
            public override void Bake(ExecuteAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent<GameObjectSync>(entity);
            }
        }
    }
    public struct GameObjectSync : IComponentData
    {
    }
}
