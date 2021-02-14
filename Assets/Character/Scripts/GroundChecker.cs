using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [HideInInspector] public bool isGrounded = false;

    public float groundDistance = 0.4f;
    public LayerMask layerToCheck;

    void Update()
    {
        isGrounded = Physics.CheckSphere(transform.position, groundDistance, layerToCheck);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, groundDistance);
    }
}
