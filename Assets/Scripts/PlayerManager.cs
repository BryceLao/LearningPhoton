using UnityEngine;
using UnityEngine.EventSystems;

using Photon.Pun;

using System.Collections;

using Com.Bryce.Unity;

namespace Com.MyCompany.MyGame {
    public class PlayerManager : MonoBehaviourPunCallbacks {
        #region Private Fields

        [Tooltip("The Beams GameObject to control")]
        [SerializeField] private GameObject beams;

        private bool IsFiring;


        #endregion

        #region Public Fields

        [Tooltip("The current Health of our player")]
        public float Health = 1f;

        #endregion

        #region MonoBehaviour CallBacks

        private void Awake() {
            if (beams == null) {
                Debug.LogError("<Color=Red><a>Missing</a></Color> Beams Reference.", this);
            }
            else {
                beams.SetActive(false);
            }
        }

        private void Update() {

            ProcessInputs();

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