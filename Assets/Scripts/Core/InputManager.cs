using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles all input in the game
public delegate void OnMovementKeyPressed(string keyName);
public delegate void OnAttackKeyPressed(string keyName);
public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    public OnMovementKeyPressed MovementKeyPressed;
    public OnAttackKeyPressed AttackKeyPressed;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != null)
            Destroy(gameObject);
    }

    private void Update()
    {
        //For Test on PC
        KeyPressInput();
    }

    private void KeyPressInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            MovementKeyPressed?.Invoke("W");
        }

        if (Input.GetKey(KeyCode.S))
        {
            MovementKeyPressed?.Invoke("S");
        }

        if (Input.GetKey(KeyCode.A))
        {
            MovementKeyPressed?.Invoke("A");
        }

        if (Input.GetKey(KeyCode.D))
        {
            MovementKeyPressed?.Invoke("D");
        }
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            MovementKeyPressed?.Invoke("C");
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            MovementKeyPressed?.Invoke("Space");
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            AttackKeyPressed?.Invoke("Space");
        }
        
        if (Input.GetKeyDown(KeyCode.U))
        {
            AttackKeyPressed?.Invoke("U");
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            AttackKeyPressed?.Invoke("I");
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            AttackKeyPressed?.Invoke("O");
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            AttackKeyPressed?.Invoke("P");
        }
    }
}
