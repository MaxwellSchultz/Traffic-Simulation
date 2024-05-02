using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NavManager : MonoBehaviour
{
    public NavMeshSurface navSurface;
    // Start is called before the first frame update
    void Start()
    {
        navSurface.BuildNavMesh();
    }

    // Update is called once per frame
    void Update()
    {
           
    }

    public void UpdateNavMesh()
    {
        navSurface.BuildNavMesh();
        GameObject[] cars = GameObject.FindGameObjectsWithTag("Car");
        foreach (GameObject car in cars)
        {
            car.GetComponent<CarAI>().ReBakePath();
        }
    }
}
