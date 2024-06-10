using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointNavigator : MonoBehaviour
{
    characterNavigationController controller;
    public Waypoint currentWaypoint;
    int direction;
    private void Awake()
    {
        controller = GetComponent<characterNavigationController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        direction = Mathf.RoundToInt(Random.Range(0f,1f));
        controller.SetDestination(currentWaypoint.GetPosition());
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.reachedDestination)
        {
            bool shouldBranch = false;
            
            if(currentWaypoint.branches !=null && currentWaypoint.branches.Count > 0)
            {
                shouldBranch = Random.Range(0f,1f) <= currentWaypoint.branchRatio ? true : false;
            }
            
            GreenLight greenLightComponent = currentWaypoint.GetComponent<GreenLight>();

            if (shouldBranch && greenLightComponent != null && greenLightComponent.greenLight)
            {
                currentWaypoint = currentWaypoint.branches[Random.Range(0, currentWaypoint.branches.Count)];
                GreenLight nextgreenLightComponent = currentWaypoint.GetComponent<GreenLight>();
                if(nextgreenLightComponent != null)
                {
                    nextgreenLightComponent.greenLight = true;
                }

            }
            else
            {
                if (direction == 0)
                {
                    if (currentWaypoint.nextWaypoint != null)
                    {
                        currentWaypoint = currentWaypoint.nextWaypoint;
                    }
                    else
                    {
                        currentWaypoint = currentWaypoint.previousWaypoint;
                        direction = 1;
                    }
                }
                else if (direction == 1)
                {
                    if (currentWaypoint.previousWaypoint != null)
                    {
                        currentWaypoint = currentWaypoint.previousWaypoint;
                    }
                    else
                    {
                        currentWaypoint = currentWaypoint.nextWaypoint;
                        direction = 0;
                    }
                }
            }
            controller.SetDestination(currentWaypoint.GetPosition());

        }
    }
}
