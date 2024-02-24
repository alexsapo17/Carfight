using Firebase.Database;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks; // Assicurati di includere questo per Task
using Firebase.Extensions;
public class PlayerNicknameDisplay : MonoBehaviourPunCallbacks
{
    public GameObject nicknameUIPrefab; // Assegna il prefab del nickname UI creato
    private GameObject nicknameUIInstance;
    private Text nicknameText;

    private DatabaseReference databaseReference;

    void Start()
    {
        if (photonView.IsMine)
        {
            // Ottieni il riferimento al database di Firebase
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference; 

            // Crea l'UI del nickname per questo giocatore
            nicknameUIInstance = Instantiate(nicknameUIPrefab, transform.position + Vector3.up * 2, Quaternion.identity);
            nicknameText = nicknameUIInstance.GetComponentInChildren<Text>();

            string userId = "userId"; // Sostituisci con il metodo per ottenere l'ID utente corretto
            databaseReference.Child("users").Child(userId).Child("nickname").GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && !task.IsFaulted)
                {
                    DataSnapshot snapshot = task.Result;
                    // Aggiorna il testo del nickname con il valore recuperato da Firebase
                    nicknameText.text = snapshot.Value.ToString();
                }
            });
        }
    }

    void Update()
    {
        if (nicknameUIInstance != null)
        {
            // Aggiorna la posizione del nickname UI per seguirlo sopra la macchina
            nicknameUIInstance.transform.position = transform.position + Vector3.up * 3;
            // Opzionalmente, ruota l'UI per farlo sempre guardare verso la camera
            nicknameUIInstance.transform.rotation = Camera.main.transform.rotation;
        }
    }

    // Assicurati di distruggere l'UI del nickname quando il giocatore viene distrutto
    void OnDestroy()
    {
        if (nicknameUIInstance != null)
        {
            Destroy(nicknameUIInstance);
        }
    }
}
