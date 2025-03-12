using UnityEngine;
using DG.Tweening;

public class Background : MonoBehaviour
{
    private SpriteRenderer sprRenderer;

    [Header("ゲーム本編でのアニメーション設定")]
    [SerializeField] private Color32 ingame_initColor  = new(255, 255, 255, 255);
    [SerializeField] private Color32 ingame_alterColor = new(90, 90, 90, 255);
    [SerializeField] private float   ingame_duration   = 1.5f;
    [Header("ゲームオーバー時のアニメーション設定")]
    [SerializeField] private Vector3 gameover_alterScale = new(0.6f, 0.6f, 1f);
    [SerializeField] private Ease    gameover_ease       = Ease.InOutBack;
    [SerializeField] private float   gameover_duration   = 0.5f;

    private void Start()
    {
        sprRenderer = GetComponent<SpriteRenderer>();
        AnimateColor_Ingame();
    }

    private void AnimateColor_Ingame()
    {
        sprRenderer.color = ingame_initColor;
        sprRenderer.DOColor(ingame_alterColor, ingame_duration)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void AnimateColor_Gameover()
    {
        DOTween.Kill(sprRenderer.color);
        sprRenderer.color = ingame_initColor;
        transform.DOScale(gameover_alterScale, gameover_duration).SetEase(gameover_ease);
    }
}