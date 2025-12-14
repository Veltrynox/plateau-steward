using UnityEngine;

namespace SubnauticaClone
{
    public class OxygenSystem : MonoBehaviour
    {
        [SerializeField] private int maxOxygen = 100;
        [SerializeField] private int oxygenDepletionAmount = 1;
        [SerializeField] private float depletionInterval = 1f;

        private int currentOxygen;
        private float lastDepletionTime;

        private void Start()
        {
            currentOxygen = maxOxygen;
            lastDepletionTime = Time.time;
        }

        private void Update()
        {
            if (transform.position.y < -0.3)
            {
                DepleteOxygen();
            }
            else
            {
                ReplenishOxygen(maxOxygen);
            }
        }

        private void DepleteOxygen()
        {
            // Only check for depletion at specified intervals
            if (Time.time - lastDepletionTime >= depletionInterval)
            {
                currentOxygen -= oxygenDepletionAmount;
                currentOxygen = Mathf.Max(0, currentOxygen);
                lastDepletionTime = Time.time;

                if (currentOxygen <= 0)
                {
                    HandleOxygenDepletion();
                }
            }
        }

        private void HandleOxygenDepletion()
        {
            Debug.Log("Oxygen depleted! Take action!");
            GameManager.Instance.GameOver();
        }

        public void ReplenishOxygen(int amount)
        {
            currentOxygen += amount;
            currentOxygen = Mathf.Min(currentOxygen, maxOxygen);
        }

        public int GetCurrentOxygen()
        {
            return currentOxygen;
        }

        public int GetMaxOxygen()
        {
            return maxOxygen;
        }

        public float GetOxygenPercentage()
        {
            return (float)currentOxygen / maxOxygen;
        }
    }
}