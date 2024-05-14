using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionResourceManager : MonoBehaviour
{
    bool[] Resources = { true, true, true, true };
    bool Available;
    int[] Requested;

    public bool RequestTurn(int id, int intent)
    {
        Requested[0] = id;
        if (id < Requested.Length-2)
        {
            Requested[1] = id + 1;
            Requested[2] = id + 2;
        } else if (id < Requested.Length-1)
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
            //failure
            default:
                break;
        }
    }
    public void ReturnPartialTurn(int id)
    {
        Resources[id] = true;
    }

}
