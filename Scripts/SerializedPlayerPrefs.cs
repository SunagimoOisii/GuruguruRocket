using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

/// <summary>
/// PlayerPrefs���g�������N���X<para></para>
/// �f�[�^��AES�ňÍ������A�C�ӂ̌^�̃I�u�W�F�N�g��ۑ��E�ǂݍ��݉\
/// </summary>
public static class SerializedPlayerPrefs
{
    //AES�Í����̃L�[�̒�����16,24,32�o�C�g�̂ݑΉ����Ă���
    private static readonly string EncryptionKey = "akvogtrfpskaieof";

    //�f�[�^�̕ۑ��\�ő�T�C�Y
    //WebGL�ł�PlayerPrefs�̏���ɍ��킹��1MB�ɐݒ�
    private static readonly int MaxSaveByteSize = 1024 * 1024;

    /// <summary>
    /// �C�ӃI�u�W�F�N�g��JSON�`���ňÍ������APlayerPrefs�ɕۑ�����
    /// </summary>
    public static void SetObject<T>(string key, T saveObj)
    {
        //obj��JSON��������A�Í�������
        //�v���~�e�B�u�^��string�͒��ڃV���A���C�Y����
        string j = JsonUtility.ToJson(saveObj);
        if (typeof(T).IsPrimitive ||
            typeof(T) == typeof(string))
        {
            j = saveObj.ToString();
        }
        else
        {
            j = JsonUtility.ToJson(saveObj);
        }
        string encryptedJ = Encrypt(j);

        //�Í���JSON�̃o�C�g�����v��
        int byteCount = Encoding.UTF8.GetByteCount(encryptedJ);

        //�T�C�Y�`�F�b�N
        if (byteCount > MaxSaveByteSize)
        {
            Debug.LogError("�ۑ��f�[�^��1MB�𒴂������ߕۑ����~");
            return;
        }

        //�Í���JSON��ۑ�
        PlayerPrefs.SetString(key, encryptedJ);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// �Í������ꂽ�f�[�^���擾���A�w�肵���^�ɕ�������
    /// </summary>
    /// <returns>�������ꂽ�I�u�W�F�N�g�B�f�[�^���Ȃ��ꍇ�̓f�t�H���g�l�B</returns>
    public static T GetObject<T>(string key, T defaultObj = default)
    {
        //�Í������ꂽJSON���擾�ł���Ε����A�ł��Ȃ���΃f�t�H���g�l��Ԃ�
        string encryptedJ = PlayerPrefs.GetString(key, null);
        if (string.IsNullOrEmpty(encryptedJ))
        {
            Debug.LogWarning("�Í���JSON���擾�ł��Ȃ��������߃f�[�^�^�f�t�H���g�l��Ԃ�");
            return defaultObj;
        }
        string j = Decrypt(encryptedJ);

        //�v���~�e�B�u�^��string�̏ꍇ�̖߂�l
        if (typeof(T).IsPrimitive || 
            typeof(T) == typeof(string))
        {
            try
            {
                return (T)Convert.ChangeType(j, typeof(T));
            }
            catch
            {
                Debug.LogError($"�f�[�^�^�̕ϊ��Ɏ��s: {typeof(T)}");
                return defaultObj;
            }
        }

        //JSON���I�u�W�F�N�g�ɕϊ����ĕԂ�
        return JsonUtility.FromJson<T>(j);
    }

    private static string Encrypt(string plainText)
    {
        //�L�[�Ə������x�N�g������Í��ϊ���𐶐�
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(EncryptionKey);
        aes.GenerateIV();
        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        //�������x�N�g���ƈÍ����f�[�^�̃o�C�g�z���
        //1�̕�����ɕϊ����ĕԂ�
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        return Convert.ToBase64String(aes.IV) + ":" + Convert.ToBase64String(encryptedBytes);
    }

    private static string Decrypt(string encryptedText)
    {
        //�����񂩂�A�������x�N�g���ƈÍ����f�[�^�ɕ�����
        string[] parts = encryptedText.Split(':');
        byte[] iv = Convert.FromBase64String(parts[0]);
        byte[] encryptedBytes = Convert.FromBase64String(parts[1]);

        //�L�[�Ə������x�N�g������Í��ϊ���𐶐�
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(EncryptionKey);
        using var decryptor = aes.CreateDecryptor(aes.Key, iv);

        //�f�[�^����x�o�C�g�z��Ƃ��ĕ������A������ɕϊ����ĕԂ�
        byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
        return Encoding.UTF8.GetString(decryptedBytes);
    }
}