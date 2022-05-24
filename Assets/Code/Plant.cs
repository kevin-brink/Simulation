using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CircleCollider2D))]
public class Plant : MonoBehaviour
{
	public static Plant plant_prefab;

	private SpriteRenderer spriteRenderer;
	public new CircleCollider2D collider;

	public float size;

	[SerializeField]
	public float max_size = 100;

	[SerializeField]
	public float scale_factor = 0.1f;

	[SerializeField]
	public float seed_time = 5f;

	[SerializeField]
	public float seeding_distance = 2f;

	[SerializeField]
	public float eat_interval = 2f;

	private float last_seed_time = 0f;
	private float last_time_eaten = 0f;

	// Plants dont mutate yet because theres no limiting factors. All these values
	//		have a clear selection pressure one direction and have no reason not
	//		to move toward zero/infinity depending on the factor. Need to come up
	//		with some mechanical reason to limit them or pressure to go the other way
	// Mutability
	protected float scale_factor_mutability = 0f;
	protected float max_size_mutability = 0f;
	protected float seed_time_mutability = 0f;
	protected float seeding_distance_mutability = 0f;
	protected float eat_interval_mutability = 0f;


	public bool can_be_eaten { get => last_time_eaten + eat_interval <= Time.time; }

	private void Start()
	{
		collider = GetComponent<CircleCollider2D>();
		collider.gameObject.tag = "plant";

		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = Resources.Load<Sprite>("bush");

		size = 10f;

		transform.localScale = new Vector3(size * scale_factor, size * scale_factor, 1);
	}

	private void Update()
	{
		if (size >= max_size)
			Seed();
		else
			Grow();
	}

	private void Grow()
	{
		if (size <= 5.0f)
			size *= 1 - (scale_factor * Time.deltaTime);
		else
			size *= 1 + (scale_factor * Time.deltaTime);

		if (size <= 2.0f)
			Destroy(gameObject);

		collider.radius = Mathf.Sqrt(size) / 100;
		transform.localScale = new Vector3(size * scale_factor, size * scale_factor, 1);
	}


	public float BeEaten()
	{
		size *= 0.5f;
		transform.localScale = new Vector3(size * scale_factor, size * scale_factor, 1);

		last_seed_time = 0;
		last_time_eaten = Time.time;

		return size;
	}

	private void Seed()
	{
		size = max_size;

		if (last_seed_time == 0f)
			last_seed_time = Time.time;

		if (last_seed_time + seed_time <= Time.time)
		{
			last_seed_time = Time.time;

			Vector2 seedling_position =
				new Vector2(Random.value * seeding_distance, Random.value * seeding_distance)
				+ (Vector2)transform.position;

			Create(this);
		}
	}


	public static void Create()
	{
		if (plant_prefab == null)
			plant_prefab = Resources.Load<Plant>("Plant_prefab");

		Vector2 position = new Vector2(Random.value * Spawner.x_size - Spawner.x_size / 2,
				Random.value * Spawner.y_size - Spawner.y_size / 2
				);
		Quaternion rotation = new Quaternion(0, 0, 0, 0);

		Plant plant = Instantiate(plant_prefab, position, rotation);

		plant.name = "Plant_" + ((int)(Random.value * 1000)).ToString().PadLeft(3, '0');
		plant.transform.localScale = new Vector3(1, 1, 1);
	}

	public static void Create(Plant parent)
	{
		Vector2 position = new Vector2(Random.value * 2, Random.value * 2) + (Vector2)parent.transform.position;
		Quaternion rotation = new Quaternion(0, 0, 0, 0);

		Plant plant = Instantiate(parent, position, rotation);

		plant.name = "Plant" + ((int)(Random.value * 1000)).ToString().PadLeft(3, '0');
		plant.transform.localScale = new Vector3(5, 5, 1);

		plant.MutateFeatures();
	}

	// TODO
	private void MutateFeatures() { }
}
