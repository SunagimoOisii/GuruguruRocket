using UnityEngine;
using UnityEngine.Audio;
using System;

/// <summary>
/// �T�E���h�Ǘ��̒��j��S���N���X<para></para>
/// BGM�ASE�A�G�t�F�N�g�A����у��\�[�X���[�_�[�𓝈�I�ɊǗ�<para></para>
/// - �e�}�l�[�W��(BGMManager, SEManager, ListenerEffector)������������<para></para>
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
            //SoundSystem�̃C���X�^���X�Ɗe�}�l�[�W���N���X�𐶐�
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
            Debug.LogError("SoundSystem: ���V�[���ɕ�����SoundSystem�����݂��Ă���");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// �}�l�[�W�������������ł̃G���[�o�͂����ʉ����邽�߂Ɏ�������
    /// </summary>
    private T CreateManager<T>(Func<T> initializer)
    {
        try
        {
            return initializer();
        }
        catch (Exception ex)
        {
            Debug.LogError($"SoundSystem: �}�l�[�W�����������ɃG���[������: {ex.Message}");
            throw;
        }
    }
}