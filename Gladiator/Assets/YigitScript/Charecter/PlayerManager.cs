using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    PlayerMovement playerMovement;
    public CharacterController characterController;

    public void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        characterController = GetComponent<CharacterController>();

    }
    public void Update()
    {
         playerMovement.HandleAllMovement();
    }
}
