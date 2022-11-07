using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardNetPlayer : MonoBehaviourPun
{
    public static List<CardNetPlayer> NetPlayers = new List<CardNetPlayer>(2);

    private Card[] cards;

    public void Set(CardPlayer player)
    {
        player.Nickname.text = photonView.Owner.NickName;
        cards = player.GetComponentsInChildren<Card>();
        foreach(Card card in cards)
        {
            Button button = card.GetComponent<Button>();
            button.onClick.AddListener(() => RemoteClickButton(card.move));
        }
    }

    private void RemoteClickButton(Move value)
    {
        if (photonView.IsMine)
        {
            photonView.RPC("RemoteClickButtonRPC", RpcTarget.Others, (int)value);
        }
    }

    [PunRPC]
    private void RemoteClickButtonRPC(int value)
    {
        foreach(Card card in cards)
        {
            if(card.move == (Move)value)
            {
                card.GetComponent<Button>().onClick.Invoke();
                break;
            }
        }
    }

    private void OnEnable()
    {
        NetPlayers.Add(this);
    }
    private void OnDisable()
    {
        NetPlayers.Remove(this);

        foreach (Card card in cards)
        {
            if (card == null) continue;
            Button button = card.GetComponent<Button>();
            button.onClick.RemoveListener(() => RemoteClickButton(card.move));
        }
    }
}
