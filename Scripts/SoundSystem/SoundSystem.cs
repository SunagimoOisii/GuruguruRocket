using UnityEngine;
using UnityEngine.Audio;
using System;

/// <summary>
/// サウンド管理の中核を担うクラス<para></para>
/// BGM、SE、エフェクト、およびリソースローダーを統一的に管理<para></para>
/// - 各マネージャ(BGMManager, SEManager, ListenerEffector)を初期化し提供<para></para>
/// </summary>
public class SoundSystem : MonoBehaviour
{
    public static SoundSystem Instance { get; private set; }

    public BGMManager BGM { get; private set; }
    public SEManager SE { get; private set; }
    public ListenerEffector Effector { get; private set; }
    public SoundLoader Loader { get; private set; }

    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioMixerGroup bgmMixerGroup;
    [SerializeField] private AudioMixerGroup seMixerGroup;

    [SerializeField] private int initSEPoolSize = 4;

    private void Awake()
    {
        if (Instance == null)
        {
            //SoundSystemのインスタンスと各マネージャクラスを生成
            Instance = this;
            DontDestroyOnLoad(gameObject);
            try
            {
                Loader   = new();
                BGM      = CreateManager(() => new BGMManager(bgmMixerGroup, Loader));
                SE       = CreateManager(() => new SEManager(seMixerGroup, Loader, initSEPoolSize));
                Effector = CreateManager(() => new ListenerEffector(mixer));
            }
            catch
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Debug.LogError("SoundSystem: 同シーンに複数のSoundSystemが存在している");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// マネージャ初期化処理でのエラー出力を共通化するために実装した
    /// </summary>
    private T CreateManager<T>(Func<T> initializer)
    {
        try
        {
            return initializer();
        }
        catch (Exception ex)
        {
            Debug.LogError($"SoundSystem: マネージャ初期化中にエラーが発生: {ex.Message}");
            throw;
        }
    }
}