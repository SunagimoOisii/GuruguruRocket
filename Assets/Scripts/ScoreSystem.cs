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

    [Header("�e�L�X�g�J���[�ݒ�")]
    [SerializeField] private Color initTextColor;
    [SerializeField] private float textFadeAlpha;

    public enum TextAnimScale
    {
        Small,
        Medium,
        Large
    }
    private TextAnimScale currentAnimSize = TextAnimScale.Small;
    [Header("�e�L�X�g�g�k�A�j���[�V�����ݒ�")]
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
            Debug.LogError("ScoreCounter���V�[����ɕ������݂��Ă���");
            Destroy(instance.gameObject);
        }

        Score = 0;
        scoreText.text = Score.ToString("D7");
    }

    private void FixedUpdate()
    {
        //���P�b�g�Əd�Ȃ��Ă���Ƃ��̓e�L�X�g�𔖂��\������
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
        //�X�R�A���f
        Score += value;
        if (Score > 9999999)
        {
            Score = 9999999;
        }
        if (Score <= 0)
        {
            Score = 0;
        }

        //�X�R�A�e�L�X�g���f�A�A�j���[�V����
        scoreText.text = Score.ToString("D7");
        AnimateScoreText(animSize);
    }

    private void AnimateScoreText(TextAnimScale animSize)
    {
        //���ݎ��s���̃A�j���[�V�����T�C�Y��菬������΃X�L�b�v
        //�傫����Ύ��s���A�j���[�V�������㏑�����Ď��s����
        if (animSize < currentAnimSize) return;
        
        //�A�j���[�V�������e�̐ݒ�
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

        //�e�L�X�g�g�k�A�j���[�V�������s
        currentAnimSize = animSize;
        scoreText.rectTransform.DOScale(scaleMultiplier, duration)
            .OnComplete(() =>
            {
                scoreText.rectTransform.DOScale(1f, duration)
                    .OnComplete(() =>
                    {
                        //���S�ɏI�������猻�݂̃A�j���[�V�����T�C�Y�����Z�b�g
                        currentAnimSize = TextAnimScale.Small;
                    });
            });
    }
}