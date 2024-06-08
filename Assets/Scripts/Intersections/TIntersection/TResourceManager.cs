using Org.BouncyCastle.Bcpg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TResourceManager : MonoBehaviour
{
    LinkedList<GameObject> far = new LinkedList<GameObject>();
    LinkedList<GameObject> close = new LinkedList<GameObject>();
    int[] waiting = new int[3] { -1, -1, -1};
    Intersection intersection;
    void Start()
    {
        intersection = gameObject.GetComponent<Intersection>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //Request a turn
    public bool Request(int id, int intent, GameObject car)
    {
        bool returnVal = false;
        switch (id)
        {
            case 0:
            {
                    if (intent == 1) { returnVal = true; }
                    else if (close.Count == 0) { returnVal = true; }
                    else { returnVal = false; waiting[0] = intent; }
                    far.AddLast(car);
                    break;
            }
            case 1:
            {
                    returnVal = true;
                    close.AddLast(car);
                    break;
            }
            case 2:
            {
                    if (far.Count == 0 && close.Count == 0) { returnVal = true; }
                    else if (close.Count == 0 && intent < 2) {  returnVal = true; }
                    else { waiting[2] = intent; }
                    break;
            }
        }
        return returnVal;
    }

    public void Dequeue(GameObject car)
    {
        far.Remove(car);
        close.Remove(car);
    }
    public bool Prompt()
    {
        bool returnVal = false;
        if (waiting[0] != -1)
        {
            if (close.Count == 0 )
            {
                intersection.Go(0);
                waiting[0] = -1;
                returnVal = true;
            }
        } 
        else if (waiting[2] != -1)
        {
            switch (waiting[2])
            {
                case 0:
                    {
                        if (close.Count==0) { returnVal = true; waiting[2] = -1; }
                        break;
                    }
                case 2:
                    {
                        if (far.Count == 0 && close.Count == 0) { returnVal = true; waiting[2] = -1; }
                        break;
                    }
            }
        }
        return returnVal;
    }
}
