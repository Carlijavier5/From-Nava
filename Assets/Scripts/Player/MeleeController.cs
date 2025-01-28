using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeController : MonoBehaviour {

    [SerializeField] private Collider2D hitboxUp, hitboxRight,
                                        hitboxDown, hitboxLeft;
    [SerializeField] private int meleeDamage;
    [SerializeField] private float startDelay, attackDuration;
    private readonly HashSet<BaseObject> collisionSet = new();

    public void DoMelee(Vector3 dir) => StartCoroutine(IDoMelee(dir));

    private IEnumerator IDoMelee(Vector3 dir) {
        Collider2D atkCollider = dir == Vector3.up ? hitboxUp
                               : dir == Vector3.right ? hitboxRight
                               : dir == Vector3.down ? hitboxDown
                               : hitboxLeft;
        yield return new WaitForSeconds(startDelay);
        atkCollider.enabled = true;
        yield return new WaitForSeconds(attackDuration);
        atkCollider.enabled = false;
        collisionSet.Clear();
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.TryGetComponent(out BaseObject baseObject)) {
            if (!collisionSet.Contains(baseObject)) {
                collisionSet.Add(baseObject);
                baseObject.Damage(meleeDamage);
            }
        }
    }
}
