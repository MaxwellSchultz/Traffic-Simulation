using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StopLight : Intersection
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
    StopLightManager StopLightManager;
    [SerializeField]
    private GameObject[] IntersectionBlocks;
    [SerializeField]
    private GameObject[] Paths;
    private Queue<int> queue;


    // Track Cars and Intent
    private Queue<Tuple<GameObject, int>>[] WaitingCars = Enumerable.Range(1, 5).Select(i => new Queue<Tuple<GameObject,int>>()).ToArray();
    private LinkedList<GameObject> AllowedCars = new LinkedList<GameObject>();

    private bool isWaiting;
    // Start is called before the first frame update
    void Start()
    {
        queue = new Queue<int>();
        //Timer = 0;

        TrackedCars = new LinkedList<GameObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            other.gameObject.transform.parent.Find("StopBox").gameObject.SetActive(false);
            AllowedCars.AddFirst(other.gameObject);
            /*other.gameObject.transform.parent.GetComponentInChildren<SensorManager>().Active(true);
            TrackedCars.Remove(other.gameObject);
            StopWaiting();*/
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            other.GetComponentInParent<CarAI>().ReBakePath();
            other.gameObject.transform.parent.Find("StopBox").gameObject.SetActive(true);
            AllowedCars.Remove(other.gameObject);
            /*other.gameObject.transform.parent.GetComponentInChildren<SensorManager>().Active(true);
            TrackedCars.Remove(other.gameObject);
            StopWaiting();*/
        }
    }
    public override void SignalIntent(int id, int intent, GameObject car)
    {

        //StopLightManager.AllowLeft((id>=2?id-2:id+2), false);

        Tuple<GameObject,int> ticket = new Tuple<GameObject, int>(car, intent);
        WaitingCars[id].Enqueue(ticket);
        bool allowedTurn = StopLightManager.Request(id, intent,car);
        if (!allowedTurn)
        {
            car.GetComponent<CarAI>().Stop();
        } else
        {
            Go(id);
        }


    }

    private void LockPath(GameObject car, int id, int intent)
    {
        List<Vector3> path = PathManager.GetTurn(intent, id);

        car.GetComponent<CarAI>().ConformToPath(path);
    }
    public override void Go(int id)
    {
        Tuple<GameObject, int> car;
        if (WaitingCars[id].TryDequeue(out car))
        {
            LockPath(car.Item1, id, car.Item2);
            AllowedCars.AddFirst(car.Item1);
            car.Item1.GetComponent<CarAI>().Go();
        }
    }


}
