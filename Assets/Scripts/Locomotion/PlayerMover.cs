using UnityEngine;

public class PlayerMover : MonoBehaviour
{
	[SerializeField] float moveSpeed = 10f;
	[SerializeField] float maxSpeed = 10f;
	[SerializeField] float airSpeed = 5f;
	[SerializeField] float jumpSpeed = 10f;

	private Rigidbody2D myRigidbody;
	private BoxCollider2D myFeet;

	private void Start()
	{
		myRigidbody = GetComponent<Rigidbody2D>();
		myFeet = GetComponent<BoxCollider2D>();
	}

	public void Move(Vector2 moveThrottle)
	{
		float moveSpeedX = 0f;
		bool playerIsTouchingGround = myFeet.IsTouchingLayers(LayerMask.GetMask("Ground"));

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
		bool playerHasHorizontalSpeed = (Mathf.Abs(moveThrottle.x) > Mathf.Epsilon);
		if (playerHasHorizontalSpeed)
		{
			transform.localScale = new Vector3(Mathf.Sign(moveThrottle.x), transform.localScale.y, transform.localScale.z);
		}
	}

	public void Jump()
	{
		bool playerIsTouchingGround = myFeet.IsTouchingLayers(LayerMask.GetMask("Ground"));

		if (!playerIsTouchingGround) { return; }

		float currentJumpSpeed = jumpSpeed * 100f;
		Vector2 jumpForce = new Vector2(0f, currentJumpSpeed);
		myRigidbody.AddForce(jumpForce);
	}
}