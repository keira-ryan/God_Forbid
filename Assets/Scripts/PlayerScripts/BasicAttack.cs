using System;
using DamageSystem;
using UnityEngine;

public class BasicAttack: MonoBehaviour
{
    private float timeBetweenAttacks;
    public float startTimeBetweenAttacks;
    

    public Transform attackPosition;
    public LayerMask enemyMask;
    
    public float attackRange;
    public int damage;

    void Update()
    {
        if (timeBetweenAttacks <= 0)
        {
            if(Input.GetMouseButton(0))
            {
                HandleBasicAttack();
            }
            timeBetweenAttacks = startTimeBetweenAttacks;
        }
        else
        {
            timeBetweenAttacks -= Time.deltaTime;
        }
    }

    public void HandleBasicAttack()
    {
        Collider2D[] enemiestoDamage = Physics2D.OverlapCircleAll(attackPosition.position, attackRange, enemyMask);
        for (int i = 0; i < enemiestoDamage.Length; i++)
        {
            enemiestoDamage[i].GetComponent<Hurtbox>().DecrementHealth(damage);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPosition.position, attackRange);
    }
}
