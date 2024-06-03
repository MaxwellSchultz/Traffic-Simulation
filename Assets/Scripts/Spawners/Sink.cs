using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sink : MonoBehaviour, IsHitReaction
{
    public Material hitMaterial;
    private Material originalMaterial;
    public GameObject myUIPrefab;
    private GameObject myUI;
    public GameObject textUIPrefab;
    private GameObject textUI;
    private GameObject myCanvas;
    private Renderer objectRenderer;
    public int numCarsEaten = 0;
    public float totalCarsLife = 0;
    public float averageCarLife = 0;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        originalMaterial = objectRenderer.material;
        myCanvas = GameObject.Find("SceneCanvas");

        // Show UI
        myUI = Instantiate(myUIPrefab);
        myUI.SetActive(false);
        myUI.transform.SetParent(myCanvas.transform, false);

        textUI = Instantiate(textUIPrefab);
        textUI.SetActive(true);
        textUI.transform.SetParent(myCanvas.transform, false);
        textUI.GetComponent<FollowWorld>().lookAt = GetComponent<Transform>();
    }
    void FixedUpdate()
    {
        if (numCarsEaten != 0)
            averageCarLife = totalCarsLife / numCarsEaten;

        textUI.GetComponent<UIText>().text.text = "# Cars Eaten: " + numCarsEaten.ToString()
                                                    + "\nAvg Lifespan: " + averageCarLife.ToString("0.00");
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
        //Debug.Log("Trigger enter car " + other.gameObject.tag);
        if (other.gameObject.CompareTag("Car"))
        {
            totalCarsLife += other.GetComponentInParent<CarAI>().timeElapsed;
            Destroy(other.GetComponentInParent<CarAI>().textUI);
            DestroyParentAndChildrenRecursive(other.transform.parent.gameObject);
            numCarsEaten++;
            SinkLogger.Instance.Log(numCarsEaten + "," + averageCarLife);
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
