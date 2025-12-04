using UnityEngine;
using System.Collections.Generic;

namespace SubnauticaClone
{
    /// <summary>
    /// Manages the scattering of prefabs across a target surface, with editor tools for scattering and clearing, and runtime optimization using chunking.
    /// </summary>
    [ExecuteInEditMode]
    public class ScatterSystem : MonoBehaviour
    {
        [Header("Target Settings")]
        public GameObject targetSurface;
        public List<GameObject> prefabsToSpawn = new List<GameObject>();

        [Header("Scatter Settings")]
        public int spawnCount = 100;
        public int randomSeed = 42;
        public bool alignToNormal = true;

        [Header("Transform Randomization")]
        public float minScale = 0.8f;
        public float maxScale = 1.2f;
        public Vector3 randomRotation = new Vector3(0, 360, 0);

        [Header("Optimization / Chunking")]
        private Transform player;
        private LevelBuilder levelBuilder;
        [Tooltip("Size of one grid cell in meters.")]
        public float chunkSize = 50f;
        [Tooltip("How many chunks away from the player should be visible?")]
        public int renderDistance = 1;

        [Header("Management")]
        [HideInInspector]
        public List<GameObject> spawnedObjects = new List<GameObject>();
        private Dictionary<Vector2Int, List<GameObject>> worldChunks = new Dictionary<Vector2Int, List<GameObject>>();
        private Vector2Int currentChunkCoord;

        private void Start()
        {
            levelBuilder = LevelBuilder.Instance;

            if (levelBuilder != null && levelBuilder.Player != null)
            {
                player = levelBuilder.Player.transform;
            }

            InitializeChunks();
            UpdateVisibleChunks(true);
        }

        private void Update()
        {
            if (player == null) return;
            Vector2Int playerChunk = GetChunkCoordinate(player.position);
            if (playerChunk != currentChunkCoord)
            {
                currentChunkCoord = playerChunk;
                UpdateVisibleChunks();
            }
        }

        // --- OPTIMIZATION LOGIC ---

        void InitializeChunks()
        {
            worldChunks.Clear();
            foreach (var obj in spawnedObjects)
            {
                if (obj == null) continue;

                Vector2Int coord = GetChunkCoordinate(obj.transform.position);

                if (!worldChunks.ContainsKey(coord))
                {
                    worldChunks.Add(coord, new List<GameObject>());
                }

                worldChunks[coord].Add(obj);
                obj.SetActive(false);
            }
        }

        void UpdateVisibleChunks(bool forceUpdate = false)
        {
            int startX = currentChunkCoord.x - renderDistance;
            int endX = currentChunkCoord.x + renderDistance;
            int startY = currentChunkCoord.y - renderDistance;
            int endY = currentChunkCoord.y + renderDistance;

            foreach (var chunk in worldChunks)
            {
                Vector2Int chunkPos = chunk.Key;
                bool shouldBeVisible =
                    chunkPos.x >= startX && chunkPos.x <= endX &&
                    chunkPos.y >= startY && chunkPos.y <= endY;

                if (chunk.Value[0].activeSelf != shouldBeVisible || forceUpdate)
                {
                    foreach (var obj in chunk.Value)
                    {
                        if (obj != null && obj.activeSelf != shouldBeVisible)
                            obj.SetActive(shouldBeVisible);
                    }
                }
            }
        }

        Vector2Int GetChunkCoordinate(Vector3 position)
        {
            return new Vector2Int(
                Mathf.FloorToInt(position.x / chunkSize),
                Mathf.FloorToInt(position.z / chunkSize)
            );
        }

        // --- SCATTER LOGIC ---

        public void Scatter()
        {
            if (targetSurface == null || prefabsToSpawn.Count == 0) return;
            Collider targetCollider = targetSurface.GetComponent<Collider>();
            if (targetCollider == null) return;

            ClearScattered();
            Random.InitState(randomSeed);
            Bounds bounds = targetCollider.bounds;

            int attempts = 0;
            int maxAttempts = spawnCount * 10;
            int successfulSpawns = 0;

            while (successfulSpawns < spawnCount && attempts < maxAttempts)
            {
                attempts++;
                float randomX = Random.Range(bounds.min.x, bounds.max.x);
                float randomZ = Random.Range(bounds.min.z, bounds.max.z);
                Vector3 rayOrigin = new Vector3(randomX, bounds.max.y + 5f, randomZ);

                RaycastHit hit;
                if (targetCollider.Raycast(new Ray(rayOrigin, Vector3.down), out hit, 1000f))
                {
                    SpawnItem(hit);
                    successfulSpawns++;
                }
            }
        }

        void SpawnItem(RaycastHit hit)
        {
            GameObject prefab = prefabsToSpawn[Random.Range(0, prefabsToSpawn.Count)];
            // TODO: PrefabUtility.InstantiatePrefab for editor, but Instantiate is fine for now
            GameObject newObj = Instantiate(prefab, hit.point, Quaternion.identity);
            newObj.transform.parent = this.transform;

            if (alignToNormal) newObj.transform.up = hit.normal;

            // Random Rotate
            newObj.transform.Rotate(new Vector3(
                Random.Range(-randomRotation.x, randomRotation.x),
                Random.Range(-randomRotation.y, randomRotation.y),
                Random.Range(-randomRotation.z, randomRotation.z)
            ));

            // Random Scale
            newObj.transform.localScale = Vector3.one * Random.Range(minScale, maxScale);

            spawnedObjects.Add(newObj);
        }

        public void ClearScattered()
        {
            foreach (var obj in spawnedObjects)
            {
                if (obj != null) DestroyImmediate(obj);
            }
            spawnedObjects.Clear();
            worldChunks.Clear();
        }
    }
}