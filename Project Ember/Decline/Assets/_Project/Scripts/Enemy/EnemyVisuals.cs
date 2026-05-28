using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVisuals : MonoBehaviour
{
    private Renderer _renderer;
    private Color _originalColor;

    private Coroutine _flashCoroutine;

    #region - Lyfe Cycle _
    private void Start()
    {
        _renderer = GetComponentInChildren<Renderer>();
        if (_renderer != null) _originalColor = _renderer.material.color;
    }
    #endregion
    #region - Core Logic -
    public void FlashBurn()
    {
        if (_renderer == null) return;
        StartFlash(new Color(1f, 0.4f, 0f), 0.1f);
    }
    public void FlashHit()
    {
        if (_renderer == null) return;
        StartFlash(Color.red, 0.1f);
    }
    private void StartFlash(Color flashColor, float duration)
    {
        if (_flashCoroutine != null) StopCoroutine(_flashCoroutine);
        _flashCoroutine = StartCoroutine(FlashRoutine(flashColor, duration));
    }
    private IEnumerator FlashRoutine(Color flashColor , float duration)
    {
        _renderer.material.color = flashColor;
        yield return new WaitForSeconds(duration);
        _renderer.material.color = _originalColor;
        _flashCoroutine = null;
    }
    #endregion
}
