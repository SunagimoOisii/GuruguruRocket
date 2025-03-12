using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "NewRocketParameter", menuName = "ParameterDataBase/RocketParameter")]
public class RocketParameter : ScriptableObject
{
    [HideInInspector] public bool IsDead { get; set; } = false;
    [HideInInspector] public bool HasBarrier { get; set; } = false;

    [Header("スポーン設定(Ingame遷移時始めのスポーン)")]
    [SerializeField] private Vector3 spawn_scale    = new(3.5f, 3.5f, 1);
    [SerializeField] private Ease    spawn_ease     = Ease.Linear;
    [SerializeField] private float   spawn_duration = 0.55f;
    public Vector3 SpawnScale => spawn_scale;
    public Ease    SpawnEase => spawn_ease;
    public float   SpawnDuration => spawn_duration;

    [Header("移動に関する設定")]
    [SerializeField] private float   rot_speed            = -270f;
    [SerializeField] private Vector3 rot_initScale        = new(3.5f, 3.5f, 1);
    [SerializeField] private float   rot_scale            = 3.25f;
    [SerializeField] private float   rot_scalingDuration  = 0.2f;
    [SerializeField] private Ease    rot_scalingSmallEase = Ease.OutBack;
    [SerializeField] private Ease    rot_scalingInitEase  = Ease.OutBounce;
    [SerializeField] private float   move_force = 40f;
    [SerializeField] private Vector2 minPos = new(-8.7f, -4.85f);
    [SerializeField] private Vector2 maxPos = new(8.7f, 4.85f);
    public float   RotationSpeed => rot_speed;
    public Vector3 RotationInitialScale => rot_initScale;
    public float   RotationScale => rot_scale;
    public float   RotationScalingDuration => rot_scalingDuration;
    public Ease    RotationScalingSmallEase => rot_scalingSmallEase;
    public Ease    RotationScalingInitEase => rot_scalingInitEase;
    public float   MoveForce => move_force;
    public Vector2 MinPosition => minPos;
    public Vector2 MaxPosition => maxPos;

    [Header("やられ時に関する設定")]
    [SerializeField] private GameObject dead_particlePrefab;
    [SerializeField] private float      dead_slowTimeScale;
    [SerializeField] private float      dead_slowTimeDuration;
    public GameObject DeadParticlePrefab => dead_particlePrefab;
    public float      DeadSlowTimeScale => dead_slowTimeScale;
    public float      DeadSlowTimeDuration => dead_slowTimeDuration;

    [Header("ロケット移動時のカメラ揺れ設定")]
    [SerializeField] private float camShake_second   = 0.5f;
    [SerializeField] private float camShake_strength = 0.1f;
    [SerializeField] private int   camShake_vibrato  = 35;
    public float CameraShakeDuration => camShake_second;
    public float CameraShakeStrength => camShake_strength;
    public int CameraShakeVibrato => camShake_vibrato;

    [Header("スコアボーナス設定")]
    [SerializeField] private int bonus_survive = 10;
    [SerializeField] private int bonus_move    = 50;
    public int BonusSurvive => bonus_survive;
    public int BonusMove => bonus_move;

    [Header("バリア設定")]
    [SerializeField] private float barrier_duration      = 15f;
    [SerializeField] private float barrier_blinkTime     = 3f;
    [SerializeField] private float barrier_blinkInterval = 0.1f;
    public float BarrierDuration => barrier_duration;
    public float BarrierBlinkTime => barrier_blinkTime;
    public float BarrierBlinkInterval => barrier_blinkInterval;
}