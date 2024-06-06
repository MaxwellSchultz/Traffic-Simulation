using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class Source : NetworkBehaviour, IsHitReaction
{
    public Material hitMaterial; // Reference to the material to be applied when hit
    public GameObject myUIPrefab;
    private GameObject myUI;
    private GameObject carTextUI;
    public GameObject prefabToSpawn;
    private Material originalMaterial;
    private Renderer objectRenderer;
    private GameObject myCanvas;
    private Collider spawnCollider;
    private bool canSpawn = true;
    private float rateOfCars = 5; // Default rate of cars per minute that will be spawned
    private float timeSinceLastCar = 0;
    public int numCarsSpanwed = 0;
    private float totalWaitingTime = 0f;
    private MovingAverageCalculator averageCalculator = new MovingAverageCalculator();

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

        numCarsSpanwed++;
        averageCalculator.AddValue(0);
        SourceLogger.Instance.Log(rateOfCars + "," + averageCalculator.CalculateMovingSum(Time.time));
        timeSinceLastCar = 0;
        UpdateUI();

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

    void UpdateUI()
    {

        myUI.transform.Find("StatsText").GetComponent<TextMeshProUGUI>().text = "Number of Car Spawned: " + numCarsSpanwed.ToString()
                                                        + "\nTarget Spawn Rate: " + (60 / (60 / rateOfCars)) + "/min"
                                                        + "\nCurrent Avg Spawn Rate: " + averageCalculator.CalculateMovingSum(Time.time).ToString() + "/min";
    }
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
        UpdateUI();
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
        UpdateUI();
        RpcSliderValueChanged(value);
    }
    [ClientRpc]
    private void RpcSliderValueChanged(float newValue)
    {
        myUI.GetComponentInChildren<Slider>().value = newValue;
        UpdateUI();
    }
    [Command]
    public void CmdOnToggle(bool state)
    {
        canSpawn = state;
        UpdateUI();
        RpcToggleChanged(state);
    }
    [ClientRpc]
    private void RpcToggleChanged(bool newValue)
    {
        UpdateUI();
        myUI.GetComponentInChildren<Toggle>().isOn = newValue;
    }
    public void OnDestroy()
    {
        myUI.GetComponentInChildren<Slider>().onValueChanged.RemoveListener(CmdSliderValueChanged);
        myUI.GetComponentInChildren<Toggle>().onValueChanged.RemoveListener(CmdOnToggle);
    }
}
