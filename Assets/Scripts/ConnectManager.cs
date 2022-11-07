using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField usernameInput;
    public TMP_Text feedbackText;

    private void Start()
    {
        usernameInput.text = PlayerPrefs.GetString(PropertyNames.Player.Nickname, "");
    }

    public void Connect()
    {
        feedbackText.text = "";

        if(usernameInput.text.Length < 3)
        {
            feedbackText.text = "Username min 3 characters";
            return;
        }

        PlayerPrefs.SetString(PropertyNames.Player.Nickname, usernameInput.text);
        PhotonNetwork.NickName = usernameInput.text;
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
        feedbackText.text = "Connecting...";
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Connected to Master");
        feedbackText.text = "Connected to Master";
        StartCoroutine(LoadLevelAfterConnectedAndReady());
    }

    IEnumerator LoadLevelAfterConnectedAndReady()
    {
        while (!PhotonNetwork.IsConnectedAndReady)
        {
            yield return null;
        }
        SceneManager.LoadScene("Lobby");
    }
}
