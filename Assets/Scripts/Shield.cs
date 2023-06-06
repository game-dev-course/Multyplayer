using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Shield : NetworkBehaviour
{
    [Networked(OnChanged = nameof(NetworkShieldTouchedChanged))]
    public bool NetworkShieldTouched { get; set; }

    private static void NetworkShieldTouchedChanged(Changed<Shield> changed)
    {
        changed.Behaviour.ShiftShield();

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if (other.CompareTag("Player"))
        {
            Debug.Log("Shield collide with player!");
            other.gameObject.GetComponent<RaycastAttack>().HasShield = true;
            NetworkShieldTouched = true;
        }
    }

    private void ShiftShield()
    {
        // Create vector3 to shift the cube far from the map.
        int offset = 1000;
        gameObject.transform.position += new Vector3(offset, offset, offset);
    }
}