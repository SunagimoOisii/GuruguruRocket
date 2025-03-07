using UnityEngine;
using TMPro;
using DG.Tweening;

public class ScoreSystem : MonoBehaviour
{
    public static ScoreSystem instance;

    public int Score { get; private set; } = 0;
    [SerializeField] private TextMeshProUGUI scoreText;

    [SerializeField] private SpriteRenderer rocketSprRenderer;
    [SerializeField] private Canvas myCanvas;
    private readonly OverlapDetector_2D overlapDetector = new();
    private RectTransform myRectT;

    [Header("テキストカラー設定")]
    [SerializeField] private Color initTextColor;
    [SerializeField] private float textFadeAlpha;

    public enum TextAnimScale
    {
        Small,
        Medium,
        Large
    }
    private TextAnimScale currentAnimSize = TextAnimScale.Small;
    [Header("テキスト拡縮アニメーション設定")]
    [SerializeField] private float animScale_small  = 1.2f;
    [SerializeField] private float animScale_medium = 1.5f;
    [SerializeField] private float animScale_large  = 2f;
    [SerializeField] private float duration_small   = 0.15f;
    [SerializeField] private float duration_medium  = 0.2f;
    [SerializeField] private float duration_large   = 0.25f;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            myRectT = GetComponent<RectTransform>();
        }
        else
        {
            Debug.LogError("ScoreCounterがシーン上に複数存在している");
            Destroy(instance.gameObject);
        }

        Score = 0;
        scoreText.text = Score.ToString("D7");
    }

    private void FixedUpdate()
    {
        //ロケットと重なっているときはテキストを薄く表示する
        if (overlapDetector.DetectSpriteUIOverlap(rocketSprRenderer, myRectT, myCanvas))
        {
            scoreText.color = new(initTextColor.r, initTextColor.b, initTextColor.g, textFadeAlpha);
        }
        else
        {
            scoreText.color = initTextColor;
        }
    }

    public void InitScore()
    {
        Score = 0;
        scoreText.text = Score.ToString("D7");
    }

    public void AddScore(int value, TextAnimScale animSize)
    {
        //スコア反映
        Score += value;
        if (Score > 9999999)
        {
            Score = 9999999;
        }
        if (Score <= 0)
        {
            Score = 0;
        }

        //スコアテキスト反映、アニメーション
        scoreText.text = Score.ToString("D7");
        AnimateScoreText(animSize);
    }

    private void AnimateScoreText(TextAnimScale animSize)
    {
        //現在実行中のアニメーションサイズより小さければスキップ
        //大きければ実行中アニメーションを上書きして実行する
        if (animSize < currentAnimSize) return;
        
        //アニメーション内容の設定
        float scaleMultiplier = 0f;
        float duration        = 0f;
        switch (animSize)
        {
            case TextAnimScale.Small:
                scaleMultiplier = animScale_small;
                duration        = duration_small;
                break;

            case TextAnimScale.Medium:
                scaleMultiplier = animScale_medium;
                duration        = duration_medium;
                break;

            case TextAnimScale.Large:
                scaleMultiplier = animScale_large;
                duration        = duration_large;
                break;
        }

        //テキスト拡縮アニメーション実行
        currentAnimSize = animSize;
        scoreText.rectTransform.DOScale(scaleMultiplier, duration)
            .OnComplete(() =>
            {
                scoreText.rectTransform.DOScale(1f, duration)
                    .OnComplete(() =>
                    {
                        //完全に終了したら現在のアニメーションサイズをリセット
                        currentAnimSize = TextAnimScale.Small;
                    });
            });
    }
}