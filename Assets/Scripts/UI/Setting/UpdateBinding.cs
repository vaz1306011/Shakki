using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UpdateBinding : MonoBehaviour
{
    [SerializeField] UnityEvent loadBind;
    [SerializeField] Slider BGMSlider, SESlider;

    public void UpdateBindings()
    {
        loadBind.Invoke();
        foreach (var rebind in GetComponentsInChildren<Rebind>())
            rebind.RefreshText();
        BGMSlider.value = GameSetting.setting.BGMVolume;
        SESlider.value = GameSetting.setting.SEVolume;
    }
}
