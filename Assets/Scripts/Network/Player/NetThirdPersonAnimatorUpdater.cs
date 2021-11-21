using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using Cinemachine;
using Unity.Netcode;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof(ThirdPersonCharacter))]
    public class NetThirdPersonAnimatorUpdater : NetworkBehaviour
    {
        ThirdPersonCharacter m_Character;
        Animator m_Animator;
        float m_Forward = 0f;

        private void Start()
        {
            m_Character = GetComponent<ThirdPersonCharacter>();
            m_Animator = GetComponent<Animator>();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (!IsClient || IsOwner)
            {
                enabled = false;
                return;
            }
        }

        private void FixedUpdate()
        {
            m_Animator.SetFloat("Forward", m_Forward);
        }

        [ServerRpc]
        void SetForwardServerRpc(float forward)
        {
            m_Forward = forward;
        }
    }
}
