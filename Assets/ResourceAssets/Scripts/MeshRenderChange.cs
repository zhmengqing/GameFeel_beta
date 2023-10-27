using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshRenderChange : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer;

    void Start()
    {
        Mesh mesh = skinnedMeshRenderer.sharedMesh;
        GameObject newGameObject = new GameObject("NewMeshObject");
        MeshFilter meshFilter = newGameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        newGameObject.transform.parent = transform;
        MeshRenderer meshRenderer = newGameObject.AddComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
