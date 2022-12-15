using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace ReLeaf
{
    public class NowSceneManager : MonoBehaviour
    {
        public static string NowSceneName;  //���݂̃V�[�������i�[


        // Start is called before the first frame update
        void Start()
        {
            NowSceneName = SceneManager.GetActiveScene().name;
        }

        // Update is called once per frame
        void Update()
        {
            var current = Keyboard.current;  //�L�[�{�[�h�擾

            if (current == null) return;  //�ڑ��`�F�b�N

            //�|�[�Y��ʂ��o�����̃g���K�[
            if (current.pKey.wasPressedThisFrame)
            {
                SceneManager.LoadScene("Menu");
            }
        }

        //���݂̃V�[������n��
        public static string GetNowSceneName()
        {
            return NowSceneName;
        }
    }
}
