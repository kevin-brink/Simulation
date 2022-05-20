using System.Collections.Generic;

using UnityEngine;


public class PredatorCreature : BaseCreature
{
	protected new void Start()
	{
		base.Start();
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = Resources.Load<Sprite>("predator");

		collider.gameObject.tag = "predator";
		sightComponent.ray_positions = new() { 1f, 2f, 5f, 10f, 15f, 30f };
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
		Create(this, current_energy, max_energy, move_speed, transform.position);
	}

	public static void Create(PredatorCreature parent, float starting_energy, float max_energy, float move_speed, Vector2 parent_position)
	{
		Vector2 position = new Vector2(Random.value * 2, Random.value * 2) + parent_position;
		Quaternion rotation = new Quaternion(0, 0, Random.value, Random.value);

		PredatorCreature pred = Instantiate(parent, position, rotation);

		pred.name = "Predator_" + ((int)(Random.value * 1000)).ToString().PadLeft(3, '0');
		pred.transform.localScale = new Vector3(5, 5, 1);

		pred.MutateFeatures(starting_energy, max_energy, move_speed);
	}
}
