using System;
using UnityEngine;
using UnityEngine.UI;

namespace SubnauticaClone
{
    public class InventoryMenu : MonoBehaviour
    {
        [SerializeField] private Button dropButton;

        public void Open(string itemName, Action onDropAction)
        {
            gameObject.SetActive(true);

            dropButton.onClick.RemoveAllListeners();

            dropButton.onClick.AddListener(() =>
            {
                onDropAction?.Invoke();
                Close();
            });
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}