using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioImage : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] Sprite unmute;
    [SerializeField] Sprite mute;

    Image _image;

    void Start()
    {
        _image = GetComponent<Image>();
    }

    void Update()
    {
        if (audioSource.mute || audioSource.volume == 0)
        {
            _image.sprite = mute;
        }
        else
        {
            _image.sprite = unmute;

        }
    }

    public void SwitchMute()
    {
        audioSource.mute = !audioSource.mute;
    }

}
