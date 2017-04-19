using Assets.Scripts.Control;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.HexGridGenerator
{
    public class Tile : MonoBehaviour
    {
        public CubeIndex Index;

        public List<Tile> Neighbours { get; set; }

        //PUBLIC ONLY FOR DEBUG
        public int _missingNeighbours = 0;

        public int MissingNeighbours
        {
            get { return this._missingNeighbours; }
            set
            {
                this._missingNeighbours = value;
                if (this._missingNeighbours >= 2 && this.Hp > 0)
                {
                    if (!Grid.inst.BorderHexes.ContainsKey(this.Index.GetHashCode()))
                    {
                        //The hex is starting to lose hp
                        Grid.inst.AddToBorder(this);

                        //Start shake
                        this.Shake();
                    }
                }
                else
                {
                    //The hex is no longer losing hp
                    Grid.inst.RemoveFromBorder(this);

                    //Stop shaking
                    this.ShakeStop();
                }
                if (this._missingNeighbours > 6)
                {
                    Debug.Log("PLS");
                }
            }
        }

        private int _maxHp;
        public int MaxHp
        {
            get;
            private set;
        }

        public int _hp = -1;
        public int Hp
        {
            get { return this._hp; }
            private set
            {
                //Do not do anything if new hp is the same as before
                if (value == this._hp)
                    return;

                var preHp = this._hp;
                this._hp = Mathf.Clamp(value, 0, this._maxHp);
                if (this._hp == 0 && preHp > 0)
                {
                    //Tile is dead
                    Grid.inst.RemoveFromBorder(this);
                    foreach (var neighbours in this.Neighbours)
                    {
                        neighbours.NotifyLostNeighbour();
                    }
                    this.Fall();
                }
                else if (preHp == 0)
                {
                    //Tile is restored
                    this.Show();
                    foreach (var neighbours in this.Neighbours)
                    {
                        neighbours.NotifyGainNeighbour();
                    }
                    if (this._missingNeighbours >= 2 && !Grid.inst.BorderHexes.ContainsKey(this.Index.GetHashCode()))
                    {
                        Grid.inst.AddToBorder(this);
                        this.Shake();
                    }
                }
            }
        }

        public bool IsTileWaving
        {
            get { return _isWaving; }
            set
            {
                if (_isWaving == false && value == true)
                {
                    RaycastHit hitInfo;
                    if (Physics.Raycast(new Ray(this.transform.position + new Vector3(0, .5f, 0), Vector3.up), out hitInfo, 20))
                    {
                        switch (hitInfo.collider.tag)
                        {
                            case "Hunter":
                                hitInfo.collider.gameObject.GetComponent<HunterController>().HitPoint -= 1;
                                Instantiate(Grid.inst.TileHitVfxPrefab, hitInfo.collider.transform.position, hitInfo.collider.transform.rotation);
                                break;
                            case "Shooter":
                                hitInfo.collider.gameObject.GetComponent<ShooterController>().HitPoint -= 1;
                                Instantiate(Grid.inst.TileHitVfxPrefab, hitInfo.collider.transform.position, hitInfo.collider.transform.rotation);
                                break;
                            case "Charger":
                                hitInfo.collider.gameObject.GetComponent<ChargerController>().HitPoint -= 1;
                                Instantiate(Grid.inst.TileHitVfxPrefab, hitInfo.collider.transform.position, hitInfo.collider.transform.rotation);
                                break;
                            default:
                                break;
                        }
                    }
                }
                _isWaving = value;
            }
        }

        private bool _isShaking = false;
        private bool _isWaving = false;
        private bool _repairing = false;
        private float _waveAnimationTimer = 0.0f;
        private int _hash = 0;

        public void Initialize()
        {
            this.RandomizeHp();
            //Setting up neighbours
            this.Neighbours = Grid.inst.Neighbours(this);
            this.MissingNeighbours = 6 - this.Neighbours.Count;
            this.gameObject.layer = 8; // Platform layer
        }

        //Periodic erosion
        public void Erode()
        {
            //Every second, a tile loses HexErodeRate*1000*MissingNeighbours hp
            int lostHp = (int)(Time.fixedDeltaTime * Grid.inst.HexErodeRate * 1000.0f) * (this.MissingNeighbours - 1);
            this.Hp -= lostHp;
        }

        //Instantaneous erosion
        public void Erode(int damage)
        {
            this.Hp -= damage;
        }

        //Annihilation
        public void ErodeKill()
        {
            this.Hp = 0;
        }

        public void RepairFromPosition(Vector3 startPos, bool isFromGun)
        {
            if (this.Hp > 0)
            {
                this.Hp = this._maxHp;
            }
            else
            {
                if (this._repairing)
                {
                    return;
                }
                else
                {
                    iTween.iTween.StopByName(this.gameObject, "Fall");

                    this._repairing = true;
                    this.GetComponent<MeshRenderer>().enabled = true;
                    this.GetComponent<MeshCollider>().enabled = false;
                    if (isFromGun)
                        ApplyRepairTweenFromGun(startPos);
                    else
                        ApplyRepairTweenGeneral();
                }
            }
        }

        public void ApplyRepairTweenFromGun(Vector3 startPos)
        {
            this.transform.localScale = Vector3.zero;
            this.transform.position = startPos;
            iTween.iTween.MoveTo(this.gameObject, iTween.iTween.Hash
                ("name", "Repair"
                    , "position", Vector3.zero
                    , "time", 0.25f
                    , "looptype", iTween.iTween.LoopType.none
                    , "easetype", iTween.iTween.EaseType.easeOutSine
                    , "islocal", true
                    , "oncomplete", "RepairComplete"
                    , "ignoretimescale", true
                ));

            iTween.iTween.ScaleTo(this.gameObject, iTween.iTween.Hash
                ("name", "Repair"
                    , "scale", Vector3.one
                    , "time", 0.25f
                    , "looptype", iTween.iTween.LoopType.none
                    , "easetype", iTween.iTween.EaseType.easeOutSine
                    , "ignoretimescale", true
                ));
        }

        public void ApplyRepairTweenGeneral()
        {
            this.transform.localScale = Vector3.zero;
            this.transform.localPosition = Vector3.zero;

            iTween.iTween.ScaleTo(this.gameObject, iTween.iTween.Hash
                ("name", "Repair"
                    , "scale", Vector3.one
                    , "time", 0.5f
                    , "looptype", iTween.iTween.LoopType.none
                    , "easetype", iTween.iTween.EaseType.easeOutSine
                    , "oncomplete", "RepairComplete"
                    , "ignoretimescale", true
                ));
            GetComponent<MeshRenderer>().material.Lerp(GetComponent<MeshRenderer>().material, Grid.inst.ShinyTileMaterial, 0.4f);

        }

        //This hex lost one of its neighbours
        public void NotifyLostNeighbour()
        {
            this.MissingNeighbours += 1;
        }

        //This hex gained a new neighbour
        public void NotifyGainNeighbour()
        {
            this.MissingNeighbours -= 1;
        }

        private void RepairComplete()
        {
            this._repairing = false;
            this.GetComponent<MeshCollider>().enabled = true;
            this.RandomizeHp();
            GetComponent<MeshRenderer>().material.Lerp(GetComponent<MeshRenderer>().material, Grid.inst.hexMaterial, 1f);
        }

        private void RandomizeHp()
        {
            //Random range of hp
            var random = 2 * Grid.inst.HexHpVariation * (UnityEngine.Random.value - 0.5f) * (float)Grid.inst.HexHP;
            this._maxHp = Grid.inst.HexHP + (int)random;
            this.Hp = this._maxHp;
        }

        private void Shake()
        {
            if (!this._isShaking)
            {
                iTween.iTween.ShakePosition(this.gameObject, iTween.iTween.Hash(
                    "name", "Shake",
                    "amount", new Vector3(Grid.inst.ShakeStrenght, Grid.inst.ShakeStrenght, Grid.inst.ShakeStrenght),
                    "time", 300.0f,
                    "looptype", iTween.iTween.LoopType.loop,
                    "onstarttarget", this.gameObject,
                    "onstart", "ResetTransform",
                    "ignoretimescale", false
                    ));

                this._isShaking = true;
            }
        }

        private void ShakeStop()
        {
            if (this._hp > 0)
            {
                this.ResetTransform();
            }
            iTween.iTween.StopByName(this.gameObject, "Shake");
            this._isShaking = false;
        }

        private bool IsShaking()
        {
            return _isShaking;
        }

        private void Fall()
        {
            this.ShakeStop();
            iTween.iTween.MoveBy(this.gameObject, iTween.iTween.Hash
                ("name", "Fall"
                    , "amount", -20 * this.transform.up
                    , "time", 2.0f
                    , "looptype", iTween.iTween.LoopType.none
                    , "easetype", iTween.iTween.EaseType.easeInQuart
                    , "oncomplete", "Hide"
                    , "ignoretimescale", false
                ));
            iTween.iTween.ScaleTo(this.gameObject, iTween.iTween.Hash
                ("name", "Fall"
                    , "scale", Vector3.zero
                    , "time", 2.0f
                    , "looptype", iTween.iTween.LoopType.none
                    , "easetype", iTween.iTween.EaseType.easeInExpo
                    , "ignoretimescale", false
                ));
            //this.GetComponent<MeshCollider>().enabled = false;
        }

        public void Hide()
        {
            this.GetComponent<MeshRenderer>().enabled = false;
            //this.GetComponent<MeshCollider>().enabled = false;
        }

        private void Show()
        {
            iTween.iTween.Stop(this.gameObject);
            this.ResetTransform();
            this.GetComponent<MeshRenderer>().enabled = true;
            this.GetComponent<MeshCollider>().enabled = true;
        }

        private void ResetTransform()
        {
            this.transform.localPosition = Vector3.zero;
            this.transform.localRotation = Quaternion.identity;
            this.transform.localScale = Vector3.one;
            this._waveAnimationTimer = 0.0f;
            this.IsTileWaving = false;
        }

        private void UpdateYAxis()
        {
            this.transform.localPosition = new Vector3(0.0f, 1.5f * Mathf.Sin(2*Mathf.PI * _waveAnimationTimer) / (_waveAnimationTimer + (Mathf.PI / 4)), 0.0f);
            _waveAnimationTimer += Time.deltaTime;
        }

        private void ToWavingState()
        {
            this.IsTileWaving = true;
        }

        #region Coordinate Conversion Functions
        public static OffsetIndex CubeToEvenFlat(CubeIndex c)
        {
            OffsetIndex o;
            o.row = c.x;
            o.col = c.z + (c.x + (c.x & 1)) / 2;
            return o;
        }

        public static CubeIndex EvenFlatToCube(OffsetIndex o)
        {
            CubeIndex c;
            c.x = o.col;
            c.z = o.row - (o.col + (o.col & 1)) / 2;
            c.y = -c.x - c.z;
            return c;
        }

        public static OffsetIndex CubeToOddFlat(CubeIndex c)
        {
            OffsetIndex o;
            o.col = c.x;
            o.row = c.z + (c.x - (c.x & 1)) / 2;
            return o;
        }

        public static CubeIndex OddFlatToCube(OffsetIndex o)
        {
            CubeIndex c;
            c.x = o.col;
            c.z = o.row - (o.col - (o.col & 1)) / 2;
            c.y = -c.x - c.z;
            return c;
        }

        public static OffsetIndex CubeToEvenPointy(CubeIndex c)
        {
            OffsetIndex o;
            o.row = c.z;
            o.col = c.x + (c.z + (c.z & 1)) / 2;
            return o;
        }

        public static CubeIndex EvenPointyToCube(OffsetIndex o)
        {
            CubeIndex c;
            c.x = o.col - (o.row + (o.row & 1)) / 2;
            c.z = o.row;
            c.y = -c.x - c.z;
            return c;
        }

        public static OffsetIndex CubeToOddPointy(CubeIndex c)
        {
            OffsetIndex o;
            o.row = c.z;
            o.col = c.x + (c.z - (c.z & 1)) / 2;
            return o;
        }

        public static CubeIndex OddPointyToCube(OffsetIndex o)
        {
            CubeIndex c;
            c.x = o.col - (o.row - (o.row & 1)) / 2;
            c.z = o.row;
            c.y = -c.x - c.z;
            return c;
        }

        public static Tile operator +(Tile one, Tile two)
        {
            Tile ret = new Tile();
            ret.Index = one.Index + two.Index;
            return ret;
        }
        #endregion

        #region Generation
        public static Vector3 Corner(Vector3 origin, float radius, int corner, HexOrientation orientation)
        {
            float angle = 60 * corner;
            if (orientation == HexOrientation.Pointy)
                angle += 30;
            angle *= Mathf.PI / 180;
            return new Vector3(origin.x + radius * Mathf.Cos(angle), 0.0f, origin.z + radius * Mathf.Sin(angle));
        }

        public static void GetHexMesh(float radius, HexOrientation orientation, ref Mesh mesh)
        {
            mesh = new Mesh();

            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            for (int i = 0; i < 6; i++)
                verts.Add(Corner(Vector3.zero, radius, i, orientation));

            tris.Add(0);
            tris.Add(2);
            tris.Add(1);

            tris.Add(0);
            tris.Add(5);
            tris.Add(2);

            tris.Add(2);
            tris.Add(5);
            tris.Add(3);

            tris.Add(3);
            tris.Add(5);
            tris.Add(4);

            //UVs are wrong, I need to find an equation for calucalting them
            uvs.Add(new Vector2(0.5f, 1f));
            uvs.Add(new Vector2(1, 0.75f));
            uvs.Add(new Vector2(1, 0.25f));
            uvs.Add(new Vector2(0.5f, 0));
            uvs.Add(new Vector2(0, 0.25f));
            uvs.Add(new Vector2(0, 0.75f));

            mesh.vertices = verts.ToArray();
            mesh.triangles = tris.ToArray();
            mesh.uv = uvs.ToArray();

            mesh.name = "Hexagonal Plane";

            mesh.RecalculateNormals();
        }

        public override int GetHashCode()
        {
            if (this._hash == 0)
            {
                this._hash = (this.Index.x.GetHashCode() ^ (this.Index.y.GetHashCode() + (int)(Mathf.Pow(2, 32) / (1 + Mathf.Sqrt(5)) / 2) + (this.Index.x.GetHashCode() << 6) + (this.Index.x.GetHashCode() >> 2)));
            }
            return this._hash;
        }
        #endregion

        #region A* Herustic Variables
        public int MoveCost { get; set; }
        public int GCost { get; set; }
        public int HCost { get; set; }
        public int FCost { get { return this.GCost + this.HCost; } }
        public Tile Parent { get; set; }
        #endregion
    }

    [System.Serializable]
    public struct OffsetIndex
    {
        public int row;
        public int col;

        public OffsetIndex(int row, int col)
        {
            this.row = row; this.col = col;
        }
    }

    [System.Serializable]
    public struct CubeIndex
    {
        public int x;
        public int y;
        public int z;

        public CubeIndex(int x, int y, int z)
        {
            this.x = x; this.y = y; this.z = z;
        }

        public CubeIndex(int x, int z)
        {
            this.x = x; this.z = z; this.y = -x - z;
        }

        public static CubeIndex operator +(CubeIndex one, CubeIndex two)
        {
            return new CubeIndex(one.x + two.x, one.y + two.y, one.z + two.z);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            CubeIndex o = (CubeIndex)obj;
            if ((System.Object)o == null)
                return false;
            return ((this.x == o.x) && (this.y == o.y) && (this.z == o.z));
        }

        public override int GetHashCode()
        {
            return (this.x.GetHashCode() ^ (this.y.GetHashCode() + (int)(Mathf.Pow(2, 32) / (1 + Mathf.Sqrt(5)) / 2) + (this.x.GetHashCode() << 6) + (this.x.GetHashCode() >> 2)));
        }

        public override string ToString()
        {
            return string.Format("[" + this.x + "," + this.y + "," + this.z + "]");
        }
    }
}