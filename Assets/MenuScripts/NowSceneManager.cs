using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace ReLeaf
{
    public class NowSceneManager : MonoBehaviour
    {
        public static string NowSceneName;  //現在のシーン名を格納


        // Start is called before the first frame update
        void Start()
        {
            NowSceneName = SceneManager.GetActiveScene().name;
        }

        // Update is called once per frame
        void Update()
        {
            var current = Keyboard.current;  //キーボード取得

            if (current == null) return;  //接続チェック

            //ポーズ画面を出す仮のトリガー
            if (current.pKey.wasPressedThisFrame)
            {
                SceneManager.LoadScene("Menu");
            }
        }

        //現在のシーン名を渡す
        public static string GetNowSceneName()
        {
            return NowSceneName;
        }
    }
}
