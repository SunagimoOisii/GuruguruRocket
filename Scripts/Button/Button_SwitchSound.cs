using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// BGM,SEの有効化,無効化を行う
/// </summary>
public class Button_SwitchSound : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private SoundSystem soundSystem;
    [SerializeField] private Image iconImage;
    [SerializeField] private bool isBGMSwitcher = false;
    private bool canPlaySound = true;

    [Header("有効,無効状態でのアイコンの色設定")]
    [SerializeField] private Color32 color_on  = Color.black;
    [SerializeField] private Color32 color_off = Color.gray;

    public void OnPointerDown(PointerEventData ed)
    {
        //パラメータ名と設定ボリュームを決定
        //また、有効無効に応じてアイコンの色を変える
        string paramName;
        if (isBGMSwitcher) paramName = "Volume_BGM";
        else               paramName = "Volume_SE";

        float setVolume;
        if (canPlaySound)
        {
            setVolume = -80;
            iconImage.color = color_off;
        }
        else
        {
            setVolume = 0f;
            iconImage.color = color_on;
        }
        canPlaySound = !canPlaySound;

        soundSystem.Effector.SetMixerParameter(paramName, setVolume);
    }
}