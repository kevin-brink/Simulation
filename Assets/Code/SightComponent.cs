using System.Collections.Generic;

using UnityEngine;

public class SightComponent : MonoBehaviour
{
	public List<float> ray_positions = new();

	[SerializeField]
	public float sight_distance = 10f;

	[SerializeField]
	private bool show_rays = false;

	// See if getting targets periodically rather than constantly will improve performance
	private List<Collider2D> hit_list = new();
	private readonly float target_update_interval = 0.1f;
	private float last_target_update = 0f;

	public void Start()
	{
		// Set this randomly between 0 and the interval so things arent all updating on the same frame
		last_target_update = UnityEngine.Random.value * target_update_interval + Time.time;
	}

	public List<Collider2D> GetTargets()
	{
		if (last_target_update + target_update_interval > Time.time)
			return hit_list;

		List<Ray> rays = GetRays();

		List<Collider2D> hits = new();
		foreach (Ray ray in rays)
		{
			hits.Add(Physics2D.Raycast(ray.origin, ray.direction, sight_distance).collider);
		}

		last_target_update = Time.time;
		hit_list = hits;
		return hit_list;
	}

	private List<Ray> GetRays()
	{
		List<Ray> temp_rays = new List<Ray>();
		foreach (float ray in ray_positions)
		{
			temp_rays.Add(new Ray(transform.position, Quaternion.AngleAxis(ray, transform.forward) * transform.up));
			temp_rays.Add(new Ray(transform.position, Quaternion.AngleAxis(-ray, transform.forward) * transform.up));
		}

		// Need to do this one so that the ray begins just outside of the collider
		List<Ray> rays = new();
		foreach (Ray temp_ray in temp_rays)
		{
			CircleCollider2D collider = GetComponentInParent<BaseCreature>().collider;
			Vector3 start_position = temp_ray.direction.normalized * (collider.radius * transform.localScale.magnitude) + temp_ray.origin;
			rays.Add(new Ray(start_position, temp_ray.direction));
		}

		if (show_rays)
		{
			foreach (Ray ray in rays)
			{
				Debug.DrawRay(ray.origin, ray.direction * sight_distance, Color.white);
			}
		}

		return rays;
	}

	public void Kill()
	{
		hit_list.Clear();
		Destroy(gameObject);
	}
}
