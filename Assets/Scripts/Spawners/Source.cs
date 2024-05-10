using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Source : NetworkBehaviour, IsHitReaction 
{
    public Material hitMaterial; // Reference to the material to be applied when hit
    public GameObject myUIPrefab;
    public GameObject prefabToSpawn;
    private Material originalMaterial;
    private Renderer objectRenderer;
    private GameObject myUI;
    private GameObject myCanvas;
    private Collider spawnCollider;
    private bool canSpawn = true;
    private float rateOfCars = 5; // Default rate of cars per minute that will be spawned
    private float timeSinceLastCar = 0;


    // Method to spawn the prefab on the server
    public void SpawnPrefab()
    {
        // Check if we are the server
        if (!isServer)
        {
            Debug.LogError("Cannot spawn prefab: Not running as server.");
            return;
        }

        // Check if the prefab is registered as spawnable
        if (!NetworkManager.singleton.spawnPrefabs.Contains(prefabToSpawn))
        {
            Debug.LogError("Prefab is not registered as spawnable.");
            return;
        }

        // Spawn the prefab on the server
        GameObject spawnedPrefab = Instantiate(prefabToSpawn, transform.position, transform.rotation);
        CarAI carAI = spawnedPrefab.GetComponent<CarAI>();
        if (carAI)
        {
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Sink");
            int rand = Random.Range(0, gameObjects.Length);
            carAI.CustomDestination = gameObjects[rand].transform;
        }
        timeSinceLastCar = 0;
        spawnedPrefab.SetActive(true);
        NetworkServer.Spawn(spawnedPrefab);
    }


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
                SpawnPrefab();
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
