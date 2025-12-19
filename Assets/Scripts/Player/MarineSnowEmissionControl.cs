using UnityEngine;

namespace SubnauticaClone
{
    public class MarineSnowEmissionControl : MonoBehaviour
    {
        private ParticleSystem marineSnowParticleSystem;
        private bool wasUnderwater;

        private void Awake()
        {
            marineSnowParticleSystem = GetComponentInParent<ParticleSystem>();
            marineSnowParticleSystem.Stop();
        }

        private void Update()
        {
            Debug.Log($"IsPlayerUnderwater: {GameManager.Instance.IsPlayerUnderwater}");
            if (marineSnowParticleSystem == null) return;

            bool isUnderwater = GameManager.Instance.IsPlayerUnderwater;

            if (isUnderwater && !wasUnderwater)
            {
                marineSnowParticleSystem.Play();
            }
            else if (!isUnderwater && wasUnderwater)
            {
                marineSnowParticleSystem.Stop();
            }

            wasUnderwater = isUnderwater;
        }
    }
}