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
            Connect();
        }

        #endregion

        #region Public Methods

        public void Connect() {
            if (PhotonNetwork.IsConnected) {
                PhotonNetwork.JoinRandomRoom();
            }

            else {
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
        }

        #endregion

        #region MonoBehaviourPunCallbacks Callbacks

        public override void OnConnectedToMaster() {
            PhotonNetwork.JoinRandomRoom();
        }

        public override void OnDisconnected(DisconnectCause cause) {
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
