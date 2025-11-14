using UnityEngine;
using UnityEngine.UI;

namespace SubnauticaClone
{
    public class PlayerStats : MonoBehaviour
    {
        [SerializeField] private OxygenSystem oxygenSystem;
        [SerializeField] private Image oxygenBar;

        private void Start()
        {
            oxygenSystem = FindFirstObjectByType<OxygenSystem>();
        }

        private void Update()
        {
            UpdateOxygenBar();
        }

        private void UpdateOxygenBar()
        {
            if (oxygenSystem != null && oxygenBar != null)
            {
                // change rect transform X scale to reflect oxygen percentage
                float oxygenPercentage = oxygenSystem.GetOxygenPercentage();
                oxygenBar.rectTransform.localScale = new Vector3(oxygenPercentage, 1, 1);
            }
        }
    }
}