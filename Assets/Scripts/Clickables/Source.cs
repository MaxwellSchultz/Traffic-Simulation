using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Source : MonoBehaviour, IsHitReaction
{   
    public Material hitMaterial; // Reference to the material to be applied when hit
    private Material originalMaterial;
    private Renderer objectRenderer;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        originalMaterial = objectRenderer.material;
    }

    public void ReactToHit()
    {
        objectRenderer.material = hitMaterial;
    }

    public void UnreactToHit()
    {
        objectRenderer.material = originalMaterial;
    }
}
