using Cinemachine;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Assumes client authority
/// </summary>
[RequireComponent(typeof(ServerPlayerMove))]
[DefaultExecutionOrder(1)] // after server component
public class ClientPlayerMove : NetworkBehaviour
{
    public GameObject m_playerGraphics;

    private ServerPlayerMove m_serverPlayer;
    private Vector3 m_move = Vector3.zero;
    private Camera m_mainCamera;
    private CinemachineFreeLook m_freelookCam;
    
    private void Awake()
    {
        m_serverPlayer = GetComponent<ServerPlayerMove>();
        m_mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        m_freelookCam = m_mainCamera.GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineFreeLook>();
        m_playerGraphics.transform.parent = null;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsClient || !IsOwner)
        {
            enabled = false;
            return;
        }
        m_freelookCam.LookAt = transform;
        m_freelookCam.Follow = transform;
    }

    private void Update()
    {
        m_move.x = Input.GetAxis("Horizontal");
        m_move.z = Input.GetAxis("Vertical");
        //m_playerGraphics.transform.position = Vector3.Lerp(m_playerGraphics.transform.localPosition, m_playerGraphics.transform.localPosition + m_move, Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (Mathf.Abs(m_move.magnitude) > 0.01f) m_serverPlayer.MovePlayerServerRpc(m_move);
    }
}