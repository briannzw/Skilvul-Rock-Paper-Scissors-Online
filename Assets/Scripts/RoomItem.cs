using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    public TMP_Text roomNameText;
    public Button button;
    
    LobbyManager manager;

    private string roomName;

    public void Set(LobbyManager _manager, RoomInfo roomInfo)
    {
        manager = _manager;
        roomNameText.text = roomInfo.Name + $" ({roomInfo.PlayerCount}/{roomInfo.MaxPlayers})";
        roomName = roomInfo.Name;
        button.interactable = roomInfo.IsOpen;
    }

    public void JoinRoom()
    {
        manager.JoinRoom(roomName);
    }
}
