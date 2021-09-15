using System;
using UnityEngine;
using UnityEngine.EventSystems;

using Photon.Pun;

using System.Collections;

using Com.Bryce.Unity;
using Photon.Realtime;
using UnityEngine.SceneManagement;

namespace Com.MyCompany.MyGame {
    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable{

        #region IPunObservable Implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
            if (stream.IsWriting) {
                stream.SendNext(IsFiring);
            }
            else {
                this.IsFiring = (bool) stream.ReceiveNext();
            }

            if (stream.IsWriting) {
                stream.SendNext(IsFiring);
                stream.SendNext(Health);
            }
            else {
                this.IsFiring = (bool) stream.ReceiveNext();
                this.Health = (float) stream.ReceiveNext();
            }
        }

        #endregion

        #region Private Fields

        [Tooltip("The Beams GameObject to control")]
        [SerializeField] private GameObject beams;

        private bool IsFiring;

        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene Scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode) {
            this.CalledOnLevelWasLoaded(Scene.buildIndex);
        }
        #endregion

        #region Public Fields

        [Tooltip("The Player's UI GameObject Prefab")]
        [SerializeField] public GameObject PlayerUiPrefab;

        [Tooltip("The current Health of our player")]
        public float Health = 1f;

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        #endregion

        #region MonoBehaviour CallBacks

        private void Awake() {
            if (beams == null) {
                Debug.LogError("<Color=Red><a>Missing</a></Color> Beams Reference.", this);
            }
            else {
                beams.SetActive(false);
            }

            if (photonView.IsMine) {
                PlayerManager.LocalPlayerInstance = this.gameObject;
            }
            
            DontDestroyOnLoad(this.gameObject);

        }

        private void Start() {
            CameraWork _cameraWork = this.gameObject.GetComponent<CameraWork>();
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;

            if (_cameraWork != null) {
                if (photonView.IsMine) {
                    _cameraWork.OnStartFollowing();
                }
            }

            else {
                Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
            }

            if (PlayerUiPrefab != null) {
                GameObject _uiGo =  Instantiate(PlayerUiPrefab);
                _uiGo.SendMessage ("SetTarget", this, SendMessageOptions.RequireReceiver);
            }
            else {
                Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
            }
        }

        private void Update() {
            if(photonView.IsMine){
                ProcessInputs();
            }

            if (beams != null && IsFiring != beams.activeInHierarchy) {
                beams.SetActive(IsFiring);
            }
            if (photonView.IsMine) {
                ProcessInputs();
                if (Health <= 0f)
                {
                    GameManager.Instance.LeaveRoom();
                }
            }
        }

        private void OnTriggerEnter(Collider other) {
            if (!photonView.IsMine) {
                return;
            }
            if (!other.name.Contains("Beam")) {
                return;
            }
            Health -= 0.1f;
        }

        private void OnTriggerStay(Collider other) {
            if (!photonView.IsMine) {
                return;
            }

            if (!other.name.Contains("Beam")) {
                return;
            }

            Health -= 0.1f * Time.deltaTime;
        }

        private void OnLevelWasLoaded(int level) {
            this.CalledOnLevelWasLoaded(level);

            GameObject _uiGo = Instantiate(this.PlayerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }

        private void CalledOnLevelWasLoaded(int level) {
            if (!Physics.Raycast(transform.position, -Vector3.up, 5f)) {
                transform.position = new Vector3(0f, 5f, 0f);
            }
        }
        public override void OnDisable() {
            base.OnDisable ();
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        #endregion

        #region Custom

        private void ProcessInputs() {
            if (Input.GetButtonDown("Fire1")) {
                if (!IsFiring) {
                    IsFiring = true;
                }
            }
            if (Input.GetButtonUp("Fire1")) {
                if (IsFiring) {
                    IsFiring = false;
                }
            }
        }

        #endregion
    }
}