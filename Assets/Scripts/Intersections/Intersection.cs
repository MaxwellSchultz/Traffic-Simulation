using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Intersection : MonoBehaviour
{

    public abstract void Go(int id);
    public abstract void SignalIntent(int id, int intent, GameObject car);

}
