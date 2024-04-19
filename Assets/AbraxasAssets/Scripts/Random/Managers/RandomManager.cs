using Abraxas.Network.Managers;
using System.Collections;
using Unity.Netcode;
using Zenject;

namespace Abraxas.Random.Managers
{
	class RandomManager : NetworkedManager, IRandomManager
	{
		private readonly NetworkVariable<int> randomSeed = new(
			0,
			NetworkVariableReadPermission.Everyone,
			NetworkVariableWritePermission.Server
		);
		public IEnumerator InitializeRandomSeed()
		{
			if (!IsServer) yield break;
			randomSeed.Value = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
			yield return WaitForClients();
		}

		[Inject]
		public void Construct()
		{
            randomSeed.OnValueChanged += OnRandomSeedChanged;
        }

		public override void OnDestroy()
		{
			randomSeed.OnValueChanged -= OnRandomSeedChanged;
			base.OnDestroy();
		}

		public int Range(int v, int length)
		{
			return UnityEngine.Random.Range(v, length);
		}

		private void OnRandomSeedChanged(int oldSeed, int newSeed)
		{
			UnityEngine.Random.InitState(newSeed);
			if (!IsServer) AcknowledgeServerRpc();
		}
	}
}
