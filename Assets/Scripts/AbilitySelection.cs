using UnityEngine;
using UnityEngine.UI; // Per lavorare con l'UI
using System.Collections;

public class AbilitySelection : MonoBehaviour
{


    public void SelectAbility(string abilityName)
    {
        PlayerPrefs.SetString("SelectedAbility", abilityName);
        PlayerPrefs.Save();
        Debug.Log($"AbilitySelection: Selezione abilità - {abilityName} salvata nelle PlayerPrefs.");
        UpdateButtonColors();
    }
       public void StartDelayedUpdateButtonColors()
    {
        StartCoroutine(DelayedUpdateButtonColors());
    }
IEnumerator DelayedUpdateButtonColors()
{
    yield return new WaitForSeconds(0.1f); // Attendi per un breve periodo
    UpdateButtonColors();
}

    public void UpdateButtonColors()
    {
        // Ottieni l'abilità selezionata dalle PlayerPrefs
        string selectedAbility = PlayerPrefs.GetString("SelectedAbility", "");
        Debug.Log($"AbilitySelection: UpdateButtonColors - Abilità selezionata dalle PlayerPrefs: '{selectedAbility}'.");

        // Trova tutti i pulsanti con il tag "AbilityButton"
        GameObject[] abilityButtons = GameObject.FindGameObjectsWithTag("AbilityButton");
        Debug.Log($"AbilitySelection: Trovati {abilityButtons.Length} pulsanti con tag 'AbilityButton'.");

        foreach (GameObject buttonObj in abilityButtons)
        {
            Text buttonText = buttonObj.GetComponentInChildren<Text>(); // Ottieni il componente Text del figlio
            if (buttonText != null)
            {
                if (buttonObj.name.Contains(selectedAbility))
                {
                    buttonText.color = Color.green; // Abilità selezionata
                    Debug.Log($"AbilitySelection: Impostato testo verde per {buttonObj.name}.");
                }
                else
                {
                    buttonText.color = Color.white; // Abilità non selezionata
                    Debug.Log($"AbilitySelection: Impostato testo bianco per {buttonObj.name}.");
                }
            }
            else
            {
                Debug.Log($"AbilitySelection: Nessun componente Text trovato in {buttonObj.name}.");
            }
        }
    }
}
