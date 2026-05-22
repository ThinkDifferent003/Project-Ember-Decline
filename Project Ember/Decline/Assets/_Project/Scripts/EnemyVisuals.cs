using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVisuals : MonoBehaviour
{
    private Renderer _renderer;
    private Color _originalColor;
    private bool _isFlashingFromHit = false;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer != null) _originalColor = _renderer.material.color;
    }
    public void FlashBurn()
    {
        if (_renderer == null) return;
        if (_isFlashingFromHit) return;
        CancelInvoke(nameof(ResetColor));
        _renderer.material.color = new Color(1f, 0.4f, 0f);
        Invoke(nameof(ResetColor), 0.1f);
    }
    public void FlashHit()
    {
        if (_renderer == null) return;
        StopAllCoroutines();
        CancelInvoke(nameof(ResetColor));
        _isFlashingFromHit = true;
        StartCoroutine(FlashRoutine());
    }
    private IEnumerator FlashRoutine()
    {
        _renderer.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        _renderer.material.color = _originalColor;
        _isFlashingFromHit = false;
    }
    private void ResetColor()
    {
        if (_renderer != null) _renderer.material.color = _originalColor;
    }
}
