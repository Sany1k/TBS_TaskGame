using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUIBehaviour : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI turnNumber;
    [SerializeField] private TextMeshProUGUI amountAttackText;
    [SerializeField] private TextMeshProUGUI amountMoveText;
    [SerializeField] private Button endTurnButton;
    [SerializeField] private GameObject yourTurnShowable;
    [SerializeField] private GameObject enemyTurnShowable;

    private void Start()
    {
        endTurnButton.onClick.AddListener(OnEndPlayerTurnEvent);
    }

    public void SubscribeOnGameManagerEvents()
    {
        GameManager.Instance.PlayerTurnEndEvent += CheckWhoseTurn;
        GameManager.Instance.TimerChangedEvent += UpdateTimerText;
        GameManager.Instance.NextGameTurnsEvent += ShowNextTurn;

        CheckWhoseTurn(OwnerType.Host); // Host start first
    }

    public void UpdateAttacksAmout(int amount)
    {
        amountAttackText.text = amount.ToString();
    }

    public void UpdateMovesAmout(int amount)
    {
        amountMoveText.text = amount.ToString();
    }

    private void CheckWhoseTurn(OwnerType nextTurnType)
    {
        if (nextTurnType == GameManager.Instance.LocalOwnerTurn)
        {
            enemyTurnShowable.SetActive(false);
            yourTurnShowable.SetActive(true);
            endTurnButton.gameObject.SetActive(true);
        }
        else
        {
            yourTurnShowable.SetActive(false);
            enemyTurnShowable.SetActive(true);
            endTurnButton.gameObject.SetActive(false);
        }
    }

    private void UpdateTimerText(int newValue)
    {
        timerText.text = TimeSpan.FromSeconds(newValue).ToString(@"mm\:ss");
    }

    private void ShowNextTurn(int newValue)
    {
        turnNumber.text = $"Turn: {newValue}";
    }

    private void OnEndPlayerTurnEvent()
    {
        GameManager.Instance.SwitchPlayerTurn();
        endTurnButton.gameObject.SetActive(false);
    }
}
