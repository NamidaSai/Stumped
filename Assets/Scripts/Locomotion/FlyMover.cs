using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyMover : MonoBehaviour, IMover
{
	[SerializeField] float lifetime = 5f;
	[SerializeField] float settleGravity = 9.8f;
	[SerializeField] float fadeDelay = 4f;
	[SerializeField] float fadeAffordance = 1f;
	[SerializeField] float moveSpeed = 12f;
	[SerializeField] float maxSpeedMove = 1.5f;
	[SerializeField] float maxSpeedFlight = 24f;
	[SerializeField] float airSpeed = 24f;
	[SerializeField] float antiGravity = 9.8f;
	[SerializeField] float jumpSpeed = 1f;
	[SerializeField] GameObject stumpSprite = default;
	[SerializeField] SpriteRenderer flyRenderer = default;
	[SerializeField] Sprite flyStartSprite = default;
	[SerializeField] Sprite flyHalfSprite = default;
	[SerializeField] Sprite flyEndSprite = default;
	[SerializeField] public Sprite flyDiscardSprite = default;

	private float currentTimer = 0f;
	private float currentAntiGravity = 0f;
	private float timeIncrement = 0f;
	private bool flightStarted = false;

	private Rigidbody2D myRigidbody;
	private BoxCollider2D myFeet;
	private bool playerIsTouchingGround;
	private bool playerIsTouchingObstacle;
	private AudioManager audioManager;

	private void Start()
	{
		myRigidbody = GetComponentInParent<Rigidbody2D>();
		myFeet = GetComponent<BoxCollider2D>();
		audioManager = FindObjectOfType<AudioManager>();
	}

	private void OnEnable()
	{
		flyRenderer.GetComponent<SpriteRenderer>().sprite = flyStartSprite;
	}

	private void Update()
	{
		CountDown();
	}

	private void FixedUpdate()
	{
		ApplyBonusGravity();
		if (playerIsTouchingGround || playerIsTouchingObstacle)
		{
			ClampMoveVelocity(maxSpeedMove);
			GetComponent<Animator>().ResetTrigger("Jump");
		}
		else
		{
			ClampMoveVelocity(maxSpeedFlight);
		}
	}

	private void ApplyBonusGravity()
	{
		playerIsTouchingGround = myFeet.IsTouchingLayers(LayerMask.GetMask("Ground"));
		if (!playerIsTouchingGround)
		{
			Vector2 currentVelocity = myRigidbody.velocity;
			currentVelocity.y += currentAntiGravity * Time.deltaTime;
			myRigidbody.velocity = currentVelocity;
		}
	}

	private void ClampMoveVelocity(float maxSpeed)
	{
		if (playerIsTouchingGround)
		{
			Vector2 currentVelocity = myRigidbody.velocity;
			currentVelocity.x = Mathf.Clamp(currentVelocity.x, -maxSpeed, maxSpeed);
			myRigidbody.velocity = currentVelocity;
		}
	}

	public void Move(Vector2 moveThrottle)
	{
		if (myRigidbody == null) { return; }

		float moveSpeedX;

		if (playerIsTouchingGround)
		{
			moveSpeedX = moveSpeed * Time.deltaTime * moveThrottle.x;
		}
		else
		{
			moveSpeedX = airSpeed * Time.deltaTime * moveThrottle.x;
		}

		float currentMoveSpeed = moveSpeedX * 100f;
		Vector2 moveForce = new Vector2(currentMoveSpeed, 0f);
		myRigidbody.AddForce(moveForce);

		FlipSprite(moveThrottle);
	}

	private void FlipSprite(Vector2 moveThrottle)
	{
		bool playerHasHorizontalSpeed = Mathf.Abs(moveThrottle.x) > Mathf.Epsilon;

		if (playerHasHorizontalSpeed)
		{
			GetComponent<Animator>().SetBool("isWalking", true);
			stumpSprite.transform.localScale = new Vector3(Mathf.Sign(moveThrottle.x), stumpSprite.transform.localScale.y, stumpSprite.transform.localScale.z);
		}
		else
		{
			GetComponent<Animator>().SetBool("isWalking", false);
		}
	}

	public void Jump()
	{
		if (!playerIsTouchingGround && !playerIsTouchingObstacle) { return; }

		PlayJumpAnimation();
		PlayJumpSFX();

		if (GetComponent<Pickup>().GetState() != LocomotionState.WIN)
		{
			GetComponentInParent<VehicleHandler>().currentPickup.GetComponent<Pickup>().isUsed = true;
		}

		flightStarted = true;
		currentTimer = lifetime;
		currentAntiGravity = antiGravity;
		timeIncrement = 0f;
		float currentJumpSpeed = jumpSpeed * 100f;
		Vector2 jumpForce = new Vector2(0f, currentJumpSpeed);
		myRigidbody.AddForce(jumpForce);
	}

	private void CountDown()
	{
		if (currentTimer <= 0f && flightStarted)
		{
			StartCoroutine(FlyFade());
			flyRenderer.sprite = flyHalfSprite;
			return;
		}

		if (currentTimer > 0f)
		{
			currentTimer -= Time.deltaTime;
		}

		if (currentAntiGravity > settleGravity)
		{
			timeIncrement += Time.deltaTime / lifetime;
			currentAntiGravity = Mathf.Lerp(antiGravity, settleGravity, timeIncrement);
		}
	}

	private IEnumerator FlyFade()
	{
		yield return new WaitForSeconds(fadeDelay - fadeAffordance);

		flyRenderer.sprite = flyEndSprite;
		yield return new WaitForSeconds(fadeAffordance);

		GetComponentInParent<VehicleHandler>().DropVehicle();
	}

	public void StopFlight()
	{
		GetComponentInParent<VehicleHandler>().currentPickup.GetComponentInChildren<SpriteRenderer>().sprite = flyDiscardSprite;
		audioManager.Stop("FLYJump");
		flightStarted = false;
	}

	private void PlayJumpSFX()
	{
		audioManager.Play("FLYJump");
	}

	private void PlayJumpAnimation()
	{
		if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Jump"))
		{
			GetComponent<Animator>().ResetTrigger("Jump");
			return;
		}
		GetComponent<Animator>().SetTrigger("Jump");
	}
}