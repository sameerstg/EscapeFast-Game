using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float horizontalChange;
    public float speed;
    public InputSystem input;
    Rigidbody rb;
    public HorizontalPosition horizontalPosition;
    public bool canMove;
    private void Awake()
    {
        input = new();
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        horizontalPosition = HorizontalPosition.middle;
        canMove = true;
    }
    private void OnEnable()
    {
        input.Enable();
        input.ActionMap.Left.started += Left;
        input.ActionMap.Right.started += Right;
        
    }
    private void Update()
    {
        //rb.AddForce(Vector3.forward * 1000);
        rb.velocity = Vector3.forward * speed;
    }
    private void Right(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!canMove)
        {
            return;
        }
        if (horizontalPosition == HorizontalPosition.middle)
        {
            horizontalPosition = HorizontalPosition.right;
        }
        else if (horizontalPosition == HorizontalPosition.left)
        {
            horizontalPosition = HorizontalPosition.middle;

        }
        else
        {
            return;
        }
        ChangeHorizontalPosition(Vector3.right * horizontalChange);
    }

    private void Left(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!canMove)
        {
            return;
        }
        if (horizontalPosition == HorizontalPosition.middle)
        {
            horizontalPosition = HorizontalPosition.left;
        }
        else if (horizontalPosition == HorizontalPosition.right)
        {
            horizontalPosition = HorizontalPosition.middle;

        }
        else
        {
            return;
        }
        ChangeHorizontalPosition(Vector3.left * horizontalChange);
    }

    public void ChangeHorizontalPosition(Vector3 change)
    {
        canMove = false;
        iTween.MoveAdd(gameObject,iTween.Hash("time",0.3f,"amount",change, "oncomplete",nameof(OnMoveComplete)));
        //transform.position += change;
    }
    void OnMoveComplete()
    {
        canMove = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlatTrigger"))
        {
            PlatformGenerator._instance.PlatformPool(other.gameObject);
        }
    }

}
public enum HorizontalPosition
{
    middle,left,right
}
