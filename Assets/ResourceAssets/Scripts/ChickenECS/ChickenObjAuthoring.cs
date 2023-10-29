using Unity.Entities;
using UnityEngine;

namespace ChickenECS
{
    public class ChickenObjAuthoring : MonoBehaviour
    {
        class Baker : Baker<ChickenObjAuthoring>
        {
            public override void Bake(ChickenObjAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new ChickenPosComponent());
            }
        }
    }

}
