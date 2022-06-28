using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
	public Poolable template = null;
	private readonly Queue<Poolable> _pool = new Queue<Poolable>();

	public virtual Poolable Claim()
	{
		Poolable instance;

		if (_pool.Count > 0)
		{
			instance = _pool.Dequeue();
		}
		else
		{
			instance = Instantiate(template, null);
			instance.SetPool(this);
		}

		instance.gameObject.SetActive(true);
		instance.enabled = true;
		return instance;
	}

	public virtual void Release(Poolable instance)
	{
		instance.enabled = false;
		instance.gameObject.SetActive(false);
		_pool.Enqueue(instance);
	}
}
