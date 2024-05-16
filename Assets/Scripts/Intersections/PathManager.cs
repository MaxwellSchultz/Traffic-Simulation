using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    [SerializeField, Header("Left Turns")]
    private Transform[] leftTurnRoots;

    private List<Vector3>[] LeftTurnList = new List<Vector3>[5];

    [SerializeField,Header("Right Turns")]
    private Transform[] rightTurnRoots;

    private List<Vector3>[] RightTurnList = new List<Vector3>[5];

    [SerializeField,Header("Straight Paths")]
    private Transform[] straightPathRoots;

    private List<Vector3>[] StraightPathList = new List<Vector3>[5];

    [SerializeField, Header("UTurns")]
    private Transform[] UTurnRoots;

    private List<Vector3>[] UTurnPathList = new List<Vector3>[5];

    // Start is called before the first frame update
    void Start()
    {
        
        for (int i = 0; i < leftTurnRoots.Length; i++)
        {
            LeftTurnList[i] = new List<Vector3>();
            for(int j = 0; j < leftTurnRoots[i].childCount; j++)
            {
                print(leftTurnRoots[i].GetChild(j).gameObject);
                LeftTurnList[i].Add(leftTurnRoots[i].GetChild(j).position);
            }
        }
        for (int i = 0;i < rightTurnRoots.Length; i++)
        {
            RightTurnList[i] = new List<Vector3>();
            for( int j = 0; j< rightTurnRoots[i].childCount; j++)
            {
                RightTurnList[i].Add(rightTurnRoots[i].GetChild(j).position);
            }
        }
        for (int i = 0; i < straightPathRoots.Length; i++)
        {
            StraightPathList[i] = new List<Vector3>();
            for(int j = 0; j < straightPathRoots[i].childCount; j++)
            {
                StraightPathList[i].Add(straightPathRoots[i].GetChild(j).position);
            }
        }
        for (int i = 0;i<UTurnRoots.Length; i++)
        {
            UTurnPathList[i] = new List<Vector3>();
            for(int j = 0; j < UTurnRoots[i].childCount;j++)
            {
                UTurnPathList[i].Add(UTurnRoots[i].GetChild(j).position);
            }
        }
    }


    /*
     * 0 - Right Turn
     * 1 - Straight
     * 2 - Left Turn
     * 3 - UTurn
     */
    public List<Vector3> GetTurn(int turn, int i)
    {
        switch (turn)
        {
                case 0:
                return GetRight(i);
                case 1:
                return GetStraight(i);
                case 2:
                return GetLeft(i);
                case 3:
                return GetUTurn(i);
                default:
                return null;
        }

        List<Vector3> GetRight(int i) { return RightTurnList[i]; }
        List<Vector3> GetLeft(int i) { return LeftTurnList[i]; }
        List<Vector3> GetStraight(int i) { return StraightPathList[i]; }
        List<Vector3> GetUTurn(int i) { return UTurnPathList[i]; }
    }


}
