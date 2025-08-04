using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject hostMenuCanvas;
    [SerializeField] private GameObject clientMenuCanvas;
    [SerializeField] private Button hostBackButton;
    [SerializeField] private Button clientBackButton;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;

    private void ShowMainMenu()
    {
        mainMenuCanvas.SetActive(true);
        hostMenuCanvas.SetActive(false);
        clientMenuCanvas.SetActive(false);
    }

    private void ShowHostMenu() 
    {
        hostMenuCanvas.SetActive(true);
        mainMenuCanvas.SetActive(false);
    }

    private void ShowClientMenu()
    {
        clientMenuCanvas.SetActive(true);
        mainMenuCanvas.SetActive(false);
    }

    public void StartGameAsHost()
    {
        NetworkManager.Singleton.StartHost();
        SceneManager.LoadScene(1);
    }

    public void StartGameAsClient()
    {
        NetworkManager.Singleton.StartClient();
        SceneManager.LoadScene(1);
    }

    private void OnEnable()
    {
        hostButton.onClick.AddListener(ShowHostMenu);
        clientButton.onClick.AddListener(ShowClientMenu);
        hostBackButton.onClick.AddListener(ShowMainMenu);
        clientBackButton.onClick.AddListener(ShowMainMenu);
    }

    private void OnDisable()
    {
        hostButton.onClick.RemoveListener(ShowHostMenu);
        clientButton.onClick.RemoveListener(ShowClientMenu);
        hostBackButton.onClick.RemoveListener(ShowMainMenu);
        clientBackButton.onClick.RemoveListener(ShowMainMenu);
    }
}
