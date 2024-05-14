using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TrafficLightManager : Intersection
{
    private Queue<GameObject>[] pileup;
    [SerializeField]
    private NavManager navManager;
    [SerializeField]
    private GameObject[] IntersectionBlocks;
    private Coroutine[] coroutines;
    private bool[] active;
    private bool lightSwitch;
    [SerializeField]
    private float lightTimer = 30f;
    private float time;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            pileup[i] = new Queue<GameObject>();
            active[i] = false;
            coroutines[i] = StartCoroutine(GoCoroutine(i));
        }
        time = Time.time;
        lightSwitch = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (Time.time - time > lightTimer)
        {
            Cycle();
            time = Time.time;
        }
    }

    IEnumerator GoCoroutine(int i)
    {
        if (active[i])
        {
            for (int j = 0; j < pileup[i].Count; j++)
            {
                GameObject car;
                if (pileup[i].TryDequeue(out car))
                {
                    if (car != null)
                    {
                        yield return false;
                    }
                    CarAI cai = car.GetComponent<CarAI>();
                    if (cai != null)
                    {
                        yield return false;
                    }
                    cai.ReBakePath();
                    yield return new WaitForSeconds(.5f);
                }

            }
        }
        yield return null;
    }

    private void Cycle()
    {
        IntersectionBlocks[0].SetActive(lightSwitch);
        IntersectionBlocks[1].SetActive(lightSwitch);
        // Active is inverse to active state of block
        active[0] = !lightSwitch;
        active[1] = !lightSwitch;
        IntersectionBlocks[2].SetActive(!lightSwitch);
        IntersectionBlocks[3].SetActive(!lightSwitch);
        active[2] = lightSwitch;
        active[3] = lightSwitch;
        lightSwitch = !lightSwitch;
    }
    public void Enqueue(int i, GameObject car)
    {
        pileup[i].Enqueue(car);
    }
    public override void Go(int id)
    {
        throw new NotImplementedException();
    }
}
