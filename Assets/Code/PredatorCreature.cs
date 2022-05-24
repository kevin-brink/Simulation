using System.Collections.Generic;
using System.Linq;

using UnityEngine;


public class PredatorCreature : BaseCreature
{
	public static PredatorCreature pred_prefab;

	private static float ave_move_speed
	{
		get
		{
			List<float> move_speeds = new();
			foreach (PredatorCreature pred in pred_list)
				move_speeds.Add(pred.move_speed);

			if (pred_list.Count == 0)
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
			foreach (PredatorCreature pred in pred_list)
				max_energies.Add(pred.max_energy);

			if (pred_list.Count == 0)
				return 0;
			else
				return (Queryable.Average(max_energies.AsQueryable()));
		}
	}

	private static readonly List<PredatorCreature> pred_list = new();

	protected new void Start()
	{
		base.Start();
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = Resources.Load<Sprite>("predator");

		collider.gameObject.tag = "predator";
		sightComponent.ray_positions = new() { 1f, 2f, 5f, 10f, 15f, 30f };
		sightComponent.sight_distance = 20f;
	}

	protected new void Update() => base.Update();

	protected override Vector2? SelectTarget(
		List<RaycastHit2D> predators, List<RaycastHit2D> prey, List<RaycastHit2D> plants
		)
	{
		if (prey.Count > 0)
		{
			prey.Sort((RaycastHit2D x, RaycastHit2D y) => x.distance.CompareTo(y.distance));

			return prey[0].point;
		}

		return null;
	}

	public void OnTriggerEnter2D(Collider2D other)
	{
		switch (other.gameObject.tag)
		{
			case "predator":
				// Do nothing
				break;

			case "prey":
				// Eat this dude
				PreyCreature prey = other.GetComponentInParent<PreyCreature>();
				current_energy += prey.BeEaten();
				break;

			case "plant":
				// Do nothing
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
		if (pred_prefab == null)
			pred_prefab = Resources.Load<PredatorCreature>("Predator_prefab");

		Vector2 position = new Vector2(Random.value * Spawner.x_size - Spawner.x_size / 2,
				Random.value * Spawner.y_size - Spawner.y_size / 2
				);
		Quaternion rotation = new Quaternion(0, 0, Random.value, Random.value);

		PredatorCreature pred = Instantiate(pred_prefab, position, rotation);

		pred.name = "Predator_" + ((int)(Random.value * 1000)).ToString().PadLeft(3, '0');
		pred.transform.localScale = new Vector3(5, 5, 1);

		pred_list.Add(pred);
	}

	public static void Create(PredatorCreature parent, float starting_energy, float max_energy, float move_speed)
	{
		Vector2 position = new Vector2(Random.value * 2, Random.value * 2) + (Vector2)parent.transform.position;
		Quaternion rotation = new Quaternion(0, 0, Random.value, Random.value);

		PredatorCreature pred = Instantiate(parent, position, rotation);

		pred.name = "Predator_" + ((int)(Random.value * 1000)).ToString().PadLeft(3, '0');
		pred.transform.localScale = new Vector3(5, 5, 1);

		pred.MutateFeatures(starting_energy, max_energy, move_speed);

		pred_list.Add(pred);
		Debug.Log($"Predator: Average move speed: " + ave_move_speed);
		Debug.Log($"Predator: Average max energy: " + ave_max_energy);
		Debug.Log("");
	}

	public override void Kill()
	{
		pred_list.Remove(this);
		Destroy(gameObject);
	}
}
