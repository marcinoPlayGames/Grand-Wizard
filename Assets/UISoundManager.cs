using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISoundManager : MonoBehaviour
{
    public static UISoundManager Instance;

    [SerializeField]
    AudioSource uiAudioSource;

    [SerializeField]
    AudioClip hoverSound;
    [SerializeField]
    AudioClip clickSound;

    public enum UISoundType
    {
        Hover,
        Click
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(UISoundType uiSoundType)
    {
        switch (uiSoundType)
        {
            case UISoundType.Hover:
                uiAudioSource.clip = hoverSound;
                break;

            case UISoundType.Click:
                uiAudioSource.clip = clickSound;
                break;
        }

        uiAudioSource.Play();
    }
}
