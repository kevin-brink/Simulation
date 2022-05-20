using UnityEngine;

public class MoveComponent : MonoBehaviour
{
	[SerializeField]
	protected float move_speed;

	[SerializeField]
	private float rotate_speed = 720f;

	public void SetSpeed(float move_speed) => this.move_speed = move_speed;

	public void Move(Vector2 movement_direction)
	{
		movement_direction.Normalize();

		transform.Translate(movement_direction * move_speed * Time.deltaTime, Space.World);

		if (movement_direction != Vector2.zero)
		{
			Quaternion to_rotation = Quaternion.LookRotation(Vector3.forward, movement_direction);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, to_rotation, rotate_speed * Time.deltaTime);
		}
	}

	public void MoveToward(Vector2 target)
	{
		Move(target - (Vector2)transform.position);
	}
}
