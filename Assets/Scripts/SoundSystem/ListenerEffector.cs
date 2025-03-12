using UnityEngine;
using UnityEngine.Audio;
using System;
using System.Collections.Generic;

/// <summary>
/// SoundSystemが操作するクラスの１つ<para></para>
/// AudioListenerにエフェクトフィルターを動的に追加し制御を行う
/// (エフェクトフィルターとはAudioReverbFilterやAudioEchoFilterなどのことで、
/// 本クラスではBehaviourクラスを基底型として統一的に扱う)
/// </summary>
public class ListenerEffector
{
    private readonly AudioListener listener;
    private readonly AudioMixer audioMixer;

    //各フィルターのインスタンスを管理
    private readonly Dictionary<Type, Component> filterDict = new();

    public ListenerEffector(AudioMixer audioMixer)
    {
        this.audioMixer = audioMixer;

        //AudioListenerを検索
        listener = UnityEngine.Object.FindObjectOfType<AudioListener>();
        if (listener == null)
        {
            Debug.LogError("ListenerEffector: AudioListenerがシーン内で見つからない");
        }
    }

    /// <typeparam name="FilterT">適用するフィルターの型</typeparam>
    /// <param name="configure">フィルターの設定を行うアクション</param>
    /// <remarks>使用例: effector.ApplyFilter<AudioReverbFilter>(filter => filter.reverbLevel = Mathf.Clamp(reverbLevel, -10000f, 2000f));</remarks>
    public void ApplyFilter<FilterT>(Action<FilterT> configure) where FilterT : Behaviour
    {
        if (filterDict.TryGetValue(typeof(FilterT), out var component) == false)
        {
            component = listener.gameObject.AddComponent<FilterT>();
            filterDict[typeof(FilterT)] = component;
        }

        var filter = component as FilterT;
        filter.enabled = true;
        configure?.Invoke(filter);
    }

    public void DisableAllEffects()
    {
        foreach (var filter in filterDict.Values)
        {
            if (filter is Behaviour b)
            {
                b.enabled = false;
            }
        }
    }

    public void SetMixerParameter(string exposedParamName, float value)
    {
        if (audioMixer.SetFloat(exposedParamName, value) == false)
        {
            Debug.LogWarning($"ListenerEffector: パラメータ '{exposedParamName}' の設定に失敗");
        }
    }

    public float? GetMixerParameter(string exposedParamName)
    {
        if (audioMixer.GetFloat(exposedParamName, out float value))
        {
            return value;
        }
        else
        {
            Debug.LogWarning($"ListenerEffector: パラメータ '{exposedParamName}' の取得に失敗");
            return null;
        }
    }
}