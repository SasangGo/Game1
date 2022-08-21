using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    // 플레이어 움직임 관련 사운드
    public AudioClip JumpSound;
    public AudioClip MoveSound;

    // UI or System 관련 사운드
    public AudioClip clickSound;
    public AudioClip LevelUpSound;
    public AudioClip SkillGetSound;

    // 투사체 관련 사운드
    public AudioClip turretBulletSound;
    public AudioClip missileSound;

    // 오디오 소스
    public AudioSource bgmAudioSource;
    public AudioSource playerAudioSource;
    public AudioSource uIAudioSource;
    public AudioSource objectAudioSource;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySound(AudioSource audioSource, AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }
    public void PlaySound(AudioSource audioSource, AudioClip clip, float delayTime)
    {
        audioSource.clip = clip;
        audioSource.PlayDelayed(delayTime);
    }

    public void StopSound(AudioSource audioSource)
    {
        audioSource.Stop();
    }
}
