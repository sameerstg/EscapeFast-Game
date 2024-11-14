using System;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] PlayerController controller;
    private void Start()
    {
        controller.OnPlayerStateChange += OnPlayerStateChanged;
    }

    private void OnPlayerStateChanged(PlayerState state)
    {
        if (state == PlayerState.Up)
        {
            animator.SetBool("Jump", true);
        }
        else
        {
            animator.SetBool("Jump", false);
        }
    }
}
