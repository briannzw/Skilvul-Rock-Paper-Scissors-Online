using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomSettings : MonoBehaviourPunCallbacks
{
    public PropertySetting[] propertySettings;
    public Button applyButton;

    private void Start()
    {
        applyButton.interactable = PhotonNetwork.IsMasterClient;
        applyButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        propertySettings = GetComponentsInChildren<PropertySetting>();
    }

    public void ApplySettings()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        foreach (PropertySetting setting in propertySettings)
        {
            Hashtable property = new Hashtable();
            property.Add(setting.propertyKey, Mathf.RoundToInt(setting.slider.value));
            PhotonNetwork.CurrentRoom.SetCustomProperties(property);
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        applyButton.interactable = PhotonNetwork.IsMasterClient;
        applyButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
    }
}
