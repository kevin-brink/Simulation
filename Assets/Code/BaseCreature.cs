
using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(MoveComponent))]
[RequireComponent(typeof(SightComponent))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CircleCollider2D))]
public abstract class BaseCreature : MonoBehaviour
{
	protected MoveComponent moveComponent;
	protected SightComponent sightComponent;
	protected SpriteRenderer spriteRenderer;
	public new CircleCollider2D collider;

	[SerializeField]
	protected float starting_energy = 60f;
	[SerializeField]
	protected float max_energy = 100f;
	protected float current_energy;

	[SerializeField]
	public float breed_interval = 3f;
	protected float last_breed = 0f;

	// Energy cost should increase with max size/energy and move speed (more later)
	protected float energy_cost { get => move_speed * (max_energy / 100); }

	[SerializeField]
	protected float move_speed = 5f;

	private Vector2? target;

	// Mutability
	protected float max_energy_mutability = 1f;
	protected float move_speed_mutability = 1f;

	protected virtual void Start()
	{
		sightComponent = GetComponent<SightComponent>();
		moveComponent = GetComponent<MoveComponent>();
		collider = GetComponent<CircleCollider2D>();

		target = transform.position;

		moveComponent.SetSpeed(move_speed);

		current_energy = starting_energy;
		//Debug.Log($"Set energy to {current_energy}.");
	}

	protected virtual void Update()
	{
		if (current_energy > max_energy && last_breed + breed_interval <= Time.time)
			Breed();

		if (current_energy <= 0)
		{
			Kill();
		}
		else
		{
			current_energy -= Time.deltaTime * energy_cost;
		}

		if (target is not null && ((Vector2)transform.position - (Vector2)target).magnitude < 1)
		{
			target = null;
		}

		Vector2? new_target = SelectTarget(sightComponent.GetTargets());
		if (new_target is not null)
		{
			target = new_target;
		}

		if (target is null)
			target = new(Random.value * Spawner.x_size - Spawner.x_size / 2,
				Random.value * Spawner.y_size - Spawner.y_size / 2
				);

		moveComponent.MoveToward((Vector2)target);
	}

	protected abstract Vector2? SelectTarget(List<Collider2D> predators, List<Collider2D> prey, List<Collider2D> plants);
	protected Vector2? SelectTarget(List<Collider2D> targets)
	{
		List<Collider2D> predators = new();
		List<Collider2D> plants = new();
		List<Collider2D> prey = new();

		foreach (Collider2D target in targets)
		{
			// idk why this is ever happening, but...it is. Investigate at some point
			if (target == null)
				continue;

			switch (target.gameObject.tag)
			{
				case "predator":
					predators.Add(target);
					break;

				case "prey":
					prey.Add(target);
					break;

				case "plant":
					plants.Add(target);
					break;

				default:
					break;
			}
		}

		return SelectTarget(predators, prey, plants);
	}

	protected void MutateFeatures(float starting_energy, float max_energy, float move_speed)
	{
		this.starting_energy = starting_energy;
		this.max_energy = max_energy + (Random.value * 2 * max_energy_mutability) - max_energy_mutability;
		this.move_speed = move_speed + (Random.value * 2 * move_speed_mutability) - move_speed_mutability;

		last_breed = Time.time;
	}

	protected abstract void Breed();
	public abstract void Kill();
}
