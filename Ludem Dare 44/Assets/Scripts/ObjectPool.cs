using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
	[SerializeField] private ObjectPoolItem item;
	[SerializeField] private int amount;
	private Queue<ObjectPoolItem> items;

	private void Start()
	{
		items = new Queue<ObjectPoolItem>();
		for (int i = 0; i < amount; i++)
		{
			ObjectPoolItem tmp = Instantiate(item, transform);
			items.Enqueue(tmp);
			tmp.gameObject.SetActive(false);
		}
	}

	public ObjectPoolItem Instantiate(Vector3 position, Quaternion rotation)
	{
		ObjectPoolItem tmp = items.Dequeue();
		items.Enqueue(tmp);

		tmp.gameObject.SetActive(true);
		tmp.transform.position = position;
		tmp.transform.rotation = rotation;

		tmp.OnInstantiate();

		return tmp;
	}
}
