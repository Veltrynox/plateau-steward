using UnityEngine;

namespace SubnauticaClone
{
    /// <summary>
    /// Manages the instantiation and setup of player-related prefabs and UI elements in the scene.
    /// </summary>
    public class LevelBuilder : SingletonBase<LevelBuilder>
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject playerHUDPrefab;
        [SerializeField] private GameObject levelGUIPrefab;
        [SerializeField] private GameObject gameManagerPrefab;

        [Header("Dependencies")]
        [SerializeField] private PlayerSpawner playerSpawner;

        public GameObject PlayerHUD { get; private set; }
        public GameObject LevelGUI { get; private set; }
        public GameObject GameManager { get; private set; }
        public GameObject Player { get; private set; }

        #region Unity events

        protected override void Awake()
        {
            base.Awake();

            PlayerHUD = Instantiate(playerHUDPrefab);
            LevelGUI = Instantiate(levelGUIPrefab);
            GameManager = Instantiate(gameManagerPrefab);

            playerSpawner.Spawn(PlayerHUD);
            Player = playerSpawner.Player;
        }
        #endregion
    }
}