using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CardGameManager : MonoBehaviour, IOnEventCallback
{
    public CardPlayer P1, P2;
    public int damageValue = 10;
    public int restoreValue = 5;
    public GameState currentState, nextState = GameState.NetPlayerInitialization;
    public GameObject gameOverPanel;
    public TMP_Text gameOverText;
    public TMP_Text pingText;
    public bool isOnline = true;
    public int scorePerWin = 100;

    private CardPlayer winner, loser;

    HashSet<int> syncReadyPlayers = new HashSet<int>(2);

    public enum GameState
    {
        SyncState,
        NetPlayerInitialization,
        ChooseMove,
        Move,
        Damages,
        Draw,
        End,
    }

    private void Start()
    {
        gameOverPanel.SetActive(false);
        InitializeRoomSettings();
        if (!isOnline)
        {
            P2.GetComponent<Bot>().enabled = true;
            currentState = GameState.ChooseMove;
            return;
        }
        PhotonNetwork.Instantiate("CardNetPlayer", Vector3.zero, Quaternion.identity);
        StartCoroutine(CheckPing());
    }

    private void InitializeRoomSettings()
    {
        if(PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(PropertyNames.Room.MaxHealth, out var healthValue))
        {
            P1.Health = (int)healthValue;
            P1.MaxHealth = (int)healthValue;
            P1.UpdateHealthUI();

            P2.Health = (int)healthValue;
            P2.MaxHealth = (int)healthValue;
            P2.UpdateHealthUI();
        }
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(PropertyNames.Room.DamageValue, out var damageValue))
        {
            this.damageValue = (int)damageValue;
        }
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(PropertyNames.Room.RestoreValue, out var restoreValue))
        {
            this.restoreValue = (int)restoreValue;
        }
    }

    void Update()
    {
        switch (currentState)
        {
            case GameState.SyncState:
                {
                    if (syncReadyPlayers.Count == 2)
                    {
                        syncReadyPlayers.Clear();
                        currentState = nextState;
                    }
                    break;
                }
            case GameState.NetPlayerInitialization:
                {
                    if(CardNetPlayer.NetPlayers.Count == 2)
                    {
                        foreach(CardNetPlayer netPlayer in CardNetPlayer.NetPlayers)
                        {
                            if (netPlayer.photonView.IsMine)
                            {
                                netPlayer.Set(P1);
                            }
                            else
                            {
                                netPlayer.Set(P2);
                            }
                        }
                        ChangeState(GameState.ChooseMove);
                    }
                    break;
                }
            case GameState.ChooseMove:
                {
                    if (P1.MoveValue != null && P2.MoveValue != null)
                    {
                        P1.AnimateMove();
                        P2.AnimateMove();
                        P1.CanClick(false);
                        P2.CanClick(false);
                        ChangeState(GameState.Move);
                    }
                    break;
                }
            case GameState.Move:
                {
                    if(P1.isAnimating() == false && P2.isAnimating() == false)
                    {
                        winner = CalculateRound();
                        loser = GetRoundLoser();
                        if (loser != null)
                        {
                            loser.AnimateDamaged();
                            ChangeState(GameState.Damages);
                        }
                        else
                        {
                            P1.AnimateDraw();
                            P2.AnimateDraw();
                            ChangeState(GameState.Draw);
                        }
                    }
                    break;
                }
            case GameState.Damages:
                {
                    if (P1.isAnimating() == false && P2.isAnimating() == false)
                    {
                        winner.ChangeHealth(restoreValue);
                        loser.ChangeHealth(-damageValue);

                        winner.ChangeConsecutiveWin(1);
                        loser.ChangeConsecutiveWin(-1);

                        winner.ChangeScore(scorePerWin);

                        CardPlayer won = GetGameWinner();

                        if(won == null)
                        {
                            ResetPlayers();
                            P1.CanClick(true);
                            P2.CanClick(true);
                            ChangeState(GameState.ChooseMove);
                        }
                        else
                        {
                            gameOverPanel.SetActive(true);
                            gameOverText.text = ((won == P1) ? $"{P1.Nickname.text}" : $"{P2.Nickname.text}")  + " wins this game!\nScore : " + winner.score;
                            ResetPlayers();
                            ChangeState(GameState.End);
                        }
                    }
                    break;
                }
            case GameState.Draw:
                {
                    if (P1.isAnimating() == false && P2.isAnimating() == false)
                    {
                        ResetPlayers();
                        P1.CanClick(true);
                        P2.CanClick(true);
                        ChangeState(GameState.ChooseMove);
                    }
                    break;
                }
            case GameState.End:
                {
                    break;
                }
        }
    }

    int ping = 0;

    IEnumerator CheckPing()
    {
        ping = PhotonNetwork.GetPing();
        pingText.text = "Ping : " + ping.ToString() + "ms";

        if (ping >= 200) pingText.color = Color.red;
        else if (ping >= 100) pingText.color = Color.yellow;
        else pingText.color = Color.green;

        yield return new WaitForSeconds(1);
        StartCoroutine(CheckPing());
    }

    private CardPlayer CalculateRound()
    {
        Move? Player1Move = P1.MoveValue;
        Move? Player2Move = P2.MoveValue;

        if(Player1Move == Move.Rock)
        {
            if (Player2Move == Move.Scissors) return P1;
            if (Player2Move == Move.Paper) return P2;
        }
        else if(Player1Move == Move.Paper)
        {
            if (Player2Move == Move.Rock) return P1;
            if (Player2Move == Move.Scissors) return P2;
        }
        else if(Player1Move == Move.Scissors)
        {
            if (Player2Move == Move.Rock) return P2;
            if (Player2Move == Move.Paper) return P1;
        }

        return null;
    }

    private void ChangeState(GameState nextState)
    {
        if (!isOnline)
        {
            currentState = nextState;
            return;
        }

        if (this.nextState == nextState) return;

        int actorNum = PhotonNetwork.LocalPlayer.ActorNumber;
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
        raiseEventOptions.Receivers = ReceiverGroup.Others;
        PhotonNetwork.RaiseEvent(1, actorNum, raiseEventOptions, SendOptions.SendReliable);
        currentState = GameState.SyncState;
        this.nextState = nextState;

        if (!syncReadyPlayers.Contains(actorNum))
        {
            syncReadyPlayers.Add(actorNum);
        }
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 1)
        {
            int actorNum = (int)photonEvent.CustomData;

            if (!syncReadyPlayers.Contains(actorNum))
            {
                syncReadyPlayers.Add(actorNum);
            }
        }
    }

    private void ResetPlayers()
    {
        winner = null;
        loser = null;
        P1.Reset();
        P2.Reset();
    }

    private CardPlayer GetRoundLoser()
    {
        if (winner == null) return null;
        if (winner == P1) return P2;
        return P1;
    }

    private CardPlayer GetGameWinner()
    {
        if (P1.Health <= 0) return P2;
        else if (P2.Health <= 0) return P1;
        return null;
    }

    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }
}
