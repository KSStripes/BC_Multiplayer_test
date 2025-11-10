using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer_Shoot : NetworkBehaviour
{
    [SerializeField] private GameObject barrelEnd;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] float bulletSpeed = 50f;

    // Update is called once per frame
    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetMouseButtonDown(0))
        {
            RequestShootServerRpc();
        }
    }

    [ServerRpc]
    private void RequestShootServerRpc(ServerRpcParams rpcParams = default)
    {
        GameObject bullet = Instantiate(bulletPrefab, barrelEnd.transform.position, barrelEnd.transform.rotation);

        bullet.GetComponent<NetworkObject>().Spawn();
        bullet.GetComponent<Bullet>().Initialize(barrelEnd.transform.forward);
        NotifyShootClientRpc();
    }

    [ClientRpc]
    private void NotifyShootClientRpc()
    {
       // Update client side UI
    }
}