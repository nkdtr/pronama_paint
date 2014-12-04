//#define USE_SKINNED_MESH

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// create monster cell tree by manual input
public class StrokeSeed : MonoBehaviour {
    
//    public SkinnedMeshRenderer skinnedMesh;

    /*
    class MarkerInfo
    {
        public string tag;
        public FixedSizeIconControl obj;
    }
     */

    public GameObject meshObj;
    public GameObject markersRoot;

//    public MonsterCellObject_Manual cellObjPrefab;
    //SkinnedMetaballCell _rootCell;
    protected MetaballCellCluster _cells = new MetaballCellCluster();

//    MonsterCellObject_Manual _currentDrag = null;

    public float radius = 1.0f;

    public float minDistance = 1.0f;
    public float maxDistance = 1.2f;

//    public GoalObjectSeed goalObjectSeedPrefab;
//    public StartObjectSeed startObjectSeedPrefab;

//    public UICamera sceneCamera;
    public Camera sceneCamera;

    public AudioClip addSE;
    public AudioClip removeSE;
    public AudioClip touchSE;

    public AudioClip gimmickSE;

    MetaballCell _startCell;
    MetaballCell _goalCell;

    public Material regularMaterial;
    public Material simpleMaterial;

    public bool InvokeEditEvent = false;
    public bool WireFrame = false;
    public bool bEditable = false;

//    GoalObject _goalObj;
//    StartObject _startObj;

    bool _bVerified = false;
    bool _bDirty = false;

//    List<MarkerInfo> _markers = new List<MarkerInfo>();

    protected virtual bool ContinueAdding
    {
        get { return false; }
    }

    void SetDirty()
    {
        _bDirty = true;
        _bVerified = false;

        /*
        MonsterManualGameControl control = FindObjectOfType<MonsterManualGameControl>();
        if (control)
        {
            control.SetDungeonDirty();
        }
         */
    }
    public void Verify()
    {
        _bVerified = true;
    }

    public Mesh CurrentMesh
    {
        get
        {
            return meshObj.GetComponent<MeshFilter>().sharedMesh;
        }
    }

	// Use this for initialization
	void Awake () {

        InitializeCellCluster();

        if (PlayerPrefs.GetInt("UseSimpleMaterial", 0) > 0)
        {
            meshObj.renderer.material = simpleMaterial;
        }
        else
        {
            meshObj.renderer.material = regularMaterial;
        }
	}

    void InitializeCellCluster()
    {
        _cells.ClearCells();
        _cells.BaseRadius = radius;
        _cells.AddCell(Vector3.zero, minDistance);

        _startCell = _goalCell = _cells.GetCell(0);

        SetupModel();
    }

    public void SetMaterial(Material mat)
    {
        meshObj.renderer.material = mat;
//        skinnedMesh.material = mat;
    }

    // mirror another seed
    /*
    public void Mirror(MonsterSeed other)
    {
        DungeonData data = other.CreateDungeonData();

        InitializeWithDungeonData(data);
    }
    */

    public virtual void Clear()
    {
        InitializeCellCluster();
    }

    /*
public void SetCoreActivity( bool bActive )
{
    bool bActualActive = bEditable && bActive;

    List<MonsterCellObjectCore_Manual> cores = Utils.GetComponentsRecursive<MonsterCellObjectCore_Manual>(transform);
    foreach (MonsterCellObjectCore_Manual core in cores)
    {
        core.gameObject.SetActive(bActualActive);
    }
}
     */

    // Update is called once per frame
	void Update () {
        bool bAddable = true;
        /*
        MonsterManualGameControl ctrl = FindObjectOfType<MonsterManualGameControl>();
        if (ctrl != null)
        {
            bAddable = ctrl.CanAddCell();
        }
        else
        {
            bAddable = _cells.CellCount < DungeonMakerConfiguration.Instance.DungeonCellCount;
        }
         */

        Camera tgtCam = sceneCamera;
        // try to spawn depending on distance
        Ray touchRay = tgtCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(touchRay, out hit, 100.0f))
        {
            Vector3 pos = hit.point;
        }
#if false
        if (/*_currentDrag &&*/ bAddable)
        {
            Camera tgtCam = sceneCamera;
            // try to spawn depending on distance
            Ray touchRay = tgtCam.ScreenPointToRay(Input.mousePosition);

            Vector3 camToTarget = _currentDrag.core.transform.position - tgtCam.transform.position;

            float distanceToSurface = Vector3.Dot(camToTarget, tgtCam.transform.forward);

            float spd = Vector3.Dot(touchRay.direction, tgtCam.transform.forward);

            Vector3 camToCollisionPoint = touchRay.direction * (distanceToSurface / spd);

            Vector3 collisionPoint = tgtCam.transform.position + camToCollisionPoint;


            Vector3 collisionPointLocal = meshObj.transform.InverseTransformPoint(collisionPoint);
            Vector3 currentDragLocal = meshObj.transform.InverseTransformPoint(_currentDrag.core.transform.position);

            Vector3 dif = collisionPointLocal - currentDragLocal;

            if (dif.magnitude >= _currentDrag.Cell.radius * minDistance)
            {
                if (dif.magnitude > _currentDrag.Cell.radius * maxDistance)
                {
                    dif = dif.normalized * _currentDrag.Cell.radius * maxDistance;
                    collisionPointLocal = currentDragLocal + dif;
                }
                // add new cell
                //bool bAdded = _currentDrag.Cell.AddChild(collisionPointLocal, _currentDrag.Cell.radius, minDistance);
                //bool bAdded = _rootCell.AddChild(collisionPointLocal, _currentDrag.Cell.radius, minDistance);
                MetaballCell newCell = _cells.AddCell(collisionPointLocal, minDistance);

                if (newCell != null)
                {
                    audio.PlayOneShot(addSE);
                    SetDirty();

                    TutorialManager.TryRemoveTutorial("TUT_EX_04");
                 //   _rootCell.DoForeach((c) => c.CalcRotation());
                    SetupModel();
                    // temp
                    if (ContinueAdding)
                    {
                        // does not work correctly (does not stop adding on release)
                        throw new System.NotImplementedException();

                        _currentDrag = null;
                        /*
                        // find new cellobj
                        List<MonsterCellObject_Manual> cells = Utils.GetComponentsRecursive<MonsterCellObject_Manual>(meshObj.transform);
                        foreach (MonsterCellObject_Manual c in cells)
                        {
                            if (c.Cell == newCell)
                            {
                                _currentDrag = c;
                                break;
                            }
                        }
                         */
                    }
                    else
                    {
                        _currentDrag = null;
                    }
					
                    /*
					MonsterManualGameControl game = FindObjectOfType<MonsterManualGameControl>();
					if( game )
					{
						game.OnCellCountChanged();
					}
                     */
                }
            }
        }
#endif
	}

    /*
    protected virtual void ModifyMeshData(Mesh mesh)
    {
    }
     */

    protected void SetupModel()
    {
        Utils.DestroyChildren(meshObj.transform);

        Mesh mesh;
       // Transform[] bones;
        //		MonsterMeshGenerator_Tree.Instance.CreateMesh( _individual, skinnedMesh.transform, out mesh, out bones );
      //  MetaballGenerator.Instance.CreateMeshWithSkeleton(_rootCell, meshObj.transform, out mesh, out bones, cellObjPrefab, true);
        MetaballGenerator.Instance.CreateMesh(_cells, meshObj.transform, out mesh, null, true);
        mesh.RecalculateBounds();
//        mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 500.0f);
        //mesh.RecalculateBounds();

       // skinnedMesh.GetComponent<MeshCollider>().sharedMesh = mesh;

        MeshFilter mf = meshObj.GetComponent<MeshFilter>();

        if (WireFrame)
        {
            Utils.ConvertMeshIntoWireFrame(mesh);
        }
//        ModifyMeshData(mesh);

        mf.sharedMesh = mesh;

#if USE_SKINNED_MESH
        SkinnedMeshRenderer skinnedMesh = meshObj.GetComponent<SkinnedMeshRenderer>();
        skinnedMesh.localBounds = new Bounds(Vector3.zero, Vector3.one * 500.0f);

        skinnedMesh.bones = bones;
        skinnedMesh.sharedMesh = mesh;
#else
//        meshObj.GetComponent<MeshRenderer>().sharedMesh = mesh;
#endif

#if false
        List<MonsterCellObject_Manual> cells = Utils.GetComponentsRecursive<MonsterCellObject_Manual>(meshObj.transform);
        foreach( MonsterCellObject_Manual cell in cells )
        {
            cell.SetSeed(this);

            /*
            if ( startObjectSeedPrefab != null && cell.Cell == _startCell )
            {
                GameObject startObj = (GameObject)Instantiate(startObjectSeedPrefab.gameObject);
                startObj.transform.position = cell.transform.position;
                startObj.transform.parent = meshObj.transform;
                startObj.transform.localRotation = Quaternion.identity;
                startObj.transform.localScale = Vector3.one;
            }

            if ( goalObjectSeedPrefab != null && cell.Cell == _goalCell )
            {
                GameObject goalObj = (GameObject)Instantiate(goalObjectSeedPrefab.gameObject);
                goalObj.transform.position = cell.transform.position;
                goalObj.transform.parent = meshObj.transform;
                goalObj.transform.localRotation = Quaternion.identity;
                goalObj.transform.localScale = Vector3.one;
            }
             */
        }
#endif

#if false
        if (InvokeEditEvent)
        {
            MonsterManualGameControl game = FindObjectOfType<MonsterManualGameControl>();
            if (game)
            {
                game.OnCellCountChanged();
            }
        }
#endif
    }
    /*
    public void SetMarker(string tag, FixedSizeIconControl markerPrefab, Vector3 position, Quaternion rotation)
    {
        if (markersRoot != null)
        {
            RemoveMarker(tag);

            FixedSizeIconControl obj = ((GameObject)(Instantiate(markerPrefab.gameObject))).GetComponent<FixedSizeIconControl>();

            MarkerInfo newMarker = new MarkerInfo();
            newMarker.tag = tag;
            newMarker.obj = obj;

            _markers.Add(newMarker);

            obj.transform.parent = markersRoot.transform;
            obj.transform.localPosition = position;
            obj.transform.localScale = Vector3.one;
            obj.transform.localRotation = rotation;

            MonsterManualGameControl control = FindObjectOfType<MonsterManualGameControl>();

            if (control != null)
            {
                Camera cam = control.editorCamera.camera;
                obj.targetCamera = cam;
                obj.Recalcurate();
            }
        }
    }

    public void RemoveMarker(string tag)
    {
        if (markersRoot != null)
        {
            MarkerInfo foundMarker = null;
            foreach (MarkerInfo m in _markers)
            {
                if (m.tag == tag)
                {
                    foundMarker = m;
                    break;
                }
            }
            if (foundMarker != null)
            {
                Destroy(foundMarker.obj.gameObject);
                _markers.Remove(foundMarker);
            }
        }
    }

    public void ClearMarker()
    {
        if (markersRoot != null)
        {
            _markers.Clear();
            Utils.DestroyChildren(markersRoot.transform);
        }
    }

    public void SetStartCell(MetaballCell cell)
    {
        if (_cells.FindCell(cell) >= 0)
        {
            if (_startCell != cell)
            {
                SetDirty();
                audio.PlayOneShot(gimmickSE);
                _startCell = cell;
                SetupModel();
            }
        }
    }

    public void SetGoalCell(MetaballCell cell)
    {
        if (_cells.FindCell(cell) >= 0)
        {
            if (_goalCell != cell)
            {
                SetDirty();
                audio.PlayOneShot(gimmickSE);
                _goalCell = cell;
                SetupModel();
            }
        }
    }
    */
    /*
    public void SetCurrentDragCell(MonsterCellObject_Manual cell)
    {
        audio.PlayOneShot(touchSE);
        _currentDrag = cell;
    }
     */

    public int GetCellCount()
    {
        return _cells.CellCount;
    }

    bool CanDeleteCell(MetaballCell cell)
    {
        return (GetCellCount() > 1 && cell != _startCell && cell != _goalCell);
    }

    public bool TryDeleteCell(MetaballCell cell)
    {
        if (CanDeleteCell(cell))
        {
            _cells.RemoveCell(cell);
            SetupModel();

            audio.PlayOneShot(removeSE);
            SetDirty();

            /*
			MonsterManualGameControl game = FindObjectOfType<MonsterManualGameControl>();
			if( game )
			{
				game.OnCellCountChanged();
			}
             */
            return true;
        }
        return false;
    }

    string GetCellPositionsString()
    {
      //  return _rootCell.GetStringExpression();
        return _cells.GetPositionsString();
    }

    void SetCellPositionsString(string data)
    {
        _cells.ReadPositionsString(data);
        /*
        SkinnedMetaballCell newCell = SkinnedMetaballCell.ConstructFromString(data, radius);
        _rootCell = newCell;
         */
    }

    /*
    public DungeonData CreateDungeonData()
    {
        DungeonData retval = new DungeonData(DungeonData.Mode.upload);

        retval.positions = GetCellPositionsString();
        retval.cellCount = GetCellCount();

        if (_startCell != null)
        {
            retval.start = _cells.FindCell(_startCell);
        }
        else
        {
            retval.start = -1;
        }

        if (_goalCell != null)
        {
            retval.goal = _cells.FindCell(_goalCell);
        }
        else
        {
            retval.goal = -1;
        }

        return retval;
    }

    public void InitializeWithDungeonData(DungeonData data)
    {
        SetCellPositionsString(data.positions);

        _startCell = _cells.GetCell(data.start);
        _goalCell = _cells.GetCell(data.goal);


        SetupModel();
    }

    public Vector3 GetStartPos()
    {
        return _startCell != null ? _startCell.modelPosition : Vector3.zero;
    }
     */
}
