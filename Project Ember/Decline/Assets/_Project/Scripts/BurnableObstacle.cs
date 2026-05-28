using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnableObstacle : MonoBehaviour , IBurnable
{
    [Header("Stats")]
    [SerializeField] private float _durability = 50f;
    [SerializeField] private float _shrinkFactor = 0.01f;

    #region - Core Logic -
    public void Burn(float damage)
    {
        _durability -= damage;
        UpdateVisuals(damage);
        if (_durability <= 0) DestroyObj();
    }
    private void UpdateVisuals(float damage)
    {
        transform.localScale -= Vector3.one * (damage * _shrinkFactor);
    }
    private void DestroyObj()
    {
        Destroy(gameObject);
    }
    #endregion
}
