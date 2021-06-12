using UnityEngine;

public class BasicMover : MonoBehaviour, IMover
{
	[SerializeField] float moveSpeed = 10f;
	[SerializeField] float maxSpeed = 10f;
	[SerializeField] float airSpeed = 5f;
	[SerializeField] float bonusGravity = 9.8f;
	[SerializeField] float jumpSpeed = 10f;

	private Rigidbody2D myRigidbody;
	private BoxCollider2D myFeet;
	private bool playerIsTouchingGround;

	private void Start()
	{
		myRigidbody = GetComponentInParent<Rigidbody2D>();
		myFeet = GetComponent<BoxCollider2D>();
	}

	private void FixedUpdate()
	{
		ApplyBonusGravity();
		ClampMoveVelocity();
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
			transform.localScale = new Vector3(Mathf.Sign(moveThrottle.x), transform.localScale.y, transform.localScale.z);
		}
	}

	public void Jump()
	{
		if (!playerIsTouchingGround) { return; }

		float currentJumpSpeed = jumpSpeed * 100f;
		Vector2 jumpForce = new Vector2(0f, currentJumpSpeed);
		myRigidbody.AddForce(jumpForce);
	}
}