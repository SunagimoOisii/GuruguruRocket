using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// BGM,SE�̗L����,���������s��
/// </summary>
public class Button_SwitchSound : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private SoundSystem soundSystem;
    [SerializeField] private Image iconImage;
    [SerializeField] private bool isBGMSwitcher = false;
    private bool canPlaySound = true;

    [Header("�L��,������Ԃł̃A�C�R���̐F�ݒ�")]
    [SerializeField] private Color32 color_on  = Color.black;
    [SerializeField] private Color32 color_off = Color.gray;

    public void OnPointerDown(PointerEventData ed)
    {
        //�p�����[�^���Ɛݒ�{�����[��������
        //�܂��A�L�������ɉ����ăA�C�R���̐F��ς���
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