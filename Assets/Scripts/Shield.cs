using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Shield : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    [Networked(OnChanged = nameof(NetworkShieldTouchedChanged))]
    public bool NetworkShieldTouched { get; set; }
    private static void NetworkShieldTouchedChanged(Changed<Shield> changed) {
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
        gameObject.transform.position += new Vector3(1000, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
}