using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingDebuff : MonoBehaviour
{
    private void OnEnable()
    {
        Events.Instance.OnBeginningStateStarted += DeleteMe;
    }

    private void OnDisable()
    {
        Events.Instance.OnBeginningStateStarted -= DeleteMe;
    }

    private void DeleteMe(object[] vals)
    {
        Destroy(this);
    }
}