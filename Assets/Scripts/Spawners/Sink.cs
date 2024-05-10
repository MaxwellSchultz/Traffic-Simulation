using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sink : MonoBehaviour, IsHitReaction
{   
    public GameObject myUIPrefab;
    public Material hitMaterial;
    private Material originalMaterial;
    private GameObject myUI;
    private GameObject myCanvas;
    private Renderer objectRenderer;
    

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        originalMaterial = objectRenderer.material;
        myCanvas = GameObject.Find("SceneCanvas");
        
        // Show UI
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

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger enter car " + other.gameObject.tag);
        if (other.gameObject.CompareTag("Car"))
        {
            DestroyParentAndChildrenRecursive(other.transform.parent.gameObject);
        }
    }

     void DestroyParentAndChildrenRecursive(GameObject parent)
    {
        // Iterate through all child objects of the parent
        for (int i = parent.transform.childCount - 1; i >= 0; i--)
        {
            // Get the child transform
            Transform child = parent.transform.GetChild(i);

            // Recursively destroy all children of the child
            DestroyParentAndChildrenRecursive(child.gameObject);

            // Destroy the child GameObject
            Destroy(child.gameObject);
        }

        // Destroy the parent GameObject
        Destroy(parent);
    }

}
