using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IHealth
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth;

    private int currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void DealDamage(int damageDealt)
    {
        currentHealth -= damageDealt;

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    public void RestoreHealth(int addHealth)
    {
        currentHealth += addHealth;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    private void Die()
    {
        Debug.Log("Health is 0, game over");
    }
}
