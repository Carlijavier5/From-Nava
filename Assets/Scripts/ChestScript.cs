using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ChestScript : IInteractable
{
    private ClaimCollectible collectible;

	private CinemachineVirtualCamera virtualCamera;
	private Transform returnToPlayer;

    private Animator animator;
    [SerializeField] private GameObject[] doors;

    [SerializeField] private bool startsActive;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private bool hasDoors;

    private Color spriteColor;
    private bool hasOpened;

    // Start is called before the first frame update
    void Start()
    {
        spriteColor = GetComponent<SpriteRenderer>().color;
        collectible = GetComponent<ClaimCollectible>();
        collectible.OnCollectibleClaimed += ChestScript_OnCollectibleClaimed;
        playerController = PlayerController.Instance;

		virtualCamera = ReferenceSingleton.Instance.mainCamera.GetComponentInChildren<CinemachineVirtualCamera>();
        returnToPlayer = PlayerController.Instance.transform;
        animator = GetComponent<Animator>();
        if (!startsActive) StartCoroutine(CameraTransitionIn());
    }

    //Overrides the method in IInteractable such that the intended behavior for interacting is executed
    //In this case we will now call AwaitCollectible;
    protected override void InteractBehavior() {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("ChestIdle")) { //Scuffed check but necessary (for now) to ensure that chest spam isn't possible
            StartCoroutine(AwaitCollectible());
        }
    }

    IEnumerator CameraTransitionIn() {
        spriteColor.a = 0f;
        GetComponent<SpriteRenderer>().color = spriteColor;
        playerController.DeactivateMovement();
        while (spriteColor.a < 1.02f) {
            //print(spriteColor.a);
            spriteColor.a += 0.02f;
            GetComponent<SpriteRenderer>().color = spriteColor;
            yield return new WaitForSeconds(0.01f);
        }
		//yield return new WaitForSeconds(0.5f); //delay before moving camera //maybe dissolve/fade chest in over 0.5f seconds???
		virtualCamera.Follow = transform;
		yield return new WaitForSeconds(2f);
        virtualCamera.Follow = returnToPlayer;
        playerController.ActivateMovement();
	}

    // VISUAL BUG: When exiting and reentering range after having claimed already, the button prompt will show up when it shouldn't.
    IEnumerator AwaitCollectible() {
		awaitingCollectible = true;
        FadeButton();
		collectible.Collect();
        animator.SetBool("OpeningChest", true);
		AudioControl.Instance.PlaySFX("Chest Open", gameObject);
        var timer = 2.5f;
        while (awaitingCollectible || timer > 0) {
            if (timer > 0) timer -= Time.deltaTime;
            yield return null;
        } animator.SetBool("OpeningChest", false);
		AudioControl.Instance.PlaySFX("Chest Close", gameObject);
		if (hasDoors && !hasOpened) {
            foreach (GameObject door in doors) {
                door.GetComponent<Door>().OpenDoor();
            }
            hasOpened = true;
        }
        CreateButtonTutorial();
    }

    private void ChestScript_OnCollectibleClaimed() {
        awaitingCollectible = false;
    }
}