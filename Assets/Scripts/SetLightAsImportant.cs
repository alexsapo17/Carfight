using UnityEngine;

public interface ILightCookie
{
    void NotifyNewLight(Light newLight);
}

public class SetLightAsImportant : MonoBehaviour, ILightCookie
{
    private void OnEnable()
    {
        // Registrati per ricevere notifiche sulle nuove luci
        LightCookieManager.Instance.RegisterLightCookie(this);
    }

    public void NotifyNewLight(Light newLight)
    {
        // Verifica se il GameObject ha un componente Light
        if (newLight != null)
        {
            // Imposta la luce come "importante"
            newLight.renderingLayerMask = 1; // Imposta il layer di rendering su Default per renderizzare sempre la luce
        }
    }
}

public class LightCookieManager : MonoBehaviour
{
    public static LightCookieManager Instance;

    private void Awake()
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

    public void RegisterLightCookie(ILightCookie lightCookie)
    {
        // Ispeziona tutte le luci presenti nella scena
        Light[] lights = FindObjectsOfType<Light>();
        foreach (Light light in lights)
        {
            lightCookie.NotifyNewLight(light);
        }
    }
    
    // Metodo per notificare il manager delle luci quando una nuova luce viene attivata
    private void OnEnable()
    {
        Light newLight = GetComponent<Light>();
        if (newLight != null)
        {
            foreach (ILightCookie lightCookie in FindObjectsOfType<MonoBehaviour>() as ILightCookie[])
            {
                lightCookie.NotifyNewLight(newLight);
            }
        }
    }
}
