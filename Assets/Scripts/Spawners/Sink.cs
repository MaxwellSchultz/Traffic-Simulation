using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sink : MonoBehaviour, IsHitReaction
{   
    public int numCarsEaten = 0;
    public float totalCarsLife = 0;
    public float averageCarLife = 0;
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
        //myUI.SetActive(false);
        myUI.transform.SetParent(myCanvas.transform, false);
        myUI.GetComponent<FollowWorld>().lookAt = GetComponent<Transform>();
    }

    void FixedUpdate()
    {
        if (numCarsEaten != 0)
            averageCarLife = totalCarsLife / numCarsEaten;
        
        myUI.GetComponent<SinkUIText>().sinkText.text = "# Cars Eaten: " + numCarsEaten.ToString() 
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
        if (other.gameObject.CompareTag("Car"))
        {
            totalCarsLife += other.GetComponentInParent<CarAI>().timeElapsed;
            Destroy(other.GetComponentInParent<CarAI>().carUI);
            Destroy(other.gameObject);
            numCarsEaten++;
        }
    }

}
