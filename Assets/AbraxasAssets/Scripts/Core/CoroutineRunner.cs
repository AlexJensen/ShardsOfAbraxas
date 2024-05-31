using UnityEngine;

namespace Abraxas.Core
{
    public class CoroutineRunner : MonoBehaviour
    {
        private static CoroutineRunner instance;

        public static CoroutineRunner Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject runnerObject = new("CoroutineRunner");
                    instance = runnerObject.AddComponent<CoroutineRunner>();
                    if (Application.isPlaying)
                    {
                        DontDestroyOnLoad(runnerObject);
                    }
                }
                return instance;
            }
        }
    }
}
