using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
    public CardPlayer botPlayer;
    public CardPlayer otherPlayer;

    Card[] cards;

    private void Start()
    {
        cards = botPlayer.GetComponentsInChildren<Card>();
    }

    private void Update()
    {
        if (botPlayer.chosenCard != null) return;

        if(otherPlayer.chosenCard != null)
        {
            ChooseAttack();
        }
    }

    public void ChooseAttack()
    {
        int randomMove = Random.Range(0, cards.Length);
        //int selection = (lastSelected + randomMove) % cards.Length;
        //lastSelected = selection;
        botPlayer.SetMoveCard(cards[randomMove]);
    }
}
