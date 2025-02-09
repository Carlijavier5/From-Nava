using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BigGuyDirection {
    RIGHT,
    LEFT,
    UP,
    DOWN
}

public class BigGuyMelee : MonoBehaviour
{
    public PlayerController player;
    private Vector3 dir;

    public GameObject windBlast;

    public Spell spell;

    public bool isWindBlasting = false;
    public float currTime;

    private Vector2 fireRight = Vector2.right;
    private Vector2 fireLeft = Vector2.left;
    private Vector2 fireUp = Vector2.up;
    private Vector2 fireDown = Vector2.down;

    [SerializeField] private BigGuyDirection facingDir;

    private void Update() {
        currTime += Time.deltaTime;
        if (isWindBlasting) {
            Fire();
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.TryGetComponent(out BaseObject baseObject)) {
            baseObject.Damage(2);
        }
        dir = player.gameObject.GetComponent<Transform>().position - transform.position;
        if (player.PlayerHealth > 0) {
            player.gameObject.GetComponent<Rigidbody2D>().AddRelativeForce(2000*dir);
        }
    }

    void Fire() {
        if (currTime >= 0) {
            //Instantiate(windBlast, this.gameObject.GetComponent<Transform>().position, Quaternion.identity);
            Spell spellGO;
            switch (facingDir) {
                case BigGuyDirection.RIGHT:
                    AudioControl.Instance.PlaySFX(spell.spell.sfxString, PlayerController.Instance.gameObject, 0.1f, 0.5f);
                    spellGO = Instantiate(spell, this.gameObject.GetComponent<Transform>().position, Quaternion.identity);
                    spellGO.CastSpell(this, fireDown);
                    AudioControl.Instance.PlaySFX(spell.spell.sfxString, PlayerController.Instance.gameObject, 0.1f, 0.5f);
                    spellGO = Instantiate(spell, this.gameObject.GetComponent<Transform>().position, Quaternion.identity);
                    spellGO.CastSpell(this, fireRight);
                    AudioControl.Instance.PlaySFX(spell.spell.sfxString, PlayerController.Instance.gameObject, 0.1f, 0.5f);
                    spellGO = Instantiate(spell, this.gameObject.GetComponent<Transform>().position, Quaternion.identity);
                    spellGO.CastSpell(this, fireUp);
                    break;
                case BigGuyDirection.LEFT:
                    AudioControl.Instance.PlaySFX(spell.spell.sfxString, PlayerController.Instance.gameObject, 0.1f, 0.5f);
                    spellGO = Instantiate(spell, this.gameObject.GetComponent<Transform>().position, Quaternion.identity);
                    spellGO.CastSpell(this, fireDown);
                    AudioControl.Instance.PlaySFX(spell.spell.sfxString, PlayerController.Instance.gameObject, 0.1f, 0.5f);
                    spellGO = Instantiate(spell, this.gameObject.GetComponent<Transform>().position, Quaternion.identity);
                    spellGO.CastSpell(this, fireLeft);
                    AudioControl.Instance.PlaySFX(spell.spell.sfxString, PlayerController.Instance.gameObject, 0.1f, 0.5f);
                    spellGO = Instantiate(spell, this.gameObject.GetComponent<Transform>().position, Quaternion.identity);
                    spellGO.CastSpell(this, fireUp);
                    break;
                case BigGuyDirection.UP:
                    AudioControl.Instance.PlaySFX(spell.spell.sfxString, PlayerController.Instance.gameObject, 0.1f, 0.5f);
                    spellGO = Instantiate(spell, this.gameObject.GetComponent<Transform>().position, Quaternion.identity);
                    spellGO.CastSpell(this, fireUp);
                    AudioControl.Instance.PlaySFX(spell.spell.sfxString, PlayerController.Instance.gameObject, 0.1f, 0.5f);
                    spellGO = Instantiate(spell, this.gameObject.GetComponent<Transform>().position, Quaternion.identity);
                    spellGO.CastSpell(this, fireLeft);
                    AudioControl.Instance.PlaySFX(spell.spell.sfxString, PlayerController.Instance.gameObject, 0.1f, 0.5f);
                    spellGO = Instantiate(spell, this.gameObject.GetComponent<Transform>().position, Quaternion.identity);
                    spellGO.CastSpell(this, fireRight);
                    break;
                case BigGuyDirection.DOWN:
                    AudioControl.Instance.PlaySFX(spell.spell.sfxString, PlayerController.Instance.gameObject, 0.1f, 0.5f);
                    spellGO = Instantiate(spell, this.gameObject.GetComponent<Transform>().position, Quaternion.identity);
                    spellGO.CastSpell(this, fireDown);
                    AudioControl.Instance.PlaySFX(spell.spell.sfxString, PlayerController.Instance.gameObject, 0.1f, 0.5f);
                    spellGO = Instantiate(spell, this.gameObject.GetComponent<Transform>().position, Quaternion.identity);
                    spellGO.CastSpell(this, fireLeft);
                    AudioControl.Instance.PlaySFX(spell.spell.sfxString, PlayerController.Instance.gameObject, 0.1f, 0.5f);
                    spellGO = Instantiate(spell, this.gameObject.GetComponent<Transform>().position, Quaternion.identity);
                    spellGO.CastSpell(this, fireRight);
                    break;
            }
            currTime = -0.1f;
        }
    }
}
