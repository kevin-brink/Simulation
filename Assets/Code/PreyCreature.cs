using System.Collections.Generic;

using UnityEngine;

public class PreyCreature : BaseCreature
{
	protected new void Start()
	{
		base.Start();
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = Resources.Load<Sprite>("prey");

		collider.gameObject.tag = "prey";
		sightComponent.ray_positions = new() { 45f, 40f, 50f, 60f, 20f, 5f, 1f };
	}

	protected new void Update() => base.Update();

	protected override Vector2? SelectTarget(List<RaycastHit2D> predators, List<RaycastHit2D> prey, List<RaycastHit2D> plants)
	{
		if (predators.Count > 0)
		{
			// Sort list of found predators by distance 
			predators.Sort((RaycastHit2D x, RaycastHit2D y) => x.distance.CompareTo(y.distance));
			// Get the vector pointing exactly away from the nearest one
			Vector2 target = 2 * transform.position - predators[0].transform.position;
			return target;
		}

		List<RaycastHit2D> plant_targets = new();
		foreach (RaycastHit2D target in plants)
		{
			Plant plant = target.collider.GetComponentInParent<Plant>();
			if (plant is not null && plant.can_be_eaten)
			{
				plant_targets.Add(target);
			}

		}

		if (plant_targets.Count > 0)
		{
			plant_targets.Sort((RaycastHit2D x, RaycastHit2D y) => x.distance.CompareTo(y.distance));
			return plant_targets[0].point;
		}

		return null;
	}

	public float BeEaten()
	{
		Destroy(gameObject);
		return current_energy / 2;
	}

	public void OnTriggerEnter2D(Collider2D other)
	{
		switch (other.gameObject.tag)
		{
			case "predator":
				// Predator handles this
				break;

			case "prey":
				// Do nothing for now, just continue about your day
				break;

			case "plant":
				Plant plant = other.GetComponentInParent<Plant>();
				if (plant.can_be_eaten)
					current_energy += plant.BeEaten();
				break;

			default:
				break;
		}
	}

	protected override void Breed()
	{
		current_energy *= 0.5f;
		last_breed = Time.time;
		Create(this, current_energy, max_energy, move_speed, transform.position);
	}

	public static void Create(PreyCreature parent, float starting_energy, float max_energy, float move_speed, Vector2 parent_position)
	{
		Vector2 position = new Vector2(Random.value * 2, Random.value * 2) + parent_position;
		Quaternion rotation = new Quaternion(0, 0, Random.value, Random.value);

		PreyCreature prey = Instantiate(parent, position, rotation);

		prey.name = "Prey_" + ((int)(Random.value * 1000)).ToString().PadLeft(3, '0');
		prey.transform.localScale = new Vector3(5, 5, 1);

		prey.MutateFeatures(starting_energy, max_energy, move_speed);
	}
}
