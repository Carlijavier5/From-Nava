using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyObjectController : MonoBehaviour
{
    [SerializeField] private Damageable damageableModule;
    [SerializeField] private SpellSO spellData;
    private float timer;

    void Start() {
        damageableModule.OnDeath += DamageableModule_OnDeath;
    }

    private void DamageableModule_OnDeath() {
        Destroy(gameObject, 2);
    }

    // Update is called once per frame
    void Update() {
        timer += Time.deltaTime;
        if (timer >= spellData.lifetime) {
            damageableModule.Kill();
        }
    }
}
