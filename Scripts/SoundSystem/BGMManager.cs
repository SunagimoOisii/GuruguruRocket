using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Audio;
using System.Threading;

/// <summary>
/// BGMの再生、停止、クロスフェード機能を提供するクラス<para></para>
/// - AudioSourceを使用してBGMを制御<para></para>
/// - 非同期でのBGMロードとクロスフェード機能に対応<para></para>
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

        //BGM用AudioSourceを生成
        var source1 = CreateAudioSourceObj(mixerGroup, "BGMSource_0");
        var source2 = CreateAudioSourceObj(mixerGroup, "BGMSource_1");
        bgmSources = (source1, source2);
    }

    /// <summary>
    /// 指定AudioMixerGroupを用いてAudioSourceを生成<para></para>
    /// AudioSourceはDontDestroyOnLoadのオブジェクトに配置される
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
    /// 非同期で指定アドレスのAudioClipをロードし、再生を開始する
    /// </summary>
    /// <param name="volume">音量(範囲: 0～1)</param>
    public async UniTask Play(string resourceAddress, float volume = 1.0f)
    {
        var clip = await loader.LoadClip(resourceAddress);
        if (clip == null)
        {
            Debug.LogError($"BGMManager: リソース '{resourceAddress}' のロードに失敗");
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
    /// 現在再生中のBGMから別のBGMにクロスフェード<para></para>
    /// 再生中BGMと同じモノを指定した場合何も行われない
    /// </summary>
    /// <param name="duration">クロスフェードの時間(秒)</param>
    public async UniTask CrossFade(string resourceAddress, float duration, float newVolume = 1.0f)
    {
        if (resourceAddress == currentBGMAddress)
        {
            Debug.Log($"BGMManager: 同BGM '{resourceAddress}' が指定されたため、クロスフェードを中止");
            return;
        }

        //既存のクロスフェード処理をキャンセル
        fadeCancellationToken?.Cancel();
        fadeCancellationToken = new();
        var token = fadeCancellationToken.Token;

        try
        {
            var newActiveSource = bgmSources.inactive;
            var oldActiveSource = bgmSources.active;

            //新しいBGMをロード
            var clip = await loader.LoadClip(resourceAddress);
            if (clip == null)
            {
                Debug.LogError($"BGMManager: リソース '{resourceAddress}' のロードに失敗しました。");
                return;
            }

            newActiveSource.clip = clip;
            newActiveSource.volume = 0;
            newActiveSource.Play();

            //クロスフェード処理
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

            //再生中のリソースアドレスを更新
            currentBGMAddress = resourceAddress;
        }
        catch (OperationCanceledException)
        {
            Debug.Log("BGMManager: クロスフェード処理を中断");
        }
    }
}