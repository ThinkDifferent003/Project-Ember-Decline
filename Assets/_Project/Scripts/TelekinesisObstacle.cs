using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelekinesisObstacle : MonoBehaviour
{
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _knockbackForce = 1f;
    [SerializeField] private float _stunDur = 0.2f;
    private bool _isBeingThrow;
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Launch()
    {
        _isBeingThrow = true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (_isBeingThrow) return;
        EnemyHealth enemy = collision.gameObject.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            Vector3 hitDir = (transform.position - collision.transform.position).normalized;
            enemy.TakeDamage(_damage , hitDir, _knockbackForce, _stunDur);
            _isBeingThrow = false;
        }
    }
}
