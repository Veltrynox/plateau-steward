using UnityEngine;
using UnityEngine.Events;

namespace SubnauticaClone
{
    public class StoryInteractable : MonoBehaviour, IInteractable
    {
        [Header("--- Conditions ---")]
        [Tooltip("Does the player need a specific item to succeed?")]
        public ItemData requiredItem;
        [Tooltip("Should the item be removed upon success?")]
        public bool consumeItem = true;

        [Tooltip("Does a previous task need to be finished first? (Optional)")]
        public string requiredTaskID;

        [Header("--- Outcomes ---")]
        [Tooltip("What happens when conditions are MET?")]
        public UnityEvent OnInteractionSuccess;

        [Tooltip("What happens when conditions are NOT MET?")]
        public UnityEvent OnInteractionFail;

        private bool isInteractionFinished = false;

        public void Interact(GameObject interactor)
        {
            if (isInteractionFinished) return;

            bool conditionsMet = true;

            if (!string.IsNullOrEmpty(requiredTaskID))
            {
                if (!TaskManager.Instance.IsTaskComplete(requiredTaskID))
                {
                    conditionsMet = false;
                }
            }

            InventoryManager inventory = interactor.GetComponent<InventoryManager>();
            if (requiredItem != null)
            {
                if (inventory == null || !inventory.HasItem(requiredItem))
                {
                    conditionsMet = false;
                }
            }

            if (conditionsMet)
            {
                if (requiredItem != null && consumeItem && inventory != null)
                {
                    inventory.RemoveItem(requiredItem, 1);
                }

                OnInteractionSuccess.Invoke();

                isInteractionFinished = true; 
            }
            else
            {
                OnInteractionFail.Invoke();
            }
        }

        public void TriggerNotification(string message)
        {
            NotificationManager.Instance.ShowNotification(message);
        }

        public void TriggerTaskComplete(string taskID)
        {
            TaskManager.Instance.CompleteTask(taskID);
        }
    }
}