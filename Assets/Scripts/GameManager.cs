using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public CardPlayer P1, P2;
    public GameState currentState = GameState.ChooseMove;
    public GameObject gameOverPanel;
    public TMP_Text gameOverText;

    private CardPlayer winner, loser;

    public enum GameState
    {
        ChooseMove,
        Move,
        Damages,
        Draw,
        End,
    }

    private void Start()
    {
        gameOverPanel.SetActive(false);
    }

    void Update()
    {
        switch (currentState)
        {
            case GameState.ChooseMove:
                {
                    if (P1.MoveValue != null && P2.MoveValue != null)
                    {
                        P1.AnimateMove();
                        P2.AnimateMove();
                        P1.CanClick(false);
                        P2.CanClick(false);
                        currentState = GameState.Move;
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
                            currentState = GameState.Damages;
                        }
                        else
                        {
                            P1.AnimateDraw();
                            P2.AnimateDraw();
                            currentState = GameState.Draw;
                        }
                    }
                    break;
                }
            case GameState.Damages:
                {
                    if (P1.isAnimating() == false && P2.isAnimating() == false)
                    {
                        winner.ChangeHealth(5);
                        loser.ChangeHealth(-10);

                        CardPlayer won = GetGameWinner();

                        if(won == null)
                        {
                            ResetPlayers();
                            P1.CanClick(true);
                            P2.CanClick(true);
                            currentState = GameState.ChooseMove;
                        }
                        else
                        {
                            gameOverPanel.SetActive(true);
                            gameOverText.text = ((won == P1) ? "Player 1" : "Player 2")  + " wins this game!";
                            ResetPlayers();
                            currentState = GameState.End;
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
                        currentState = GameState.ChooseMove;
                    }
                    break;
                }
            case GameState.End:
                {
                    break;
                }
        }
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
