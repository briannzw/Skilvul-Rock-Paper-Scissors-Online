using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItem : MonoBehaviour
{
    public Image avatarImage;
    public TMP_Text playerText;
    public Sprite[] avatarSprites;

    public void Set(Photon.Realtime.Player player)
    {
        if(player.CustomProperties.TryGetValue(PropertyNames.Player.AvatarIndex, out var value))
        {
            avatarImage.sprite = avatarSprites[(int)value];
        }

        playerText.text = player.NickName;
        if (player == PhotonNetwork.MasterClient)
            playerText.text += " (Master)";
    }
}
