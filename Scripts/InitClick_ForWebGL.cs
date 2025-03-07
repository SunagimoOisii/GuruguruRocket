using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ブラウザの仕様の問題で、WebGL版ではゲームスタートでそのままサウンドが流れない。
/// そのため始めにプレイヤーにクリックさせるシーンをワンクッション設ける
/// </summary>
public class InitClick_ForWebGL : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            SceneManager.LoadScene("PlayScene");
        }
    }
}