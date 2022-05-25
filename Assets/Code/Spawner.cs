
using UnityEngine;

public class Spawner : MonoBehaviour
{
	[SerializeField]
	public float start_x_size = 100f;
	[SerializeField]
	public float start_y_size = 100f;

	public static float x_size;
	public static float y_size;

	public int init_predator_count = 2;
	public int init_prey_count = 5;
	public int init_plant_count = 20;

	private void Start()
	{
		x_size = start_x_size;
		y_size = start_y_size;

		for (int i = 0; i < init_predator_count; ++i)
			PredatorCreature.Create();

		for (int i = 0; i < init_prey_count; ++i)
			PreyCreature.Create();

		for (int i = 0; i < init_plant_count; ++i)
			Plant.Create();
	}
}
