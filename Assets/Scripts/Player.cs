using System;
using System.Security.Cryptography;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Car car;
    public InputSystem input;
    public float steerAmount;

    Rigidbody rb;
    private void Awake()
    {
        input = new();
        rb = GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        input.Enable();
        input.ActionMap.MouseDelta.started += ChangePosition;

    }

    private void ChangePosition(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        steerAmount += context.ReadValue<Vector2>().x;
    }


    private void Update()
    {

        car.variation = car.GetChange(1, steerAmount);
        rb.velocity = car.variation;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlatTrigger"))
        {
            PlatformGenerator._instance.PlatformPool(other.gameObject);
        }
    }

}
[Serializable]
public class Car
{
    public string name;
    public float forwardAcceleration = 1;
    public float speed = 10;
    public float turnSpeed = 20;
    public Vector3 variation;

    public float GetHorizontalVariation(float steerAmount)
    {
        return steerAmount * Time.deltaTime * turnSpeed;
    }
    public float GetForwardVariation(float accelerator)
    {
        forwardAcceleration += Time.deltaTime * 0.5f;
        var val = accelerator * forwardAcceleration * speed * Time.deltaTime;
        return val;
    }
    public Vector3 GetChange(float accelerator, float steerAmount)
    {
        return new Vector3(GetHorizontalVariation(steerAmount), 0, GetForwardVariation(accelerator));
    }

}
