using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;

public class CameraEffects : MonoBehaviour
{
    [SerializeField] private Camera useCam;

    [Header("デバッグログを使用するかどうか")]
    [SerializeField] private bool isEnabledLogs = true;
    private enum LogLevel
    {
        Safe,
        Warning,
        Error
    }

    /// <summary>
    /// DebugクラスのLog,LogWarning,LogError関数をラップした関数<para></para>
    /// ログ有効化フラグをみて、ログを出すか否か適宜分岐するために実装
    /// </summary>
    private void Log(string message, LogLevel ll)
    {
        if (isEnabledLogs == false) return;
        
        //ログの出力
        switch(ll)
        {
            case LogLevel.Safe:
                Debug.Log(message);
                break;

            case LogLevel.Warning:
                Debug.LogWarning(message);
                break;

            case LogLevel.Error:
                Debug.LogError(message);
                break;
        }
    }

    public void SetCamera(Camera newCam) { useCam = newCam; }

    #region カメラ移動
    public bool IsMoving => moveCTS != null;
    private CancellationTokenSource moveCTS;

    public async UniTask BeginMove(Vector3 targetPos, float duration, Ease ease = Ease.Linear)
    {
        moveCTS?.Cancel(); //既存タスクをキャンセル
        moveCTS = new ();

        try
        {
            await Move(moveCTS.Token, targetPos, duration, ease);
        }
        finally
        {
            moveCTS?.Dispose();
            moveCTS = null;
        }
    }

    public void EndMove()
    {
        if (moveCTS == null) return;

        moveCTS.Cancel();
        moveCTS.Dispose();
        moveCTS = null;
        Log("カメラ移動処理終了", LogLevel.Safe);
    }

    private async UniTask Move(CancellationToken token,
        Vector3 targetPos, float duration, Ease ease)
    {
        Vector3 startPos = transform.position;
        float elapsed = 0;

        Log($"カメラ移動開始　目標座標:{targetPos}", LogLevel.Safe);
        while (token.IsCancellationRequested == false && 
               elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float easedT = DOVirtual.EasedValue(0, 1, elapsed / duration, ease);

            transform.position = Vector3.Lerp(startPos, targetPos, easedT);
            Debug.Log(transform.position);
            await UniTask.Yield();
        }

        if (token.IsCancellationRequested) return;

        //最後に正確な位置を設定
        transform.position = targetPos;
        Log("カメラ移動完了", LogLevel.Safe);
    }
    #endregion

    #region ターゲット追跡
    public bool IsFollowing => followCTS != null;
    private CancellationTokenSource followCTS;

    public UniTask BeginFollow(Transform target, float speed, Vector3 offset = default)
    {
        //ターゲット追跡の開始(既に追跡処理が行われている場合、それは終了させる)
        followCTS?.Cancel();
        followCTS = new();
        return FollowTarget(followCTS.Token, target, speed, offset);
    }
    public void EndFollow()
    {
        if (followCTS == null) return;

        followCTS?.Cancel();
        followCTS = null;
        Log("ターゲット追跡を終了", LogLevel.Safe);
    }

    private async UniTask FollowTarget(CancellationToken token,
        Transform target, float speed, Vector3 offset)
    {
        Log(target + "の追跡を開始", LogLevel.Safe);
        while (token.IsCancellationRequested == false)
        {
            if (target == null)
            {
                Log("追跡対象がnullのため追従を終了", LogLevel.Warning);
                break;
            }

            //カメラ座標をターゲットに追従
            Vector3 desiredPos  = target.position + offset;
            Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, speed * Time.unscaledDeltaTime);
            transform.position  = smoothedPos;

            await UniTask.Yield();
        }
    }
    #endregion

    #region カメラズーム
    public bool IsZooming => zoomCTS != null;
    private CancellationTokenSource zoomCTS;

    public async UniTask BeginZoom(float targetZoom, float duration, Ease ease = Ease.Linear)
    {
        //既存タスクをキャンセル
        zoomCTS?.Cancel();
        zoomCTS = new();

        try
        {
            await Zoom(zoomCTS.Token, targetZoom, duration, ease);
        }
        finally
        {
            zoomCTS?.Dispose();
            zoomCTS = null;
        }
    }

    public void EndZoom()
    {
        if (zoomCTS == null) return;

        zoomCTS.Cancel();
        zoomCTS.Dispose();
        zoomCTS = null;
        Log("ズーム処理終了", LogLevel.Safe);
    }

    private async UniTask Zoom(CancellationToken token,
        float targetZoom, float duration, Ease ease)
    {
        float startZoom;
        if (useCam.orthographic) startZoom = useCam.orthographicSize;
        else                     startZoom = useCam.fieldOfView;

        //ズーム処理
        float elapsed = 0;
        Log($"ズーム開始  目標:{targetZoom}", LogLevel.Safe);
        while (token.IsCancellationRequested == false &&
               elapsed < duration) 
        {
            elapsed += Time.unscaledDeltaTime;
            float easedT = DOVirtual.EasedValue(0, 1, elapsed / duration, ease);

            if (useCam.orthographic)
            {
                useCam.orthographicSize = Mathf.Lerp(startZoom, targetZoom, easedT);
            }
            else
            {
                useCam.fieldOfView = Mathf.Lerp(startZoom, targetZoom, easedT);
            }

            await UniTask.Yield();
        }

        if (token.IsCancellationRequested) return;
        
        //最後にズーム度合いを明示的に設定(補間のズレを防ぐ)
        if (useCam.orthographic) useCam.orthographicSize = targetZoom;
        else                     useCam.fieldOfView      = targetZoom;
        Log("ズーム処理完了", LogLevel.Safe);
    }
    #endregion

    #region カメラ揺れ
    public bool IsShaking => shakeCTS != null;
    private CancellationTokenSource shakeCTS;

    public async UniTask BeginShake(float duration = 0.5f,
        float strength = 1f, int vibrato = 10, float randomness = 90f)
    {
        //カメラ揺れの開始(既に処理が行われている場合、それは終了させる)
        shakeCTS?.Cancel();
        shakeCTS = new();

        try
        {
            await ShakeCamera(shakeCTS.Token, duration, strength, vibrato, randomness);
        }
        finally 
        {
            shakeCTS?.Cancel();
            shakeCTS = null;
        }
    }

    public void EndShake()
    {
        if (shakeCTS == null) return;

        shakeCTS.Cancel();
        shakeCTS.Dispose();
        shakeCTS = null;
        Log("カメラ揺れ終了", LogLevel.Safe);
    }

    private async UniTask ShakeCamera(CancellationToken token,
        float duration, float strength, int vibrato, float randomness)
    {
        Log("カメラ揺れ開始", LogLevel.Safe);
        var shakeTween = useCam.transform.DOShakePosition(duration, strength, vibrato, randomness);
        float elapsed = 0;
        while (token.IsCancellationRequested == false &&
               elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            await UniTask.Yield();
        }

        //カメラ揺れ終了
        if (token.IsCancellationRequested)
        {
            shakeTween.Kill();
            Log("カメラ揺れを中断", LogLevel.Safe);
        }
        else
        {
            Log("カメラ揺れ完了", LogLevel.Safe);
        }
    }
    #endregion

    #region 座標範囲制限
    private Vector3 minPos;
    private Vector3 maxPos;

    public void SetClampRange(Vector3 min, Vector3 max)
    {
        if (min.x > max.x ||
            min.y > max.y ||
            min.z > max.z)
        {
            Log("ClampRangeの設定が不正(maxがmin以下になっている)。　"
                + $"min({min.x}, {min.y},{min.z}) > max({max.x}, {max.y},{max.z})",
                LogLevel.Warning);
            return;
        }
        minPos = min;
        maxPos = max;
    }

    public void ClampPosition()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minPos.x, maxPos.x);
        pos.y = Mathf.Clamp(pos.y, minPos.y, maxPos.y);
        pos.z = Mathf.Clamp(pos.z, minPos.z, maxPos.z);
        transform.position = pos;
    }
    #endregion
}