using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Diagnostics;
public class UCS : Algorithm
{
    private List<UCSNode> _open = new List<UCSNode>();
    private List<UCSNode> _close = new List<UCSNode>();


    private UCSNode _currentNode;

    public AlgorithmType type = AlgorithmType.UCS;
    
    public override void  OnNewMap()
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
    
    public override void StartGame()
    {
        _retrieval.Clear();

        _open.Clear();
        _close.Clear();
        RemoveAllPath();

        GameObject startObject = GameObject.FindGameObjectWithTag("Start");
        GameObject goalObject = GameObject.FindGameObjectWithTag("Goal");
        if (startObject != null)
        {
            _startNode = new UCSNode(ConvertMapLocation(startObject.transform.position), 0,
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
            _startNode = new UCSNode(new MapLocation(locations[0].x, locations[0].z), 0, objectStart, null);
        }
        if (goalObject != null)
        {
            _goalNode = new UCSNode(ConvertMapLocation(goalObject.transform.position), 0,
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
            _goalNode = new UCSNode(new MapLocation(locations[locations.Count - 1].x, locations[locations.Count - 1].z), 0, objectTarget, null);
        }

    }    
    public override void OnSetGoalNode(MapLocation map)
    {
        GameObject objectTarget = Instantiate(end, new Vector3(map.x, 0f, map.z), Quaternion.identity, maze.transform);
        _goalNode = new UCSNode(map,0, objectTarget, null);
    }
    public override AlgorithmType GetAlgorithmType()
    {
        return type;
    }

    public override void OnBeginSearch()
    {
        if(machine.isCoroutineDone)
        {
            UnityEngine.Debug.Log("UCS");
            machine.isCoroutineDone = false;
            StartCoroutine(Search());
        }    
    }
    IEnumerator Search()
    {
        StartGame();

        _open.Add((UCSNode)_startNode);
        _currentNode = (UCSNode)_startNode;


        timer = new Stopwatch();
        timer.Reset();
        timer.Start();


        while (_open.Count > 0)
        {
            _open = _open.OrderBy(p => p.cost).ToList<UCSNode>();
            UCSNode node = _open.ElementAt(0);
            _open.RemoveAt(0);

            if (node.Equals(_goalNode))
            {

                _goalNode.SetParent(node);
                break;
            }

            foreach (MapLocation dir in maze._directions)
            {
                MapLocation neighbour = dir + node._location;
                float cost = Vector3.Distance(node._location.ConvertToVector3(), neighbour.ConvertToVector3());
                
                UCSNode neighbourNode = new UCSNode(neighbour, node.cost + cost, null, null);

                if (maze._map[neighbour.x, neighbour.z] == 1) continue;

                if (IsInList(neighbour, _close)) continue;
                if (IsInList(neighbour, _open))
                {
                    continue;
                }

                GameObject pathBlock = Instantiate(path, new Vector3(neighbour.x * maze.scale, 0f, neighbour.z * maze.scale), Quaternion.identity);


                neighbourNode.SetParent(node);
                neighbourNode.SetNodeObject(pathBlock);
                neighbourNode._nodeObject.GetComponent<MeshRenderer>().material = openMaterial;
                this._open.Add(neighbourNode);

                yield return new WaitForSeconds(stepTime);
            }
            if (node != null && node._nodeObject != null)
            {
                if (node._nodeObject.GetComponent<MeshRenderer>() != null)
                    node._nodeObject.GetComponent<MeshRenderer>().material = closeMaterial;
                _close.Add(node);
            }
        }
        machine.isCoroutineDone = true;
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
    }

    

    private UCSNode GetNodeInOpen(MapLocation neighbour)
    {
        foreach (UCSNode node in _open)
        {
            if (neighbour.Equals(node._location))
                return node;
        }
        return null;
    }
    private bool IsInList(MapLocation location, List<UCSNode> list)
    {
        foreach (UCSNode p in list)
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
