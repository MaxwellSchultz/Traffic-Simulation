using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianSpawner : MonoBehaviour
{
    public GameObject pedestrianPrefab;
    public int pedestriansToSpawn;
    public List<Transform> excludeWaypoints; // List of waypoints to exclude

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        int count = 0;
        while (count < pedestriansToSpawn)
        {
            Transform spawnPoint;
            do
            {
                // Select a random child waypoint
                spawnPoint = transform.GetChild(Random.Range(0, transform.childCount));
            }
            // Ensure the selected spawn point is not in the excludeWaypoints list
            while (excludeWaypoints.Contains(spawnPoint));
            
            // Instantiate pedestrian at the valid spawn point
            GameObject obj = Instantiate(pedestrianPrefab);
            obj.GetComponent<WaypointNavigator>().currentWaypoint = spawnPoint.GetComponent<Waypoint>();
            obj.transform.position = spawnPoint.position;

            yield return new WaitForEndOfFrame();
            count++;
        }
    }
}
