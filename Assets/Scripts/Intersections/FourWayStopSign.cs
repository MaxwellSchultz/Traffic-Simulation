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
    private int IntersectionIter;
    private Queue<int> queue;
    private int lastOpen;


    // Track Cars and Intent
    private Tuple<GameObject,int>[] WaitingCars;
    private LinkedList<GameObject> AllowedCars = new LinkedList<GameObject>();

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
        if (!AllowedCars.Contains(car) && WaitingCars[id]!=null)
        {
            car.GetComponent<CarAI>().Stop();
            WaitingCars[id] = new Tuple<GameObject, int>(car, intent);
            IntersectionResourceManager.Request(id, intent);
        }

    }

    private void LockPath(GameObject car,int id,int intent)
    {
        List<Vector3> path = new List<Vector3>();
        Transform[] points = PathManager.GetTurn(intent, id);
        foreach (Transform t in points)
        {
            path.Add(t.position);
        }
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
