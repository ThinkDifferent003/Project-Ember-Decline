using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using System;
using TMPro;
using UnityEngine.SearchService;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    public static event Action<string> OnDialogueEventTriggered;
    public static event Action OnDialogueStart;
    public static event Action OnDialogueEnd;

    [SerializeField] private TextAsset _inkJsonAsset;
    [SerializeField] private GameObject _dialoguePanel;
    [SerializeField] private TextMeshProUGUI _dialogueText;
    [SerializeField] private GameObject _choicesContainer;
    [SerializeField] private GameObject _choiceButtonPrefab;
    [SerializeField] private bool _isDialogueActive = false;
    private Story _story;
    private Coroutine _typingCoroutine;
    public bool IsDialogueActive => _isDialogueActive;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        if (_inkJsonAsset != null) _story = new Story(_inkJsonAsset.text);
        _dialoguePanel.SetActive(false);
    }
    private void Update()
    {
        if (_isDialogueActive && _story.currentChoices.Count == 0)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                AdvanceDialogue();
            }
        }
    }
    public void StartDialogue(string knotName)
    {
        if (_story == null) return;
        _dialoguePanel.SetActive(true);
        _isDialogueActive = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        OnDialogueStart?.Invoke();
        try
        {
            _story.ChoosePathString(knotName);
            AdvanceDialogue();
        }
        catch (Exception e)
        {
            Debug.LogError($"[DialogueManager] Impossibile trovare il nodo '{knotName}' in Ink. Errore: {e.Message}");
            EndDialogue();
        }
    }
    private void AdvanceDialogue()
    {
        ClearChoicesUI();
        if (_story.canContinue)
        {
            string nextLine = _story.Continue();
            nextLine = nextLine.Trim();
            if (!string.IsNullOrEmpty(nextLine))
            {
                if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
                _typingCoroutine = StartCoroutine(TypeText(nextLine));
            }
            HandleTags(_story.currentTags);
            if (_story.currentChoices.Count > 0) DisplayChoices();
        }
        else EndDialogue();
    }
    private void DisplayChoices()
    {
        List<Choice> currentChoises = _story.currentChoices;
        foreach (Choice choice in currentChoises)
        {
            GameObject buttonObj = Instantiate(_choiceButtonPrefab, _choicesContainer.transform);
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = choice.text;
            Button button = buttonObj.GetComponent<Button>();
            button.onClick.AddListener(() => MakeChoice(choice.index));
        }
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_choicesContainer.GetComponent<RectTransform>());
    }
    private void MakeChoice(int index)
    {
        _story.ChooseChoiceIndex(index);
        AdvanceDialogue();
    }
    private void HandleTags(List<string> tags)
    {
        foreach (string tag in tags)
        {
            if (tag.StartsWith("EVENTO:"))
            {
                string eventName = tag.Replace("EVENTO:", "").Trim();
                OnDialogueEventTriggered?.Invoke(eventName);
            }
        }
    }
    public void SetInkVariable(string variable, object value)
    {
        if (_story != null)
        {
            try
            {
                _story.variablesState[variable] = value;
            }
            catch (Exception)
            {
                Debug.LogWarning($"[DialogueManager] La variabile '{variable}' non esiste nel file Ink globale.");
            }
        }
    }
    private void ClearChoicesUI()
    {
        foreach (Transform child in _choicesContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }
    public void EndDialogue()
    {
        _isDialogueActive = false;
        _dialoguePanel.SetActive(false);
        _dialogueText.text = "";
        ClearChoicesUI();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        OnDialogueEnd?.Invoke();
    }
    private IEnumerator TypeText(string line)
    {
        _dialogueText.text = "";
        foreach (char letter in line.ToCharArray())
        {
            _dialogueText.text += letter;
            yield return new WaitForSeconds(0.02f);
        }
        _typingCoroutine = null;
    }
}
