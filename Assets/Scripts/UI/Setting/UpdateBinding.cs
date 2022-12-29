using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UpdateBinding : MonoBehaviour
{
    [SerializeField] UnityEvent _loadBind;

    public void UpdateBindings()
    {
        _loadBind.Invoke();
        foreach (var rebind in GetComponentsInChildren<Rebind>())
            rebind.UpdateText();
    }
}
