using UnityEngine;

namespace SubnauticaClone
{
    /// <summary>
    /// Manages the spawning and initial setup of the player character in the scene.
    /// </summary>
    public class PlayerSpawner : MonoBehaviour
    {
        [Header("Prefab")]
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Transform spawnPoint;

        public GameObject Player { get; private set; }

        public void Spawn(GameObject hud)
        {
            Player = Instantiate(playerPrefab);
            Player.transform.parent = transform;
            Player playerComponent = Player.GetComponent<Player>();
            Player.transform.position = spawnPoint.position;
            playerComponent.Construct(hud, spawnPoint.rotation, spawnPoint.position);
        }
    }
}