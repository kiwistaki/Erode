using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.HexGridGenerator
{
    public class Grid : MonoBehaviour
    {
        public static Grid inst;

        //Map settings
        public MapShape mapShape = MapShape.Rectangle;
        public int mapWidth;
        public int mapHeight;

        //Hex Settings
        public HexOrientation hexOrientation = HexOrientation.Flat;
        public float hexRadius = 1;
        public Material hexMaterial;
        public Mesh hexMesh = null;
        public float GridScale = 0.6f;
        public float CenterHexHeight = 0.085f;
        public int HexHP = 10000;
        [Range(0.0f, 1.0f)]
        public float HexHpVariation = 0.1f;
        public float HexErodeRate = 1.0f;
        [Range(0.01f, 0.5f)]
        public float ShakeStrenght = 0.05f;
        public GameObject TileHitVfxPrefab;
        public Material ShinyTileMaterial;
        public GameObject ShinyTileVFXPrefab1;
        public GameObject ShinyTileVFXPrefab2;



        //Generation Options
        public bool addColliders = true;

        //Internal variables
        private Dictionary<string, Tile> _grid = new Dictionary<string, Tile>();
        private Dictionary<int, Tile> _borderHexes = new Dictionary<int, Tile>();
        private List<Tile> _addToBorder = new List<Tile>();
        private List<Tile> _removeFromBorder = new List<Tile>();

        private CubeIndex[] directions =
            new CubeIndex[] {
                new CubeIndex(1, -1, 0),
                new CubeIndex(1, 0, -1),
                new CubeIndex(0, 1, -1),
                new CubeIndex(-1, 1, 0),
                new CubeIndex(-1, 0, 1),
                new CubeIndex(0, -1, 1)
            };

        private bool _isGridWaving = false;

        #region Getters and Setters
        public Dictionary<string, Tile> Tiles
        {
            get { return this._grid; }
        }

        public Dictionary<int, Tile> BorderHexes
        {
            get { return this._borderHexes; }
            set { this._borderHexes = value; }
        }

        public bool IsGridWaving
        {
            get { return _isGridWaving; }
            set { _isGridWaving = value; StartCoroutine(WaitTimeBetweenWaves()); }
        }
        #endregion

        #region Public Methods
        public void AddToBorder(Tile tile)
        {
            var it = this._removeFromBorder.FindIndex(x => x.GetHashCode() == tile.GetHashCode());
            if (it != -1)
            {
                this._removeFromBorder.Remove(tile);
            }
            else
            {
                var it2 = this._addToBorder.FindIndex(x => x.GetHashCode() == tile.GetHashCode());
                if (it2 == -1)
                {
                    this._addToBorder.Add(tile);
                }
            }
        }

        public void RemoveFromBorder(Tile tile)
        {
            var it = this._addToBorder.FindIndex(x => x.GetHashCode() == tile.GetHashCode());
            if (it != -1)
            {
                this._addToBorder.Remove(tile);
            }
            else
            {
                var it2 = this._removeFromBorder.FindIndex(x => x.GetHashCode() == tile.GetHashCode());
                if (it2 == -1)
                {
                    this._removeFromBorder.Add(tile);
                }
            }
        }

        public Tile GetRandomBorderTile()
        {
            if (this.BorderHexes.Count != 0)
            {
                var rnd = Random.value;
                var randomEntry = this.BorderHexes.ElementAt((int)(rnd * this.BorderHexes.Count - 0.1));
                return randomEntry.Value;
            }
            else
            {
                return this.GetRandomTile(false, false);
            }
        }

        public void GenerateGrid()
        {
            //Generating a new grid, clear any remants and initialise values
            this.ClearGrid();
            this.GetMesh();

            //Generate the grid shape
            switch (this.mapShape)
            {
                case MapShape.Hexagon:
                    this.GenHexShape();
                    break;

                case MapShape.Rectangle:
                    this.GenRectShape();
                    break;

                case MapShape.Parrallelogram:
                    this.GenParrallShape();
                    break;

                case MapShape.Triangle:
                    this.GenTriShape();
                    break;

                default:
                    break;
            }

            InitializeTiles();
        }

        public void ClearGrid()
        {
            //Debug.Log("Clearing grid...");
            foreach (var tile in this._grid.Values)
            {
                DestroyImmediate(tile.transform.parent.gameObject, false);
            }

            this._addToBorder.Clear();
            this._borderHexes.Clear();
            this._removeFromBorder.Clear();

            this._grid.Clear();
            this.transform.localScale = Vector3.one;
        }

        public Tile TileAt(CubeIndex index)
        {
            Tile find;
            if (this._grid.TryGetValue(index.ToString(), out find))
                return find;
            return null;
        }

        public Tile TileAt(int x, int y, int z)
        {
            return this.TileAt(new CubeIndex(x, y, z));
        }

        public Tile TileAt(int x, int z)
        {
            return this.TileAt(new CubeIndex(x, z));
        }

        public List<Tile> Neighbours(Tile tile)
        {
            List<Tile> ret = new List<Tile>();
            CubeIndex o;

            for (int i = 0; i < 6; i++)
            {
                o = tile.Index + this.directions[i];
                Tile find;
                if (this._grid.TryGetValue(o.ToString(), out find))
                    ret.Add(find);
            }
            return ret;
        }

        public List<Tile> Neighbours(CubeIndex index)
        {
            return this.Neighbours(this.TileAt(index));
        }

        public List<Tile> Neighbours(int x, int y, int z)
        {
            return this.Neighbours(this.TileAt(x, y, z));
        }

        public List<Tile> Neighbours(int x, int z)
        {
            return this.Neighbours(this.TileAt(x, z));
        }

        public List<Tile> TilesInRange(Tile center, int range)
        {
            //Return tiles rnage steps from center, http://www.redblobgames.com/grids/hexagons/#range
            List<Tile> ret = new List<Tile>();
            CubeIndex o;

            for (int dx = -range; dx <= range; dx++)
            {
                for (int dy = Mathf.Max(-range, -dx - range); dy <= Mathf.Min(range, -dx + range); dy++)
                {
                    o = new CubeIndex(dx, dy, -dx - dy) + center.Index;
                    Tile find;
                    if (this._grid.TryGetValue(o.ToString(), out find))
                        ret.Add(find);
                }
            }
            return ret;
        }

        public List<Tile> TilesInRange(CubeIndex index, int range)
        {
            return this.TilesInRange(this.TileAt(index), range);
        }

        public List<Tile> TilesInRange(int x, int y, int z, int range)
        {
            return this.TilesInRange(this.TileAt(x, y, z), range);
        }

        public List<Tile> TilesInRange(int x, int z, int range)
        {
            return this.TilesInRange(this.TileAt(x, z), range);
        }

        //THIS FUNCTION IS SLOW, DO NOT CALL IT AT EVERY UPDATE
        public Tile GetRandomTile(bool excludeDead, bool excludeBorder)
        {
            Tile[] query;
            if (excludeDead && excludeBorder)
            {
                query = this._grid.Values.Where(
                x =>
                x.Hp > 0 &&
                !this._borderHexes.ContainsKey(x.GetHashCode())
                ).ToArray();
            }
            else if (excludeBorder)
            {
                query = this._grid.Values.Where(
                x =>
                !this._borderHexes.ContainsKey(x.GetHashCode())
                ).ToArray();
            }
            else if (excludeDead)
            {
                query = this._grid.Values.Where(
                x =>
                x.Hp > 0
                ).ToArray();
            }
            else
            {
                query = this._grid.Values.ToArray();
            }

            if (query.Length > 0)
            {
                return query[(int)Random.Range(0.0f, query.Length - 0.1f)];
            }
            else
            {
                return null;
            }
        }

        public int Distance(CubeIndex a, CubeIndex b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
        }

        public int Distance(Tile a, Tile b)
        {
            return this.Distance(a.Index, b.Index);
        }
        #endregion

        #region Private Methods
        private void Awake()
        {
            if (!inst)
                inst = this;
        }

        private void InitializeTiles()
        {
            //Scaling the grid
            this.transform.localScale = new Vector3(this.GridScale, this.GridScale, this.GridScale);
            //Fixing center hex height
            var pos = this.TileAt(0, 0, 0).transform.position;
            pos.y = this.CenterHexHeight;
            this.TileAt(0, 0, 0).transform.position = pos;

            foreach (var tile in this._grid.Values)
            {
                tile.Initialize();
            }
        }

        private void FixedUpdate()
        {
            //Adding new tiles to border
            foreach (var tile in this._addToBorder)
            {
                this.BorderHexes.Add(tile.GetHashCode(), tile);
            }
            this._addToBorder.Clear();

            //Removing dead tiles from border
            foreach (var tile in this._removeFromBorder)
            {
                this.BorderHexes.Remove(tile.GetHashCode());
            }
            this._removeFromBorder.Clear();

            //Erode the border only after cleaning dead tiles
            foreach (var tile in this.BorderHexes.Values)
            {
                tile.Erode();
            }
        }

        private IEnumerator WaitTimeBetweenWaves()
        {
            yield return new WaitForSeconds(0.5f);
            _isGridWaving = false;
        }
        #endregion

        #region Generation
        private void GenHexShape()
        {
            //Debug.Log("Generating hexagonal shaped grid...");

            Tile tile;
            Vector3 pos = Vector3.zero;

            int mapSize = Mathf.Max(this.mapWidth, this.mapHeight);
            for (int q = -mapSize; q <= mapSize; q++)
            {
                int r1 = Mathf.Max(-mapSize, -q - mapSize);
                int r2 = Mathf.Min(mapSize, -q + mapSize);
                for (int r = r1; r <= r2; r++)
                {
                    switch (this.hexOrientation)
                    {
                        case HexOrientation.Flat:
                            pos.x = this.hexRadius * 3.0f / 2.0f * q;
                            pos.z = this.hexRadius * Mathf.Sqrt(3.0f) * (r + q / 2.0f);
                            break;

                        case HexOrientation.Pointy:
                            pos.x = this.hexRadius * Mathf.Sqrt(3.0f) * (q + r / 2.0f);
                            pos.z = this.hexRadius * 3.0f / 2.0f * r;
                            break;
                    }
                    pos.y = Random.value * 0.28f;
                    tile = this.CreateHexGO(pos, ("Hex[" + q + "," + r + "," + (-q - r).ToString() + "]"));
                    tile.Index = new CubeIndex(q, r, -q - r);
                    this._grid.Add(tile.Index.ToString(), tile);
                }
            }
        }

        private void GenRectShape()
        {
            Debug.Log("Generating rectangular shaped grid...");

            Tile tile;
            Vector3 pos = Vector3.zero;

            switch (this.hexOrientation)
            {
                case HexOrientation.Flat:
                    for (int q = 0; q < this.mapWidth; q++)
                    {
                        int qOff = q >> 1;
                        for (int r = -qOff; r < this.mapHeight - qOff; r++)
                        {
                            pos.x = this.hexRadius * 3.0f / 2.0f * q;
                            pos.z = this.hexRadius * Mathf.Sqrt(3.0f) * (r + q / 2.0f);

                            tile = this.CreateHexGO(pos, ("Hex[" + q + "," + r + "," + (-q - r).ToString() + "]"));
                            tile.Index = new CubeIndex(q, r, -q - r);
                            this._grid.Add(tile.Index.ToString(), tile);
                        }
                    }
                    break;

                case HexOrientation.Pointy:
                    for (int r = 0; r < this.mapHeight; r++)
                    {
                        int rOff = r >> 1;
                        for (int q = -rOff; q < this.mapWidth - rOff; q++)
                        {
                            pos.x = this.hexRadius * Mathf.Sqrt(3.0f) * (q + r / 2.0f);
                            pos.z = this.hexRadius * 3.0f / 2.0f * r;

                            tile = this.CreateHexGO(pos, ("Hex[" + q + "," + r + "," + (-q - r).ToString() + "]"));
                            //tile.transform.parent.rotation = Quaternion.Euler(90, 180, 0);
                            tile.Index = new CubeIndex(q, r, -q - r);
                            this._grid.Add(tile.Index.ToString(), tile);
                        }
                    }
                    break;
            }
        }

        private void GenParrallShape()
        {
            Debug.Log("Generating parrellelogram shaped grid...");

            Tile tile;
            Vector3 pos = Vector3.zero;

            for (int q = 0; q <= this.mapWidth; q++)
            {
                for (int r = 0; r <= this.mapHeight; r++)
                {
                    switch (this.hexOrientation)
                    {
                        case HexOrientation.Flat:
                            pos.x = this.hexRadius * 3.0f / 2.0f * q;
                            pos.z = this.hexRadius * Mathf.Sqrt(3.0f) * (r + q / 2.0f);
                            break;

                        case HexOrientation.Pointy:
                            pos.x = this.hexRadius * Mathf.Sqrt(3.0f) * (q + r / 2.0f);
                            pos.z = this.hexRadius * 3.0f / 2.0f * r;
                            break;
                    }

                    tile = this.CreateHexGO(pos, ("Hex[" + q + "," + r + "," + (-q - r).ToString() + "]"));
                    tile.Index = new CubeIndex(q, r, -q - r);
                    this._grid.Add(tile.Index.ToString(), tile);
                }
            }
        }

        private void GenTriShape()
        {
            Debug.Log("Generating triangular shaped grid...");

            Tile tile;
            Vector3 pos = Vector3.zero;

            int mapSize = Mathf.Max(this.mapWidth, this.mapHeight);

            for (int q = 0; q <= mapSize; q++)
            {
                for (int r = 0; r <= mapSize - q; r++)
                {
                    switch (this.hexOrientation)
                    {
                        case HexOrientation.Flat:
                            pos.x = this.hexRadius * 3.0f / 2.0f * q;
                            pos.z = this.hexRadius * Mathf.Sqrt(3.0f) * (r + q / 2.0f);
                            break;

                        case HexOrientation.Pointy:
                            pos.x = this.hexRadius * Mathf.Sqrt(3.0f) * (q + r / 2.0f);
                            pos.z = this.hexRadius * 3.0f / 2.0f * r;
                            break;
                    }

                    tile = this.CreateHexGO(pos, ("Hex[" + q + "," + r + "," + (-q - r).ToString() + "]"));
                    tile.Index = new CubeIndex(q, r, -q - r);
                    this._grid.Add(tile.Index.ToString(), tile);
                }
            }
        }

        private Tile CreateHexGO(Vector3 postion, string name)
        {
            GameObject go = new GameObject(name, typeof(MeshFilter), typeof(MeshRenderer), typeof(Tile));
            GameObject pa = new GameObject("root-" + name);

            if (this.addColliders)
                go.AddComponent<MeshCollider>();

            pa.transform.parent = this.transform;
            go.transform.parent = pa.transform;
            pa.transform.position = postion;

            Tile tile = go.GetComponent<Tile>();
            MeshFilter fil = go.GetComponent<MeshFilter>();
            MeshRenderer ren = go.GetComponent<MeshRenderer>();

            fil.sharedMesh = this.hexMesh;
            ren.material = this.hexMaterial;

            if (this.addColliders)
            {
                MeshCollider col = go.GetComponent<MeshCollider>();
                col.sharedMesh = this.hexMesh;
            }

            go.tag = "Tile";

            return tile;
        }

        private void GetMesh()
        {
            if (this.hexMesh == null)
            {
                Tile.GetHexMesh(this.hexRadius, this.hexOrientation, ref this.hexMesh);
            }
        }
        #endregion
    }

    [System.Serializable]
    public enum MapShape
    {
        Rectangle,
        Hexagon,
        Parrallelogram,
        Triangle
    }

    [System.Serializable]
    public enum HexOrientation
    {
        Pointy,
        Flat
    }
}