using UnityEngine;
using UnityEngine.UI;

public class ScrollViewNavigation : MonoBehaviour
{
    public ScrollRect scrollView;

    public float scrollDistancePerClick = 0.1f; // Distanza di scorrimento per ogni click
    public float scrollDuration = 0.5f; // Durata del movimento

    private bool isScrolling = false;
    private float targetPosition = 0f;
    private float startTime = 0f;

    void Update()
    {
        if (isScrolling)
        {
            float elapsedTime = Time.time - startTime;
            float normalizedTime = Mathf.Clamp01(elapsedTime / scrollDuration);

            scrollView.horizontalNormalizedPosition = Mathf.Lerp(scrollView.horizontalNormalizedPosition, targetPosition, normalizedTime);

            if (normalizedTime >= 1f)
            {
                isScrolling = false;
            }
        }
    }

    public void ScrollRight()
    {
        if (!isScrolling)
        {
            targetPosition = Mathf.Clamp01(scrollView.horizontalNormalizedPosition + scrollDistancePerClick);
            StartScrolling();
        }
    }

    public void ScrollLeft()
    {
        if (!isScrolling)
        {
            targetPosition = Mathf.Clamp01(scrollView.horizontalNormalizedPosition - scrollDistancePerClick);
            StartScrolling();
        }
    }

    private void StartScrolling()
    {
        isScrolling = true;
        startTime = Time.time;
    }
}
