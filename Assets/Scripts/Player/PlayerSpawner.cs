using UnityEngine;

namespace SubnauticaClone
{
    /// <summary>
    /// Manages the spawning and initial setup of the player character in the scene.
    /// </summary>
    public class PlayerSpawner : MonoBehaviour
    {
        [Header("Prefab")]
        [SerializeField] private Player m_PlayerPrefab;

        [SerializeField] private Transform m_SpawnPoint;

        public Player Spawn(GameObject hud)
        {
            Player player = Instantiate(m_PlayerPrefab);
            player.Construct(hud, m_SpawnPoint);
            return player;
        }

    }
}