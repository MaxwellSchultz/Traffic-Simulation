using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FourWayStopSign : Intersection
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
    IntersectionResourceManager IntersectionResourceManager;
    [SerializeField]
    private GameObject[] IntersectionBlocks;
    [SerializeField]
    private GameObject[] Paths;
    private Queue<int> queue;


    // Track Cars and Intent
    private Tuple<GameObject,int>[] WaitingCars = new Tuple<GameObject, int>[10];
    private LinkedList<GameObject> AllowedCars = new LinkedList<GameObject>();

    private bool isWaiting;
    // Start is called before the first frame update
    void Start()
    {
        queue = new Queue<int>();
        //Timer = 0;

        TrackedCars = new LinkedList<GameObject>();
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
        /*if (queue.Count>0&&!isWaiting)
        {
            CycleBlocks();
            Rebake();
            Repath();
            //Timer = 0;
        }*/
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Car"))
        {
            other.GetComponentInParent<CarAI>().ReBakePath();
            other.gameObject.transform.parent.Find("StopBox").gameObject.SetActive(true);
            AllowedCars.Remove(other.gameObject);
            /*other.gameObject.transform.parent.GetComponentInChildren<SensorManager>().Active(true);
            TrackedCars.Remove(other.gameObject);
            StopWaiting();*/
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


    public void Signal(int id, GameObject car)
    {
        queue.Enqueue(id);
        TrackedCars.AddLast(car);
    }

    public void SignalIntent(int id, int intent, GameObject car) 
    {
        if (!AllowedCars.Contains(car))
        {
            bool allowedTurn = IntersectionResourceManager.Request(id, intent);
            WaitingCars[id] = new Tuple<GameObject, int>(car, intent);
            if (!allowedTurn)
            {
                car.GetComponent<CarAI>().Stop();
            }
            else
            {
                Go(id);
            }

        }

    }

    private void LockPath(GameObject car,int id,int intent)
    {
        List<Vector3> path = PathManager.GetTurn(intent, id);

        car.GetComponent<CarAI>().ConformToPath(path);
    }

    public override void Go(int id)
    {
        Tuple<GameObject, int> car = WaitingCars[id];
        LockPath(car.Item1,id,car.Item2);
        WaitingCars[id] = null;
        AllowedCars.AddFirst(car.Item1);
        car.Item1.GetComponent<CarAI>().Go();
    }
    
}
