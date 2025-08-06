using System.Net;
using TMPro;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class ServerMenu : MonoBehaviour
{
    [SerializeField] private Button startGameButton;
    [SerializeField] private MenuBehaviour menuBehaviour;
    [SerializeField] private TextMeshProUGUI myLocalIPText;
    [SerializeField] private UnityTransport unityTransport;

    private void Awake()
    {
        myLocalIPText.text = GetMyLocalIP();
        startGameButton.onClick.AddListener(() => menuBehaviour.StartGameAsHost());
        unityTransport.SetConnectionData(GetMyLocalIP(), (ushort)7777);
    }

    private string GetMyLocalIP()
    {
        string localIp = "";
        IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());

        foreach (IPAddress ip in hostEntry.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                localIp = ip.ToString();
                break;
            }
        }
        return localIp;
    }
}
