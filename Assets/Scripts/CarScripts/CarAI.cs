using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using static Unity.VisualScripting.Member;
// using static UnityEditor.PlayerSettings;

public class CarAI : MonoBehaviour
{
    [Header("Car Wheels (Wheel Collider)")]// Assign wheel Colliders through the inspector
    public WheelCollider frontLeft;
    public WheelCollider frontRight;
    public WheelCollider backLeft;
    public WheelCollider backRight;

    [Header("Car Wheels (Transform)")]// Assign wheel Transform(Mesh render) through the inspector
    public Transform wheelFL;
    public Transform wheelFR;
    public Transform wheelBL;
    public Transform wheelBR;

    [Header("Car Front (Transform)")]// Assign a Gameobject representing the front of the car
    public Transform carFront;

    [Header("General Parameters")]// Look at the documentation for a detailed explanation 
    public List<string> NavMeshLayers;
    public int MaxSteeringAngle = 45;
    public int MaxRPM = 150;

    [Header("Debug")]
    public bool ShowGizmos;
    public bool Debugger;

    [Header("Destination Parameters")]// Look at the documentation for a detailed explanation
    public bool Patrol = true;
    public Transform CustomDestination;

    [HideInInspector] public bool move;// Look at the documentation for a detailed explanation

    private Vector3 PostionToFollow = Vector3.zero;
    private int currentWayPoint;
    private float AIFOV = 45;
    private bool allowMovement;
    private int NavMeshLayerBite;
    private List<Vector3> waypoints = new List<Vector3>();
    private float LocalMaxSpeed;
    private int Fails;
    public bool FailFOV;
    public bool FailInvalid;
    private float MovementTorque = 1;
    public float initialTime;
    private float TurnDistance =70f;
    private float TurnIntentAngle = 45f;
    private bool TurnLock;
    private bool slowSpeed;
    private float slowRpm = 150;
    void Awake()
    {
        currentWayPoint = 0;
        allowMovement = true;
        move = true;
        TurnLock = false;
        slowSpeed = false;
    }

    void Restart()
    {
        waypoints.Clear();
    }

    void Start()
    {
        FailFOV = false;
        FailInvalid = false;
        initialTime = Time.time;
        GetComponent<Rigidbody>().centerOfMass = Vector3.zero;
        CalculateNavMashLayerBite();
    }

    void FixedUpdate()
    {
        UpdateWheels();
        ApplySteering();
        PathProgress();
    }

    private void CalculateNavMashLayerBite()
    {
        if (NavMeshLayers == null || NavMeshLayers[0] == "AllAreas")
            NavMeshLayerBite = NavMesh.AllAreas;
        else if (NavMeshLayers.Count == 1)
            NavMeshLayerBite += 1 << NavMesh.GetAreaFromName(NavMeshLayers[0]);
        else
        {
            foreach (string Layer in NavMeshLayers)
            {
                int I = 1 << NavMesh.GetAreaFromName(Layer);
                NavMeshLayerBite += I;
            }
        }
    }

    private void PathProgress() //Checks if the agent has reached the currentWayPoint or not. If yes, it will assign the next waypoint as the currentWayPoint depending on the input
    {
        wayPointManager();
        Movement();
        ListOptimizer();

        void wayPointManager()
        {
            if (currentWayPoint >= waypoints.Count)
                allowMovement = false;
            else
            {
                PostionToFollow = waypoints[currentWayPoint];
                allowMovement = true;
                if (Vector3.Distance(carFront.position, PostionToFollow) < 2)
                    currentWayPoint++;
            }

            if (currentWayPoint >= waypoints.Count - 3)
                CreatePath();
        }

        void CreatePath()
        {
            if (CustomDestination == null)
            {
                if (Patrol == true)
                    RandomPath();
                else
                {
                    debug("No custom destination assigned and Patrol is set to false", false);
                    allowMovement = false;
                }
            }
            else
                CustomPath(CustomDestination);

        }

        void ListOptimizer()
        {
            if (currentWayPoint > 1 && waypoints.Count > 30)
            {
                waypoints.RemoveAt(0);
                currentWayPoint--;
            }
        }
    }

    public void RandomPath() // Creates a path to a random destination
    {
        NavMeshPath path = new NavMeshPath();
        Vector3 sourcePostion;

        if (waypoints.Count == 0)
        {
            Vector3 randomDirection = Random.insideUnitSphere * 100;
            randomDirection += transform.position;
            sourcePostion = carFront.position;
            Calculate(randomDirection, sourcePostion, carFront.forward, NavMeshLayerBite);
        }
        else
        {
            sourcePostion = waypoints[waypoints.Count - 1];
            Vector3 randomPostion = Random.insideUnitSphere * 100;
            randomPostion += sourcePostion;
            Vector3 direction = (waypoints[waypoints.Count - 1] - waypoints[waypoints.Count - 2]).normalized;
            Calculate(randomPostion, sourcePostion, direction, NavMeshLayerBite);
        }

        void Calculate(Vector3 destination, Vector3 sourcePostion, Vector3 direction, int NavMeshAreaByte)
        {
            if (NavMesh.SamplePosition(destination, out NavMeshHit hit, 50, 1 << NavMesh.GetAreaFromName(NavMeshLayers[0])) &&
                NavMesh.CalculatePath(sourcePostion, hit.position, NavMeshAreaByte, path) && path.corners.Length > 2)
            {
                if (CheckForAngle(path.corners[1], sourcePostion, direction))
                {
                    waypoints.AddRange(path.corners.ToList());
                    debug("Random Path generated successfully", false);
                }
                else
                {
                    if (CheckForAngle(path.corners[2], sourcePostion, direction))
                    {
                        waypoints.AddRange(path.corners.ToList());
                        debug("Random Path generated successfully", false);
                    }
                    else
                    {
                        debug("Failed to generate a random path. Waypoints are outside the AIFOV. Generating a new one", false);
                        FailFOV = true;
                        Fails++;
                    }
                }
            }
            else
            {
                debug("Failed to generate a random path. Invalid Path. Generating a new one", false);
                FailInvalid = true;
                Fails++;
            }
        }
    }

    public void CustomPath(Transform destination) //Creates a path to the Custom destination
    {
        const int MaxRetries = 10;
        int retryCount = 0;
        NavMeshPath path = new NavMeshPath();
        Vector3 sourcePostion;

        while (retryCount < MaxRetries)
        {

            if (waypoints.Count == 0)
            {
                sourcePostion = carFront.position;
                if (Calculate(destination.position, sourcePostion, carFront.forward, NavMeshLayerBite)) return;
            }
            else
            {
                sourcePostion = waypoints[waypoints.Count - 1];
                Vector3 direction = (waypoints[waypoints.Count - 1] - waypoints[waypoints.Count - 2]).normalized;
                if (Calculate(destination.position, sourcePostion, direction, NavMeshLayerBite)) return;
            }

            retryCount++;
        }
        debug("Reached maximum retry limit for generating a custom path.",true);

        bool Calculate(Vector3 destination, Vector3 sourcePostion, Vector3 direction, int NavMeshAreaBite)
        {
            if (NavMesh.SamplePosition(destination, out NavMeshHit hit, 25, NavMeshAreaBite) &&
                NavMesh.CalculatePath(sourcePostion, hit.position, NavMeshAreaBite, path))
            {
                if (path.corners.ToList().Count() > 1 && CheckForAngle(path.corners[1], sourcePostion, direction))
                {
                    waypoints.AddRange(path.corners.ToList());
                    debug("Custom Path generated successfully", false);
                    return true;
                }
                else
                {
                    if (path.corners.Length > 2 && CheckForAngle(path.corners[2], sourcePostion, direction))
                    {
                        waypoints.AddRange(path.corners.ToList());
                        debug("Custom Path generated successfully", false);
                        return true;
                    }
                    else
                    {
                        debug("Failed to generate a Custom path. Waypoints are outside the AIFOV. Generating a new one", false);
                        FailFOV = true;
                        Fails++;
                    }
                }
            }
            else
            {
                debug("Failed to generate a Custom path. Invalid Path. Generating a new one", false);
                FailInvalid = true;
                Fails++;
            }
            return false;
        }
    }

    private bool CheckForAngle(Vector3 pos, Vector3 source, Vector3 direction) //calculates the angle between the car and the waypoint 
    {
        Vector3 distance = (pos - source).normalized;
        float CosAngle = Vector3.Dot(distance, direction);
        float Angle = Mathf.Acos(CosAngle) * Mathf.Rad2Deg;

        if (Angle < AIFOV)
            return true;
        else
            return false;
    }

    private void ApplyBrakes() // Apply brake torque 
    {
        frontLeft.brakeTorque = 5000;
        frontRight.brakeTorque = 5000;
        backLeft.brakeTorque = 5000;
        backRight.brakeTorque = 5000;
    }


    private void UpdateWheels() // Updates the wheel's postion and rotation
    {
        ApplyRotationAndPostion(frontLeft, wheelFL);
        ApplyRotationAndPostion(frontRight, wheelFR);
        ApplyRotationAndPostion(backLeft, wheelBL);
        ApplyRotationAndPostion(backRight, wheelBR);
    }

    private void ApplyRotationAndPostion(WheelCollider targetWheel, Transform wheel) // Updates the wheel's postion and rotation
    {
        targetWheel.ConfigureVehicleSubsteps(5, 12, 15);

        Vector3 pos;
        Quaternion rot;
        targetWheel.GetWorldPose(out pos, out rot);
        wheel.position = pos;
        wheel.rotation = rot;
    }

    void ApplySteering() // Applies steering to the Current waypoint
    {
        Vector3 relativeVector = transform.InverseTransformPoint(PostionToFollow);
        float SteeringAngle = (relativeVector.x / relativeVector.magnitude) * MaxSteeringAngle;
        if (SteeringAngle > 15) LocalMaxSpeed = 100;
        else LocalMaxSpeed = MaxRPM;
        if ((LocalMaxSpeed >= slowRpm && (slowSpeed || TurnLock))) LocalMaxSpeed = slowRpm;

        frontLeft.steerAngle = SteeringAngle;
        frontRight.steerAngle = SteeringAngle;
    }

    void Movement() // moves the car forward and backward depending on the input
    {
        if (move == true && allowMovement == true)
            allowMovement = true;
        else
            allowMovement = false;

        if (allowMovement == true)
        {
            frontLeft.brakeTorque = 0;
            frontRight.brakeTorque = 0;
            backLeft.brakeTorque = 0;
            backRight.brakeTorque = 0;

            int SpeedOfWheels = (int)((frontLeft.rpm + frontRight.rpm + backLeft.rpm + backRight.rpm) / 4);

            if (SpeedOfWheels < LocalMaxSpeed)
            {
                backRight.motorTorque = 400 * MovementTorque;
                backLeft.motorTorque = 400 * MovementTorque;
                frontRight.motorTorque = 400 * MovementTorque;
                frontLeft.motorTorque = 400 * MovementTorque;
            }
            else if (SpeedOfWheels < LocalMaxSpeed + (LocalMaxSpeed * 1 / 4))
            {
                backRight.motorTorque = 0;
                backLeft.motorTorque = 0;
                frontRight.motorTorque = 0;
                frontLeft.motorTorque = 0;
            }
            else
                ApplyBrakes();

        }
        else
            ApplyBrakes();
    }

    void debug(string text, bool IsCritical)
    {
        if (Debugger)
        {
            if (IsCritical)
                Debug.LogError(text);
            else
                Debug.Log(text);
        }
    }

    private void OnDrawGizmos() // shows a Gizmos representing the waypoints and AI FOV
    {
        if (ShowGizmos == true)
        {
            for (int i = 0; i < waypoints.Count; i++)
            {
                if (i == currentWayPoint)
                    Gizmos.color = Color.blue;
                else
                {
                    if (i > currentWayPoint)
                        Gizmos.color = Color.red;
                    else
                        Gizmos.color = Color.green;
                }
                Gizmos.DrawWireSphere(waypoints[i], 2f);
            }
            CalculateFOV();
        }

        void CalculateFOV()
        {
            Gizmos.color = Color.white;
            float totalFOV = AIFOV * 2;
            float rayRange = 10.0f;
            float halfFOV = totalFOV / 2.0f;
            Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
            Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);
            Vector3 leftRayDirection = leftRayRotation * transform.forward;
            Vector3 rightRayDirection = rightRayRotation * transform.forward;
            Gizmos.DrawRay(carFront.position, leftRayDirection * rayRange);
            Gizmos.DrawRay(carFront.position, rightRayDirection * rayRange);
        }
    }

    public void ReBakePath()
    {
        Restart();
        currentWayPoint = 0;
        allowMovement = true;
        move = true;
        TurnLock = false;
        slowSpeed = false;
        PostionToFollow = Vector3.zero;
    }

    public void LockTurnSpeed()
    {
        TurnLock = true;
    }
    public void ForceSlowSpeed()
    {
        slowSpeed = true;
    }
    public void ReleaseSlowSpeed()
    {
        slowSpeed = false;
    }

    public void ConformToPath(List<Vector3> path)
    {
        currentWayPoint = 0;
        waypoints = path;
    }
    public void Go()
    {
        allowMovement = true;
        move = true;
    }
    public void Stop()
    {
        allowMovement = false;
        move = false;
    }

    public float WillTurn()
    {

        List<Vector3> nextWaypoints = SeeFuture(6);
        float lastAngle = 0;
        float ReturnVal = 0;
        Vector3 distance = transform.forward;
        float Magnitude = 0;
        foreach (var waypoint in waypoints)
        {
            if (Magnitude < TurnDistance)
            {
                distance = (waypoint - gameObject.transform.position).normalized;
                float CosAngle = Vector3.Dot(distance, gameObject.transform.forward);
                float Angle = Mathf.Acos(CosAngle) * Mathf.Rad2Deg;
                Magnitude = (waypoint - gameObject.transform.position).magnitude;
                if (Angle < TurnIntentAngle) { ReturnVal = 0; }
                else
                {
                    if (Angle < lastAngle) { ReturnVal = 0; } else { ReturnVal = Angle; }

                }
            }
        }
        if ( ReturnVal != 0)
        {
            ReturnVal *= LeftOrRight(distance);
            if (CheckUTurn() && ReturnVal < 0)
            {
                ReturnVal = 720;
            }
        }
        return ReturnVal;





        int LeftOrRight(Vector3 move)
        {
            int Intent;
            float Left = Vector3.Dot(move, -gameObject.transform.right);
            float Right = Vector3.Dot(move, gameObject.transform.right);
            if (Left > Right)
            {
                Intent = -1;
            }
            else
            {
                Intent = 1;
            }

            return Intent;
        }
        bool CheckUTurn()
        {
            bool Attempt = false;
            List<Vector3> nextMoves = SeeFuture(6);
            Vector3 forward = gameObject.transform.forward;
            Vector3 back = -forward;
            Vector3 pos = gameObject.transform.position;
            Vector3 dist;
            float smallestCast = float.MaxValue;
            float currentCast;
            float backCast;
            for (int i = 0; i < nextMoves.Count - 1; i++)
            {
                if ((nextMoves[i] - pos).magnitude > TurnDistance)
                {
                    break;
                }
                dist = (nextMoves[i] - gameObject.transform.position).normalized;
                currentCast = Vector3.Dot(dist, forward);
                if (currentCast > smallestCast)
                {
                    break;
                }
                smallestCast = currentCast;
                dist = (nextMoves[i + 1] - gameObject.transform.position).normalized;
                backCast = Vector3.Dot(dist, back);
                if (backCast > smallestCast)
                {
                    Attempt = true;
                    break;
                }


            }
            return Attempt;
        }
        List<Vector3> SeeFuture(int i)
        {
            List<Vector3> See = new List<Vector3>();
            int foreSight = i;
            int current = currentWayPoint;
            while (current < waypoints.Count && current - currentWayPoint < foreSight)
            {
                See.Add(waypoints[current]);
                current++;
                foreSight++;
            }
            return See;
        }
    }
}