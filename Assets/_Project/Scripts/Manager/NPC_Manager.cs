using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Manager : MonoBehaviour , IInteractable
{
    [SerializeField] private string _inkKnotName;
    [SerializeField] private string _interactPrompt = "Parla";

    public string GetInteractPrompt() => _interactPrompt;

    public virtual void Interact(GameObject player)
    {
        if (DialogueManager.Instance != null) DialogueManager.Instance.StartDialogue(_inkKnotName);
    }
}
