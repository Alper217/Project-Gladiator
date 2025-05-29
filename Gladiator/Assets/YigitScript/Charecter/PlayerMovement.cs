using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float walkingSpeed = 20f;
    [SerializeField] float runningSpeed = 50f;
   
    public float verticalMovement;
    public float horizontalMovement; 
    public float moveAmount;
    public Vector3 moveDirection;

    PlayerManager playerManager;
    
    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
    }   
    public void HandleAllMovement()
    {
        HandleGroundedMovement();
    }
    private void GetVerticalAndHorizontalInputs()
    {
        verticalMovement = PlayerInputManager.instance.verticalInput;
        horizontalMovement = PlayerInputManager.instance.horizontalInput;
        moveAmount = PlayerInputManager.instance.moveAmount;
    }
    private void HandleGroundedMovement()
    {
        GetVerticalAndHorizontalInputs();
        moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
        moveDirection += PlayerCamera.instance.transform.right * horizontalMovement;
        moveDirection.Normalize();
        moveDirection.y = 0f; 
        if(PlayerInputManager.instance.moveAmount > 0.5f)
        {
            playerManager.characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
        }
        else if (PlayerInputManager.instance.moveAmount <= 0.5f && PlayerInputManager.instance.moveAmount > 0)
        {
            playerManager.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
        }
    }   
}
