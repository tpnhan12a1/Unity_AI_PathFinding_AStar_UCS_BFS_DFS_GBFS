using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class BFS : Algorithm
{
    private new Node _goalNode;
    private new Node _startNode;
    private Node _currentNode;
    
    private Queue<Node> _queue = new Queue<Node>();
    private List<Node> _visits= new List<Node>();

    public AlgorithmType type = AlgorithmType.BFS;

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
    public override void OnSetGoalNode(MapLocation map)
    {
        GameObject objectTarget = Instantiate(end, new Vector3(map.x, 0f, map.z), Quaternion.identity, maze.transform);
        _goalNode = new Node(map, objectTarget, null);
    }
    public override AlgorithmType GetAlgorithmType()
    {
        return type;
    }

    public override void OnBeginSearch()
    {
        if (machine.isCoroutineDone)
        {
            machine.isCoroutineDone = false;
            UnityEngine.Debug.Log("BFS");
            StartCoroutine(Search());


        }
    }

    IEnumerator Search()
    {
        StartGame();
        // Thêm node ??u vào hàng ??i
        _queue.Enqueue(_startNode);
        _currentNode = (Node)_startNode;
        // B?t ??u tính th?i gian th?c thi c?a thu?t toán
        timer = new Stopwatch();
        timer.Reset();
        timer.Start();
        // Trong khi hàng ??i v?n còn 
        while (_queue.Count > 0)
        {
            // L?y giá tr? trong hàng ??i ra và ki?m tra v?i node g?c
            Node node = _queue.Dequeue();
            if (node.Equals(_goalNode))
            {
                _goalNode.SetParent(node);
                break;
            }
            // V?i t?ng giá tr? xung quanh
            foreach (MapLocation dir in maze._directions)
            {
                MapLocation neighbour = dir + node._location;
                Node neighbourNode = new Node(neighbour, null, null);
                // Ki?m tra giá tr? ?ó có ph?i t??ng hay không
                if (maze._map[neighbour.x, neighbour.z] == 1) continue;
                // N?u ?ã có trong hàng ??i thì b? qua
                if (IsInQueue(neighbour, _queue)) continue;
                // N?u ?ã duy?t thì b? qua
                if (IsInList(neighbour, _visits))
                {
                    continue;
                }
                //Kh?i t?o m?t ??i t??ng trên màn hình và ánh x? nó vào Node
                GameObject pathBlock = Instantiate(path, new Vector3(neighbour.x * maze.scale, 0f, neighbour.z * maze.scale), Quaternion.identity);

                neighbourNode.SetParent(node);
                neighbourNode.SetNodeObject(pathBlock);
                neighbourNode._nodeObject.GetComponent<MeshRenderer>().material = openMaterial;
                // Thêm node vào hàng ??i
                _queue.Enqueue(neighbourNode);
                // Tr? v? giá tr? ch? ??i th?i gian
                yield return new WaitForSeconds(stepTime);
            }
            // ??i màu path hi?n th? trên màn hình
            if (node != null && node._nodeObject != null)
            {
                if (node._nodeObject.GetComponent<MeshRenderer>() != null)
                    node._nodeObject.GetComponent<MeshRenderer>().material = closeMaterial;
            }
            // Thêm node vào node ?ã th?m
            _visits.Add(node);
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

    // Name: StartGame
    // Desc: Kh?i t?o 2 giá tr? node ??u và node ?ích
    public override void StartGame()
    {
        // Xóa h?t t?t c? các tr?ng thái ban ??u ?? b?t ??u m?t thu?t toán
        _retrieval.Clear();
        RemoveAllPath();
        _queue.Clear();
        _visits.Clear();
        //Kh?i t?o các node ??u và ?ích
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
            UnityEngine.Debug.Log("this is else");
            GameObject objectTarget = Instantiate(end, goalLocation, Quaternion.identity);
            _goalNode = new Node(new MapLocation(locations[locations.Count - 1].x, locations[locations.Count - 1].z), objectTarget, null);
        }
    }

    private bool IsInQueue(MapLocation location, Queue<Node> list)
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
        _startNode = null;
    }
    public override void RemoveGoal()
    {
        GameObject end = GameObject.FindGameObjectWithTag("Goal");
        Destroy(end);
        this._goalNode = null;
    }
}
