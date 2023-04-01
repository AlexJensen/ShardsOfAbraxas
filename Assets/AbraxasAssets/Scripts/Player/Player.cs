using Unity.Netcode;
using UnityEngine;

namespace Abraxas.Behaviours.Player
{
    public class Player : NetworkBehaviour
    {
        [SerializeField]
        int maxHP;
        int HP;

        [SerializeField]
        HP hp;

    }
}