using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DissolveController))]
public class Damageable : ObjectModule {

    public event System.Action<int> OnDamageTaken;
    public event System.Action OnDeath;

    [SerializeField] private DissolveController dissolveController;
    [SerializeField] private int maxHealth = 5;
    public int MaxHealth => maxHealth;
    public int CurrHealth { get; private set; }

    [Header("Flash Properties")]

    [SerializeField] private Material flashMaterial;
    [SerializeField] private float flashDuration = 0.125f;

    private Coroutine flashRoutine;

    public bool IFrameOn => localIFrameOn && externalIFrameOn;
    private bool externalIFrameOn;
    private bool localIFrameOn;

    void Start() {
        CurrHealth = maxHealth;
        baseObject.OnDamage += BaseObject_OnDamage;
    }

    private void BaseObject_OnDamage(int damageAmount) {
        if (IFrameOn) return;
        CurrHealth -= damageAmount;
        OnDamageTaken?.Invoke(damageAmount);
        if (CurrHealth <= 0) {
            externalIFrameOn = true;
            dissolveController.DissolveOut();
            baseObject.Kill();
            OnDeath?.Invoke();
        }
        Flash();
    }

    public void Kill() => BaseObject_OnDamage(maxHealth);

    public void RestoreHealth(int healAmount) {
        CurrHealth = Mathf.Max(maxHealth, CurrHealth + healAmount);
    }

    public void ToggleIFrame(bool on) => externalIFrameOn = on;
 
    public void Flash() {
        if (flashRoutine != null) {
            StopCoroutine(flashRoutine);
            baseObject.RemoveMaterial(flashMaterial);
        }
        flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine() {
        localIFrameOn = true;
        baseObject.ApplyMaterial(flashMaterial);
        yield return new WaitForSeconds(flashDuration);
        baseObject.RemoveMaterial(flashMaterial);
        localIFrameOn = false;
        flashRoutine = null;
    }

    #if UNITY_EDITOR
    private void Reset() => TryGetComponent(out dissolveController);
    #endif
}