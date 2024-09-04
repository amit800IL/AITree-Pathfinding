using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "NewMyScriptableObject", menuName = "ScriptableObjects/EnemyData")]
public class EnemyDataSO : ScriptableObject
{
    [Header("Vision Node Settings")]

    [SerializeField] public float visionDistance;

    [Header("smell Node Settings")]

    [SerializeField] public float smellRadius;

    [Header("hear Node Settings")]

    [SerializeField] public float hearRadius;

    [Header("Attack Node Settings")]

    [SerializeField] public float damage;
}
