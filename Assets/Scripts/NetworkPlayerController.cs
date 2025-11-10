using UnityEngine;
using Unity.Netcode;

public class NetworkPlayerController : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 15f; // Speed of player movement

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return; // only the owner client can control this player

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 input = new Vector3(h, 0, v); // Create movement vector

        // Rotate the tank based on input, adjusting for tank rotation
        if (h > 1)
        {
            // Rotate right
        }
        else if (h < 0)
        {
            // Rotate left
        }

        // Move the tank forward/backward based on vertical input
        Vector3 movement = new Vector3(0, 0, v) * moveSpeed * Time.deltaTime;
        transform.position += movement;

        //transform.Translate(input * moveSpeed * Time.deltaTime);

        // if (input != Vector3.zero){ // Only move if there's input
        //     MoveServerRpc(input); // Send movement to server
        // }
    }

    [ServerRpc] // Runs on server, called by client
    void MoveServerRpc(Vector3 input)
    {
        // Move the player based on input, speed, and frame time
        transform.Translate(input * moveSpeed * Time.deltaTime);
    }
}
