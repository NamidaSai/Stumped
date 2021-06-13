using UnityEngine;

public class BasicMover : MonoBehaviour, IMover
{
	[SerializeField] float moveSpeed = 10f;
	[SerializeField] float maxSpeed = 10f;
	[SerializeField] float airSpeed = 5f;
	[SerializeField] float bonusGravity = 9.8f;
	[SerializeField] float jumpSpeed = 10f;
	[SerializeField] LayerMask jumpableLayers = default;
	[SerializeField] GameObject stumpSprite = default;

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

	private void FixedUpdate()
	{
		ApplyBonusGravity();
		ClampMoveVelocity();
		if (playerIsTouchingGround || playerIsTouchingObstacle)
		{
			GetComponent<Animator>().ResetTrigger("Jump");
		}
	}

	private void ApplyBonusGravity()
	{
		playerIsTouchingGround = myFeet.IsTouchingLayers(LayerMask.GetMask("Ground"));
		if (!playerIsTouchingGround)
		{
			Vector2 currentVelocity = myRigidbody.velocity;
			currentVelocity.y -= bonusGravity * Time.deltaTime;
			myRigidbody.velocity = currentVelocity;
		}
	}

	private void ClampMoveVelocity()
	{
		playerIsTouchingObstacle = myFeet.IsTouchingLayers(jumpableLayers);
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

		if (playerIsTouchingGround || playerIsTouchingObstacle)
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

		if (moveThrottle.x < 0f)
		{
			GetComponent<Animator>().SetBool("isFacingLeft", true);
		}
		else
		{
			GetComponent<Animator>().SetBool("isFacingLeft", false);
		}
	}

	public void Jump()
	{
		if (!playerIsTouchingGround && !playerIsTouchingObstacle) { return; }

		PlayJumpAnimation();
		PlayJumpSFX();

		float currentJumpSpeed = jumpSpeed * 100f;
		Vector2 jumpForce = new Vector2(0f, currentJumpSpeed);
		myRigidbody.AddForce(jumpForce);
	}

	private void PlayJumpSFX()
	{
		switch (GetComponent<Pickup>().GetState())
		{
			case LocomotionState.BB8:
				audioManager.Play("BB8Jump");
				break;
			case LocomotionState.POGO:
				audioManager.Play("POGOJump");
				break;
			case LocomotionState.BASE:
				audioManager.Play("BASEJump");
				break;
		}
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