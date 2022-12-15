using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ReLeaf
{
    public class MenuButtonScript : MonoBehaviour
    {
        public string ReturnSceneName;  //�n���ꂽ���݂̃V�[�������i�[


        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void OnBackButton()
        {
            //���݂���Q�[���V�[���ɖ߂�
            ReturnSceneName = NowSceneManager.GetNowSceneName();
            SceneManager.LoadScene(ReturnSceneName);
        }

        public void OnRestartButton()
        {
            //���݂̃V�[���������[�h���čŏ������蒼��
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void OnTitleButton()
        {
            //�^�C�g����ʂɖ߂�
            SceneManager.LoadScene("Title");
        }
    }
}
