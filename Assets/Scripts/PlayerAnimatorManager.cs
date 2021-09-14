using UnityEngine;
using System.Collections;

namespace Com.MyCompany.MyGame {
    public class PlayerAnimatorManager : MonoBehaviour {
        #region Private Fields

        [SerializeField] private float directionDampTime = .25f;
        private Animator animator;

        #endregion

        #region MonoBehaviour CallBacks
        private void Start() {
            animator = GetComponent<Animator>();
            if (!animator) {
                Debug.LogError("PlayerAnimatorManager is Missing Animator Component", this);
            }

        }

        private void Update() {
            if (!animator) {
                return;
            }
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Base Layer.Run")) {
                if (Input.GetButtonDown("Fire2")) {
                    animator.SetTrigger("Jump");
                }
            }
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            if (v < 0) {
                v = 0;
            }
            animator.SetFloat("Speed", h * h + v * v);
            animator.SetFloat("Direction", h, directionDampTime, Time.deltaTime);
        }

        #endregion
    }
}