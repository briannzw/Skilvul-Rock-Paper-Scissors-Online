using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PropertySetting : MonoBehaviourPunCallbacks
{
    public Slider slider;
    public TMP_InputField inputField;
    public string propertyKey;

    public int initialValue = 10;
    public int minValue = 0;
    public int maxValue = 100;

    private void Start()
    {
        slider.interactable = PhotonNetwork.IsMasterClient;
        inputField.interactable = PhotonNetwork.IsMasterClient;

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(propertyKey, out var value))
        {
            UpdateSliderInputValue((int)value);
        }
        else
        {
            UpdateSliderInputValue(initialValue);

            SetCustomPropertyToServer(initialValue);
        }

        slider.minValue = minValue;
        slider.maxValue = maxValue;
    }

    public void InputFromSlider(float value)
    {
        UpdateSliderInputValue(value);
    }

    public void InputFromField(string value)
    {
        if (int.TryParse(value, out var intValue))
        {
            intValue = Mathf.Clamp(intValue, minValue, maxValue);
            UpdateSliderInputValue(intValue);
            SetCustomPropertyToServer(intValue);
        }
    }

    private void SetCustomPropertyToServer(int value)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Hashtable property = new Hashtable();
        property.Add(propertyKey, value);
        PhotonNetwork.CurrentRoom.SetCustomProperties(property);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        slider.interactable = PhotonNetwork.IsMasterClient;
        inputField.interactable = PhotonNetwork.IsMasterClient;
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if(propertiesThatChanged.TryGetValue(propertyKey, out var value))
        {
            UpdateSliderInputValue((int)value);
        }
    }

    private void UpdateSliderInputValue(float value)
    {
        slider.value = value;
        inputField.text = (Mathf.RoundToInt(value)).ToString("D");
    }
}
