using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    public float speed = 50f;
    private NetworkVariable<Vector3> direction = new(writePerm: NetworkVariableWritePermission.Server);
    public void Initialize(Vector3 dir)
    {
        if (IsServer)
        {
            direction.Value = dir.normalized;
        } 
    }

    private void FixedUpdate()
    {
        transform.position += direction.Value * speed * Time.deltaTime;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (IsServer) 
        {
            NetworkObject.Despawn();
            if (collision.gameObject.TryGetComponent<NetworkPlayerController>(out NetworkPlayerController player))
            {
                // Apply damage to player
                player.TakeDamage(10);
            }
        }
    }
}
