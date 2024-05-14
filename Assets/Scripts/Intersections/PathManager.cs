using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    [SerializeField, Header("Left Turns")]
    private Transform[] leftTurnRoots;

    private Transform[][] LeftTurnList;

    [SerializeField,Header("Right Turns")]
    private Transform[] rightTurnRoots;

    private Transform[][] RightTurnList;

    [SerializeField,Header("Straight Paths")]
    private Transform[] straightPathRoots;

    private Transform[][] StraightPathList;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < leftTurnRoots.Length; i++)
        {
            for(int j = 0; j < leftTurnRoots[i].childCount; j++)
            {
                LeftTurnList[i][j] = leftTurnRoots[i].GetChild(j);
            }
        }
        for (int i = 0;i < rightTurnRoots.Length; i++)
        {
            for( int j = 0; j< rightTurnRoots[i].childCount; j++)
            {
                RightTurnList[i][j] = rightTurnRoots[i].GetChild(j);
            }
        }
        for (int i = 0; i < straightPathRoots.Length; i++)
        {
            for(int j = 0; j < straightPathRoots[i].childCount; j++)
            {
                StraightPathList[i][j] = straightPathRoots[i].GetChild(j);
            }
        }
    }


    /*
     * 0 - Right Turn
     * 1 - Straight
     * 2 - Left Turn
     */
    public Transform[] GetTurn(int turn, int i)
    {
        switch (turn)
        {
                case 0:
                return GetRight(i);
                case 1:
                return GetStraight(i);
                case 2:
                return GetLeft(i);
                default:
                return null;
        }

        Transform[] GetRight(int i) { return RightTurnList[i]; }
        Transform[] GetLeft(int i) { return LeftTurnList[i]; }
        Transform[] GetStraight(int i) { return StraightPathList[i]; }
    }


}
