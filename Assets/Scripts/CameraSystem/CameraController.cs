using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera _mCamera; 
    private Transform _cameraTrans;
    [SerializeField] private float moveSensitivity;
    [SerializeField] private float scrollSensitivity;

    private Vector3 _mouseScreenPosNow;
    private Vector3 _mouseScreenPosBeforeDrag;
    private Vector3 _cameraWorldPosBeforeDrag;
    private float _scrollInput;
    private void Start()
    {
        _mCamera = GetComponent<Camera>();
        _cameraTrans = _mCamera.transform;
    }

    // 将一个向量从屏幕坐标单位转换为世界坐标单位，这里的单位是指长度比例关系，不能用于坐标点的转换，因为基于的坐标系不同
    private Vector3 ScreenUnitToWorldUnit(Vector3 screenUnitVector)
    {
        var screenUnitToUnityUnit = 2 * _mCamera.orthographicSize / Screen.height;
        return screenUnitVector * screenUnitToUnityUnit;
    }
    
    private void DragToMove()
    {
        // 处理的关键是，如何把鼠标的屏幕拖动量转换为相机的世界坐标拖动量
        var isSpacePressed = Input.GetKey(KeyCode.Space);   // 按住空格键+左键 拖动场景
        if (Input.GetMouseButtonDown(0) && isSpacePressed)
        {
            _mouseScreenPosBeforeDrag = Input.mousePosition;
            _cameraWorldPosBeforeDrag = _cameraTrans.position;
        }
        else if (Input.GetMouseButton(0) && isSpacePressed)
        {
            _mouseScreenPosNow = Input.mousePosition;
            _cameraTrans.position = _cameraWorldPosBeforeDrag + ScreenUnitToWorldUnit(_mouseScreenPosBeforeDrag - _mouseScreenPosNow);
        }
    }

    private void HandleCameraMove()
    {
        // DragToMove();
        // 使用按键控制相机移动
        var moveX = Input.GetAxis("Horizontal");
        var moveY = Input.GetAxis("Vertical");
        _cameraTrans.position += new Vector3(moveX, moveY, 0) * (moveSensitivity * Time.deltaTime);
    }
    
    private void HandleCameraZoom()
    {
        _scrollInput = Input.GetAxis("Mouse ScrollWheel");
        _mCamera.orthographicSize -= _scrollInput * scrollSensitivity;
    }
        
    private void Update()
    {
        HandleCameraMove();
        HandleCameraZoom();
    }
}