using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public PlayerManager player;
    public static PlayerCamera instance;
    [SerializeField] Transform cameraPivotTransform;
    public Camera cameraObject;
    [Header("Camera Settings")]
    [SerializeField] private float cameraSmoothSpeed = 1f;
    [SerializeField] private float leftAndRightRotationSpeed = 220f;
    [SerializeField] private float upAndDownRotationSpeed = 220f;
    [SerializeField] private float minimumPivot = -30f;
    [SerializeField] private float maximumPivot = 60f;
    [SerializeField] private float cameraCollisionRadius = 0.2f;
    [SerializeField] private LayerMask cameraCollisionLayerMask;

    [Header("Camera Values")]
    private Vector3 cameraVelocity;
    private Vector3 cameraObjecPosition;
    [SerializeField] private float leftAndRightLookAngle = 5f;
    [SerializeField] private float upAndDownLookAngle = 5f;
    private float cameraZPosition;
    private float targetCameraZPosition;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        cameraZPosition = cameraObject.transform.localPosition.z;
    }

    public void HandleAllCameraActions()
    {
        if (player != null)
        {
            FollowPlayer();
            RotateCamera();
            HandleCameraCollisions();
        }
    }

    private void FollowPlayer()
    {
        Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, player.transform.position, ref cameraVelocity, cameraSmoothSpeed * Time.deltaTime);
        transform.position = targetCameraPosition;
    }

    private void RotateCamera()
    {
        leftAndRightLookAngle += (PlayerInputManager.instance.cameraHorizontalInput * leftAndRightRotationSpeed) * Time.deltaTime;
        upAndDownLookAngle -= (PlayerInputManager.instance.cameraVerticalInput * upAndDownRotationSpeed) * Time.deltaTime;
        upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);


        Vector3 cameraRotation = Vector3.zero;
        Quaternion targetRotation;

        cameraRotation.y = leftAndRightLookAngle;
        targetRotation = Quaternion.Euler(cameraRotation);
        transform.rotation = targetRotation;

        cameraRotation = Vector3.zero;
        cameraRotation.x = upAndDownLookAngle;
        targetRotation = Quaternion.Euler(cameraRotation);
        cameraPivotTransform.localRotation = targetRotation;
    }

    private void HandleCameraCollisions()
    {
        targetCameraZPosition = cameraZPosition;
        RaycastHit hit;
        Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;
        direction.Normalize();
        if (Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetCameraZPosition), cameraCollisionLayerMask))
        {
            float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
            targetCameraZPosition = -(distanceFromHitObject - cameraCollisionRadius);
        }
        if(Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius)
        {
            targetCameraZPosition = -cameraCollisionRadius;
        }

        cameraObjecPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraZPosition, 0.2f);
        cameraObject.transform.localPosition = cameraObjecPosition;
    }
}
