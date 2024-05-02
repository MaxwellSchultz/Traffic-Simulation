using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intersection : MonoBehaviour
{
    private int counter = 0;
    private NavManager navManager;

    // Start is called before the first frame update
    void Start()
    {
        navManager = GameObject.FindGameObjectWithTag("NavManager").GetComponent<NavManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        counter++;

        if (counter == 300)
        {
            gameObject.transform.Rotate(gameObject.transform.rotation.x, gameObject.transform.rotation.y + 90,
                                                            gameObject.transform.rotation.z);
            navManager.UpdateNavMesh();
            counter = 0;
        }
    }
}
