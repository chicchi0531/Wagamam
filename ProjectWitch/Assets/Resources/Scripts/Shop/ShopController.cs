using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProjectWitch.Shop
{
    [RequireComponent(typeof(Animator))]
    public class ShopController : MonoBehaviour
    {
        //シーン名
        [SerializeField]
        private string mSceneName = "";

        //町シーンへの参照
        private Town.TownMenu mTownMenu = null;

        private Animator mcAnimator = null;

        void Start()
        {
            var game = Game.GetInstance();
            if(game.TownDataIn.FromTown) mTownMenu = GameObject.FindWithTag("TownController").GetComponent<Town.TownMenu>();
            mcAnimator = GetComponent<Animator>();
        }

        public void Close()
        {
            mcAnimator.SetBool("IsShow", false);
        }

        //閉じる際の最終処理（アニメーションから呼び出す
        public void Close_End()
        {
            //BGMを変更
            var game = Game.GetInstance();
            
            //町から来た場合は町へ戻る
            if (game.TownDataIn.FromTown)
            {
                game.SoundManager.Play(game.GameData.TownBGM, SoundType.BGM);
                mTownMenu.Closable = true;
            }
            else
            {
                game.SoundManager.Play(game.GameData.FieldBGM, SoundType.BGM);
                game.IsTown = false;
            }

            SceneManager.UnloadSceneAsync(mSceneName);
        }
    }
}