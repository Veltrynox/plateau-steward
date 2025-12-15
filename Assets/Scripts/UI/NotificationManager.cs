using UnityEngine;

namespace SubnauticaClone
{
    public class NotificationManager : SingletonBase<NotificationManager>
    {
        [Header("UI References")]
        [SerializeField] private Transform notificationContainer;
        [SerializeField] private GameObject notificationPrefab;

        protected override void Awake() { base.Awake(); }
        
        public void ShowNotification(string message)
        {
            GameObject newNotification = Instantiate(notificationPrefab, notificationContainer);

            NotificationItem itemScript = newNotification.GetComponent<NotificationItem>();
            if (itemScript != null)
            {
                itemScript.Setup(message);
            }

            newNotification.transform.localScale = Vector3.one;
        }
    }
}