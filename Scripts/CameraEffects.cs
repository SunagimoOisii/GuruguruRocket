using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;

public class CameraEffects : MonoBehaviour
{
    [SerializeField] private Camera useCam;

    [Header("�f�o�b�O���O���g�p���邩�ǂ���")]
    [SerializeField] private bool isEnabledLogs = true;
    private enum LogLevel
    {
        Safe,
        Warning,
        Error
    }

    /// <summary>
    /// Debug�N���X��Log,LogWarning,LogError�֐������b�v�����֐�<para></para>
    /// ���O�L�����t���O���݂āA���O���o�����ۂ��K�X���򂷂邽�߂Ɏ���
    /// </summary>
    private void Log(string message, LogLevel ll)
    {
        if (isEnabledLogs == false) return;
        
        //���O�̏o��
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

    #region �J�����ړ�
    public bool IsMoving => moveCTS != null;
    private CancellationTokenSource moveCTS;

    public async UniTask BeginMove(Vector3 targetPos, float duration, Ease ease = Ease.Linear)
    {
        moveCTS?.Cancel(); //�����^�X�N���L�����Z��
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
        Log("�J�����ړ������I��", LogLevel.Safe);
    }

    private async UniTask Move(CancellationToken token,
        Vector3 targetPos, float duration, Ease ease)
    {
        Vector3 startPos = transform.position;
        float elapsed = 0;

        Log($"�J�����ړ��J�n�@�ڕW���W:{targetPos}", LogLevel.Safe);
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

        //�Ō�ɐ��m�Ȉʒu��ݒ�
        transform.position = targetPos;
        Log("�J�����ړ�����", LogLevel.Safe);
    }
    #endregion

    #region �^�[�Q�b�g�ǐ�
    public bool IsFollowing => followCTS != null;
    private CancellationTokenSource followCTS;

    public UniTask BeginFollow(Transform target, float speed, Vector3 offset = default)
    {
        //�^�[�Q�b�g�ǐՂ̊J�n(���ɒǐՏ������s���Ă���ꍇ�A����͏I��������)
        followCTS?.Cancel();
        followCTS = new();
        return FollowTarget(followCTS.Token, target, speed, offset);
    }
    public void EndFollow()
    {
        if (followCTS == null) return;

        followCTS?.Cancel();
        followCTS = null;
        Log("�^�[�Q�b�g�ǐՂ��I��", LogLevel.Safe);
    }

    private async UniTask FollowTarget(CancellationToken token,
        Transform target, float speed, Vector3 offset)
    {
        Log(target + "�̒ǐՂ��J�n", LogLevel.Safe);
        while (token.IsCancellationRequested == false)
        {
            if (target == null)
            {
                Log("�ǐՑΏۂ�null�̂��ߒǏ]���I��", LogLevel.Warning);
                break;
            }

            //�J�������W���^�[�Q�b�g�ɒǏ]
            Vector3 desiredPos  = target.position + offset;
            Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, speed * Time.unscaledDeltaTime);
            transform.position  = smoothedPos;

            await UniTask.Yield();
        }
    }
    #endregion

    #region �J�����Y�[��
    public bool IsZooming => zoomCTS != null;
    private CancellationTokenSource zoomCTS;

    public async UniTask BeginZoom(float targetZoom, float duration, Ease ease = Ease.Linear)
    {
        //�����^�X�N���L�����Z��
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
        Log("�Y�[�������I��", LogLevel.Safe);
    }

    private async UniTask Zoom(CancellationToken token,
        float targetZoom, float duration, Ease ease)
    {
        float startZoom;
        if (useCam.orthographic) startZoom = useCam.orthographicSize;
        else                     startZoom = useCam.fieldOfView;

        //�Y�[������
        float elapsed = 0;
        Log($"�Y�[���J�n  �ڕW:{targetZoom}", LogLevel.Safe);
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
        
        //�Ō�ɃY�[���x�����𖾎��I�ɐݒ�(��Ԃ̃Y����h��)
        if (useCam.orthographic) useCam.orthographicSize = targetZoom;
        else                     useCam.fieldOfView      = targetZoom;
        Log("�Y�[����������", LogLevel.Safe);
    }
    #endregion

    #region �J�����h��
    public bool IsShaking => shakeCTS != null;
    private CancellationTokenSource shakeCTS;

    public async UniTask BeginShake(float duration = 0.5f,
        float strength = 1f, int vibrato = 10, float randomness = 90f)
    {
        //�J�����h��̊J�n(���ɏ������s���Ă���ꍇ�A����͏I��������)
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
        Log("�J�����h��I��", LogLevel.Safe);
    }

    private async UniTask ShakeCamera(CancellationToken token,
        float duration, float strength, int vibrato, float randomness)
    {
        Log("�J�����h��J�n", LogLevel.Safe);
        var shakeTween = useCam.transform.DOShakePosition(duration, strength, vibrato, randomness);
        float elapsed = 0;
        while (token.IsCancellationRequested == false &&
               elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            await UniTask.Yield();
        }

        //�J�����h��I��
        if (token.IsCancellationRequested)
        {
            shakeTween.Kill();
            Log("�J�����h��𒆒f", LogLevel.Safe);
        }
        else
        {
            Log("�J�����h�ꊮ��", LogLevel.Safe);
        }
    }
    #endregion

    #region ���W�͈͐���
    private Vector3 minPos;
    private Vector3 maxPos;

    public void SetClampRange(Vector3 min, Vector3 max)
    {
        if (min.x > max.x ||
            min.y > max.y ||
            min.z > max.z)
        {
            Log("ClampRange�̐ݒ肪�s��(max��min�ȉ��ɂȂ��Ă���)�B�@"
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