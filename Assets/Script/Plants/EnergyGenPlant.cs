using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnergyGenPlant : PlantBase
{
    #region variables and fields 
    [Header("TeleportPlant specific data")]
    [SerializeField]private ObjectPool _energyPool;
    [FormerlySerializedAs("_newenergyNoise")] [SerializeField]private AudioClip _newEnergyNoise;

    private bool _isMakingLight = true;
    private bool _disappear;
    private float _energyFilled = 1.05f;
    private int _energyMade;
    private static readonly int Power = Shader.PropertyToID("_Power");

    #endregion
    
    #region unity methods
    protected override void OnEnable()
    {
        if (Camera.main != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            transform.position = mousePos;
        }

        _energyMade = 0;
        _disappear = false;

        SpriteRend.material.SetFloat(Power, 1.05f);

        PlayerEntityInfo.Instance.PowerChange(8, true);
        StartCoroutine(c_FillWithLight());
       
    }
    public void Update()
    {
        SpriteRend.material.SetFloat(Power, _energyFilled);
       
    }
    protected override void OnTriggerEnter2D(Collider2D other)
    {
    }
    protected override void OnTriggerExit2D(Collider2D other)
    {
    }

    #endregion

    #region other functions and methods
    public override void WhenAtZeroWater()
    {
        if (_disappear) return;
        _disappear = true;
        UIManager.Instance.HealingPlantInfo(-1);
        base.WhenAtZeroWater();
    }
    #endregion

    #region coroutines

    private IEnumerator c_FillWithLight()
    {
        while(_isMakingLight)
        {
            yield return new WaitForSeconds(0.75f);
            _energyFilled -= 0.1f;

            if(_energyFilled <= 0)
            {
                _energyFilled = 1;
                _energyMade++;
                _energyPool.Claim().transform.position = this.transform.position + new Vector3(Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), 0);
                AudioManager.Instance.PlaySound(_newEnergyNoise);
            }
            if (_energyMade == 2)
            {
                WhenAtZeroWater();
            }
        }
    }
    #endregion
}