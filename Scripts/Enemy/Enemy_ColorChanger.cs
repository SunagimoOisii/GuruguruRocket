using UnityEngine;

public class Enemy_ColorChanger : MonoBehaviour
{
    [SerializeField] private Enemy_BaseParameter param;

    private SpriteRenderer sprRenderer;
    private TrailRenderer  traRenderer;

    private Rocket rocket;
    private readonly string rocketTag = "Rocket";

    private void Awake()
    {
        sprRenderer = GetComponent<SpriteRenderer>();
        traRenderer = GetComponent<TrailRenderer>();

        //ロケットのバリア装備イベントに関数を登録
        rocket = GameObject.FindWithTag(rocketTag).GetComponent<Rocket>();
        rocket.OnBarrierEquipped += BeWeakColor;
        rocket.OnBarrierStripped += BeInitColor;
        //スポーン前から装備している場合にも対応
        if (rocket.param.HasBarrier)
        {
            BeWeakColor();
        }
    }

    private void OnDestroy()
    {
        rocket.OnBarrierEquipped -= BeWeakColor;
        rocket.OnBarrierStripped -= BeInitColor;
    }

    private void BeInitColor()
    {
        sprRenderer.color    = param.InitColor;
        traRenderer.material = param.InitMaterial;
    }

    private void BeWeakColor()
    {
        sprRenderer.color    = param.WeakColor;
        traRenderer.material = param.WeakMaterial;
    }
}