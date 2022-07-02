using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public enum AlgorithmType{ AStar, UCS, GBFS, BFS, DFS }
public class AlgorithmMachine : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private CanvasManager _canvasManager;
    private float _maxDistance = 1000f;
    private Recursive _recursive;

    public bool isCoroutineDone = true;

    private AlgorithmType _currentAlgorithmType = AlgorithmType.UCS;

    private Algorithm _currentAlgorithm;
    protected Dictionary<AlgorithmType, Algorithm> _algorithms = new Dictionary<AlgorithmType, Algorithm>();
    private bool isStart = false;

    public Algorithm currentAlgorithm { get { return _currentAlgorithm; } set { _currentAlgorithm = value; } }
    public Dictionary<AlgorithmType, Algorithm> algorithms { get { return _algorithms; } }
    private void Start()
    {
        _recursive = GameObject.FindGameObjectWithTag("Maze").GetComponent<Recursive>();

        Algorithm[] algorithms = GetComponents<Algorithm>();

        foreach (Algorithm algo in algorithms)
        {
           if(algo != null && !_algorithms.ContainsKey(algo.GetAlgorithmType()))
            {
                _algorithms[algo.GetAlgorithmType()] = algo;
                Debug.Log(algo.GetAlgorithmType());
                algo.machine = this;
            }    
        }
        _algorithms.TryGetValue(_currentAlgorithmType, out _currentAlgorithm);
        
    }
    // M?i 240hz m?i 1 giây s? g?i hàm này 240 l?n
    private void Update()
    {   
        if (_currentAlgorithm == null) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(!isStart)
            {
                _currentAlgorithm.StartGame();
                isStart = true;
            }    
            else
            {
                _currentAlgorithm.OnBeginSearch();
            }    
            
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            _currentAlgorithm.OnNewMap();
            isStart = false;
        }

        if (isCoroutineDone)
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetMouseButton(0))
                {

                    Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, _maxDistance, LayerMask.GetMask("Place")))
                    {
                        //Check overflow map

                        Vector2Int clickPosition = new Vector2Int(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.z));
                        if (clickPosition.x > 0 && clickPosition.x < currentAlgorithm.maze.width - 1 && clickPosition.y > 0 && clickPosition.y < currentAlgorithm.maze.height - 1)
                        {
                            if (currentAlgorithm.maze._map[clickPosition.x, clickPosition.y] == 0)
                            {
                                currentAlgorithm.RemoveAllPath();
                                currentAlgorithm.RemoveGoal();
                                _currentAlgorithm.OnSetGoalNode(new MapLocation(clickPosition.x, clickPosition.y));
                            }
                        }
                    }
                }
            }
            else
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, _maxDistance, LayerMask.GetMask("Place")))
                {
                    GameObject go = GameObject.FindGameObjectWithTag("Goal");
                    if ( go != null)
                        if (!( new MapLocation(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.z)).Equals(
                            new MapLocation(Mathf.RoundToInt(go.transform.position.x), Mathf.RoundToInt(go.transform.position.z)))))
                        {
                            currentAlgorithm.maze.InitWall();
                            currentAlgorithm.maze.DestructionWall();
                        }    
                }
            }
 
        }
        currentAlgorithm.stepTime = _canvasManager.waitSlider.value;


        //if(Input.GetKeyDown(KeyCode.Return))
        //{
        //    StartCoroutine(_currentAlgorithm.Retrieval());
        //}    
    }

        internal void OnUpdate(long time)
        {
            _canvasManager.OnChangeTime(_currentAlgorithm.time.ToString());
            _canvasManager.OnChangeDistance(_currentAlgorithm.distance.ToString());
            _canvasManager.OnChangeError(_currentAlgorithm.error.ToString());
            _canvasManager.SwithOnPopUp();

        }

    
}
