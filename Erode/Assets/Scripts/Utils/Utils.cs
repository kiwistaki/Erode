using UnityEngine;

namespace Assets.Scripts.Utils
{
    public static class Utils
    {
        public static float getRealDeltaTime()
        {
            return Time.deltaTime / (Time.timeScale == 0 ? 1 : Time.timeScale);
        }

        public static Vector3 GetScreenPosition(Vector3 position, Canvas canvas, UnityEngine.Camera cam)
        {
            Vector3 pos;
            float width = canvas.GetComponent<RectTransform>().sizeDelta.x;
            float height = canvas.GetComponent<RectTransform>().sizeDelta.y;
            float x = cam.WorldToScreenPoint(position).x / Screen.width;
            float y = cam.WorldToScreenPoint(position).y / Screen.height;
            pos = new Vector3(width * x - width / 2, y * height - height / 2);
            return pos;
        }
    }
}
