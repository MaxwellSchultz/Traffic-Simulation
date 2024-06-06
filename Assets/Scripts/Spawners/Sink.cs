using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    private MovingAverageCalculator averageCalculator = new MovingAverageCalculator();

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
    void UpdateUI()
    {
        myUI.GetComponentInChildren<TextMeshProUGUI>().text = "# Cars Eaten: " + numCarsEaten.ToString()
                                                    + "\nAvg Lifespan: " + averageCalculator.CalculateMovingAverage(Time.time).ToString("0.00");
    }

    public void ReactToHit()
    {
        UpdateUI();
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
            averageCalculator.AddValue(Time.time - other.GetComponentInParent<CarAI>().initialTime);
            numCarsEaten++;
            UpdateUI();
            DestroyParentAndChildrenRecursive(other.transform.parent.gameObject);
            SinkLogger.Instance.Log(numCarsEaten + "," + averageCalculator.CalculateMovingAverage(Time.time));
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
