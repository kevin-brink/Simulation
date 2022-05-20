
using UnityEngine;

public class Spawner : MonoBehaviour
{
	public GameObject pred_prefab;
	public GameObject prey_prefab;
	public GameObject plant_prefab;

	public int init_predator_count = 2;
	public int init_prey_count = 5;
	public int init_plant_count = 20;

	private void Start()
	{
		pred_prefab = Resources.Load<GameObject>("Predator_prefab");
		for (int i = 0; i < init_predator_count; ++i)
			SpawnPredator();

		prey_prefab = Resources.Load<GameObject>("Prey_prefab");
		for (int i = 0; i < init_prey_count; ++i)
			SpawnPrey();

		plant_prefab = Resources.Load<GameObject>("Plant_prefab");
		for (int i = 0; i < init_plant_count; ++i)
			SpawnPlant();
	}

	private void SpawnPredator()
	{
		Vector2 position = new Vector2(Random.value * 38 - 19, Random.value * 38 - 19);
		Quaternion rotation = new Quaternion(0, 0, Random.value, Random.value);

		GameObject pred = Instantiate(pred_prefab, position, rotation);

		pred.name = "Predator_" + ((int)(Random.value * 1000)).ToString().PadLeft(3, '0');
		pred.transform.localScale = new Vector3(5, 5, 1);
	}

	private void SpawnPrey()
	{
		Vector2 position = new Vector2(Random.value * 38 - 19, Random.value * 38 - 19);
		Quaternion rotation = new Quaternion(0, 0, Random.value, Random.value);

		GameObject prey = Instantiate(prey_prefab, position, rotation);

		prey.name = "Prey_" + ((int)(Random.value * 1000)).ToString().PadLeft(3, '0');
		prey.transform.localScale = new Vector3(5, 5, 1);
	}

	private void SpawnPlant()
	{
		Vector2 position = new Vector2(Random.value * 38 - 19, Random.value * 38 - 19);
		Quaternion rotation = new Quaternion(0, 0, 0, 0);

		GameObject plant = Instantiate(plant_prefab, position, rotation);

		plant.name = "Plant_" + ((int)(Random.value * 1000)).ToString().PadLeft(3, '0');
		plant.transform.localScale = new Vector3(1, 1, 1);
	}
}
