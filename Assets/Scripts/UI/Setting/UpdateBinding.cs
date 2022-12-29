using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateBinding : MonoBehaviour
{
    public void UpdateBindings()
    {
        foreach (var rebind in GetComponentsInChildren<Rebind>())
            rebind.UpdateText();
    }
}
