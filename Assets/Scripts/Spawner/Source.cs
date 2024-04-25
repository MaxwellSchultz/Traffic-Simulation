using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Source : MonoBehaviour, IsHitReaction
{
    public Material hitMaterial; // Reference to the material to be applied when hit
    public GameObject myUIPrefab;
    public GameObject car;
    private Material originalMaterial;
    private Renderer objectRenderer;
    private GameObject myUI;
    private GameObject myCanvas;
    private Collider spawnCollider;
    private bool canSpawn = true;
    private float rateOfCars = 5; // Default rate of cars per minute that will be spawned
    private float timeSinceLastCar = 0;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        originalMaterial = objectRenderer.material;
        myCanvas = GameObject.Find("SceneCanvas");
        spawnCollider = GetComponent<BoxCollider>();

        // Show UI
        myUI = Instantiate(myUIPrefab);
        myUI.SetActive(false);
        myUI.transform.SetParent(myCanvas.transform, false);

        // Create listeners
        myUI.GetComponentInChildren<Slider>().onValueChanged.AddListener(OnSliderValueChanged);
        myUI.GetComponentInChildren<Toggle>().onValueChanged.AddListener(OnToggle);

        StartCoroutine(SpawnObjectCoroutine());
    }

    IEnumerator SpawnObjectCoroutine()
    {
        while (true)
        {
            timeSinceLastCar += Time.deltaTime;
            Debug.Log("Elapsd " + timeSinceLastCar + "     target: " + (60 / rateOfCars));
            // Check if the collider is empty before spawning
            if (timeSinceLastCar >= 60 / rateOfCars && canSpawn && !IsColliderOccupied())
            {
                GameObject newObject = Instantiate(car);
                newObject.transform.position = transform.position;
                newObject.SetActive(true);
                CarAI carAI = newObject.GetComponent<CarAI>();
                if (carAI)
                {
                    GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Sink");
                    int rand = Random.Range(0, gameObjects.Length);
                    carAI.CustomDestination = gameObjects[rand].transform;
                }
                timeSinceLastCar = 0;
            }
            yield return null; // Yield to next frame
        }
    }

    bool IsColliderOccupied()
    {
        Collider[] colliders = Physics.OverlapBox(spawnCollider.bounds.center, spawnCollider.bounds.extents, spawnCollider.transform.rotation);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != spawnCollider.gameObject)
            {
                return true;
            }
        }
        return false;
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

    public void OnSliderValueChanged(float value)
    {
        rateOfCars = value;
    }

    public void OnToggle(bool state)
    {
        canSpawn = state;
    }

    public void OnDestroy()
    {
        myUI.GetComponentInChildren<Slider>().onValueChanged.RemoveListener(OnSliderValueChanged);
        myUI.GetComponentInChildren<Toggle>().onValueChanged.RemoveListener(OnToggle);
    }
}
