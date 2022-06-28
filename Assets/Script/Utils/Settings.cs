using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private Slider _musicVOl;
    [SerializeField] private Slider _SFXvol;
    [SerializeField] private AudioClip _sfxsample;

    public void Start()
    {
        _SFXvol.value = AudioManager.Instance.VolumeSfx();
        _musicVOl.value = AudioManager.Instance.VolumeMusic();
    }
    public void LowerMusic()
    {
        AudioManager.Instance.LowerVolumeofMusic(_musicVOl.value);
    }

    public void LowerSFX()
    {
        AudioManager.Instance.LowerSfxVolume(_SFXvol.value);
        AudioManager.Instance.PlaySound(_sfxsample);

    }
}
