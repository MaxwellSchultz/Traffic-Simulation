using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourWayStopSign : MonoBehaviour
{
    private LinkedList<GameObject> TrackedCars;
    [SerializeField]
    private NavManager NavManager;
    [SerializeField]
    private float StopDelay = 500f;
    private float Timer;

    [SerializeField]
    private GameObject[] IntersectionBlocks;
    private int IntersectionIter;
    // Start is called before the first frame update
    void Start()
    {
        Timer = 0;
        IntersectionIter = 0;
        TrackedCars = new LinkedList<GameObject>();
 
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        Timer++;
        if (Timer > StopDelay)
        {
            print("aaaaaa");
            CycleBlocks();
            Rebake();
            Repath();
            Timer = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Car"))
        {
            TrackedCars.AddLast(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            GameObject car = other.gameObject;
            TrackedCars.Remove(car);
        }
    }

    private void Repath()
    {
        foreach(GameObject car in TrackedCars)
        {
            car.GetComponent<CarAI>().ReBakePath();
        }
    }
    private void Rebake()
    {
        if (NavManager != null)
        {
            NavManager.UpdateNavMesh();
        }
    }

    private void CycleBlocks()
    {
        if (IntersectionBlocks.Length > 0)
        {
            IntersectionBlocks[IntersectionIter].SetActive(true);
            IntersectionIter++;
            if (IntersectionIter >= IntersectionBlocks.Length) 
            { 
                IntersectionIter = 0; 
            }
            IntersectionBlocks[IntersectionIter].SetActive(false);

        }
    }
}
