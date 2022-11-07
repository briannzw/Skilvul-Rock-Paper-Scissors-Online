using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardPlayer : MonoBehaviour
{
    public Card chosenCard;

    public Transform cardMovePos;

    private Tweener animationTweener;

    public TMP_Text nameText;
    public HealthBar healthBar;
    public TMP_Text healthText;

    public int Health;
    public int MaxHealth;

    public TMP_Text consecutiveText;
    public int consecutiveWin;

    public TMP_Text scoreText;
    public int score;
    
    public TMP_Text Nickname { get => nameText;  }

    private void Start()
    {
        consecutiveWin = 0;
        Health = MaxHealth;
    }

    public void Reset()
    {
        if (chosenCard != null)
        {
            chosenCard.Reset();
        }
        chosenCard = null;
    }

    public Move? MoveValue
    {
        get => (chosenCard == null) ? null : chosenCard.move;
    }

    public void SetMoveCard(Card card)
    {
        if(chosenCard != null)
        {
            chosenCard.Reset();
        }

        chosenCard = card;
        //chosenCard.transform.DOScale(chosenCard.transform.localScale * 1.2f, 0.2f);
    }

    public void UpdateHealthUI()
    {
        healthText.text = Health.ToString() + "/" + MaxHealth.ToString();
        healthBar.UpdateBar((float)Health / MaxHealth);
    }
    
    public void ChangeHealth(int amount)
    {
        Health += amount;
        Health = Math.Clamp(Health, 0, MaxHealth);

        UpdateHealthUI();
    }
    public void UpdateConsecutiveWinUI()
    {
        consecutiveText.text = "Consecutive Win : " + consecutiveWin.ToString();
        if(consecutiveWin > 0)
            consecutiveText.text += " Score x" + Math.Round(Mathf.Pow(1.5f, consecutiveWin), 2);
    }

    public void ChangeConsecutiveWin(int amount)
    {
        consecutiveWin += amount;
        if (amount == -1) consecutiveWin = 0;

        UpdateConsecutiveWinUI();
    }

    public void UpdateScoreUI()
    {
        scoreText.text = "Score : " + score.ToString();
    }

    public void ChangeScore(int amount)
    {
        if (consecutiveWin > 1) amount = Mathf.RoundToInt(amount * Mathf.Pow(1.5f, consecutiveWin));
        score += amount;

        UpdateScoreUI();
    }

    public void AnimateMove()
    {
        animationTweener = chosenCard.transform.DOLocalMove(cardMovePos.localPosition, 1);
    }

    public void AnimateDamaged()
    {
        Image image = chosenCard.GetComponent<Image>();
        animationTweener = image.DOColor(Color.red, 0.1f).SetLoops(3, LoopType.Yoyo).SetDelay(0.2f);
    }

    public void AnimateDraw()
    {
        //Image image = chosenCard.GetComponent<Image>();
        //animationTweener = image.DOColor(Color.blue, 0.1f).SetLoops(3, LoopType.Yoyo).SetDelay(0.2f);
        animationTweener = chosenCard.transform.DOLocalMove(chosenCard.initialPosition, .5f).SetEase(Ease.InBack).SetDelay(0.1f);
    }

    public bool isAnimating()
    {
        return animationTweener.IsActive();
    }

    public void CanClick(bool value)
    {
        Card[] cards = GetComponentsInChildren<Card>();
        foreach(Card card in cards)
        {
            card.SetClickable(value);
        }
    }
}
