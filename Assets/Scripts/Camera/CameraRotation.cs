using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Camera
{
    public class CameraRotation : MonoBehaviour
    {
        private PlayerControls _controls;

        [SerializeField] private PlayerInput inputs;
        [SerializeField] private Transform playerBody;

        [SerializeField] private CinemachineVirtualCamera cam;
        [SerializeField] private float rotationSpeed = 1f;
        [SerializeField] private UnityEngine.Camera mainCamera;

        private Vector2 mousePos;
        private float xRotation = 0f;


        public void Start()
        {
            _controls = new PlayerControls();
            _controls.Enable();
        }

        private void GetMousePos()
        {
            mousePos = _controls.Player.Look.ReadValue<Vector2>() * (rotationSpeed * Time.deltaTime);

        }


        private void Update()
        {
            if (GameManager.Instance.currentMode == GameManager.CurrentMode.TopDown) return;
            
            GetMousePos();
            RotateCamera();
        }

        private void RotateCamera()
        {
            xRotation -= mousePos.y;
            //Clamp the rotation so the camera doesn't go upside down
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            //Rotate the camera
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            //Rotate the player on the Y axis
            playerBody.Rotate(Vector3.up * mousePos.x);
        }
        
        
        public CinemachineVirtualCamera GetCamera()
        {
            return cam;
        }
        
        public UnityEngine.Camera GetMainCamera()
        {
            return mainCamera;
        }
    }
}