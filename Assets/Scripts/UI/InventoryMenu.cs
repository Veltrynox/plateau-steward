using System;
using UnityEngine;
using UnityEngine.UI;

namespace SubnauticaClone
{
    public class InventoryMenu : MonoBehaviour
    {
        [SerializeField] private Button m_dropButton;

        public void Open(string itemName, Action onDropAction)
        {
            gameObject.SetActive(true);

            m_dropButton.onClick.RemoveAllListeners();

            m_dropButton.onClick.AddListener(() =>
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