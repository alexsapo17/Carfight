using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class FlashbangManager : MonoBehaviour
{
    public static FlashbangManager Instance;

    public Image flashEffectPanel; // Assicurati di assegnare questo pannello nell'Inspector

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TriggerFlashbangEffect(float duration, float fadeOutTime)
    {
        StartCoroutine(DoFlashEffect(duration, fadeOutTime));
            // Accedi all'AudioSource e riproduci il suono
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.Play();
            }
    }

    private IEnumerator DoFlashEffect(float duration, float fadeOutTime)
    {
        flashEffectPanel.gameObject.SetActive(true);
        flashEffectPanel.color = new Color(flashEffectPanel.color.r, flashEffectPanel.color.g, flashEffectPanel.color.b, 1);

        yield return new WaitForSeconds(duration);

        float time = 0;
        while (time < fadeOutTime)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, time / fadeOutTime);
            flashEffectPanel.color = new Color(flashEffectPanel.color.r, flashEffectPanel.color.g, flashEffectPanel.color.b, alpha);
            yield return null;
        }

        flashEffectPanel.gameObject.SetActive(false);
    }
}
