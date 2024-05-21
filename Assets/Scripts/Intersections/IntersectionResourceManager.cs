using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class IntersectionResourceManager : MonoBehaviour
{
    bool[] Resources = { true, true, true, true };
    bool Available;
    int[] Requested = new int[4];

    [SerializeField]
    Intersection Intersection;

    // Id num, intent, priority
    Queue<Tuple<int, int, int>> Tickets = new Queue<Tuple<int, int, int>>();
    Queue<Tuple<int, int, int>> HighPriority = new Queue<Tuple<int, int, int>>();

    private void Start()
    {
        
    }
    public bool Request(int id, int intent)
    {
        bool canTurn = false;
        if (Tickets.Count > 0 || HighPriority.Count > 0)
        {
            Tuple<int, int, int> ticket = new Tuple<int, int, int>(id, intent, 0);
            Tickets.Enqueue(ticket);
        } else
        {
            canTurn = RequestTurn(id,intent);
            if (!canTurn)
            {
                if (! canTurn )
                {
                    Tuple<int, int, int> ticket = new Tuple<int, int, int>(id,intent,0);
                    Tickets.Enqueue(ticket);
                }
            }
        }
        return canTurn;
    }
    private void FixedUpdate()
    {
        RoundRobin();
    }
    bool CheckTurn(int id, int intent)
    {
        Requested[0] = id;
        if (id < 2)
        {
            Requested[1] = id + 1;
            Requested[2] = id + 2;
        }
        else if (id < 3)
        {
            Requested[1] = id + 1;
            Requested[2] = 0;
        }
        else
        {
            Requested[1] = 0;
            Requested[2] = 1;
        }
        switch (intent)
        {
            // right
            case 0:
                Available = Resources[Requested[0]];
                return Available;


            // straight
            case 1:

                Available = Resources[Requested[0]] && Resources[Requested[1]];
                return Available;

            // left
            case 2:

                Available = Resources[Requested[0]] && Resources[Requested[1]] && Resources[Requested[2]];
                return Available;
            //failure
            default:
                return false;
        }
    }
    bool RequestTurn(int id, int intent)
    {
        Requested[0] = id;
        if (id < 2)
        {
            Requested[1] = id + 1;
            Requested[2] = id + 2;
        } else if (id < 3)
        {
            Requested[1] = id + 1;
            Requested[2] = 0;
        } else
        {
            Requested[1] = 0;
            Requested[2] = 1;
        }

        switch(intent)
        {
            // right
            case 0:
                Available = Resources[Requested[0]];
                if (Available)
                {
                    Resources[Requested[0]] = false;
                }
                return Available;


            // straight
            case 1:

                Available = Resources[Requested[0]]&&Resources[Requested[1]];
                if (Available)
                {
                Resources[Requested[0]] = false;
                Resources[Requested[1]] = false;
                }
                return Available;

            // left
            case 2:

                Available = Resources[Requested[0]] && Resources[Requested[1]] && Resources[Requested[2]];
                if (Available)
                {
                    Resources[Requested[0]] = false;
                    Resources[Requested[1]] = false;
                    Resources[Requested[2]] = false;
                }
                return Available;
            //UTurn
            case 3:
                Available = Resources[0] && Resources[1] && Resources[2] && Resources[3];
                if (Available)
                {
                    Resources[0] = false;
                    Resources[1] = false;
                    Resources[2] = false;
                    Resources[3] = false;
                }
                return Available;
            //failure
            default:
                return false;
        }
    }

    public void ReturnTurn(int id, int intent)
    {
        Requested[0] = id;
        if (id < Requested.Length - 2)
        {
            Requested[1] = id + 1;
            Requested[2] = id + 2;
        }
        else if (id < Requested.Length - 1)
        {
            Requested[1] = id + 1;
            Requested[2] = 0;
        }
        else
        {
            Requested[1] = 0;
            Requested[2] = 1;
        }
        switch (intent)
        {
            // right
            case 0:
                Resources[Requested[0]] = true;
                break;


            // straight
            case 1:
                Resources[Requested[0]] = true;
                Resources[Requested[1]] = true;
                break;

            // left
            case 2:
                Resources[Requested[0]] = true;
                Resources[Requested[1]] = true;
                Resources[Requested[2]] = true;
                break;
            case 3:
                Resources[0] = true;
                Resources[1] = true;
                Resources[2] = true;
                Resources[3] = true;
                break;
            //failure
            default:
                break;
        }
    }
    public void ReturnPartialTurn(int id)
    {
        Resources[id] = true;
    }

    private void RoundRobin()
    {
        Tuple<int, int, int> ticket;
        if (HighPriority.Count > 0)
        {
            ticket = HighPriority.Dequeue();
            if (RequestTurn(ticket.Item1,ticket.Item2))
            {
                // Signal Intersection with id
                Intersection.Go(ticket.Item1);
            } else
            {
                HighPriority.Enqueue(ticket);
            }
        } else if (Tickets.Count > 0)
        {
            ticket = Tickets.Dequeue();
            if (RequestTurn(ticket.Item1, ticket.Item2))
            {
                // Signal Intersection
                Intersection.Go(ticket.Item1);
            } else
            {
                ticket = new Tuple<int, int, int>(ticket.Item1, ticket.Item2, ticket.Item3+1);
                if (ticket.Item3 > 60)
                {
                    HighPriority.Enqueue(ticket);
                } else
                {
                    Tickets.Enqueue(ticket);
                }
            }
        }
    }

}
