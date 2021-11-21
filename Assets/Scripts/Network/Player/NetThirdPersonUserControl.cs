using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using Cinemachine;
using Unity.Netcode;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof(NetThirdPersonCharacter))]
    public class NetThirdPersonUserControl : NetworkBehaviour
    {
        public Transform m_FollowTarget;
        public Transform m_LookTarget;
        private NetThirdPersonCharacter m_Character; // A reference to the NetThirdPersonCharacter on the object
        private Transform m_Cam;                  // A reference to the main camera in the scenes transform
        private Vector3 m_Move;
        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.
        private CinemachineVirtualCameraBase m_VCam;

        public override void OnNetworkSpawn()
        {
            if (!IsClient || !IsOwner)
            {
                enabled = false;
                return;
            }

            m_Cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
            m_VCam = m_Cam.GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCameraBase>(); // this is fine since it only happens once.
            m_VCam.Follow = m_FollowTarget;
            m_VCam.LookAt = m_LookTarget;
            /*// get the transform of the main camera
            if (Camera.main != null)
            {
                m_Cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }*/
        }

        private void Start()
        {
            m_Character = GetComponent<NetThirdPersonCharacter>();
        }

        private void Update()
        {
            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }
        }


        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            // read inputs
            m_Move.x = CrossPlatformInputManager.GetAxis("Horizontal");
            m_Move.z = CrossPlatformInputManager.GetAxis("Vertical");
            bool crouch = Input.GetKey(KeyCode.C);

            m_Move.Normalize();

            // calculate move direction to pass to character
            if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                //m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                m_Move = m_Move.z * m_Cam.forward + m_Move.x * m_Cam.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                m_Move = m_Move.z * Vector3.forward + m_Move.x * Vector3.right;
            }
            
            /* vvvvv fix this later vvvvv */
            m_Move *= 0.5f;
#if !MOBILE_INPUT
            // walk speed multiplier
            if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 2f;
#endif

            // pass all parameters to the character control script
            m_Character.Move(m_Move.x, m_Move.z, crouch, m_Jump);
            m_Jump = false;
        }
    }
}
