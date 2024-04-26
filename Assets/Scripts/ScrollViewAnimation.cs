using UnityEngine;
using UnityEngine.UI;

public class ScrollViewNavigation : MonoBehaviour
{
    public ScrollRect scrollView;
    public GameObject startGameObject; // Gameobject da disattivare quando lo ScrollView è all'inizio
    public GameObject endGameObject; // Gameobject da disattivare quando lo ScrollView è alla fine

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

            // Se lo ScrollView è all'inizio, disattiva il gameobject di fine e attiva quello di inizio
            if (scrollView.horizontalNormalizedPosition <= 0)
            {
                startGameObject.SetActive(true);
                endGameObject.SetActive(false);
            }
            // Se lo ScrollView è alla fine, disattiva il gameobject di inizio e attiva quello di fine
            else if (scrollView.horizontalNormalizedPosition >= 1)
            {
                startGameObject.SetActive(false);
                endGameObject.SetActive(true);
            }
            else
            {
                // Se non è né all'inizio né alla fine, disattiva entrambi i gameobject
                startGameObject.SetActive(false);
                endGameObject.SetActive(false);
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
