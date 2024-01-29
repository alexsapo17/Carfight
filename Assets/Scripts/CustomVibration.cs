using UnityEngine;
using System.Collections;

public class CustomVibration : MonoBehaviour
{
    public enum VibrationType
    {
        LightShort,
        IntenseShort,
        IntenseLong,
        LightLong,
        LightMedium,
        LightToIntenseShort,
        Heartbeat,
        MorseCode,
        Explosion,
        Wakeup,
        Tremor,
        Raindrops,
        PowerUp,
        Warning,
        Victory,
        Earthquake
    }

    public VibrationType vibrationType;
    public bool vibrateOnAwake;
    public bool vibrateOnCollision;
    public bool vibrateOnClick;

public float heartbeatDuration = 1.0f; // Durata della vibrazione Heartbeat in secondi
public float delayBeforeVibrating = 0.0f; // Ritardo prima di iniziare la vibrazione

void Awake()
{
    if (vibrateOnAwake)
    {
        Vibrate();
    }
}

void OnCollisionEnter(Collision collision)
{
    if (vibrateOnCollision)
    {
        Vibrate();
    }
}

public void OnButtonClick()
{
    if (vibrateOnClick)
    {
        Vibrate();
    }
}

private void Vibrate()
{
    StartCoroutine(DelayedVibration());
}

private IEnumerator DelayedVibration()
{
    yield return new WaitForSecondsRealtime(delayBeforeVibrating);

    switch (vibrationType)
    {
            case VibrationType.LightShort:
                StartCoroutine(VibratePattern(new float[] { 0, 100 }));
                break;
            case VibrationType.IntenseShort:
                StartCoroutine(VibratePattern(new float[] { 0, 200 }));
                break;
            case VibrationType.IntenseLong:
                StartCoroutine(VibratePattern(new float[] { 0, 500 }));
                break;
            case VibrationType.LightLong:
                StartCoroutine(VibratePattern(new float[] { 0, 400 }));
                break;
            case VibrationType.LightMedium:
                StartCoroutine(VibratePattern(new float[] { 0, 300 }));
                break;
            case VibrationType.LightToIntenseShort:
                StartCoroutine(VibratePattern(new float[] { 0, 100, 200, 100 }));
                break;
            case VibrationType.Heartbeat:
                StartCoroutine(VibratePattern(new float[] { 0, 100, 200, 100, 500, 100 }));
                break;
            case VibrationType.MorseCode:
                StartCoroutine(VibratePattern(new float[] { 0, 100, 100, 100, 100, 300, 100, 300, 100, 100, 300, 100, 100, 100 }));
                break;
            case VibrationType.Explosion:
                StartCoroutine(VibratePattern(new float[] { 0, 500, 300, 300, 300, 500 }));
                break;
            case VibrationType.Wakeup:
                StartCoroutine(VibratePattern(new float[] { 0, 100, 100, 200, 100, 300 }));
                break;
            case VibrationType.Tremor:
                StartCoroutine(VibratePattern(new float[] { 0, 50, 100, 50, 50, 50, 50, 50, 50, 50 }));
                break;
            case VibrationType.Raindrops:
                StartCoroutine(VibratePattern(new float[] { 0, 50, 200, 50, 150, 50, 300, 50, 100 }));
                break;
            case VibrationType.PowerUp:
                StartCoroutine(VibratePattern(new float[] { 0, 100, 100, 200, 100, 300 }));
                break;
            case VibrationType.Warning:
                StartCoroutine(VibratePattern(new float[] { 0, 300, 100, 300, 100, 300 }));
                break;
            case VibrationType.Victory:
                StartCoroutine(VibratePattern(new float[] { 0, 500, 100, 100, 100, 500 }));
                break;
            case VibrationType.Earthquake:
                StartCoroutine(VibratePattern(new float[] { 0, 300, 100, 300, 100, 300, 100, 300, 100, 300, 100, 300 }));
                break;
        }
    }

    private IEnumerator VibratePattern(float[] pattern)
    {
        foreach (float time in pattern)
        {
            if (time == 0)
            {
                Handheld.Vibrate();
            }
            else
            {
                yield return new WaitForSecondsRealtime(time / 1000f);
            }
        }
    }
}
