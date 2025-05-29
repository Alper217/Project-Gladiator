using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;
    [SerializeField] Vector2 movementInput;
    public float horizontalInput;
    public float verticalInput;
    public float moveAmount;
 
    PlayerInput playerInput;
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
    }
    private void Update()
    {
        HandleMovementInput();
    }
    private void OnEnable()
    {
        if (playerInput == null)
        {
            playerInput = new PlayerInput();            
            playerInput.PlayerMovement.Move.performed += i => movementInput = i.ReadValue<Vector2>();
        }
        playerInput.Enable();
    }

    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;         
        horizontalInput = movementInput.x;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));

        if (moveAmount <= 0.5 && moveAmount > 0)
        {
            moveAmount = 0.5f;
        }
        else if (moveAmount > 0.5 && moveAmount < 1)
        {
            moveAmount = 1f;
        }
    }
}
