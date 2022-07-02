using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Vector3 _zoomAmout;
    [SerializeField] private float _mouseSensitivity =-0.05f;
    [SerializeField] private Maze _maze;
    // Private Field
    private Vector3 lastPosition;

    private void LateUpdate()
    {
        if( _camera != null )
        {
            
            CameraPan();
            CameraZoom();
            LimitCamera();
        }    
    }

    
    private void CameraPan()
    {
        if (Input.GetMouseButtonDown(2))
        {
            lastPosition = Input.mousePosition;
            //Debug.Log(lastPosition);
        }    

        if (Input.GetMouseButton(2))
        {
            Vector3 delta = Input.mousePosition - lastPosition;
            _camera.transform.Translate(-delta.x * _mouseSensitivity, -delta.y * _mouseSensitivity, 0);
            lastPosition = Input.mousePosition;
        }
    }

    private void CameraZoom()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            _camera.transform.position -= Input.mouseScrollDelta.y * _zoomAmout;
            Debug.Log(Input.mouseScrollDelta.y);
        }
    }
    private void LimitCamera()
    {
        if (_camera.transform.position.y < 10f)
            _camera.transform.position = new Vector3(_camera.transform.position.x, 10f, _camera.transform.position.z);
    }
}
