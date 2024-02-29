// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerListEntry.cs" company="Exit Games GmbH">
//   Part of: Asteroid Demo,
// </copyright>
// <summary>
//  Player List Entry
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

namespace Photon.Pun.Demo.Asteroids
{
    public class PlayerListEntry : MonoBehaviour
    {
        [Header("UI References")]
        public Text PlayerNameText;

        public Image PlayerColorImage;
        public Button PlayerReadyButton;
        public Image PlayerReadyImage;

        private int ownerId;
        private bool isPlayerReady;

        #region UNITY

        public void OnEnable()
        {
            PlayerNumbering.OnPlayerNumberingChanged += OnPlayerNumberingChanged;
        }

       public void Start()
{
    if (PhotonNetwork.LocalPlayer.ActorNumber != ownerId)
    {
        PlayerReadyButton.gameObject.SetActive(false);
    }
    else
    {
        // Imposta il giocatore locale come pronto automaticamente
        isPlayerReady = true; // Imposta su true per indicare che il giocatore è pronto
        SetPlayerReady(isPlayerReady); // Aggiorna l'UI per riflettere lo stato "pronto"

        Hashtable props = new Hashtable() {{AsteroidsGame.PLAYER_READY, isPlayerReady}};
        PhotonNetwork.LocalPlayer.SetCustomProperties(props); // Comunica lo stato "pronto" agli altri giocatori

        if (PhotonNetwork.IsMasterClient)
        {
            FindObjectOfType<LobbyMainPanel>().LocalPlayerPropertiesUpdated();
        }

        // Poiché il giocatore è già pronto, nascondi il pulsante
        PlayerReadyButton.gameObject.SetActive(false);
    }
}

        public void OnDisable()
        {
            PlayerNumbering.OnPlayerNumberingChanged -= OnPlayerNumberingChanged;
        }

        #endregion

        public void Initialize(int playerId, string playerName)
        {
            ownerId = playerId;
            PlayerNameText.text = playerName;
        }

        private void OnPlayerNumberingChanged()
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (p.ActorNumber == ownerId)
                {
                    PlayerColorImage.color = AsteroidsGame.GetColor(p.GetPlayerNumber());
                }
            }
        }

        public void SetPlayerReady(bool playerReady)
        {
            PlayerReadyButton.GetComponentInChildren<Text>().text = playerReady ? "Ready!" : "Ready?";
            PlayerReadyImage.enabled = playerReady;
        }
    }
}