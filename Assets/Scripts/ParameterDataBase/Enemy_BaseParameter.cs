using DG.Tweening;
using UnityEngine;

/// <summary>
/// �G�̋��ʏ���(Collision,ColorChanger)�̃p�����[�^��񋟂���
/// </summary>
[CreateAssetMenu(fileName = "NewEnemyBaseParameter", menuName = "ParameterDataBase/Enemy_BaseParameter")]
public class Enemy_BaseParameter : ScriptableObject
{
    [Header("�ȉ�Enemy_ColorChanger�̐ݒ�")]
    [Header("�ʏ펞�̃J���[�ݒ�")]
    [SerializeField] private Color32  initColor = new(255, 75, 0, 255);
    [SerializeField] private Material initMaterial = null;
    public Color32  InitColor => initColor;
    public Material InitMaterial => initMaterial;

    [Header("���P�b�g���o���A������Ԏ��̃J���[�ݒ�")]
    [SerializeField] private Color32  weakColor    = new(77, 196, 255, 255);
    [SerializeField] private Material weakMaterial = null;
    public Color32  WeakColor => weakColor;
    public Material WeakMaterial => weakMaterial;



    [Header("�ȉ�Enemy_Collision�̐ݒ�")]
    [Header("�ӂ��Ƃѓx�����̐ݒ�")]
    [SerializeField] private float blowForce_min = 10f;
    [SerializeField] private float blowForce_max = 25f;
    [SerializeField] private float torque_min = -25f;
    [SerializeField] private float torque_max = 25f;
    public float BlowForceMin => blowForce_min;
    public float BlowForceMax => blowForce_max;
    public float TorqueMin => torque_min;
    public float TorqueMax => torque_max;

    [Header("�g�k�A�j���[�V�����̐ݒ�")]
    [SerializeField] private Ease  punch_easing    = Ease.OutSine;
    [SerializeField] private float punch_magnitude = 1.01f;
    [SerializeField] private float punch_duration  = 0.75f;
    [SerializeField] private int   punch_vibrato   = 15;
    public Ease  PunchEasing => punch_easing;
    public float PunchMagnitude => punch_magnitude;
    public float PunchDuration => punch_duration;
    public int   PunchVibrato => punch_vibrato;

    [Header("���j�X�R�A�{�[�i�X")]
    [SerializeField] private int bonus_defeat;
    public int BonusDefeat => bonus_defeat;

    [Header("���ꎞ�̃J�����Y�[��,�X���[�̐ݒ�")]
    [SerializeField] private float de_zoomIn           = 4.25f;
    [SerializeField] private float de_zoomDuration     = 0.2f;
    [SerializeField] private Ease  de_zoomEase         = Ease.OutCubic;
    [SerializeField] private float de_slowTimeScale    = 0.2f;
    [SerializeField] private int   de_slowTimeDuration = 750;
    public float DefeatZoomIn => de_zoomIn;
    public float DefeatZoomDuration => de_zoomDuration;
    public Ease  DefeatZoomEase => de_zoomEase;
    public float DefeatSlowTimeScale => de_slowTimeScale;
    public int   DefeatSlowTimeDuration => de_slowTimeDuration;
}