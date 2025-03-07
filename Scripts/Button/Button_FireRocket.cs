using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Button_FireRocket : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private SoundSystem soundSystem;
    [SerializeField] private SoundParameter param;

    [SerializeField] private Rocket rocket;
    [SerializeField] private SpriteRenderer rocketSprRenderer;

    private readonly OverlapDetector_2D overlapDetector = new();
    [SerializeField] private Canvas myCanvas;
    [SerializeField] private Image  myImage;
    private RectTransform myRectT;

    [Header("�J���[�ݒ�")]
    [SerializeField] private Color initColor;
    [SerializeField] private float buttonFadeAlpha;

    private bool isHolding = false;

    private void Start()
    {
        myRectT = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData ed)
    {
        isHolding = true;
        rocket.BeSmallScale();
        _ = soundSystem.SE.Play(param.AddressSERocketRotate,volume: 0.8f);
    }

    public void OnPointerUp(PointerEventData ed)
    {
        isHolding = false;
        rocket.BeInitScale();
        rocket.Move();
    }

    private void FixedUpdate()
    {
        if (isHolding) rocket.Rotate();

        //���P�b�g�Əd�Ȃ��Ă���Ƃ��̓{�^���𔖂��\������
        if (overlapDetector.DetectSpriteUIOverlap(rocketSprRenderer, myRectT, myCanvas))
        {
            myImage.color = new(initColor.r, initColor.b, initColor.g, buttonFadeAlpha);
        }
        else
        {
            myImage.color = initColor;
        }
    }
}