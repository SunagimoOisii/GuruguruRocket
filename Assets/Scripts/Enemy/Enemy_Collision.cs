using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// �G�L�������ʂ̓����蔻��Ƃ��ꏈ��������<para></para>
/// - �V�[�����̃J�������P�݂̂̑O��
/// </summary>
public class Enemy_Collision : MonoBehaviour, ICollide
{
    [SerializeField] private Enemy_BaseParameter param;

    [SerializeField] private SoundParameter soundParam;
    private SoundSystem soundSystem;
    private string soundSystemTag = "SoundSystem";
    private static CameraEffects cam_effects;
    private static float cam_initZoom;
    private static bool isCamInitDataScanned = false;

    private Rigidbody2D rb;
    private bool isDead = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        soundSystem = GameObject.FindWithTag(soundSystemTag).GetComponent<SoundSystem>();

        //�J�����̏��f�[�^�擾(����̂�)
        if (isCamInitDataScanned == false)
        {
            var cam = Camera.main;
            cam_effects = cam.gameObject.GetComponent<CameraEffects>();
            cam_initZoom = cam.orthographicSize;
            isCamInitDataScanned = true;
        }
    }

    private void OnBecameInvisible()
    {
        if (isDead)
        {
            Destroy(GetComponent<TrailRenderer>());
            Destroy(gameObject);
        }
    }

    public void OnCollide(params object[] args)
    {
        //���P�b�g�Ƃ̏ՓˈȊO�͉������Ȃ�
        foreach (var arg in args)
        {
            if (arg is Rocket rocket)
            {
                if (rocket.param.HasBarrier)
                {
                    BlowAway();
                    ExecuteDefeatEffect().Forget();
                    ScoreSystem.instance.AddScore(param.BonusDefeat, ScoreSystem.TextAnimScale.Large);
                    _ = soundSystem.SE.Play(soundParam.AddressSEEnemyDead);
                    return;
                }

                rocket.BeDead().Forget();
            }
        }
    }

    /// <summary>
    /// ���ꎞ�A��ʊO�ɐ������ł������o
    /// </summary>
    private void BlowAway()
    {
        //���ꂽ�u�Ԃ���R���C�_�[�͖����ɂ���
        isDead = true;
        var cols = GetComponents<Collider2D>();
        foreach (var col in cols)
        {
            col.enabled = false;
        }

        //������ԗ�,�������v�Z
        float force = Random.Range(param.BlowForceMin, param.BlowForceMin);
        float angle = Random.Range(0f, 360f);
        Vector3 dir = new(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0f);

        //������ԂƂ��̉�]�͂��v�Z(Z��)
        float torque = Random.Range(param.TorqueMin, param.TorqueMax);

        //�v�Z���ʂ̔��f
        rb.AddForce(dir * force, ForceMode2D.Impulse);
        rb.AddTorque(torque, ForceMode2D.Impulse);

        //�g�k�A�j���[�V�������s
        transform.DOPunchScale(Vector3.one * param.PunchMagnitude, param.PunchMagnitude, param.PunchVibrato)
            .SetEase(param.PunchEasing);
    }

    private async UniTaskVoid ExecuteDefeatEffect()
    {
        //�X���[���[�V�����Ƌ��ɃJ�����̃Y�[���C��,�A�E�g���s��

        Time.timeScale = param.DefeatSlowTimeScale;
        cam_effects.BeginZoom(param.DefeatZoomIn, param.DefeatZoomDuration, param.DefeatZoomEase).Forget();

        await UniTask.Delay(param.DefeatSlowTimeDuration, true);

        cam_effects.BeginZoom(cam_initZoom, param.DefeatZoomDuration, param.DefeatZoomEase).Forget();
        Time.timeScale = 1f;
    }
}