using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourWayStopSign : MonoBehaviour
{
    private LinkedList<GameObject> TrackedCars;
    [SerializeField]
    private NavManager NavManager;
    //[SerializeField]
    //private float StopDelay = 100f;
    //private float Timer;
    [SerializeField]
    private PathManager PathManager;

    [SerializeField]
    private GameObject[] IntersectionBlocks;
    [SerializeField]
    private GameObject[] Paths;
    private int IntersectionIter;
    private Queue<int> queue;
    private int lastOpen;


    // Track Cars and Intent
    private Queue<Tuple<GameObject, int, int>> WaitingCars = new Queue<Tuple<GameObject, int, int>>();

    private bool isWaiting;
    // Start is called before the first frame update
    void Start()
    {
        queue = new Queue<int>();
        //Timer = 0;
        IntersectionIter = 0;
        TrackedCars = new LinkedList<GameObject>();
        lastOpen = -1;
        isWaiting = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        /*if (queue.Count > 0)
        {
            Timer++;
        }*/
        if (queue.Count>0&&!isWaiting)
        {
            CycleBlocks();
            Rebake();
            Repath();
            //Timer = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Car"))
        {
            other.gameObject.transform.parent.GetComponentInChildren<SensorManager>().Active(true);
            TrackedCars.Remove(other.gameObject);
            StopWaiting();
        }
    }
    private void OnTriggerExit(Collider other)
    {
       /* if (other.CompareTag("Car"))
        {
            GameObject car = other.gameObject;
            TrackedCars.Remove(car);
        }*/
    }

    private void Repath()
    {
        foreach(GameObject car in TrackedCars)
        {
            print("REPATH");
            car.GetComponentInParent<CarAI>().ReBakePath();
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
        if (queue.TryDequeue(out IntersectionIter))
        {

            IntersectionBlocks[IntersectionIter].SetActive(false);
            //Paths[IntersectionIter].SetActive(true);
            lastOpen = IntersectionIter;
        }
        isWaiting = true;
        
    }
    private void StopWaiting()
    {
        IntersectionBlocks[lastOpen].SetActive(true);
        //Paths[lastOpen].SetActive(false);
        isWaiting = false;
    }
    public void CloseLastOpen()
    {

        IntersectionBlocks[lastOpen].SetActive(true);
        //Paths[lastOpen].SetActive(false);
    }
    public void Signal(int id, GameObject car)
    {
        queue.Enqueue(id);
        TrackedCars.AddLast(car);
    }

    public void SignalIntent(int id, int intent, GameObject car) 
    {
        WaitingCars.Enqueue(new Tuple<GameObject, int, int>(car, id, intent));
    }

    private void LockPath(int id)
    {
        
    }
    
}
