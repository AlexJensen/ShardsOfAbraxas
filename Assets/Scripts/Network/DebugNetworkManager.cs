using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugNetworkManager : Singleton<DebugNetworkManager>
{
    public bool isDebugMode = false;

    private void Start()
    {
        if (isDebugMode)
        {
            NetworkManager.StartHost();
        }
    }
}
