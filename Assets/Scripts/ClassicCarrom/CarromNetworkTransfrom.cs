using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode.Components;
using Unity.Netcode;

public class CarromNetworkTransfrom : NetworkTransform
{

    private GameObject midPos;
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }

    protected override void Awake()
    {
        base.Awake();
        midPos = GameObject.FindGameObjectWithTag("mid_pos");
    }

    protected override void Update()
    {
        Debug.Log($"MidPos: {midPos.transform.position.x} {midPos.transform.position.y}");
        Debug.Log($"ThisPos: {transform.position.x} {transform.position.y}");

        base.Update();
        if (NetworkManager.Singleton != null && (NetworkManager.Singleton.IsConnectedClient || NetworkManager.Singleton.IsListening))
        {
            if (CanCommitToTransform)
            {
                if (IsHost)
                {
                    TryCommitTransformToServer(transform, NetworkManager.LocalTime.Time);
                }
                else
                {
                    Transform newTransform = transform;
                    newTransform.position = new Vector3(
                        2 * midPos.transform.position.x - transform.position.x,
                        2 * midPos.transform.position.y - transform.position.y,
                        transform.position.z);
                    TryCommitTransformToServer(newTransform, NetworkManager.LocalTime.Time);
                }
            }
        }

    }

}
