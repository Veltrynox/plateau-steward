using SubnauticaClone;
using UnityEngine;

namespace SubnauticaClone
{
    public class PlayerSpawner : MonoBehaviour
    {
        [Header("Prefab")]
        [SerializeField] private Player m_PlayerPrefab;

        [SerializeField] private Transform m_SpawnPoint;

        public Player Spawn(GameObject hud, GameObject gui)
        {
            Player player = Instantiate(m_PlayerPrefab);
            player.Construct(hud, gui, m_SpawnPoint);
            return player;
        }

    }
}