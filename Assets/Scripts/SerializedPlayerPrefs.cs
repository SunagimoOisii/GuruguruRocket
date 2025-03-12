using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

/// <summary>
/// PlayerPrefsを拡張したクラス<para></para>
/// データをAESで暗号化し、任意の型のオブジェクトを保存・読み込み可能
/// </summary>
public static class SerializedPlayerPrefs
{
    //AES暗号化のキーの長さは16,24,32バイトのみ対応している
    private static readonly string EncryptionKey = "akvogtrfpskaieof";

    //データの保存可能最大サイズ
    //WebGLでのPlayerPrefsの上限に合わせて1MBに設定
    private static readonly int MaxSaveByteSize = 1024 * 1024;

    /// <summary>
    /// 任意オブジェクトをJSON形式で暗号化し、PlayerPrefsに保存する
    /// </summary>
    public static void SetObject<T>(string key, T saveObj)
    {
        //objをJSON化した後、暗号化する
        //プリミティブ型とstringは直接シリアライズする
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

        //暗号化JSONのバイト数を計測
        int byteCount = Encoding.UTF8.GetByteCount(encryptedJ);

        //サイズチェック
        if (byteCount > MaxSaveByteSize)
        {
            Debug.LogError("保存データが1MBを超えたため保存中止");
            return;
        }

        //暗号化JSONを保存
        PlayerPrefs.SetString(key, encryptedJ);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 暗号化されたデータを取得し、指定した型に復元する
    /// </summary>
    /// <returns>復元されたオブジェクト。データがない場合はデフォルト値。</returns>
    public static T GetObject<T>(string key, T defaultObj = default)
    {
        //暗号化されたJSONが取得できれば復号、できなければデフォルト値を返す
        string encryptedJ = PlayerPrefs.GetString(key, null);
        if (string.IsNullOrEmpty(encryptedJ))
        {
            Debug.LogWarning("暗号化JSONが取得できなかったためデータ型デフォルト値を返す");
            return defaultObj;
        }
        string j = Decrypt(encryptedJ);

        //プリミティブ型とstringの場合の戻り値
        if (typeof(T).IsPrimitive || 
            typeof(T) == typeof(string))
        {
            try
            {
                return (T)Convert.ChangeType(j, typeof(T));
            }
            catch
            {
                Debug.LogError($"データ型の変換に失敗: {typeof(T)}");
                return defaultObj;
            }
        }

        //JSONをオブジェクトに変換して返す
        return JsonUtility.FromJson<T>(j);
    }

    private static string Encrypt(string plainText)
    {
        //キーと初期化ベクトルから暗号変換器を生成
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(EncryptionKey);
        aes.GenerateIV();
        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        //初期化ベクトルと暗号化データのバイト配列を
        //1つの文字列に変換して返す
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        return Convert.ToBase64String(aes.IV) + ":" + Convert.ToBase64String(encryptedBytes);
    }

    private static string Decrypt(string encryptedText)
    {
        //文字列から、初期化ベクトルと暗号化データに分ける
        string[] parts = encryptedText.Split(':');
        byte[] iv = Convert.FromBase64String(parts[0]);
        byte[] encryptedBytes = Convert.FromBase64String(parts[1]);

        //キーと初期化ベクトルから暗号変換器を生成
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(EncryptionKey);
        using var decryptor = aes.CreateDecryptor(aes.Key, iv);

        //データを一度バイト配列として復号し、文字列に変換して返す
        byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
        return Encoding.UTF8.GetString(decryptedBytes);
    }
}