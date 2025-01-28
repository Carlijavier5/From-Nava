using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private PlayerController playerControl;
    [SerializeField] private Damageable damageable;
    private int health;
    public Image[] healthBars;

    void Start() {
        damageable.OnDamageTaken += Damageable_OnDamageTaken;
        health = damageable.CurrHealth;
        if (health >= 9) {
            healthBars[9].gameObject.SetActive(true);
        } else if (health <= 0) {
            healthBars[0].gameObject.SetActive(true);
        } else {
            healthBars[health].gameObject.SetActive(true);
        }
    }

    private void Damageable_OnDamageTaken(int _) {
        int newHealth = damageable.CurrHealth;
        for (int i = 0; i < 10; i++) {
            healthBars[i].gameObject.SetActive(false);
        }
        if (newHealth < 0) {
            newHealth = 0;
        }
        healthBars[newHealth].gameObject.SetActive(true);
        StartCoroutine(FlashingHealth(newHealth));
    }

    public void ChangeHealth(int newHealth) {

    }

    IEnumerator FlashingHealth(int newHealth) {
        healthBars[newHealth].gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        yield return new WaitForSeconds(0.25f);
        healthBars[newHealth].gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 150);
    }
}
