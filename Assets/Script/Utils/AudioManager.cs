using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioSource _music;
    [SerializeField] private AudioSource _soundEffect;
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(transform.gameObject);
    }

    public void PlaySound(AudioClip sfx)
    {
        _soundEffect.clip = sfx;
        _soundEffect.Play();

    }

    public float VolumeMusic()
    {
        return _music.volume;
    }

    public float VolumeSfx()
    {
        return _soundEffect.volume;
    }

    public void LowerVolumeofMusic(float musicVolume)
    {
        _music.volume = musicVolume;
    }

    public void LowerSfxVolume(float sfxVolume)
    {
        _soundEffect.volume = sfxVolume;
    }
}
