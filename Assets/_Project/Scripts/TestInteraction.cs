using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInteraction : MonoBehaviour
{
    [SerializeField] private NPC_WeaponSmith _smithNPC;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.G))
        {
            if (_smithNPC != null) _smithNPC.Interact(this.gameObject);
        }
    }
}
