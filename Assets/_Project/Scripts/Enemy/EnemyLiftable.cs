using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLiftable : MonoBehaviour
{
    [SerializeField] private float _weight = 1f;

    public float Weight => _weight;
}
