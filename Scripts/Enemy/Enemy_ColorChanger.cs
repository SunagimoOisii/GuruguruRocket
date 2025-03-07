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

        //���P�b�g�̃o���A�����C�x���g�Ɋ֐���o�^
        rocket = GameObject.FindWithTag(rocketTag).GetComponent<Rocket>();
        rocket.OnBarrierEquipped += BeWeakColor;
        rocket.OnBarrierStripped += BeInitColor;
        //�X�|�[���O���瑕�����Ă���ꍇ�ɂ��Ή�
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