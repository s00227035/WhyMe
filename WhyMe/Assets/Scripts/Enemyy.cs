using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Enemyy : MonoBehaviour
{
    public AIPath aiPath;
    public int damage = 40;
    public float AttackRange = 3f;
    public float sprintMovementSpeed = 6.5f; // sprint speed
    public float slowedMovementSpeed = 2f; // slowed speed
    public float attackCooldown = 3f; // Cooldown between attacks
    public float slowedCooldown = 7f; // Time to remain slowed after attack

    private GameObject player;
    private Animator animator;
    private float attackTimer; // Timer for attack cooldown
    private float slowedTimer; // Timer for slowed state


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

    }

    void Update()
    {
        
    }
}
