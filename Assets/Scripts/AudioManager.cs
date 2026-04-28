using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioDataBaseSo audioDataBase;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource sfxSource;

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
          
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySFX(string soundName, AudioSource sfxSource)
    {
        var data = audioDataBase.Get(soundName);
        if (data == null)
        {
            Debug.Log("Attempt to play sound - " + soundName);
            return;
        }

        var clip = data.GetRandomClip();
        if (clip == null)
            return;

        sfxSource.clip = clip;
        sfxSource.PlayOneShot(clip);

    }

    }
