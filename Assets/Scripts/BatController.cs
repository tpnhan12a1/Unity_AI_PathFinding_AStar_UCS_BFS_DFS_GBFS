using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(CharacterController))]
public class BatController : MonoBehaviour
{
    [SerializeField] private float _speed = 0.005f;
    [SerializeField] private Slider _slider;
    private CharacterController _characterController;
    float _targetAngle;
    public float _turnSmoothTime = 0.1f;
    float _turnSmoothVelocity;
    AlgorithmMachine _algorithm;
    private CanvasManager _canvasManager;

    
    public float speed { get { return _speed; } set { _speed = value; } }
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _algorithm =GameObject.Find("Game Manager").GetComponent<AlgorithmMachine>();
        _canvasManager =GameObject.Find("Canvas").GetComponent<CanvasManager>();
        _slider = GameObject.Find("Slider").GetComponent<Slider>();
        
        
    }
    
    
    // Update is called once per frame
    void Update()
    {
        speed = _slider.value;

        if(_algorithm.isCoroutineDone)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            Vector3 direction = new Vector3(h, 0, v).normalized;
            if (direction.magnitude > 0.1f)
            {
                _targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

                GameObject[] paths = GameObject.FindGameObjectsWithTag("Path");
                foreach (GameObject path in paths)
                    Destroy(path);
            }

            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetAngle, ref _turnSmoothVelocity, _turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            _characterController.Move(direction * _speed*Time.deltaTime);
        }    

    }
}
