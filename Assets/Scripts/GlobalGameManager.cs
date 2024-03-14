
using System.Collections.Generic;
using UnityEngine;

public class GlobalGameManager : MonoBehaviour
{
    public static GlobalGameManager Instance;

    // Modifica per mantenere i nomi dei giocatori finti
    private List<string> fakePlayerNames = new List<string>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Metodo per aggiungere un nome di giocatore finto
    public void AddFakePlayerName(string name)
    {
        fakePlayerNames.Add(name);
    }

    // Metodo per ottenere tutti i nomi dei giocatori finti
    public List<string> GetFakePlayerNames()
    {
        return fakePlayerNames;
    }

    public void ResetFakePlayerCountAndNames()
    {
        fakePlayerNames.Clear();
    }

    // Proprietà per ottenere il conteggio dei giocatori finti
    public int FakePlayerCount
    {
        get { return fakePlayerNames.Count; }
    }
}

