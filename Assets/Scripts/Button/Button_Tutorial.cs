using UnityEngine;
using UnityEngine.EventSystems;

public class Button_Tutorial : MonoBehaviour,IPointerDownHandler
{
    [SerializeField] private SoundSystem soundSystem;
    [SerializeField] private SoundParameter param;

    [SerializeField] private GameObject tutorialImagesObj;
    private static bool isTutorialShowed = false;

    public void OnPointerDown(PointerEventData ed)
    {
        if(isTutorialShowed) tutorialImagesObj.SetActive(false);
        else                 tutorialImagesObj.SetActive(true);
        isTutorialShowed = !isTutorialShowed;

        _ = soundSystem.SE.Play(param.AddressSEButtonSelected);
    }
}