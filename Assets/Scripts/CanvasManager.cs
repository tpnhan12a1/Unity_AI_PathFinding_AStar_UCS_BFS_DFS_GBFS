using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private Recursive _recursive;
    [SerializeField]  private TMPro.TMP_InputField _widthField;
    [SerializeField] private TMP_InputField _heightField;
    [SerializeField] private TMP_InputField _difficultField;
    [SerializeField] private Button _btnGenarate;
    [SerializeField] private TMP_Dropdown _ddAlgorithm;
    

    [SerializeField] private AlgorithmMachine _machine;
    [SerializeField] private TMP_Text _time;
    [SerializeField] private TMP_Text _distance;
    [SerializeField] private TMP_Text _error;
    [SerializeField] private Button _btnShowExit;
    [SerializeField] private Slider _slider;
    [SerializeField] private Slider _waitSlider;

    [SerializeField] private Button _button;
    private bool _show = true;
    private Vector3 showPosition;
    private Vector3 exitPosition = new Vector3(750f, 0, 0);
    private GameObject[] panel;
    public Slider waitSlider => _waitSlider;
    private void Start()
    {
        panel = GameObject.FindGameObjectsWithTag("Panel");

        SwithOffPopUp();
        ShowCanvas();
        showPosition = panel[1].transform.position;

        _btnGenarate.onClick.AddListener(GenerateNewMap);
        _btnShowExit.onClick.AddListener(ShowCanvas);

        _button.onClick.AddListener(SwithOffPopUp);

    }
    public void SwithOffPopUp()
    {
        panel[0].transform.localScale = Vector3.zero;
    }
    public void SwithOnPopUp()
    {
        panel[0].transform.localScale = Vector3.one;
    }
    public void ShowCanvas()
    {
        if (_show)
        {
            panel[1].transform.localScale = Vector3.zero;
            _show = false;
        }
        else
        {
            panel[1].transform.localScale = Vector3.one;
            _show = true;
        }
    }    
    public void OnChangeAlgorithm()
    {
        Algorithm newAlgorithm = null;
        if(_machine.isCoroutineDone)
        {
            AlgorithmType newAlgorithmType = AlgorithmType.UCS;
            if (_ddAlgorithm.value == 0)
            {
                newAlgorithmType = AlgorithmType.UCS;
            }
            if (_ddAlgorithm.value == 1)
            {
                newAlgorithmType = AlgorithmType.AStar;
            }
            if (_ddAlgorithm.value == 2)
            {
                newAlgorithmType = AlgorithmType.GBFS;
            }
            if(_ddAlgorithm.value == 3)
            {
                newAlgorithmType = AlgorithmType.BFS;
            }    
            if(_ddAlgorithm.value == 4)
            {
                newAlgorithmType = AlgorithmType.DFS;
            }    
            if (_machine.algorithms.TryGetValue(newAlgorithmType, out newAlgorithm))
                _machine.currentAlgorithm = newAlgorithm;
        }
        
    }

    private void GenerateNewMap()
    {
        if (_widthField.text == "" || !IsNumber(_widthField.text)) return;
        if (_heightField.text == ""|| !IsNumber(_heightField.text)) return;
        if (_difficultField.text == ""|| !IsNumber(_difficultField.text)) return;

        int width;
        int height;
        int difficult;
        if (!int.TryParse(_widthField.text, out width)) return;
        if(!int.TryParse(_heightField.text, out height)) return;
        if (!int.TryParse(_difficultField.text, out difficult)) return;

        if (width<10 || width>50 || height<10 || height> 50 || difficult<0 || difficult>100) return;
        _recursive.width = width;
        _recursive.height = height;
        _recursive.difficult = difficult;

        _machine.currentAlgorithm.OnNewMap();

    }
    public bool IsNumber(string pText)
    {
        Regex regex = new Regex(@"^[-+]?[0-9]*.?[0-9]+$");
        return regex.IsMatch(pText);
    }

    internal void OnChangeTime(String time)
    {
        this._time.text = "TIME: "+ time + " ms";
    }
    internal void OnChangeDistance(String distance)
    {
        this._distance.text = "DISTANCE: " + distance;
    }
    internal void OnChangeError(String error)
    {
        if (error == "")
            this._error.text = "";
        else
            this._error.text = "ERROR: " + error;
    }
}
