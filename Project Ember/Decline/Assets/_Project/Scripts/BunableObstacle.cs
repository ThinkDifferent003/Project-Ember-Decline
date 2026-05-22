using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BunableObstacle : MonoBehaviour , IBurnable
{
    [SerializeField] private float _durability = 50f;

    public void Burn(float damage)
    {
        _durability -= damage;
        transform.localScale -= Vector3.one * (damage * 0.01f);
        if (_durability <= 0) Destroy(gameObject);
    }
}
