using UnityEngine;
using UnityEngine.EventSystems;

public class Button_StartIngame : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private SoundSystem soundSystem;
    [SerializeField] private SoundParameter param;

    public void OnPointerDown(PointerEventData ed)
    {
        GameStateSystem.instance.ChangeState(GameStateSystem.State.Ingame);
        _ = soundSystem.SE.Play(param.AddressSEButtonSelected);
    }
}