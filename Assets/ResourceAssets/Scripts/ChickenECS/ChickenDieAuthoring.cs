
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace ChickenECS
{
    public class ChickenDieAuthoring : MonoBehaviour
    {
        public GameObject prefab;
        class Baker : Baker<ChickenDieAuthoring>
        {
            public override void Bake(ChickenDieAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new ChickenDieComponent
                {
                    prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic)
                });
            }
        }
    }

    public struct ChickenDieComponent : IComponentData
    {
        public Entity prefab;
    }
}
