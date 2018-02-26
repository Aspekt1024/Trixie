using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioMaster : MonoBehaviour {

    public AudioClip EnemyShoot;
    public AudioClip Explosion1;
    public AudioClip Explosion2;
    public AudioClip BulbJump;
    public AudioClip Jump;
    public AudioClip Melee1;
    public AudioClip Melee2;
    public AudioClip Hit1;

    public enum AudioClips
    {
        EnemyShoot, Explosion1, Explosion2, BulbJump, Jump, Melee1, Melee2, Hit1
    }

    public static AudioMaster Instance;

    private Dictionary<AudioClips, AudioClip> audioDict;
    private AudioSource audioSource;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        audioSource = GetComponent<AudioSource>();
        CreateDictionary();
    }

    public static void PlayAudio(AudioClips clip)
    {
        Instance.audioSource.PlayOneShot(Instance.audioDict[clip]);
    }

    public static void PlayAudio(AudioClips clip, Vector2 distFromPlayer)
    {
        if (distFromPlayer.magnitude > 20f) return;

        float volume = 1f - distFromPlayer.magnitude / 1000f;

        Instance.audioSource.PlayOneShot(Instance.audioDict[clip], volume);

    }

    private void CreateDictionary()
    {
        audioDict = new Dictionary<AudioClips, AudioClip>()
        {
            { AudioClips.EnemyShoot, EnemyShoot },
            { AudioClips.Explosion1, Explosion1 },
            { AudioClips.Explosion2, Explosion2 },
            { AudioClips.BulbJump, BulbJump },
            { AudioClips.Jump, Jump },
            { AudioClips.Melee1, Melee1 },
            { AudioClips.Melee2, Melee2 },
            { AudioClips.Hit1, Hit1 },
        };
    }
}
