using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Diagnostics;
using TMPro;
public class AStar : Algorithm
{
    private List<AStarNode> _open = new List<AStarNode>();
    private List<AStarNode> _close = new List<AStarNode>();


    private AStarNode _currentNode;
    public AlgorithmType type = AlgorithmType.AStar;
    
    public override AlgorithmType GetAlgorithmType()
    {
        return type;
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
    public override void OnSetGoalNode(MapLocation map)
    {
        GameObject objectTarget = Instantiate(end, new Vector3(map.x, 0f, map.z), Quaternion.identity, maze.transform);
        _goalNode = new AStarNode(map, 0,0,0, objectTarget, null);
    }
    public override void OnBeginSearch()
    {
        if (machine.isCoroutineDone)
        {
            machine.isCoroutineDone = false;
            UnityEngine.Debug.Log("A Star");
            StartCoroutine(Search());
        }
    }
    IEnumerator  Search()
    {   
        StartGame();
        // Thêm node đầu vào danh sách mở
        _open.Add((AStarNode)_startNode);
        _currentNode = (AStarNode)_startNode;

        // Dùng để tính thời gian thực thi chương trình
        timer = new Stopwatch();
        timer.Reset();
        timer.Start();

        // Bắt đầu lập cho đến khi không có nút nào còn trong danh sách
        while (_open.Count > 0)
        {
            // Lấy node  trong danh sách mở mà có F thấp nhất
            _open = _open.OrderBy(p => p.F).ToList<AStarNode>();
            // XÓa node vừa lấy ra khỏi danh sách mở
            AStarNode node = _open.ElementAt(0);
            _open.RemoveAt(0);

            // Nếu node vừa lấy ra mà bằng node đích thì thoát khỏi vòng lập
            if (node.Equals(_goalNode))
            {
                _goalNode.SetParent(node);
                break;
            }
            // Với mỗi node kề ta tính giá trị F là khoảng cách từ node đầu cho đến node đích
            // G là khoảng cách từ node đang xét đến node kề + cho G của node hiện tại là khoảng cách từ node đầu đến node hàng xóm
            // H là khoảng cách từ node kề đến node đích
            foreach (MapLocation dir in maze._directions)
            {
                MapLocation neighbour = dir + node._location;
                float G = Vector3.Distance(node._location.ConvertToVector3(), neighbour.ConvertToVector3()) + node.G;
                float H = Vector3.Distance(neighbour.ConvertToVector3(), _goalNode._location.ConvertToVector3());
                float F = G + H;
                
                // kHởi tạo một node mới
                AStarNode neighbourNode = new AStarNode(neighbour, G, H, F, null, null);

                // Nếu nó là tường thì bỏ qua
                if (maze._map[neighbour.x, neighbour.z] == 1) continue;

                // Nếu nó nằm trong danh sách mở và có F lớn hơn node có vị trí bằng nó nằm trong danh sách mở thì bỏ qua
                if (IsInList(neighbour, _open) && IsGreater(neighbourNode, _open))
                {
                    continue;
                }

                // Nếu không thì  đổi node cha của node đó và bỏ qua các lệnh khởi tạo và các lệnh đưa lên màn hình
                // Do trong bài toán một node chỉ có 1 cha nhưng nếu tạo thêm một node để set giá trị cho cha này
                //có thể dẫn đến bị tràn CPU,nên với bài toán này chúng mấy em nó bằng ba của thằng đến sau nên trong một số trường hợp
                // khi truy hồi lại nó sẽ luôn lấy thằng cha lúc sau,sẽ dẫn đến việc đi sai một vị trí nhưng nó không đáng kể
                if (IsInList(neighbour, _open))
                {
                    GetNodeInOpen(neighbour).SetParent(node);
                    //Debug.Log("point");
                    continue;
                }
                // Nếu nó đã nằm trong danh sách đóng thì bỏ qua
                if (IsInList(neighbour, _close))
                {
                    continue;
                }

                // Khởi tạo một khối lên màn hình có vị trí bằng vị trí của node kề nhân cho giá trị scale (là giá trị mở rộng map)
                GameObject pathBlock = Instantiate(path, new Vector3(neighbour.x * maze.scale, 0f, neighbour.z * maze.scale), Quaternion.identity);
                // Gán cha của node kề bằng chín node đang xét
                neighbourNode.SetParent(node);
                
                // Gán khối của node kề bằng khối vừa khỏi tạo
                neighbourNode.SetNodeObject(pathBlock);

                // ĐƯa node vừa tạo vào danh sách mở và đổi màu thành màu openMeterial
                neighbourNode._nodeObject.GetComponent<MeshRenderer>().material = openMaterial;
                this._open.Add(neighbourNode);

                //Chờ cho đến khi tiến trình WaitForSeconds thực thi xong trong waitSpeed giây
                yield return new WaitForSeconds(stepTime);
            }
            // Sau khi xét xong thì đem nó vô danh sách đóng và đổi màu thành màu closeMaterial
            if (node != null && node._nodeObject != null)
            {
                if (node._nodeObject.GetComponent<MeshRenderer>() != null)
                    node._nodeObject.GetComponent<MeshRenderer>().material = closeMaterial;
                _close.Add(node);
            }
        }
        // lệnh này dùng để cấm quyền thực thi các lệnh từ bàn phím và chuột khi 1 thuật toán đang chạy
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

        // Kết thúc việc đếm thời gian
        timer.Stop();
        time = timer.ElapsedMilliseconds;
        // Cập nhật  giá trị thời gan chạy lên màn hình
        _machine.OnUpdate(time);
    }


    // Name: StartGame
    // Desc: Khởi tạo 2 giá trị node đầu và node đích
    public override void StartGame()
    {
        // Xóa danh sách truy hồi và các danh sách mở và đóng
        _retrieval.Clear();
        _open.Clear();
        _close.Clear();
        // Xóa tất cả các node đã được hiển thị trên màn hình 
        RemoveAllPath();

        // Tìm kiếm node đầu và node đích
        GameObject startObject = GameObject.FindGameObjectWithTag("Start");
        GameObject goalObject = GameObject.FindGameObjectWithTag("Goal");
        // Trường hợp không có node đầu thì khởi tạo node đầu và hiển thị nó trên màn hình
        if (startObject != null)
        {
            _startNode = new AStarNode(ConvertMapLocation(startObject.transform.position), 0, 0, 0,
            startObject, null);
        }
        else
        // Lấy vị trí của node đầu trên màn hình sau đó mapping vô vị trí trên ma trận recursive
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
            _startNode = new AStarNode(new MapLocation(locations[0].x, locations[0].z), 0, 0, 0, objectStart, null);
        }


        // Tương tự node đầu
        if (goalObject != null)
        {
            _goalNode = new AStarNode(ConvertMapLocation(goalObject.transform.position), 0, 0, 0,
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
            _goalNode = new AStarNode(new MapLocation(locations[locations.Count - 1].x, locations[locations.Count - 1].z), 0, 0, 0, objectTarget, null);
        }
    }
    //Name: GetNodeInOpen
    //Desc: Trả  về node mà có vị trí bằng vị trí neighbour nếu không thì trả về null
    private AStarNode GetNodeInOpen(MapLocation neighbour)
    {
        foreach(AStarNode node in _open)
        {
            if(neighbour.Equals(node._location))
                return node;
        }    
        return null;
    }
    //Name: IsInList
    //Desc: Kiểm tra một node mà vị trí node đó có trong danh sách list hay không
    private bool IsInList(MapLocation location, List<AStarNode> list)
    {
        foreach (AStarNode p in list)
        {
            if (p._location.Equals(location))
                return true;
        }
        return false;
    }
    //Name: IsGreater
    // Desc: Kiểm tra một node xem có F lớn hơn một giá trị mà có cùng vị trí location
    public bool IsGreater(AStarNode lastNode, List<AStarNode> nodes)
    {
        foreach (AStarNode node in nodes)
        {
            if (node.Equals(lastNode))
                if (lastNode.F > node.F)
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
        Destroy (end);
        this._goalNode = null;
    }    
}
