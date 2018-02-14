using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.ClearBonus
{
    public class MuseumController : MonoBehaviour
    {

        //拡大イメージ
        [SerializeField]
        private Image mScaleImage = null;

        [SerializeField]
        private Canvas mCanvas = null;

        [SerializeField]
        private ClearBonusController mController = null;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetButtonDown("Cancel") && mController.State == EState.Museum)
            {
                Close();
            }
        }

        public void Show()
        {
            mCanvas.enabled = true;
        }

        public void Close()
        {
            var game = Game.GetInstance();

            mScaleImage.gameObject.SetActive(false);

            mCanvas.enabled = false;
            game.SoundManager.PlaySE(SE.Cancel);
            mController.ShowTop();
        }

        //CGを拡大したものを表示
        public void ShowScaleImage(Sprite sprite)
        {
            mScaleImage.gameObject.SetActive(true);
            mScaleImage.sprite = sprite;
        }

        //CGを拡大したものを非表示
        public void HideScaleImage()
        {
            mScaleImage.gameObject.SetActive(false);
        }
    }
}