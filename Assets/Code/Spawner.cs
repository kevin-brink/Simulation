
using UnityEngine;

public class Spawner : MonoBehaviour
{
	public GameObject prey_prefab;
	public GameObject plant_prefab;

	public static float x_size = 200f;
	public static float y_size = 200f;

	public int init_predator_count = 2;
	public int init_prey_count = 5;
	public int init_plant_count = 20;

	private void Start()
	{
		for (int i = 0; i < init_predator_count; ++i)
			PredatorCreature.Create();

		for (int i = 0; i < init_prey_count; ++i)
			PreyCreature.Create();

		for (int i = 0; i < init_plant_count; ++i)
			Plant.Create();
	}
}
