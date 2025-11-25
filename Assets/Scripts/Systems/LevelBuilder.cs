using UnityEngine;

namespace SubnauticaClone
{
    public class LevelBuilder : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject m_PlayerHUDPrefab;
        [SerializeField] private GameObject m_LevelGUIPrefab;

        [Header("Dependencies")]
        [SerializeField] private PlayerSpawner m_PlayerSpawner;

        private void Awake()
        {
            GameObject hud = Instantiate(m_PlayerHUDPrefab);
            GameObject gui = Instantiate(m_LevelGUIPrefab);

            m_PlayerSpawner.Spawn(hud, gui);
        }
    }
}