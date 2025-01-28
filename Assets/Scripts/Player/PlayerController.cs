//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

//Handle input and movement on Player
public class PlayerController : Singleton<PlayerController>, IPushable
{
    [SerializeField] bool isDark;
    [SerializeField] private float speed = 7f;

    [SerializeField] private Damageable damageableModule;
    [SerializeField] private DissolveController dissolveController;

    public bool IsAlive => damageableModule.CurrHealth > 0;
    public int PlayerHealth => damageableModule.CurrHealth;
    public Transform spawn;

    [SerializeField] private MeleeController meleeController;

    [SerializeField] private Transform rightCast;
    [SerializeField] private Transform leftCast;
    [SerializeField] private Transform upCast;
    [SerializeField] private Transform downCast;

    [SerializeField] private Transform[] castDirTransforms;

    private Vector2 movement;
    private Rigidbody2D rb;
 
    public float collisionOffset = 0.05f;
    public Transform castPoint;

    public Vector2 facingDir;
    private GameObject bruhLight;

    private bool isPushed;
    private float pushDist;
    private float pushSpd;
    private Vector3 pushDir;

    public Vector2 FacingDir
    {
        get => facingDir;
    }

    public Animator animator;

    public bool canMove = true;
    private bool canChangeDir = true;

    private PlayerInput input;

    public ParticleSystem dust;

    private bool hasSetDir;

    public bool hasDoneMoveTooltip = false;
    //These two variables are intended for use to fix movement issues where casting a spell would enable the player to move regardless of status
    //By restoring to if we should be able to move during the current moment, we resolve these issues. 
    //For further detail, go to SafeActivateMovement.
    private bool shouldMove = true;
    private bool shouldChangeDir = true;

    private void Awake() {
        InitializeSingleton();
    }

    private void Start() {
        damageableModule.OnDeath += DamageableModule_OnDeath;
        input = GetComponent<PlayerInput>();
        bruhLight = this.transform.GetChild(0).gameObject;
        facingDir = Vector2.up;
        castPoint = upCast;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        isDark = (spawn.gameObject.tag == "DarkRoom");
        bruhLight.SetActive(isDark);
        canMove = true;
        canChangeDir = true;
    }

    private void DamageableModule_OnDeath() {
        StartCoroutine(Die());
    }

    private void FixedUpdate() {
        if (input.actions["MainMenu"].IsPressed()) {
            SceneManager.LoadScene(0);
        }
        if (canMove) {
            rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
        }
        if (isPushed) {
            PushTranslate();
        }
    }

    private void OnMove(InputValue movementValue) {
        if (IsAlive) {
            movement = movementValue.Get<Vector2>();
            ChooseFacingDir();
        }
    }

    private void ChooseFacingDir ()
    {
        if (canChangeDir == false) {
            return;
        }
        if(!hasSetDir && movement.x > 0) {
            facingDir = Vector2.right;
            castPoint = rightCast;
            hasSetDir = true;
        }
        if(!hasSetDir && movement.x < 0) {
            facingDir = Vector2.left;
            castPoint = leftCast;
            hasSetDir = true;
        }
        if(!hasSetDir && movement.y > 0) {
            facingDir = Vector2.up;
            castPoint = upCast;
            hasSetDir = true;
        }
        if(!hasSetDir && movement.y < 0) {
            facingDir = Vector2.down;
            castPoint = downCast;
            hasSetDir = true;
        }
        if (movement.magnitude > 0) {
            if (!hasDoneMoveTooltip) {
                hasDoneMoveTooltip = true;
            }
            animator.SetFloat("X", movement.x);
            animator.SetFloat("Y", movement.y);
            animator.SetBool("isWalking", true);
            hasSetDir = false;
        } 
        else 
            animator.SetBool("isWalking", false);
            hasSetDir = false;
    }

    private IEnumerator Die() {
		DeactivateMovement();
        damageableModule.RestoreHealth(damageableModule.MaxHealth);
        movement = new Vector2(0, 0);
        animator.SetBool("isWalking", false);
        animator.SetBool("isHurt", true);
        yield return new WaitForSeconds(1.5f);
        ReferenceSingleton.Instance.transition.FadeOut();
        yield return new WaitForSeconds(1.5f);
        transform.position = spawn.transform.position;
        animator.SetBool("isHurt", false);
		dissolveController.DissolveIn();
        ReferenceSingleton.Instance.transition.FadeIn();
        yield return new WaitForSeconds(2f);
        damageableModule.ToggleIFrame(false);
        ActivateMovement();
    }

    void OnMelee() {
        if (canMove) {
			AudioControl.Instance.PlaySFX("Melee Cast", gameObject, 0.2f, 0.5f);
			//Doing this next line will trigger the animation on the player.
            //The trigger will activate the player's melee hitboxes as well disabling the player's movement
            //through a direct function call to SafeDeactivateMovement in Unity Animator. Once the animation is finished playing, it
            //will then directly call SafeActivateMovement. Check each individual animation for melee to ensure that everything is
            //accounted for.
            animator.SetTrigger("doMelee");
            meleeController.DoMelee(facingDir);
            //ScreenShake.Instance.ShakeScreen(5f, .1f);
		}
    }

    //SafeActivateMovement is an alternate for Activate Movement intended to resolve issues with being able to move during
    //cutscenes that were intended to be unmovable. There are two different Activate and Deactivate Movements:
    //Safe will restore to if we should be able to move right now or not - this will be called when the player uses melee, casts a spell, or opens settings.
    //Regular will be used during cutscenes, camera transitions, and notifications.
    public void SafeActivateMovement() {
        canMove = shouldMove;
        canChangeDir = shouldChangeDir;
        ChooseFacingDir();
    }
    
    public void SafeDeactivateMovement() {
        canMove = false;
        canChangeDir = false;
        animator.SetBool("isWalking", false);
    }

    public void ActivateMovement() {
        shouldMove = true;
        shouldChangeDir = true;
        damageableModule.ToggleIFrame(false);
        SafeActivateMovement();
    }

    public void DeactivateMovement() {
        shouldMove = false;
        shouldChangeDir = false;
        damageableModule.ToggleIFrame(true);
        SafeDeactivateMovement();
    }


    public void ChangeSpawn(Transform newSpawn) {
        spawn = newSpawn;
		if (spawn.gameObject.tag == "DarkRoom") {
			isDark = true;
            bruhLight.SetActive(isDark);
		} else {
			isDark = false;
            bruhLight.SetActive(isDark);
		}
	}

    public void Kill() => damageableModule.Kill();

    //adding push behavior for spikes
    public void Push(Vector2 dir, float dist, float spd) {
        canMove = false;
        isPushed = true;
        pushDir = new Vector3(dir.x, dir.y, 0);
        pushDist = dist;
        pushSpd = spd;
        dust.Play();
    }

    public void PushTranslate() {
        if (pushDist <= 0) {
            canMove = true;
            isPushed = false;
        } else {
            canMove = false;
            transform.Translate(pushDir * pushSpd * Time.deltaTime);
            pushDist -= (pushDir *  pushSpd * Time.deltaTime).magnitude;
        }
    }

    public bool GetPushed() {
        return isPushed;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (!other.gameObject.CompareTag("WindBlast")) {
            if (isPushed) {
                pushDist = 0;
                isPushed = false;
                canMove = true;
            }
        }
    }


    //This cannot be the best way to go about this
    //Create an event that will be called when the player presses the right mouse button
    //This is done through Unity NewInput system
    //Then send this event out to its listeners, being in NotepadLogic.
    public static event System.EventHandler OnClearNotepad;
    private void OnClear() {
        OnClearNotepad?.Invoke(this, null);
    }
}


