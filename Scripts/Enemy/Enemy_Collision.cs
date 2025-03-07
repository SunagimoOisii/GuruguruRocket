using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// 敵キャラ共通の当たり判定とやられ処理を実装<para></para>
/// - シーン内のカメラが１つのみの前提
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

        //カメラの諸データ取得(初回のみ)
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
        //ロケットとの衝突以外は何もしない
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
    /// やられ時、画面外に吹っ飛んでいく演出
    /// </summary>
    private void BlowAway()
    {
        //やられた瞬間からコライダーは無効にする
        isDead = true;
        var cols = GetComponents<Collider2D>();
        foreach (var col in cols)
        {
            col.enabled = false;
        }

        //吹っ飛ぶ力,方向を計算
        float force = Random.Range(param.BlowForceMin, param.BlowForceMin);
        float angle = Random.Range(0f, 360f);
        Vector3 dir = new(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0f);

        //吹っ飛ぶときの回転力を計算(Z軸)
        float torque = Random.Range(param.TorqueMin, param.TorqueMax);

        //計算結果の反映
        rb.AddForce(dir * force, ForceMode2D.Impulse);
        rb.AddTorque(torque, ForceMode2D.Impulse);

        //拡縮アニメーション実行
        transform.DOPunchScale(Vector3.one * param.PunchMagnitude, param.PunchMagnitude, param.PunchVibrato)
            .SetEase(param.PunchEasing);
    }

    private async UniTaskVoid ExecuteDefeatEffect()
    {
        //スローモーションと共にカメラのズームイン,アウトを行う

        Time.timeScale = param.DefeatSlowTimeScale;
        cam_effects.BeginZoom(param.DefeatZoomIn, param.DefeatZoomDuration, param.DefeatZoomEase).Forget();

        await UniTask.Delay(param.DefeatSlowTimeDuration, true);

        cam_effects.BeginZoom(cam_initZoom, param.DefeatZoomDuration, param.DefeatZoomEase).Forget();
        Time.timeScale = 1f;
    }
}