using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private IInteractable _interactable;

    private void Update()
    {
        if (_interactable != null && Input.GetKeyDown(KeyCode.E))
        {
            if (DialogueManager.Instance != null && !DialogueManager.Instance.IsDialogueActive) _interactable.Interact(this.gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null) _interactable = interactable;
    }
    private void OnTriggerExit(Collider other)
    {
        IInteractable interactable = other?.GetComponent<IInteractable>();
        if (interactable != null && interactable == _interactable) _interactable = null;
    }
}
