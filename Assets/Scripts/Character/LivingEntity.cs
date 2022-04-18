using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour
{

    internal bool isDead;
    internal int currentHealth;
    public int maxHealth;
    public string playerTag;

    public void OnDamage(int damageAmount)
    {
        if (isDead)
            return;

        if(currentHealth > 0)
        {
            currentHealth -= damageAmount;

            EventManager.Instance.HealthChange((float)currentHealth / (float)maxHealth, playerTag);

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                isDead = true;
                //Death logic
                PlayersManager.Instance.Death?.Invoke((FighterControllerBase)this);
            }
        }
    }

    public void UpdateHealth(float currentHealthValue)
    {
        if (isDead)
            return;

        currentHealth = (int)currentHealthValue;
        EventManager.Instance.HealthChange(currentHealth / (float)maxHealth, playerTag);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;
            //Death logic
            PlayersManager.Instance.Death?.Invoke((FighterControllerBase)this);
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        EventManager.Instance.HealthChange((float)currentHealth / (float)maxHealth, playerTag);
    }
}
