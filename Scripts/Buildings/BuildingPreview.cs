using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPreview : MonoBehaviour
{
    public Material canPlaceMaterial;
    public Material cantPlaceMaterial;
    private MeshRenderer[] meshRenderers;
    private List<GameObject> collidingObjects = new List<GameObject>();

    void Awake()
    {
        meshRenderers = transform.GetComponentsInChildren<MeshRenderer>();
    }

    public void CanPlace() 
    {
        SetMaterial(canPlaceMaterial);
    }

    public void CantPlace() 
    {
        SetMaterial(cantPlaceMaterial);
    }

    void SetMaterial(Material mat) 
    {
        for (int x = 0; x < meshRenderers.Length; x++) {
            Material[] mats = new Material[meshRenderers[x].materials.Length];

            for (int y = 0; y < mats.Length; y++) {
                mats[y] = mat;
            }

            meshRenderers[x].materials = mats;
        }
    }

    public bool CollidingWithObjects() 
    {
        collidingObjects.RemoveAll(x => x == null);
        return collidingObjects.Count > 0;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 12)
            collidingObjects.Add(other.gameObject);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != 12)
            collidingObjects.Remove(other.gameObject);
    }
}
