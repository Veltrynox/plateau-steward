using UnityEngine;
using TMPro;
using System.Collections;

public class NotificationItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private CanvasGroup canvasGroup;

    private float displayDuration = 6.0f;
    private float fadeDuration = 0.5f;

    public void Setup(string message)
    {
        messageText.text = message;
        StartCoroutine(LifeCycle());
    }

    private IEnumerator LifeCycle()
    {
        yield return new WaitForSeconds(displayDuration);

        float timer = 0;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
            yield return null;
        }

        Destroy(gameObject);
    }
}