using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// �u���E�U�̎d�l�̖��ŁAWebGL�łł̓Q�[���X�^�[�g�ł��̂܂܃T�E���h������Ȃ��B
/// ���̂��ߎn�߂Ƀv���C���[�ɃN���b�N������V�[���������N�b�V�����݂���
/// </summary>
public class InitClick_ForWebGL : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            SceneManager.LoadScene("PlayScene");
        }
    }
}