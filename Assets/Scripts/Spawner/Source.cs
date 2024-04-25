using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Source : MonoBehaviour, IsHitReaction
{   
    public Material hitMaterial; // Reference to the material to be applied when hit
    public GameObject myUIPrefab;
    private Material originalMaterial;
    private Renderer objectRenderer;
    private GameObject myUI;
    private GameObject myCanvas;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        originalMaterial = objectRenderer.material;
        myCanvas = GameObject.Find("SourceCanvas");
        myUI = Instantiate(myUIPrefab);
        myUI.SetActive(false);
        myUI.transform.SetParent(myCanvas.transform, false);
    }

    public void ReactToHit()
    {
        objectRenderer.material = hitMaterial;
        myUI.SetActive(true);
    }

    public void UnreactToHit()
    {
        objectRenderer.material = originalMaterial;
        myUI.SetActive(false);
    }

    public void OnSliderChange() {

        Debug.Log("slide change");
    }

    public void OnToggleActive() {

        Debug.Log("toggle change");
    }
}
