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
        [SerializeField] private GameObject m_gameManagerPrefab;

        [Header("Dependencies")]
        [SerializeField] private PlayerSpawner m_playerSpawner;

        public GameObject PlayerHUD { get; private set; }
        public GameObject LevelGUI { get; private set; }
        public GameObject GameManager { get; private set; }
        public GameObject Player { get; private set; }

        #region Unity events

        protected override void Awake()
        {
            base.Awake();

            PlayerHUD = Instantiate(m_playerHUDPrefab);
            LevelGUI = Instantiate(m_levelGUIPrefab);
            GameManager = Instantiate(m_gameManagerPrefab);

            m_playerSpawner.Spawn(PlayerHUD);
            Player = m_playerSpawner.Player;
        }
        #endregion
    }
}