using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public CardPlayer player;
    public Move move;

    public Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 initialScale;
    Color originalColor;

    bool isClickable;

    private void Start()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.rotation;
        initialScale = transform.localScale;
        originalColor = GetComponent<Image>().color;
        isClickable = true;
    }

    public void Reset()
    {
        transform.localPosition = initialPosition;
        transform.localScale = initialScale;
        transform.rotation = initialRotation;
        GetComponent<Image>().color = originalColor;
    }

    public void Choose()
    {
        if (!isClickable) return;

        player.SetMoveCard(this);
    }

    public void SetClickable(bool canClick)
    {
        isClickable = canClick;
        GetComponent<Button>().enabled = canClick;
    }
}
