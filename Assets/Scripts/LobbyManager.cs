using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField roomNameInputField;
    public TMP_Text feedbackText;
    public GameObject roomPanel;
    public TMP_Text roomText;
    public GameObject roomListObject;
    public GameObject playerListObject;
    public RoomItem roomItemPrefab;
    public PlayerItem playerItemPrefab;
    public Button startGameButton;

    List<RoomItem> roomItemList = new List<RoomItem>();
    List<PlayerItem> playerItemList = new List<PlayerItem>();

    Dictionary<string, RoomInfo> roomInfoCache = new Dictionary<string, RoomInfo>();

    private void Start()
    {
        PhotonNetwork.JoinLobby();
    }

    public void CreateNewRoom()
    {
        if(roomNameInputField.text.Length < 3)
        {
            feedbackText.text = "Room Name min 3 characters";
            return;
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 5;
        PhotonNetwork.CreateRoom(roomNameInputField.text, roomOptions);
    }

    public void StartGame(string levelName)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel(levelName);
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void LeaveRoom()
    {
        StartCoroutine(LeaveRoomCR());
    }

    IEnumerator LeaveRoomCR()
    {
        PhotonNetwork.LeaveRoom();
        while (PhotonNetwork.InRoom || !PhotonNetwork.IsConnectedAndReady)
        {
            yield return null;
        }
        PhotonNetwork.JoinLobby();
        roomPanel.SetActive(false);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room Created - " + PhotonNetwork.CurrentRoom.Name);
        feedbackText.text = "Room Created - " + PhotonNetwork.CurrentRoom.Name;
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Create Room Failed - " + returnCode + ", " + message);
        feedbackText.text = "Create Room Failed - " + returnCode.ToString() + ", " + message;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Room Joined - " + PhotonNetwork.CurrentRoom.Name);
        feedbackText.text = "Room Joined - " + PhotonNetwork.CurrentRoom.Name;
        roomText.text = PhotonNetwork.CurrentRoom.Name;
        roomPanel.SetActive(true);

        // Update Player List
        UpdatePlayerList();
        SetStartGameButton();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        SetStartGameButton();
    }

    private void SetStartGameButton()
    {
        startGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);

        startGameButton.interactable = PhotonNetwork.CurrentRoom.PlayerCount > 1;
    }

    public void UpdatePlayerList()
    {
        foreach(PlayerItem item in playerItemList)
        {
            Destroy(item.gameObject);
        }

        playerItemList.Clear();

        foreach(var (id, player) in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerItem newPlayerItem = Instantiate(playerItemPrefab, playerListObject.transform);
            newPlayerItem.Set(player);

            playerItemList.Add(newPlayerItem);

            if (player == PhotonNetwork.LocalPlayer)
            {
                newPlayerItem.transform.SetAsFirstSibling();
            }
        }

        SetStartGameButton();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(RoomInfo roomInfo in roomList)
        {
            roomInfoCache[roomInfo.Name] = roomInfo;
        }

        foreach(RoomItem item in roomItemList)
        {
            Destroy(item.gameObject);
        }

        roomItemList.Clear();

        List<RoomInfo> roomInfoList = new List<RoomInfo>(roomInfoCache.Count);
        foreach(RoomInfo roomInfo in roomInfoCache.Values)
        {
            if (roomInfo.IsOpen) roomInfoList.Add(roomInfo);
        }
        foreach (RoomInfo roomInfo in roomInfoCache.Values)
        {
            if (!roomInfo.IsOpen) roomInfoList.Add(roomInfo);
        }


        foreach (RoomInfo roomInfo in roomInfoList)
        {
            if (!roomInfo.IsVisible || roomInfo.PlayerCount == 0) continue;

            var newRoomItem = Instantiate(roomItemPrefab, roomListObject.transform);
            newRoomItem.Set(this, roomInfo);
            roomItemList.Add(newRoomItem);
        }
    }
}
