using UnityEngine;

namespace MageRunner.Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private int targetWidth;
        [SerializeField] private float pixelsToUnits;
        [SerializeField] private RectTransform bottomPanel;

        private int _height;

        private void Awake()
        {
            _height = Mathf.RoundToInt(targetWidth / (float) Screen.width * Screen.height);
            print (Screen.width + "   " + Screen.height + "    " + _height);

            if (_height > 1080)
                UnityEngine.Camera.main.orthographicSize = _height / pixelsToUnits / 2;
        }

        public void ScaleLevelCamera(Transform level)
        {
            // 4:3
            if (_height >= 1440 && _height < 1536)
            {
                level.localScale = new Vector3(1, 1.2f, 1);
                level.position = new Vector3(0, 1.02f, 0);
                transform.position = new Vector3(0, 0, -10);
                bottomPanel.anchoredPosition = new Vector3(0, 143, 0);
                bottomPanel.sizeDelta = new Vector2(bottomPanel.sizeDelta.x, 285);
            }
        
            // 5:4
            else if (_height >= 1536)
            {
                level.localScale = new Vector3(1, 1.25f, 1);
                level.position = new Vector3(0, 1.3f, 0);
                transform.position = new Vector3(0, 0, -10);
                bottomPanel.anchoredPosition = new Vector3(0, 152, 0);
                bottomPanel.sizeDelta = new Vector2(bottomPanel.sizeDelta.x, 302);
            }
        }
    }
}
