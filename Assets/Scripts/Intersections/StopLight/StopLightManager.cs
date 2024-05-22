using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopLightManager : MonoBehaviour
{
    LightColor[] lights = new LightColor[4];

    int lightCount = 4;

    [SerializeField]
    float lightTime = 10f; // time for each light cycle
    float lightSwapTime = 2f; // time for swap
    float lastLightChange;
    bool lightTransition;

    [SerializeField]
    Intersection Intersection;
    Queue<Tuple<int, int>>[] LightQueues = new Queue<Tuple<int, int>>[4];

    int numQueues = 4;
    bool[] allowLeft = new bool[4];

    int stateCount = 4;
    int currentState;
    LightColor[][] lightStates = new LightColor[4][];
    LightColor[] State_1 = 
    { 
        LightColor.GreenArrow,
        LightColor.Red,
        LightColor.GreenArrow,
        LightColor.Red,
    };
    LightColor[] State_2 = 
    {
        LightColor.Green,
        LightColor.Red,
        LightColor.Green,
        LightColor.Red,
    };
    LightColor[] State_3 =
    {
        LightColor.Red,
        LightColor.GreenArrow,
        LightColor.Red,
        LightColor.GreenArrow,
    };
    LightColor[] State_4 =
    {
        LightColor.Red,
        LightColor.Green,
        LightColor.Red,
        LightColor.Green,
    };



    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numQueues; i++)
        {
            LightQueues[i] = new Queue<Tuple<int, int>>();
        }
        lights = new LightColor[lightCount];
        for (int i = 0;i < lightCount;i++) // Set initial light state
        {
            lights[i] = LightColor.Red;
        }
        lastLightChange = 0;
        lightTransition = false;
        for (int i = 0;i<allowLeft.Length;i++)
            allowLeft[i] = true;

        // Return to later to implement dynamic light rules
        lightStates[0] = State_1;
        lightStates[1] = State_2;
        lightStates[2] = State_3;
        lightStates[3] = State_4;
        currentState = 0;
    }
    private void FixedUpdate()
    {
        CycleLights();
        CheckQueues();
    }

    public bool Request(int id, int intent) // Called to request a path
    {
        bool canGo = false;
        if (lights[id] == LightColor.Green) // If signaled
        {
            if (intent < 2 || allowLeft[id]) // Right or straight || Left and Uturn Legal
            {
                canGo = true;
            }
        } else if (lights[id] == LightColor.GreenArrow)
        {
            if (intent == 2) // Lefts allowed
            {
                canGo = true;
            } else if (intent == 3 && allowLeft[id]) // Uturn attempt and no oncoming traffic
            {
                canGo = true;
            }
        }
        if (!canGo)
        {
            Tuple<int, int> ticket = new Tuple<int, int>(id, intent);
            LightQueues[id].Enqueue(ticket);
        } else
        {
            if (LightQueues[id].Peek() == null)
            {
                allowLeft[(id >= 2 ? id - 2 : id + 2)] = true;
            }
        }

        return canGo;
    }
    private void Revoke(int id) // Called to signal ligth to stop all cars in these zones
    {

    }
    private void CheckQueues()
    {
        Tuple<int,int> output;
        for (int i = 0; i < numQueues; i++)
        {
            if (lights[i] == LightColor.Green) 
                CheckQueue(i);
            else if (lights[i] == LightColor.GreenArrow)
                CheckQueueArrow(i);
            if (!LightQueues[i].TryPeek(out output))
            {
                allowLeft[(i >= 2 ? i - 2 : i + 2)] = true;
            }
        }
        void CheckQueue(int id)
        {
            Tuple<int, int> ticket; 
            if (LightQueues[id].TryPeek(out ticket))
            {
                if (ticket.Item2 < 2 || allowLeft[id])
                {
                    LightQueues[id].Dequeue();
                    Intersection.Go(ticket.Item1);
                }
            }

        }
        void CheckQueueArrow(int id)
        {
            Tuple<int, int> ticket;
            if (LightQueues[id].TryPeek(out ticket))
            {
                if (ticket.Item2 == 3)
                {
                    LightQueues[id].Dequeue();
                    Intersection.Go(ticket.Item1);
                    
                } else if (ticket.Item2 == 4 && allowLeft[id])
                {
                    LightQueues[id].Dequeue();
                    Intersection.Go(ticket.Item1);
                }
            }

        }
    }
    
    
    public void AllowLeft(int id, bool allow)
    {
        allowLeft[id] = allow;
    }
    void CycleLights()
    {
        print(Time.time);
        print(lastLightChange);
        if (lightTransition) // Mid transition
        {
            if (Time.time - lastLightChange > lightSwapTime)
            {
                CompleteCycle();
                lastLightChange = Time.time;
            }
        }
        else // Most of the time
        {
            if (Time.time - lastLightChange > lightTime)
            {
                LightTransition();
                lastLightChange = Time.time;
            }
        }

        void CompleteCycle()
        {
            currentState = (currentState >= stateCount - 1) ? 0 : currentState + 1;

            for (int i = 0; i < lights.Length; i++)
            {
                lights[i] = lightStates[currentState][i];
            }
            lightTransition = false;
            print(lightStates[currentState]);
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
            print("Yellow");
        }
    }

}
