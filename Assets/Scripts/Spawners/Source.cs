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
    [SyncVar(hook = nameof(ToggleChanged))]
    private bool canSpawn = true;
    [SyncVar(hook = nameof(SliderValueChanged))]
    private float rateOfCars = 5; // Default rate of cars per minute that will be spawned
    private float timeSinceLastCar = 0;


    // Method to spawn the prefab on the server
    [Server]
    public void SpawnPrefab()
    {

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
        myUI.GetComponentInChildren<Slider>().onValueChanged.AddListener(CmdSliderValueChanged);
        myUI.GetComponentInChildren<Toggle>().onValueChanged.AddListener(CmdOnToggle);

        StartCoroutine(SpawnObjectCoroutine());
    }
    [Server]
    IEnumerator SpawnObjectCoroutine()
    {
        while (true)
        {
            timeSinceLastCar += Time.deltaTime;
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
    [Command]
    public void CmdSliderValueChanged(float value)
    {
        rateOfCars = value;
    }
    private void SliderValueChanged(float oldValue, float newValue)
    {
        myUI.GetComponentInChildren<Slider>().value = newValue;
    }
    [Command]
    public void CmdOnToggle(bool state)
    {
        canSpawn = state;
    }
    private void ToggleChanged(bool oldValue, bool newValue)
    {
        myUI.GetComponentInChildren<Toggle>().isOn = newValue;
    }
    public void OnDestroy()
    {
        myUI.GetComponentInChildren<Slider>().onValueChanged.RemoveListener(CmdSliderValueChanged);
        myUI.GetComponentInChildren<Toggle>().onValueChanged.RemoveListener(CmdOnToggle);
    }
}
