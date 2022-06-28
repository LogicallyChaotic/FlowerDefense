using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyPickup : Poolable
{
    [SerializeField] private AudioClip _newenergyNoise;
    [SerializeField] private Animator _anim;
    private static readonly int Flash = Animator.StringToHash("Flash");

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        collision.GetComponent<PlayerEntityInfo>().PowerChange(6, false);
        AudioManager.Instance.PlaySound(_newenergyNoise);

        Release();
    }

    public void OnEnable()
    {
        StartCoroutine(C_Destroy());
    }

    private IEnumerator C_Destroy()
    {
        yield return new WaitForSeconds(1.5f);
        GetComponent<Animator>().SetTrigger(Flash);
        yield return new WaitForSeconds(1.0f);
        Release();
    }
}
