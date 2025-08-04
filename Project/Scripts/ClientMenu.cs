using System;
using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class ClientMenu : MonoBehaviour
{
    [SerializeField] private MenuBehaviour menuBehaviour;
    [SerializeField] private TMP_InputField addressInputField;
    [SerializeField] private Button connectButton;
    [SerializeField] private UnityTransport unityTransport;

    private string hostIPAddress = "127.0.0.1";


    private void SetIPAddress(string text)
    {
        if (text == "") hostIPAddress = "127.0.0.1";
        hostIPAddress = addressInputField.text;
    }

    private void TryConnectToHost()
    {
        if (ValidateIPv4(hostIPAddress))
        {
            unityTransport.SetConnectionData(hostIPAddress, (ushort)7777);
            menuBehaviour.StartGameAsClient();
        }
    }

    private bool ValidateIPv4(string ipString)
    {
        if (String.IsNullOrEmpty(ipString))
        {
            return false;
        }

        string[] splitValues = ipString.Split('.');
        if (splitValues.Length != 4)
        {
            return false;
        }

        byte tempForParsing;

        return splitValues.All(r => byte.TryParse(r, out tempForParsing));
    }

    private void OnEnable()
    {
        addressInputField.onEndEdit.AddListener(SetIPAddress);
        connectButton.onClick.AddListener(TryConnectToHost);
    }

    private void OnDisable()
    {
        addressInputField.onEndEdit.RemoveListener(SetIPAddress);
        connectButton.onClick.RemoveListener(TryConnectToHost);
    }
}
