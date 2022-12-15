using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ReLeaf
{
    public class MenuButtonScript : MonoBehaviour
    {
        public string ReturnSceneName;  //渡された現在のシーン名を格納


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
            //現在いるゲームシーンに戻る
            ReturnSceneName = NowSceneManager.GetNowSceneName();
            SceneManager.LoadScene(ReturnSceneName);
        }

        public void OnRestartButton()
        {
            //現在のシーンをリロードして最初からやり直す
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void OnTitleButton()
        {
            //タイトル画面に戻る
            SceneManager.LoadScene("Title");
        }
    }
}
