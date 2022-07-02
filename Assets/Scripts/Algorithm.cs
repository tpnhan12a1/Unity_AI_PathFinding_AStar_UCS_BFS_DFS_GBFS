using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Linq;
public class Algorithm : MonoBehaviour
{
    [SerializeField] private Recursive _maze;
    [SerializeField] private GameObject _start;
    [SerializeField] private GameObject _end;
    [SerializeField] private GameObject _path;
    [SerializeField] private Material _closeMaterial;
    [SerializeField] private Material _openMaterial;
    [SerializeField] private Material _finishMaterial;

    public float stepTime = 0.01f;
    public float speed = 0.1f;
    
    protected List<MapLocation> _retrieval = new List<MapLocation>();

    public AlgorithmMachine _machine;
    public float startTime;
    public float endTime;

    public Stopwatch timer;
    public long time;
    public float distance;
    public string error;

    public virtual AlgorithmType GetAlgorithmType()
    {
        return AlgorithmType.UCS;
    }

    

    protected Node _goalNode;
    protected Node _startNode;


    public Recursive maze { get { return _maze; } }
    public GameObject start { get { return _start; } }
    public GameObject end { get { return _end; } }
    public GameObject path { get { return _path; } }
    public Material closeMaterial { get { return _closeMaterial; } }
    public Material openMaterial { get { return _openMaterial; } }
    public Material finishMaterial { get { return _finishMaterial; } } 

    public Node goalNode { get { return _goalNode; } }
    public Node startNode { get { return _startNode; } }
    public AlgorithmMachine machine { get { return _machine; } set { this._machine = value; } }
    public virtual void  RemoveAllPath()
    {
        LineRenderer line = GameObject.FindGameObjectWithTag("Line").GetComponent<LineRenderer>();
        line.positionCount = 1;
    }    
    public virtual void RemovePlace()
    {
        Destroy(GameObject.FindGameObjectWithTag("Place"));
    }    
    public MapLocation ConvertMapLocation(Vector3 pos)
    {
        return new MapLocation(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.z));
    }
    public virtual void RemoveStart()
    {
        GameObject start = GameObject.FindGameObjectWithTag("Start");
        Destroy(start);
        _startNode = null;
    }
    public virtual void RemoveGoal()
    {
        GameObject end = GameObject.FindGameObjectWithTag("Goal");
        Destroy(end);
        _goalNode = null;
    }
    public virtual void OnSetGoalNode(MapLocation map)
    {

    }    
    public virtual Algorithm OnUpdate()
    {
        return this;
    }

    public virtual void OnBeginSearch()
    {

    }
    public virtual void StartGame()
    {

    }
    public virtual void OnNewMap()
    {
        if (machine.isCoroutineDone)
        {
            RemovePlace();
            RemoveAllPath();
            RemoveGoal();
            RemoveStart();
            GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");

            foreach (GameObject wallObj in walls)
            {
                Destroy(wallObj);
            }
            maze.NewMap();
        }
    }
    //public virtual IEnumerator Retrieval()
    //{
    //    GameObject start = GameObject.FindGameObjectWithTag("Start");
    //    GameObject goal = GameObject.FindGameObjectWithTag("Goal");
        
    //    if( start !=null && goal != null )
    //    {
    //       while(_retrieval.Count > 1)
    //        {
    //            Vector3 currentPosition = _retrieval.ElementAt(0).ConvertToVector3();
    //            Vector3 nextPosition = _retrieval.ElementAt(1).ConvertToVector3();
    //            _retrieval.RemoveAt(0);
    //            _retrieval.RemoveAt(0);
    //            float distance = Vector3.Distance(currentPosition, nextPosition);

    //            start.transform.rotation = Quaternion.LookRotation(nextPosition);

    //            start.transform.position = nextPosition;
    //            UnityEngine.Debug.Log("Point");
    //            yield return new WaitForSeconds(1);
                
    //        }
    //        UnityEngine.Debug.Log("Last");
    //    }    
    //}    
}
