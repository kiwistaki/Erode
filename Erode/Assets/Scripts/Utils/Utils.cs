using Assets.Scripts.Control;
using Assets.Scripts.HexGridGenerator;
using System.Collections.Generic;
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

        public static List<List<Tile>> GetConcentricCircles(Tile t, int radius)
        {
            // Initialize the "wave"
            List<List<Tile>> concentricCircles = new List<List<Tile>>();
            for (int i = 0; i < radius; i++)
                concentricCircles.Add(new List<Tile>());
            // Find all the concentric circles from the center
            foreach (KeyValuePair<string, Tile> pair in Grid.inst.Tiles)
            {
                int distance = Grid.inst.Distance(pair.Value, t);
                if (distance < radius)
                    concentricCircles[distance].Add(pair.Value);
            }
            return concentricCircles;
        }

        public static void StartTileWave(Tile t, int waveRadius)
        {
            // Initialize the "wave"
            List<List<Tile>> concentricCircles = GetConcentricCircles(t, waveRadius);

            // Set all the iTweens
            float delay = 0.0f;
            foreach (List<Tile> circle in concentricCircles)
            {
                foreach (Tile tile in circle)
                {
                    iTween.iTween.PunchPosition(tile.gameObject, iTween.iTween.Hash
                       ("name", "Wave"
                           , "time", 0.5f
                           , "delay", delay
                           , "onstarttarget", tile.gameObject
                           , "onstart", "ToWavingState"
                           , "onupdatetarget", tile.gameObject
                           , "onupdate", "UpdateYAxis"
                           , "oncompletetarget", tile.gameObject
                           , "oncomplete", "ResetTransform"
                           , "ignoretimescale", false
                       ));
                }
                delay += 0.05f;
            }
        }

        public static Tile GetClosestTile(Vector3 pos)
        {
            RaycastHit hitInfo;
            LayerMask mask = (1 << 8);
            Tile hittedTile = null;
            float radius = 0f, maxRadius = 5f, angle = 0f, maxAngle = 2 * Mathf.PI, distance = 10f;
            while (hittedTile == null && radius <= maxRadius)
            {
                while (hittedTile == null && angle <= maxAngle)
                {
                    Vector3 dir = (Vector3.right * Mathf.Cos(angle) * radius + Vector3.forward * Mathf.Sin(angle) * radius) + pos;
                    dir += distance * Vector3.down;
                    dir -= pos;
                    if (Physics.Raycast(new Ray(pos, dir), out hitInfo, Mathf.Infinity, mask))
                    {
                        hittedTile = hitInfo.collider.gameObject.GetComponent<Tile>();                        
                    }
                    angle += Mathf.PI / 6;
                    if (radius == 0f) break;
                }
                angle = 0f;
                radius += 1f;
            }
            
            return hittedTile;
        }
    }
}
