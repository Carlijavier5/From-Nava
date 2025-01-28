using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseObject))]
public class DissolveController : MonoBehaviour {

    [SerializeField] private BaseObject baseObject;
    [SerializeField] private Material material;
    private float amount = 1;
    private float speed = 1;
    private Coroutine dissolveRoutine;

    public void DissolveOut() {
        if (dissolveRoutine != null) {
            StopCoroutine(dissolveRoutine);
            baseObject.RemoveMaterial(material);
        }
        dissolveRoutine = StartCoroutine(DissolveOutCoroutine());
    }

    public void DissolveIn() {
        if (dissolveRoutine != null) {
            StopCoroutine(dissolveRoutine);
            baseObject.RemoveMaterial(material);
        }
        dissolveRoutine = StartCoroutine(DissolveInCoroutine());
    }

    public IEnumerator DissolveOutCoroutine() {
        baseObject.ApplyMaterial(material);
        MaterialPropertyBlock mpb = new();
        while (amount > -0.1) {
            amount -= Time.deltaTime * speed;
            foreach (SpriteRenderer spriteRenderer in baseObject.SpriteRenderers) {
                spriteRenderer.GetPropertyBlock(mpb);
                mpb.SetFloat("_DissolveAmount", amount);
                spriteRenderer.SetPropertyBlock(mpb);
            }
            yield return null;
        }
        dissolveRoutine = null;
    }

    public IEnumerator DissolveInCoroutine() {
        baseObject.ApplyMaterial(material);
        MaterialPropertyBlock mpb = new();
        while (amount <= 1) {
            amount += Time.deltaTime * speed;
            foreach (SpriteRenderer spriteRenderer in baseObject.SpriteRenderers) {
                spriteRenderer.GetPropertyBlock(mpb);
                mpb.SetFloat("_DissolveAmount", amount);
                spriteRenderer.SetPropertyBlock(mpb);
            }
            yield return null;
        }
        dissolveRoutine = null;
        baseObject.RemoveMaterial(material);
    }

    #if UNITY_EDITOR
    private void Reset() => TryGetComponent(out baseObject);
    #endif
}
