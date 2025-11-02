using UnityEngine;
using Unity.Netcode;

public class NewMonoBehaviourScript : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 15f; // Speed of player movement

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return; // Ensure only the owner can control this player

        float h = Input.GetAxis("Horizontal"); // Get left/right input (-1 to 1)
        float v = Input.GetAxis("Vertical");   // Get forward/back input (-1 to 1)

        Vector3 input = new Vector3(h, 0, v); // Create movement vector

        // Move the player based on input, speed, and frame time
        transform.Translate(input * moveSpeed * Time.deltaTime);
        
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
