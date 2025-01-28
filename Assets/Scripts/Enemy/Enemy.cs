using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum EnemyState {
    IDLE,
    WANDER,
    CHASE,
    ATTACK,
    PUSHED,
    STUNNED,
    SLOWED,
    DEAD
}

public class Enemy : BaseObject, IPushable
{
    public event Action<Enemy, bool> OnPlayerInRange;
    public event Action OnDeath;

    public bool IsAlive { get; private set; } = true;

    [SerializeField] private GameObject tutorialSpellObject;

    private bool isPushed;
    private float pushDist;
    private float pushSpd;
    private Vector3 pushDir;

    public UnityEvent onMeleeHit;

    public EnemyState currState;

    protected override void Start() {
        base.Start();
        OnPlayerInRange += BattleManager.Instance.RegisterEnemy;

        isPushed = false;
    }

    void OnDestroy() {
        OnPlayerInRange?.Invoke(this, false);
        OnDeath?.Invoke();
    }

    public override void Kill() => StartCoroutine(DeathSequence());

    public void Push(Vector2 dir, float dist, float spd) {
        isPushed = true;
        pushDir = new Vector3(dir.x, dir.y, 0);
        pushDist = dist;
        pushSpd = spd;
    }

    public void PushTranslate() {
        if (pushDist <= 0) {
            isPushed = false;
        } else {
            transform.Translate(pushDir * pushSpd * Time.deltaTime);
            pushDist -= (pushDir *  pushSpd * Time.deltaTime).magnitude;
        }
    }

    public bool GetPushed() {
        return isPushed;
    }

    IEnumerator DeathSequence() {
        IsAlive = false;
        yield return new WaitForSeconds(1f);
        if (tutorialSpellObject) {
            tutorialSpellObject.transform.position = gameObject.transform.position;
        }
        Destroy(gameObject);
    }

    public void ReactToPlayerInRange(bool playerInRange) {
        OnPlayerInRange?.Invoke(this, playerInRange);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (!other.gameObject.CompareTag("WindBlast")) {
            if (isPushed) {
                pushDist = 0;
                isPushed = false;
            }
        }
    }
}
