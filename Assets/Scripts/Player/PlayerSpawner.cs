using UnityEngine;

namespace SubnauticaClone
{
    /// <summary>
    /// Manages the spawning and initial setup of the player character in the scene.
    /// </summary>
    public class PlayerSpawner : MonoBehaviour
    {
        [Header("Prefab")]
        [SerializeField] private GameObject m_PlayerPrefab;
        [SerializeField] private Transform m_SpawnPoint;

        public GameObject Player { get; private set; }

        public void Spawn(GameObject hud)
        {
            Player = Instantiate(m_PlayerPrefab);
            Player.transform.parent = transform;
            Player playerComponent = Player.GetComponent<Player>();
            Player.transform.position = m_SpawnPoint.position;
            playerComponent.Construct(hud, m_SpawnPoint.rotation, m_SpawnPoint.position);
        }
    }
}