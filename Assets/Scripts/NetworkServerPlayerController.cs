using Unity.Netcode;
using UnityEngine;

public class NetworkServerPlayerController : NetworkBehaviour
{
    public float moveSpeed = 15f;
    private Vector2 moveInput;
    void Update()
    {
        if (IsOwner && !IsServer)
        {
            //Input
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            SubmitInputServerRpc(input);
        }

        if (IsServer)
        {
            //Movement
            Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed * Time.deltaTime;
            transform.position += movement;
        }
    }
    
    // called by client, runs on server, tells server the move input from client
    [ServerRpc]
    private void SubmitInputServerRpc(Vector2 input)
    {
        moveInput = input;
    }
}
