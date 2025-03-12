using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Audio;
using System.Threading;

/// <summary>
/// BGM�̍Đ��A��~�A�N���X�t�F�[�h�@�\��񋟂���N���X<para></para>
/// - AudioSource���g�p����BGM�𐧌�<para></para>
/// - �񓯊��ł�BGM���[�h�ƃN���X�t�F�[�h�@�\�ɑΉ�<para></para>
/// </summary>

public class BGMManager
{
    private readonly SoundLoader loader;

    private GameObject sourceParentObj = null;
    private (AudioSource active, AudioSource inactive) bgmSources;

    private CancellationTokenSource fadeCancellationToken;
    private string currentBGMAddress;

    public BGMManager(AudioMixerGroup mixerGroup, SoundLoader loader)
    {
        this.loader = loader;

        //BGM�pAudioSource�𐶐�
        var source1 = CreateAudioSourceObj(mixerGroup, "BGMSource_0");
        var source2 = CreateAudioSourceObj(mixerGroup, "BGMSource_1");
        bgmSources = (source1, source2);
    }

    /// <summary>
    /// �w��AudioMixerGroup��p����AudioSource�𐶐�<para></para>
    /// AudioSource��DontDestroyOnLoad�̃I�u�W�F�N�g�ɔz�u�����
    /// </summary>
    private AudioSource CreateAudioSourceObj(AudioMixerGroup mixerGroup, string name)
    {
        if (sourceParentObj == null)
        {
            sourceParentObj = new("BGM_AudioSources");
            UnityEngine.Object.DontDestroyOnLoad(sourceParentObj);
        }

        var sourceObj = new GameObject(name);
        sourceObj.transform.parent = sourceParentObj.transform;

        var source = sourceObj.AddComponent<AudioSource>();
        source.loop                  = true;
        source.playOnAwake           = false;
        source.outputAudioMixerGroup = mixerGroup;
        return source;
    }

    /// <summary>
    /// �񓯊��Ŏw��A�h���X��AudioClip�����[�h���A�Đ����J�n����
    /// </summary>
    /// <param name="volume">����(�͈�: 0�`1)</param>
    public async UniTask Play(string resourceAddress, float volume = 1.0f)
    {
        var clip = await loader.LoadClip(resourceAddress);
        if (clip == null)
        {
            Debug.LogError($"BGMManager: ���\�[�X '{resourceAddress}' �̃��[�h�Ɏ��s");
            return;
        }

        bgmSources.active.clip   = clip;
        bgmSources.active.volume = volume;
        bgmSources.active.Play();
    }

    public void Stop()
    {
        bgmSources.active.Stop();
        bgmSources.active.clip = null;
    }

    /// <summary>
    /// ���ݍĐ�����BGM����ʂ�BGM�ɃN���X�t�F�[�h<para></para>
    /// �Đ���BGM�Ɠ������m���w�肵���ꍇ�����s���Ȃ�
    /// </summary>
    /// <param name="duration">�N���X�t�F�[�h�̎���(�b)</param>
    public async UniTask CrossFade(string resourceAddress, float duration, float newVolume = 1.0f)
    {
        if (resourceAddress == currentBGMAddress)
        {
            Debug.Log($"BGMManager: ��BGM '{resourceAddress}' ���w�肳�ꂽ���߁A�N���X�t�F�[�h�𒆎~");
            return;
        }

        //�����̃N���X�t�F�[�h�������L�����Z��
        fadeCancellationToken?.Cancel();
        fadeCancellationToken = new();
        var token = fadeCancellationToken.Token;

        try
        {
            var newActiveSource = bgmSources.inactive;
            var oldActiveSource = bgmSources.active;

            //�V����BGM�����[�h
            var clip = await loader.LoadClip(resourceAddress);
            if (clip == null)
            {
                Debug.LogError($"BGMManager: ���\�[�X '{resourceAddress}' �̃��[�h�Ɏ��s���܂����B");
                return;
            }

            newActiveSource.clip = clip;
            newActiveSource.volume = 0;
            newActiveSource.Play();

            //�N���X�t�F�[�h����
            float t = 0;
            float initOldVolume = oldActiveSource.volume;

            while (t < duration)
            {
                if (token.IsCancellationRequested) return;

                float progress = t / duration;
                oldActiveSource.volume = Mathf.Lerp(initOldVolume, 0.0f, progress);
                newActiveSource.volume = Mathf.Lerp(0.0f, newVolume, progress);
                t += Time.deltaTime;
                await UniTask.Yield();
            }

            oldActiveSource.Stop();
            bgmSources = (newActiveSource, oldActiveSource);

            //�Đ����̃��\�[�X�A�h���X���X�V
            currentBGMAddress = resourceAddress;
        }
        catch (OperationCanceledException)
        {
            Debug.Log("BGMManager: �N���X�t�F�[�h�����𒆒f");
        }
    }
}