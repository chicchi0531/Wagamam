using UnityEngine;
using System.Collections;
using ProjectWitch.Extention;

namespace ProjectWitch.Field
{
    public class CameraController : MonoBehaviour
    {

        //マップの大きさ
        private Vector3 mMapSize;

        //操作対象のカメラ
        [SerializeField]
        private Camera mMoveCamera = null;

        //拡縮の最小値
        [SerializeField]
        private float mScaleMin = 1.0f;

        //拡縮の最大値
        [SerializeField]
        private float mScaleMax = 7.0f;

        //拡縮のスピード
        [SerializeField]
        private float mScaleSpeed = 1.0f;

        //全体表示のカメラ
        [SerializeField]
        private Camera mOverLookCamera = null;

        //全体表示の際に非表示にするオブジェクト
        [SerializeField]
        private GameObject[] mHideGameObjects = null;


        //カメラが操作可能か
        public bool IsPlayable { get; set; }

        //全体表示になっているか
        public bool IsOverlook { get; set; }

        //目的地に到達するまでの時間
        [SerializeField]
        private float mDuration = 0.5f;

        //カメラの移動用内部変数
        private float mProgress = -1.0f;
        private Vector2 mTargetPos = Vector2.zero;
        private Vector3 mCurrentPos = Vector2.zero;
        
        void Start()
        {
            var collider = GetComponent<BoxCollider>();
            mMapSize = collider.size / 2;

            //デフォルトは操作可能
            IsPlayable = true;
                        
        }

        void Update()
        {
            ScaleCamera();
            DeltaMove();
        }

        //カメラ切り替え true:全体表示にする false:縮小ビューにする
        public void ChangeOverLookCamera(bool enable)
        {
            mMoveCamera.enabled = !enable;
            mOverLookCamera.enabled = enable;
            foreach (var obj in mHideGameObjects)
                obj.SetActive(!enable);
        }

        //変数入力
        private Vector3 mOldMousePos;

        void OnMouseDown()
        {
            var ray = RectTransformUtility.ScreenPointToRay(mMoveCamera, Input.mousePosition);

            RaycastHit rHit;
            if (Physics.Raycast(ray, out rHit))
            {
                mOldMousePos = rHit.point;
            }
            else
            {
                mOldMousePos = Vector3.zero;
            }
        }

        void OnMouseDrag()
        {
            Vector3 mousePos;
            var ray = RectTransformUtility.ScreenPointToRay(mMoveCamera, Input.mousePosition);

            RaycastHit rHit;
            if (Physics.Raycast(ray, out rHit))
            {
                mousePos = rHit.point;
            }
            else
            {
                mousePos = Vector3.zero;
            }

            //カメラが操作可能かどうか
            if (!IsPlayable)
            {
                mOldMousePos = mousePos;
                return;
            }

            var tmp = mousePos - mOldMousePos;
            tmp.z = 0.0f;

            mMoveCamera.transform.position -= tmp;

            //カメラの移動制限
            Vector3 cameraPos = mMoveCamera.transform.position;
            mMoveCamera.transform.position = AdjustCameraPos(cameraPos);

        }

        //カメラ移動（ターゲット指定）
        public IEnumerator MoveTo(Vector2 targetpos)
        {
            mTargetPos = targetpos;
            mCurrentPos = mMoveCamera.transform.position;
            mProgress = mDuration;

            while (mProgress >= 0) yield return null;
            yield return null;
        }

        //拡大縮小処理
        private void ScaleCamera()
        {
            if(IsPlayable)
            {
                var scrollVal = Input.GetAxis("MapScalling");
                scrollVal = scrollVal * Time.deltaTime * mScaleSpeed;

                mMoveCamera.orthographicSize -= scrollVal;
                mMoveCamera.orthographicSize = MathEx.Saturate(mMoveCamera.orthographicSize, mScaleMin, mScaleMax);

                mMoveCamera.transform.position = AdjustCameraPos(mMoveCamera.transform.position);
            }
        }

        //マップ外に出てしまったカメラを正しい位置に戻すメソッド
        private Vector3 AdjustCameraPos(Vector3 position)
        {
            var cameraPos = position;
            float cameraSizeY = mMoveCamera.orthographicSize;
            float aspect = (float)Screen.width / (float)Screen.height;
            float cameraSizeX = cameraSizeY * aspect;

            if (cameraPos.x < -mMapSize.x + cameraSizeX)
            {
                cameraPos.x = -mMapSize.x + cameraSizeX;
            }
            if (cameraPos.x > mMapSize.x - cameraSizeX)
            {
                cameraPos.x = mMapSize.x - cameraSizeX;
            }
            if (cameraPos.y < -mMapSize.y + cameraSizeY)
            {
                cameraPos.y = -mMapSize.y + cameraSizeY;
            }
            if (cameraPos.y > mMapSize.y - cameraSizeY)
            {
                cameraPos.y = mMapSize.y - cameraSizeY;
            }

            return cameraPos;
        }

        private void DeltaMove()
        {
            if(mProgress >= 0.0f)
            {

                //線形補間で位置を出す
                mMoveCamera.transform.position = new Vector3(mCurrentPos.x * mProgress + mTargetPos.x * (1 - mProgress),
                                                         mCurrentPos.y * mProgress + mTargetPos.y * (1 - mProgress), mCurrentPos.z);

                mProgress -= Time.deltaTime;
            }
        }
    }

}