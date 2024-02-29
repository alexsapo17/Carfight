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

        // Assicurati di essere autenticato e di usare l'ID utente corretto
        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        if (auth.CurrentUser != null)
        {
            string userId = auth.CurrentUser.UserId; // Usa l'ID utente autenticato
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
}


    void Update()
    {
        if (nicknameUIInstance != null)
        {
            // Aggiorna la posizione del nickname UI per seguirlo sopra la macchina
            nicknameUIInstance.transform.position = transform.position + Vector3.up * 4;
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
