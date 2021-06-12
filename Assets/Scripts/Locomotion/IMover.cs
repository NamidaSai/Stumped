using UnityEngine;

public interface IMover
{
	public void Move(Vector2 moveThrottle);
	public void Jump();
}