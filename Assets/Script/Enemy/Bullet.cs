using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Poolable
{
    private float _speed;
    private Vector2 _moveDir;

    public float Damage { get; private set; }
    public float waitTime = 1f;
    void OnEnable()
    {
        StartCoroutine(c_Destroy());
    }

    void Update() => transform.Translate(_moveDir * (_speed * Time.deltaTime));

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Release();
        }
    }

    public void InitBullet(Vector2 moveDir, float speed, float damage)
    {
        _speed = speed;
        _moveDir = moveDir;
        this.Damage = damage;
    }

    private IEnumerator c_Destroy()
    {
        yield return new WaitForSeconds(waitTime);
        Release();
    }
   
}
