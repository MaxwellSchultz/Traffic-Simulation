using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StopLightManager : MonoBehaviour
{
    LightColor[] lights = new LightColor[4];

    int lightCount = 4;

    bool[] waitingOnLeft = new bool[4]{ false, false, false, false };
    [SerializeField]
    float currentTime = 0;
    float lightTime = 500f; // time for each light cycle
    float lightSwapTime = 200f; // time for swap
    bool lightTransition;

    GameObject[] InIntersection = new GameObject[4] { null, null, null, null };
    bool[] Blocked = new bool[4] {false,false, false, false};

    LightResource resource;

    [SerializeField]
    GameObject[] RedLights;
    [SerializeField]
    GameObject[] GreenLights;


    [SerializeField]
    Intersection Intersection;
    Queue<int>[] LightQueues = Enumerable.Range(1, 5).Select(i => new Queue<int>()).ToArray();

    int numQueues = 4;
    int[] allowLeft = { 0,0,0,0 };

    int stateCount = 6;
    int currentState;
    LightColor[][] lightStates = new LightColor[6][];
    LightColor[] State_1 = 
    { 
        LightColor.Red,
        LightColor.Red,
        LightColor.GreenArrow,
        LightColor.Red,
    };
    LightColor[] State_2 =
{
        LightColor.GreenArrow,
        LightColor.Red,
        LightColor.Red,
        LightColor.Red,
    };
    LightColor[] State_3 = 
    {
        LightColor.Green,
        LightColor.Red,
        LightColor.Green,
        LightColor.Red,
    };
    LightColor[] State_4 =
    {
        LightColor.Red,
        LightColor.Red,
        LightColor.Red,
        LightColor.GreenArrow,
    };
    LightColor[] State_5 =
    {
        LightColor.Red,
        LightColor.GreenArrow,
        LightColor.Red,
        LightColor.Red,
    };
    LightColor[] State_6 =
    {
        LightColor.Red,
        LightColor.Green,
        LightColor.Red,
        LightColor.Green,
    };



    // Start is called before the first frame update
    void Start()
    {
        resource = GetComponent<LightResource>();
        for (int i = 0; i < numQueues; i++)
        {
            //LightQueues[i] = new Queue<int>();
        }
        lights = new LightColor[lightCount];
        for (int i = 0;i < lightCount;i++) // Set initial light state
        {
            lights[i] = LightColor.Red;
        }
        lightTransition = false;


        // Return to later to implement dynamic light rules
        lightStates[0] = State_1;
        lightStates[1] = State_2;
        lightStates[2] = State_3;
        lightStates[3] = State_4;
        lightStates[4] = State_5;
        lightStates[5] = State_6;
        currentState = 0;
    }
    private void FixedUpdate()
    {
        currentTime++;
        CycleLights();
        CheckQueues();
    }

    public void TellClear(GameObject car)
    {
        for (int i = 0; i < InIntersection.Length; i++) { if (InIntersection[i] == car) { InIntersection[i] = null; Blocked[i] = false; } } 
    }
    public void TellNotClear(GameObject car, int id)
    {
        InIntersection[id] = car;
        Blocked[id] = true; 
    }

    public bool Request(int id, int intent, GameObject car) // Called to request a path
    {
        BlockTurn(id);
        resource.AssignCar(car,id);
        bool canGo = false;
        if (lights[id] == LightColor.Green) // If signaled
        {
            if (intent < 2) // Right or straight
            {
                canGo = true;
                Blocked[id] = true;
            }
        } /*else if (lights[id] == LightColor.GreenArrow)
        {
            if (intent == 2) // Lefts allowed
            {
                canGo = true;
            } else if (intent == 3 && allowLeft[id]) // Uturn attempt and no oncoming traffic
            {
                canGo = true;
            }
        }*/
        if (!canGo)
        {
            //print(id);
            //print(intent);
            LightQueues[id].Enqueue(intent);
        } /*else
        {
            int output;
            if (!LightQueues[id].TryPeek(out output))
            {
                
                allowLeft[(id >= 2 ? id - 2 : id + 2)] = true;
            }
            if (!allowLeft[(id >= 2 ? id - 2 : id + 2)] && intent>2)
            {
                canGo = false;
                LightQueues[id].Enqueue(intent);
            }
        }*/

        return canGo;
    }
    private void Revoke(int id) // Called to signal ligth to stop all cars in these zones
    {

    }

    private void CheckQueues()
    {
        for (int i = 0; i < numQueues; i++)
        {
            print(i + " " +lights[i]);
            if (lights[i] == LightColor.Green) { CheckQueue(i); CheckWaitingOnLeft(i); } 
            else if (lights[i] == LightColor.GreenArrow) { CheckQueueArrow(i); }
            
        }
        void CheckQueue(int id)
        {
            int ticket; 
            if (LightQueues[id].TryPeek(out ticket))
            {
                if (ticket < 2)
                {
                    LightQueues[id].Dequeue();
                    //print("signaling");
                    waitingOnLeft[id] = false;
                    Go(id);
                }
                else
                {
                    waitingOnLeft[id] = true;
                }
                /*else if(RequestTurn(id)) {
                    LightQueues[id].Dequeue();
                    //print("signaling");
                    waitingOnLeft[id] = false;
                    Intersection.Go(id);*/
            }

        }
        void CheckQueueArrow(int id)
        {
            int ticket;
            if (LightQueues[id].TryPeek(out ticket))
            {

                LightQueues[id].Dequeue();
                waitingOnLeft[id] = false;
                Go(id);
                    

            }
            //CheckQueue(id);
        }
        void CheckWaitingOnLeft(int id)
        {
            int ticket;

            if (LightQueues[id].TryPeek(out ticket))
            {
                int opposite = id >= 2 ? id - 2 : id + 2;
                if (waitingOnLeft[id] && !Blocked[opposite])
                {
                    LightQueues[id].Dequeue();
                    waitingOnLeft[id] = false;
                    Go(id);
                    //LightQueues[id >= 2 ? id - 2 : id + 2].Dequeue();
                    //waitingOnLeft[id >= 2 ? id - 2 : id + 2] = false;
                    //Intersection.Go(id >= 2 ? id - 2 : id + 2);
                } else if (waitingOnLeft[id] && waitingOnLeft[opposite])
                {
                    int rand = UnityEngine.Random.Range(0, 2);
                    if (rand == 0)
                    {
                        LightQueues[id].Dequeue();
                        waitingOnLeft[id] = false;
                        Go(id);
                    } else
                    {
                        LightQueues[opposite].Dequeue();
                        waitingOnLeft[opposite] = false;
                        Go(opposite);
                    }
                }
            }


        }
    }
    
    void CycleLights()
    {
        //print(Time.time);
        //print(lastLightChange);
        if (lightTransition) // Mid transition
        {
            if (currentTime > lightSwapTime)
            {
                CompleteCycle();
                //lastLightChange = currentTime;
                currentTime = 0;
            }
        }
        else // Most of the time
        {
            if (currentTime > lightTime)
            {
                //print("transitions");
                LightTransition();
                //lastLightChange = currentTime;
                currentTime = 0;
            }
        }

        void CompleteCycle()
        {
            currentState = (currentState >= stateCount - 1) ? 0 : currentState + 1;

            for (int i = 0; i < lights.Length; i++)
            {
                lights[i] = lightStates[currentState][i];
                if (lightStates[currentState][i] == LightColor.Green || lightStates[currentState][i] == LightColor.GreenArrow)
                {
                    GreenLights[i].SetActive(true);
                    RedLights[i].SetActive(false);
                } else
                {
                    GreenLights[i].SetActive(false);
                    RedLights[i].SetActive(true);
                }
            }
            lightTransition = false;
            //print(lightStates[currentState]);
        }
        void LightTransition()
        {
            for (int i = 0; i < lights.Length; i++)
            {
                if (lights[i] == LightColor.Green || lights[i] == LightColor.GreenArrow)
                {
                    lights[i] = LightColor.Yellow;
                    Revoke(i);
                }
            }
            lightTransition = true;
            //print("Yellow");
        }
    }
    void Go(int index)
    {
        Blocked[index] = true;
        Intersection.Go(index);
    }
    bool RequestTurn(int id)
    {
        int turnBlock = (id >= 2 ? id - 2 : id + 2);
        return allowLeft[turnBlock] <= 0;
    }
    void BlockTurn(int id)
    {
        int turnBlock = (id >= 2 ? id - 2 : id + 2);
        allowLeft[turnBlock] += 1;
    }
    public void ReturnPartialTurn(int id)
    {
        
        allowLeft[id] -= 1;
    }

}
