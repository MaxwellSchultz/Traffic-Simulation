using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private GameObject spawnObject;
    [SerializeField]
    private float delay = 5.0f;
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void FixedUpdate()
    {
        if ( timer > delay) {
            GameObject newObject = Instantiate(spawnObject);
            newObject.transform.position = transform.position;
            newObject.SetActive(true);
            CarAI carAI = newObject.GetComponent<CarAI>();
            if (carAI)
            {
                GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Sink");
                int rand = Random.Range(0, gameObjects.Length);
                carAI.CustomDestination = gameObjects[rand].transform;
            }
            timer = 0;
        }
        timer++;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Car"))
        {
            Destroy(other.gameObject);
        }
    }
}
