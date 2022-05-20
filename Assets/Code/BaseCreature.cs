
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
		if (current_energy > max_energy)
			Breed();

		if (current_energy <= 0)
		{
			Debug.Log("I am dead. Oh boy.");
			Destroy(gameObject);
		}
		else
		{
			//Debug.Log($"Current energy: {current_energy}");
			current_energy -= Time.deltaTime;
		}

		if (((Vector2)transform.position - (Vector2)target).magnitude < 1)
		{
			Debug.Log(gameObject.name + " target reached...");
			target = null;
		}

		Vector2? new_target = SelectTarget(sightComponent.GetTargets());
		if (new_target is not null)
		{
			Debug.Log(gameObject.name + " acquired new target...");
			target = new_target;
		}

		if (target is null)
			target = new(Random.value * 38 - 19, Random.value * 38 - 19);

		moveComponent.MoveToward((Vector2)target);
	}

	protected abstract Vector2? SelectTarget(List<RaycastHit2D> predators, List<RaycastHit2D> prey, List<RaycastHit2D> plants);
	protected Vector2? SelectTarget(List<RaycastHit2D> targets)
	{
		List<RaycastHit2D> predators = new();
		List<RaycastHit2D> plants = new();
		List<RaycastHit2D> prey = new();

		foreach (RaycastHit2D target in targets)
		{
			switch (target.collider.gameObject.tag)
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
	}

	protected abstract void Breed();
}
