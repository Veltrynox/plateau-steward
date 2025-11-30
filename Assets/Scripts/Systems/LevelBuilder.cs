using UnityEngine;

namespace SubnauticaClone
{
    /// <summary>
    /// Manages the instantiation and setup of player-related prefabs and UI elements in the scene.
    /// </summary>
    public class LevelBuilder : SingletonBase<LevelBuilder>
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject m_playerHUDPrefab;
        [SerializeField] private GameObject m_levelGUIPrefab;

        [Header("Dependencies")]
        [SerializeField] private PlayerSpawner m_playerSpawner;

        public GameObject PlayerHUD { get; private set; }
        public GameObject LevelGUI { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            PlayerHUD = Instantiate(m_playerHUDPrefab);
            LevelGUI = Instantiate(m_levelGUIPrefab);

            m_playerSpawner.Spawn(PlayerHUD);
        }
    }
}