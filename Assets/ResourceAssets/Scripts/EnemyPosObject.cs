using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPosObject : MonoBehaviour
{
    public int maxDieNum;

    [HideInInspector]
    public TrigerObj triger;

    [HideInInspector]
    public TransformObj trans;
}

public class TrigerObj
{
    public bool isTriger;
    public bool isDestroy;
}

public class TransformObj
{
    public Vector3 pos;
    public Quaternion rot;
}
