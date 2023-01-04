using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioImage : MonoBehaviour
{
    [SerializeField] AudioSource _audioSource;
    [SerializeField] Sprite _Unmute;
    [SerializeField] Sprite _mute;

    Image _image;

    void Start()
    {
        _image = GetComponent<Image>();
    }

    void Update()
    {
        if (_audioSource.mute || _audioSource.volume == 0)
        {
            _image.sprite = _mute;
        }
        else
        {
            _image.sprite = _Unmute;

        }
    }

    public void SwitchMute()
    {
        _audioSource.mute = !_audioSource.mute;
    }

}
