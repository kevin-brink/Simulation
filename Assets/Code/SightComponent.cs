using System;
using System.Collections.Generic;

using UnityEngine;

public class SightComponent : MonoBehaviour
{
	protected List<float> ray_positions = new();

	[SerializeField]
	protected float sight_distance = 10f;

	[SerializeField]
	private bool show_rays = false;

	private readonly Dictionary<Target, int> targets;

	public void Start()
	{
	}

	public List<RaycastHit2D> GetTargets()
	{
		List<Ray> rays = new List<Ray>();
		foreach (float ray in ray_positions)
		{
			rays.Add(new Ray(transform.position, Quaternion.AngleAxis(ray, transform.forward) * transform.up));
			rays.Add(new Ray(transform.position, Quaternion.AngleAxis(-ray, transform.forward) * transform.up));
		}

		if (show_rays)
		{
			foreach (Ray ray in rays)
			{
				Debug.DrawRay(ray.origin, ray.direction * sight_distance, Color.white);
			}
		}

		List<RaycastHit2D> hits = new();
		foreach (Ray ray in rays)
		{
			RaycastHit2D[] current_hits;
			current_hits = Physics2D.RaycastAll(transform.position, ray.direction, sight_distance);

			if (current_hits.Length > 1)
			{
				Array.Sort(current_hits, (RaycastHit2D x, RaycastHit2D y) => x.distance.CompareTo(y.distance));
				hits.Add(current_hits[1]);
			}
		}

		//Debug.Log($"{hits.Count} ray hits");
		return hits;
	}
}

public struct Target
{
	public object target;
	public float distance;
	public float time_spotted;
}
