using UnityEngine;
using UnityEngine.Audio;
using System;
using System.Collections.Generic;

/// <summary>
/// SoundSystem�����삷��N���X�̂P��<para></para>
/// AudioListener�ɃG�t�F�N�g�t�B���^�[�𓮓I�ɒǉ���������s��
/// (�G�t�F�N�g�t�B���^�[�Ƃ�AudioReverbFilter��AudioEchoFilter�Ȃǂ̂��ƂŁA
/// �{�N���X�ł�Behaviour�N���X�����^�Ƃ��ē���I�Ɉ���)
/// </summary>
public class ListenerEffector
{
    private readonly AudioListener listener;
    private readonly AudioMixer audioMixer;

    //�e�t�B���^�[�̃C���X�^���X���Ǘ�
    private readonly Dictionary<Type, Component> filterDict = new();

    public ListenerEffector(AudioMixer audioMixer)
    {
        this.audioMixer = audioMixer;

        //AudioListener������
        listener = UnityEngine.Object.FindObjectOfType<AudioListener>();
        if (listener == null)
        {
            Debug.LogError("ListenerEffector: AudioListener���V�[�����Ō�����Ȃ�");
        }
    }

    /// <typeparam name="FilterT">�K�p����t�B���^�[�̌^</typeparam>
    /// <param name="configure">�t�B���^�[�̐ݒ���s���A�N�V����</param>
    /// <remarks>�g�p��: effector.ApplyFilter<AudioReverbFilter>(filter => filter.reverbLevel = Mathf.Clamp(reverbLevel, -10000f, 2000f));</remarks>
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
            Debug.LogWarning($"ListenerEffector: �p�����[�^ '{exposedParamName}' �̐ݒ�Ɏ��s");
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
            Debug.LogWarning($"ListenerEffector: �p�����[�^ '{exposedParamName}' �̎擾�Ɏ��s");
            return null;
        }
    }
}