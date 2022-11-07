using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class AvatarSelector : MonoBehaviour
{
    public Sprite[] avatarSprites;
    public Image avatarImage;

    private int selectedIndex = 0;

    private void Start()
    {
        selectedIndex = PlayerPrefs.GetInt(PropertyNames.Player.AvatarIndex, -1);
        if(selectedIndex == -1)
        {
            selectedIndex = 0;
        }
        avatarImage.sprite = avatarSprites[selectedIndex];
        ShiftSelectedIndex(0);
    }

    public void ShiftSelectedIndex(int shift)
    {
        selectedIndex += shift;
        if(selectedIndex >= avatarSprites.Length)
            selectedIndex = 0;

        if(selectedIndex < 0)
            selectedIndex = avatarSprites.Length - 1;

        PlayerPrefs.SetInt(PropertyNames.Player.AvatarIndex, selectedIndex);

        Hashtable property = new Hashtable();
        property.Add(PropertyNames.Player.AvatarIndex, selectedIndex);
        PhotonNetwork.LocalPlayer.SetCustomProperties(property);
        avatarImage.sprite = avatarSprites[selectedIndex];
    }
}
