using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poolable : MonoBehaviour
{
    private ObjectPool _pool;
    public void SetPool(ObjectPool pool)
    {
        _pool = pool;
    }

    public void Release()
    {
        _pool.Release(this);
    }
}