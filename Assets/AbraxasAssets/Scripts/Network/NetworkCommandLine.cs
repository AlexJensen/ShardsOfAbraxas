using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Abraxas.Network
{
    /// <summary>
    /// NetworkCommandLine is a class that starts the network manager client or server based on command line arguments.
    /// </summary>
    public class NetworkCommandLine : MonoBehaviour
    {
        NetworkManager _netManager;

        void Start()
        {
            _netManager = GetComponentInParent<NetworkManager>();

            if (Application.isEditor) return;

            var args = GetCommandlineArgs();

            if (args.TryGetValue("-mlapi", out string mlapiValue))
            {
                switch (mlapiValue)
                {
                    case "server":
                        _netManager.StartServer();
                        break;
                    case "host":
                        _netManager.StartHost();
                        break;
                    case "client":
                        _netManager.StartClient();
                        break;
                }
            }
        }

        private Dictionary<string, string> GetCommandlineArgs()
        {
            Dictionary<string, string> argDictionary = new();

            var args = System.Environment.GetCommandLineArgs();

            for (int i = 0; i < args.Length; ++i)
            {
                var arg = args[i].ToLower();
                if (arg.StartsWith("-"))
                {
                    var value = i < args.Length - 1 ? args[i + 1].ToLower() : null;
                    value = value?.StartsWith("-") ?? false ? null : value;

                    argDictionary.Add(arg, value);
                }
            }
            return argDictionary;
        }
    }
}