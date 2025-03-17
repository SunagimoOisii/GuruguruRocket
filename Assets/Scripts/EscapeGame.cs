using UnityEngine;

public class EscapeGame : MonoBehaviour
{
    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //ゲーム終了処理
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
        }
    }
}
