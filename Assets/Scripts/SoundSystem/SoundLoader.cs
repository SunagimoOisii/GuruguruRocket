using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// SoundSystem�����삷��N���X��1��<para></para>
/// �T�E���h���\�[�X�����[�h���A�L���b�V�����Ǘ�����<para></para>
/// - Addressable�����AudioClip��񓯊��Ƀ��[�h�\<para></para>
/// - �L���b�V�����ꂽ���\�[�X�̃N���[���A�b�v�▾���I�ȉ�����\<para></para>
/// </summary>
public class SoundLoader
{
    private readonly Dictionary<string, AudioClip> soundCache = new();
    private readonly Dictionary<string, float> lastAccessTime = new();

    public async UniTask<AudioClip> LoadClip(string resourceAddress)
    {
        //�w�胊�\�[�X���L���b�V���ɑ��݂���ꍇ�A�����Ԃ�
        if (soundCache.TryGetValue(resourceAddress, out var cachedClip))
        {
            UpdateAccessTime(resourceAddress);
            return cachedClip;
        }

        //�w�胊�\�[�X�̃��[�h
        var handle = Addressables.LoadAssetAsync<AudioClip>(resourceAddress);

        try
        {
            //���[�h�����Ȃ�L���b�V���ɕۑ�
            var clip = await handle.Task;
            if (clip != null)
            {
                soundCache[resourceAddress] = clip;
                UpdateAccessTime(resourceAddress);
                return clip;
            }
            else
            {
                throw new InvalidOperationException($"SoundLoader: ���\�[�X '{resourceAddress}' �̃��[�h�Ɏ��s");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"SoundLoader: ���\�[�X '{resourceAddress}' �̃��[�h���ɃG���[������: {ex.Message}");
            throw new InvalidOperationException($"SoundLoader: ���\�[�X '{resourceAddress}' �̃��[�h���ɃG���[������", ex);
        }
    }

    public void UnloadClip(string resourceAddress)
    {
        if (soundCache.TryGetValue(resourceAddress, out var clip))
        {
            Addressables.Release(clip);
            soundCache.Remove(resourceAddress);
            lastAccessTime.Remove(resourceAddress);
        }
    }

    /// <param name="idleTimeThreshold">���g�p�Ɣ��f���鎞�ԁi�b�j</param>
    public void CleanupUnusedClips(float idleTimeThreshold)
    {
        var currentTime  = Time.time;
        var keysToRemove = new List<string>();

        foreach (var entry in lastAccessTime)
        {
            if (currentTime - entry.Value > idleTimeThreshold)
            {
                keysToRemove.Add(entry.Key);
            }
        }

        foreach (var key in keysToRemove)
        {
            UnloadClip(key);
        }
    }

    private void UpdateAccessTime(string resourceAddress)
    {
        lastAccessTime[resourceAddress] = Time.time;
    }

    public void ClearCache()
    {
        foreach (var clip in soundCache.Values)
        {
            Addressables.Release(clip);
        }
        soundCache.Clear();
        lastAccessTime.Clear();
    }
}