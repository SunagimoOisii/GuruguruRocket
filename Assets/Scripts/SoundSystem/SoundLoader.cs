using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// SoundSystemが操作するクラスの1つ<para></para>
/// サウンドリソースをロードし、キャッシュを管理する<para></para>
/// - Addressableを介してAudioClipを非同期にロード可能<para></para>
/// - キャッシュされたリソースのクリーンアップや明示的な解放が可能<para></para>
/// </summary>
public class SoundLoader
{
    private readonly Dictionary<string, AudioClip> soundCache = new();
    private readonly Dictionary<string, float> lastAccessTime = new();

    public async UniTask<AudioClip> LoadClip(string resourceAddress)
    {
        //指定リソースがキャッシュに存在する場合、それを返す
        if (soundCache.TryGetValue(resourceAddress, out var cachedClip))
        {
            UpdateAccessTime(resourceAddress);
            return cachedClip;
        }

        //指定リソースのロード
        var handle = Addressables.LoadAssetAsync<AudioClip>(resourceAddress);

        try
        {
            //ロード成功ならキャッシュに保存
            var clip = await handle.Task;
            if (clip != null)
            {
                soundCache[resourceAddress] = clip;
                UpdateAccessTime(resourceAddress);
                return clip;
            }
            else
            {
                throw new InvalidOperationException($"SoundLoader: リソース '{resourceAddress}' のロードに失敗");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"SoundLoader: リソース '{resourceAddress}' のロード中にエラーが発生: {ex.Message}");
            throw new InvalidOperationException($"SoundLoader: リソース '{resourceAddress}' のロード中にエラーが発生", ex);
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

    /// <param name="idleTimeThreshold">未使用と判断する時間（秒）</param>
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