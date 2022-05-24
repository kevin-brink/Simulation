using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class PreyCreature : BaseCreature
{
	public static PreyCreature prey_prefab;

	private static float ave_move_speed
	{
		get
		{
			List<float> move_speeds = new();
			foreach (PreyCreature pred in prey_list)
				move_speeds.Add(pred.move_speed);

			if (prey_list.Count == 0)
				return 0;
			else
				return (Queryable.Average(move_speeds.AsQueryable()));
		}
	}
	private static float ave_max_energy
	{
		get
		{
			List<float> max_energies = new();
			foreach (PreyCreature prey_list in prey_list)
				max_energies.Add(prey_list.max_energy);

			if (prey_list.Count == 0)
				return 0;
			else
				return (Queryable.Average(max_energies.AsQueryable()));
		}
	}

	private static readonly List<PreyCreature> prey_list = new();

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
		Create(this, current_energy, max_energy, move_speed);
	}

	public static void Create()
	{
		if (prey_prefab == null)
			prey_prefab = Resources.Load<PreyCreature>("Prey_prefab");

		Vector2 position = new Vector2(Random.value * Spawner.x_size - Spawner.x_size / 2,
				Random.value * Spawner.y_size - Spawner.y_size / 2
				);
		Quaternion rotation = new Quaternion(0, 0, Random.value, Random.value);

		PreyCreature prey = Instantiate(prey_prefab, position, rotation);

		prey.name = "Prey_" + ((int)(Random.value * 1000)).ToString().PadLeft(3, '0');
		prey.transform.localScale = new Vector3(5, 5, 1);

		prey_list.Add(prey);
	}

	public static void Create(PreyCreature parent, float starting_energy, float max_energy, float move_speed)
	{
		Vector2 position = new Vector2(Random.value * 2, Random.value * 2) + (Vector2)parent.transform.position;
		Quaternion rotation = new Quaternion(0, 0, Random.value, Random.value);

		PreyCreature prey = Instantiate(parent, position, rotation);

		prey.name = "Prey_" + ((int)(Random.value * 1000)).ToString().PadLeft(3, '0');
		prey.transform.localScale = new Vector3(5, 5, 1);

		prey.MutateFeatures(starting_energy, max_energy, move_speed);

		prey_list.Add(prey);
		Debug.Log($"Prey: Average move speed: " + ave_move_speed);
		Debug.Log($"Prey: Average max energy: " + ave_max_energy);
		Debug.Log("");
	}

	public override void Kill()
	{
		prey_list.Remove(this);
		Destroy(gameObject);
	}
}
