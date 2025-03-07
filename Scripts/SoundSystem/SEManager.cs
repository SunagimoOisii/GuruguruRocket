using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Audio;
using System.Threading;
using System.Linq;

/// <summary>
/// SoundSystem�����삷��N���X��1��<para></para>
/// - AudioSource���v�[���Ǘ����A�����I��SE���Đ�<para></para>
/// - �v�[���T�C�Y�̊Ǘ��Ɩ��g�p���\�[�X�̒���N���[���A�b�v���s��<para></para>
/// </summary>
public class SEManager
{
    private readonly SoundLoader loader;

    private GameObject sourceParentObj;
    private Queue<AudioSource> audioSourcePool;
    private readonly Dictionary<AudioSource, float> lastUsedTimeDict;
    private readonly CancellationTokenSource cleanupTokenSource;
    private readonly int maxPoolSize;

    public SEManager(AudioMixerGroup mixerGroup, SoundLoader loader,
        int initialPoolSize, int maxPoolSize = 10, float sourceRemoveTime = 30f, float unusedLifetime = 30f)
    {
        this.loader = loader;

        //SE�pAudioSource��������GameObject�̐e�I�u�W�F�N�g�𐶐�
        sourceParentObj = new("SE_AudioSources");
        Object.DontDestroyOnLoad(sourceParentObj);

        this.maxPoolSize = maxPoolSize;
        audioSourcePool = new();
        lastUsedTimeDict = new();

        //AudioSource�v�[���̏�����
        for (int i = 0; i < initialPoolSize; i++)
        {
            var source = CreateAudioSource(mixerGroup, $"SESource_{i}");
            audioSourcePool.Enqueue(source);
            lastUsedTimeDict[source] = Time.time;
        }

        //��莞�Ԗ��g�p��AudioSource���폜���鏈�����쓮������
        cleanupTokenSource = new();
        CleanupPeriodically(cleanupTokenSource.Token, sourceRemoveTime, unusedLifetime).Forget();
    }

    /// <summary>
    /// ���g�p��AudioSource���폜����
    /// </summary>
    /// <param name="interval">�폜�����s����Ԋu(�b)</param>
    /// <param name="unusedLifeTime">������폜�ΏۂɂȂ邩(�b)</param>
    private async UniTask CleanupPeriodically(CancellationToken token,
        float interval, float unusedLifetime)
    {
        while (token.IsCancellationRequested == false)
        {
            CleanupUnusedAudioSources(unusedLifetime);
            await UniTask.Delay((int)(interval * 1000), cancellationToken: token);
        }
    }

    private void CleanupUnusedAudioSources(float unusedLifeTime)
    {
        var currentTime = Time.time;
        var toRemove = new List<AudioSource>();
        foreach (var entry in lastUsedTimeDict)
        {
            if (currentTime - entry.Value > unusedLifeTime)
            {
                toRemove.Add(entry.Key);
            }
        }

        //�w�莞�Ԃ̊Ԗ��g�p��AudioSource(�Ƃ��ꂪ�A�^�b�`���ꂽGameObject)���폜
        foreach (var source in toRemove)
        {
            audioSourcePool = new Queue<AudioSource>(audioSourcePool.Where(s => s != source));
            lastUsedTimeDict.Remove(source);
            Object.Destroy(source.gameObject);
        }
    }

    public async UniTask Play(string resourceAddress, Vector3 position = default,
        float volume = 1.0f, float pitch = 1.0f)
    {
        //�T�E���h���\�[�X�̃��[�h
        var clip = await loader.LoadClip(resourceAddress);
        if (clip == null)
        {
            Debug.LogWarning($"SEManager: �w�胊�\�[�X '{resourceAddress}' ��������Ȃ�");
            return;
        }

        //�Đ������̐ݒ�
        var source = GetAvailableAudioSource();
        source.pitch = pitch;
        source.volume = volume;
        source.transform.position = position;
        source.PlayOneShot(clip);

        //AudioSource�̎g�p���Ԃ��L�^
        lastUsedTimeDict[source] = Time.time;
    }

    private AudioSource GetAvailableAudioSource()
    {
        //�v�[�����ōĐ����łȂ�AudioSource������΁A�����Ԃ�
        foreach (var source in audioSourcePool)
        {
            if (source != null && source.isPlaying == false)
            {
                lastUsedTimeDict[source] = Time.time;
                return source;
            }
        }

        //�v�[������̏ꍇ�₷�ׂč폜�ς݂̏ꍇ
        if (audioSourcePool.Count == 0 || audioSourcePool.Peek() == null)
        {
            Debug.LogWarning("SEManager: AudioSource�v�[�����󂩖����ȎQ�Ƃ��܂�ł��邽�ߐV�K�쐬����");
            var newS = CreateAudioSource(null, $"SESource_{audioSourcePool.Count}");
            audioSourcePool.Enqueue(newS);
            return newS;
        }

        //�v�[�����ő�T�C�Y�ɒB���Ă���ꍇ
        if (audioSourcePool.Count >= maxPoolSize)
        {
            var oldestSource = audioSourcePool.Dequeue();

            if (oldestSource == null)
            {
                Debug.LogWarning("SEManager: �ė��p�Ώۂ�AudioSource�������̂��ߐV�K�쐬����");
                var newS = CreateAudioSource(audioSourcePool.Peek()?.outputAudioMixerGroup, $"SESource_{audioSourcePool.Count}");
                audioSourcePool.Enqueue(newS);
                return newS;
            }

            audioSourcePool.Enqueue(oldestSource);
            lastUsedTimeDict[oldestSource] = Time.time;
            return oldestSource;
        }

        //�v�[�����ő�T�C�Y�ɒB���Ă��Ȃ��ꍇ
        var newSource = CreateAudioSource(audioSourcePool.Peek().outputAudioMixerGroup, $"SESource_{audioSourcePool.Count}");
        audioSourcePool.Enqueue(newSource);
        lastUsedTimeDict[newSource] = Time.time;
        return newSource;
    }

    /// <summary>
    /// AudioSource���v�[������擾���A�K�v�ɉ����ĐV����AudioSource���쐬<para></para>
    /// �ė��p�\��AudioSource�����݂��Ȃ��ꍇ�́A�ŌÂ�AudioSource���ė��p����
    /// </summary>
    private AudioSource CreateAudioSource(AudioMixerGroup mixerGroup, string name)
    {
        var sourceObj = new GameObject(name);
        sourceObj.transform.parent = sourceParentObj.transform;

        var source = sourceObj.AddComponent<AudioSource>();
        source.spatialBlend          = 1.0f;
        source.playOnAwake           = false;
        source.outputAudioMixerGroup = mixerGroup;
        return source;
    }
}