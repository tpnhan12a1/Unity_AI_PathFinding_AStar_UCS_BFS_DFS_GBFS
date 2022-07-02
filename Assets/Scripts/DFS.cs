using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class DFS : Algorithm
{
    [SerializeField] private LineRenderer lineRenderer = null;
    
    private new Node _goalNode;
    private new Node _startNode;
    private Node _currentNode;

    private Stack<Node> _stack = new Stack<Node>();
    private List<Node> _visits = new List<Node>();

    public AlgorithmType type = AlgorithmType.DFS;

    public override void OnSetGoalNode(MapLocation map)
    {
        GameObject objectTarget = Instantiate(end, new Vector3(map.x,0f,map.z), Quaternion.identity, maze.transform);
        _goalNode = new Node(map, objectTarget, null);
    }
    
    public override void OnNewMap()
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
    public override AlgorithmType GetAlgorithmType()
    {
        return type;
    }
    public override void StartGame()
    {
        base.StartGame();
        _retrieval.Clear();
       
        machine.isCoroutineDone = true;
        RemoveAllPath();
        _stack.Clear();
        _visits.Clear();

        GameObject startObject = GameObject.FindGameObjectWithTag("Start");
        GameObject goalObject = GameObject.FindGameObjectWithTag("Goal");

        if (startObject != null)
        {
            _startNode = new Node(ConvertMapLocation(startObject.transform.position),
            startObject, null);
        }
        else
        {
            List<MapLocation> locations = new List<MapLocation>();
            for (int z = 0; z < maze.height - 1; z++)
                for (int x = 0; x < maze.width - 1; x++)
                {
                    if (maze._map[x, z] != 1)
                        locations.Add(new MapLocation(x, z));
                }
            Vector3 startLocation = new Vector3(locations[0].x * maze.scale, 0, locations[0].z * maze.scale);

            GameObject objectStart = Instantiate(start, startLocation, Quaternion.identity);
            _startNode = new Node(new MapLocation(locations[0].x, locations[0].z), objectStart, null);
        }
        if (goalObject != null)
        {
            _goalNode = new Node(ConvertMapLocation(goalObject.transform.position),
             goalObject, null);
        }
        else
        {
            List<MapLocation> locations = new List<MapLocation>();
            for (int z = 0; z < maze.height - 1; z++)
                for (int x = 0; x < maze.width - 1; x++)
                {
                    if (maze._map[x, z] != 1)
                        locations.Add(new MapLocation(x, z));
                }
            Vector3 goalLocation = new Vector3(locations[locations.Count - 1].x * maze.scale, 0, locations[locations.Count - 1].z * maze.scale);

            GameObject objectTarget = Instantiate(end, goalLocation, Quaternion.identity);
            _goalNode = new Node(new MapLocation(locations[locations.Count - 1].x, locations[locations.Count - 1].z), objectTarget, null);
        }


    }
    public override void OnBeginSearch()
    {
        if ( machine.isCoroutineDone)
        {
            machine.isCoroutineDone = false;
            UnityEngine.Debug.Log("DFS");
            StartCoroutine(Search());
        }
    }

    IEnumerator Search()
    {


        StartGame();
        //Debug.Log(ConvertMapLocation(startObject.transform.position).x);
        //Debug.Log(ConvertMapLocation(startObject.transform.position).z);
        //Debug.Log(ConvertMapLocation(goalObject.transform.position).x);
        //Debug.Log(ConvertMapLocation(goalObject.transform.position).z);

        _stack.Push(_startNode);
        _currentNode = (Node)_startNode;

        timer = new Stopwatch();
        timer.Reset();
        timer.Start();

        while (_stack.Count > 0)
        {
            Node node = _stack.Pop();
            
            if (node.Equals(_goalNode))
            {
                _goalNode.SetParent(node);
                break;
            }

            foreach (MapLocation dir in maze._directions)
            {
                MapLocation neighbour = dir + node._location;
                Node neighbourNode = new Node(neighbour, null, null);
                if (maze._map[neighbour.x, neighbour.z] == 1) continue;

                //if (IsInStack(neighbour, _stack)) continue;
                if (IsInList(neighbour, _visits))
                {
                    continue;
                }
                GameObject pathBlock = Instantiate(path, new Vector3(neighbour.x * maze.scale, 0f, neighbour.z * maze.scale), Quaternion.identity);


                neighbourNode.SetParent(node);
                neighbourNode.SetNodeObject(pathBlock);
                neighbourNode._nodeObject.GetComponent<MeshRenderer>().material = openMaterial;

                _stack.Push(neighbourNode);
                yield return new WaitForSeconds(stepTime);
            }
            if (node != null && node._nodeObject != null)
            {
                if (node._nodeObject.GetComponent<MeshRenderer>() != null)
                    node._nodeObject.GetComponent<MeshRenderer>().material = closeMaterial;
            }
            _visits.Add(node);

        }

        machine.isCoroutineDone  = true;
        if (_goalNode._parent == null)
        {
            error = "Can't find directions";
            distance = 0f;
        }
        else
        {
            error = "";
            Node node = _goalNode;
            distance = 0;
            while (node._parent != null)
            {
                if (node._nodeObject != null)
                {
                    node._nodeObject.GetComponent<MeshRenderer>().material = finishMaterial;
                    distance += Vector3.Distance(node._location.ConvertToVector3(), node._parent._location.ConvertToVector3());
                    node = (Node)node._parent;
                }
                _retrieval.Add(node._location);
            }
            _retrieval.Reverse();
        }

        timer.Stop();
        time = timer.ElapsedMilliseconds;
        _machine.OnUpdate(time);

        lineRenderer.positionCount=1;
        for (int i=0;i< _retrieval.Count; i++)
        {
            lineRenderer.SetPosition(i, _retrieval[i].ConvertToVector3());
            lineRenderer.positionCount++;
        }
        lineRenderer.positionCount--;


    }
    private bool IsInStack(MapLocation location, Stack<Node> list)
    {
        foreach (Node p in list)
        {
            if (p._location.Equals(location))
                return true;
        }
        return false;
    }
    private bool IsInList(MapLocation location, List<Node> list)
    {
        foreach (Node p in list)
        {
            if (p._location.Equals(location))
                return true;
        }
        return false;
    }
    public override void RemoveAllPath()
    {
        base.RemoveAllPath();
        GameObject[] paths = GameObject.FindGameObjectsWithTag("Path");
        foreach (GameObject path in paths)
            Destroy(path);
        

    }
    public override void RemoveStart()
    {
        GameObject start = GameObject.FindGameObjectWithTag("Start");
        Destroy(start);
        this._startNode = null;
    }
    public override void RemoveGoal()
    {
        GameObject end = GameObject.FindGameObjectWithTag("Goal");
        Destroy(end);
        this._goalNode = null;
    }

}
