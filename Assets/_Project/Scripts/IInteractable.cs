using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable 
{
    string GetInteractPrompt();
    void Interact(GameObject player);
}
