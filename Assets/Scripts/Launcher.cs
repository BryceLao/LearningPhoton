using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Com.Bryce.Unity {
    public class Launcher : MonoBehaviourPunCallbacks {
        #region Private Serializable Feilds

        [Tooltip("The maximum number of players per room. When a room is full," +
                 " it can't be joined by new players, and so new room will be created")]
        [SerializeField]
        private byte maxPlayersPerRoom = 4;

        #endregion

        #region Private Fields

        private string gameVersion = "1";

        #endregion

        #region MonoBehaviour CallBacks

        private void Awake() {
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        private void Start() {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
        }

        #endregion

        #region Public Methods

        public void Connect() {
            progressLabel.SetActive(true);
            controlPanel.SetActive(false);

            if (PhotonNetwork.IsConnected) {
                PhotonNetwork.JoinRandomRoom();
            }

            else {
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
        }

        [Tooltip("The UI Panel to let the user enter name, connect and play")] 
        [SerializeField] private GameObject controlPanel;

        [Tooltip("The UI Label to inform the user that the connection is in progress")] 
        [SerializeField] private GameObject progressLabel;

        #endregion

        #region MonoBehaviourPunCallbacks Callbacks

        public override void OnConnectedToMaster() {
            PhotonNetwork.JoinRandomRoom();
        }

        public override void OnDisconnected(DisconnectCause cause) {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);

            Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
        }

        public override void OnJoinRandomFailed(short returnCode, string message) {
            Debug.Log("PUN Basics Tutorial/Launcher: OnJoinRandomFailed() was called by PUN. " +
                      "No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");
            PhotonNetwork.CreateRoom(null, new RoomOptions {MaxPlayers = maxPlayersPerRoom});
        }

        public override void OnJoinedRoom() {
            Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
        }

        #endregion

    }            
        
}
