using UnityEngine;
using UnityEngine.EventSystems;

public class Button_EscapeGame : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        //ƒQ[ƒ€I—¹ˆ—
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}