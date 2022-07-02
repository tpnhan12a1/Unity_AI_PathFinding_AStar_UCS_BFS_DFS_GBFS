using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour
{
    [SerializeField] private int _width = 10;
    [SerializeField] private int _height = 10;
    [SerializeField] private int _scale = 2;
    [SerializeField] private GameObject _place;
    [SerializeField] private GameObject _wall;
    [SerializeField] private int _difficult = 50;
    [SerializeField] private Camera _camera;


    Vector3  lastPosition;
    private float _maxDistance = 1000f;

    public List<MapLocation> _directions = new List<MapLocation>()
    {
        new MapLocation(1,-1),
        new MapLocation(1,0),
        new MapLocation(1,1),
        new MapLocation(0,1),
        new MapLocation(-1,1),
        new MapLocation(-1,0),
        new MapLocation(-1,-1),
        new MapLocation(0,-1)
    };

    public int[,] _map;

    public int width { get { return _width; } set { _width = value; } }
    public int height { get { return _height; } set { _height = value; } }
    public int scale { get { return _scale; } set { _scale = value; } }
    public int difficult { get { return _difficult; } 
        set {
            if(value > 100)
            {
                _difficult = 99;
            }
            else
            {
                _difficult = value;
            }
        } }

    void Start()
    {

        NewMap();
    }
    protected virtual void Update()
    {

    }

    public void InitWall()
    {

        if (Input.GetMouseButton(0))
        {

            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, _maxDistance, LayerMask.GetMask("Place")))
            {
                //Check overflow map

                Vector2Int clickPosition = new Vector2Int(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.z));
                if (clickPosition.x > 0 && clickPosition.x < width - 1 && clickPosition.y > 0 && clickPosition.y < height - 1)
                {
                    RemoveAllPath();
                    _map[clickPosition.x, clickPosition.y] = 1;
                    DrawMap();
                    Debug.Log(clickPosition);
                }
            }

        }    
    
        //if (Input.GetMouseButton(2))
        //{
        //    Vector3 delta = Input.mousePosition - lastPosition;
        //    _camera.transform.Translate(-delta.x * _mouseSensitivity, -delta.y * _mouseSensitivity, 0);
        //    lastPosition = Input.mousePosition;
        //}
    }    

    public void DestructionWall()
    {
        if (Input.GetMouseButton(1))
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, _maxDistance, LayerMask.GetMask("Place")))
            {

               
                Vector2Int clickPosition = new Vector2Int(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.z));
                if (clickPosition.x > 0 && clickPosition.x < width - 1 && clickPosition.y > 0 && clickPosition.y < height - 1)
                {
                    RemoveAllPath();
                    _map[clickPosition.x, clickPosition.y] = 0;
                    DrawMap();
                    Debug.Log(clickPosition);
                }
            }
        }
    }    
    public void NewMap()
    {
        InitialiseMap();
        Generate();
        DrawMap();
    }    
    public void InitialiseMap()
    {
        _map = new int[_width, _height];
        for(int x = 0; x < _width; x++)
            for(int z=0; z < _height; z++)
                _map[x,z] = 1;    //1 wall, 0 empty
    }
    public virtual void Generate()
    {
        _map[1, 1] = 0;
        
        for (int z = 1; z < _height-1; z++)
            for (int x = 1; x < _width-1; x++)
            {
                if (Random.Range(0, 100) > difficult)
                    _map[x, z] = 0;     //1 = wall  0 = corridor
            }
    }
    public void DrawMap()
    {
        RemoveAllWall();
        RemovePlace();

        Vector3 centerPosition = new Vector3(_width * 2 / _scale, -0.5f*_scale, _height * 2 / _scale);
        GameObject place = Instantiate(_place, centerPosition, Quaternion.identity);
        place.transform.parent = this.transform;
        place.transform.localScale = new Vector3(_scale*_width, 1, _scale*_height);
        
        
        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                if (_map[x, z] == 1)
                {
                    Vector3 position = new Vector3(x * _scale, 0, z * _scale);
                    GameObject wall = Instantiate(_wall, position, Quaternion.identity);
                    wall.transform.parent = this.transform;
                    if (x == 0 || x == width - 1 || z == 0 || z == height - 1)
                    {
                        wall.transform.localScale = new Vector3(1,2,1) * _scale;
                    }
                    else
                        wall.transform.localScale = new Vector3(0.5f, 2, 0.5f) * _scale;

                    if (!(x < 1 || x > width - 2 || z < 1 || z > height - 2))
                    {
                        if (_map[x - 1, z] == 1)
                        {
                            Vector3 subPosition = new Vector3(position.x - 0.375f, 0, position.z);

                            GameObject subWall = Instantiate(_wall, subPosition, Quaternion.identity);
                            subWall.transform.localScale = new Vector3(0.5f, 2, 0.5f) * _scale;

                            subWall.transform.parent = wall.transform;


                        }
                        if (_map[x + 1, z] == 1)
                        {
                            Vector3 subPosition = new Vector3(position.x + 0.375f, 0, position.z);

                            GameObject subWall = Instantiate(_wall, subPosition, Quaternion.identity);
                            subWall.transform.localScale = new Vector3(0.5f, 2, 0.5f) * _scale;

                            subWall.transform.parent = wall.transform;


                        }
                        if (_map[x, z - 1] == 1)
                        {
                            Vector3 subPosition = new Vector3(position.x, 0, position.z - 0.375f);

                            GameObject subWall = Instantiate(_wall, subPosition, Quaternion.identity);
                            subWall.transform.localScale = new Vector3(0.5f, 2, 0.5f) * _scale;

                            subWall.transform.parent = wall.transform;


                        }
                        if (_map[x, z + 1] == 1)
                        {
                            Vector3 subPosition = new Vector3(position.x, 0, position.z + 0.375f);

                            GameObject subWall = Instantiate(_wall, subPosition, Quaternion.identity);
                            subWall.transform.localScale = new Vector3(0.5f, 2, 0.5f) * _scale;

                            subWall.transform.parent = wall.transform;
                        }
                    }
                }  
            }    
        }     
    }
    public int CountSquareNeighbours(int x,int  z)
    {
        int count = 0;
        if (x <= 0 || x >= _width-1  || z <= 0|| z >= _height-1 ) return 9;
          
        if (_map[x - 1, z] == 0) count++;
        if (_map[x + 1, z] == 0) count++;
        if (_map[x, z + 1] == 0) count++;
        if (_map[x, z - 1] == 0) count++;
        if (_map[x+1, z + 1] == 0) count++;
        if (_map[x - 1, z + 1] == 0) count++;
        if (_map[x + 1, z - 1] == 0) count++;
        if (_map[x - 1, z - 1] == 0) count++;
        return count;
    }
    public void RemoveAllPath()
    {
        GameObject[] paths = GameObject.FindGameObjectsWithTag("Path");
        foreach (GameObject path in paths)
            Destroy(path);
    }
    public void RemoveAllWall()
    {
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        if(walls.Length > 0)
        {
            foreach (GameObject wall in walls)
                Destroy(wall);
        }    
    } 
    public void RemovePlace()
    {
        GameObject place = GameObject.FindGameObjectWithTag("Place");
        if(place != null)
            Destroy(place);
    }    
}
