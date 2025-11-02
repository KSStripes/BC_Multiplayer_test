using Unity.Netcode;
using UnityEngine;

public class ServerButtons : MonoBehaviour
{

    public void StartHost()
    {
        this.gameObject.SetActive(false);
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        this.gameObject.SetActive(false);
        NetworkManager.Singleton.StartClient();
    }
}
