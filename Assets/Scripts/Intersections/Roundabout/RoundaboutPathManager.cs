using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoundaboutPathManager : MonoBehaviour
{
    [SerializeField]
    private Transform[] pathRoots;
    private List<Vector3>[] paths = new List<Vector3>[5];
    [SerializeField]
    private Transform exitRoot;
    private Vector3[] exits = new Vector3[5];

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < pathRoots.Length; i++)
        {
            paths[i] = new List<Vector3>();
            for (int j = 0; j < pathRoots[i].childCount; j++)
            {
                // print(leftTurnRoots[i].GetChild(j).gameObject);
                paths[i].Add(pathRoots[i].GetChild(j).position);
            }
        }
        for (int i = 0;i < exitRoot.childCount; i++)
        {
            exits[i] = exitRoot.GetChild(i).position;
        }
        
    }
    
    public List<Vector3> GetPath(int root, int intent)
    {
        List<Vector3> fullPath = new List<Vector3>();
        List<Vector3> currentPath;
        for (int i = 0; i < intent; i++)
        {
            currentPath = paths[((root + i) % 4)];
            for (int j = 0; j < currentPath.Count();j++) { fullPath.Add(currentPath[j]); }
        }
        fullPath.Add(exits[((root+intent)%4)]);



        return fullPath;
    }
}
