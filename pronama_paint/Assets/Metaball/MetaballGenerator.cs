using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MetaballGenerator
{
	class MB3DCubeVector
	{
		public sbyte [] e = new sbyte[3];
		public sbyte x
		{
			get { return e[0]; }
			set { e [0] = value; }
		}
		public sbyte y
		{
			get { return e [1]; }
			set { e [1] = value; }
		}
		public sbyte z
		{
			get { return e [2]; }
			set { e [2] = value; }
		}
		public sbyte axisIdx=-1;

		public MB3DCubeVector()
		{
		}

		public MB3DCubeVector( sbyte x_, sbyte y_, sbyte z_ )
		{
			x=x_; y=y_; z=z_; axisIdx = -1;
			CalcAxis();
		}

		public static MB3DCubeVector operator+( MB3DCubeVector lh, MB3DCubeVector rh )
		{
			return new MB3DCubeVector ( (sbyte)(lh.x+rh.x), (sbyte)(lh.y+rh.y), (sbyte)(lh.z+rh.z) );
		}
		
		void CalcAxis()
		{
			for( sbyte i=0; i<3; ++i )
			{
				if( e[i] != 0 )
				{
					if( axisIdx==-1 )
					{
						axisIdx = i;
					}
					else
					{
						axisIdx = -1;
						break;
					}
				}
			}
		}
		
		public void SetAxisVector( sbyte axisIdx_, sbyte value )
		{
			x=y=z=0;
			if( value == 0 )
			{
				axisIdx = -1;
			}
			else
			{
				axisIdx = axisIdx_;
				e[axisIdx] = value;
			}
		}
		
		public static MB3DCubeVector operator*( MB3DCubeVector lh, sbyte rh )
		{
			return new MB3DCubeVector( (sbyte)(lh.x*rh), (sbyte)(lh.y*rh), (sbyte)(lh.z*rh) );
		}
	}
    
    class MB3DCubeInOut
	{
		// z, y, x
		public sbyte[,,] bFill = new sbyte[2,2,2];
		public int inCount;
		
		public MB3DCubeInOut()
		{
		}
		public MB3DCubeInOut( sbyte patternIdx )
		{
			sbyte [] bInOut = new sbyte[8];
			for( int i=0; i<8; ++i )
			{
				bInOut[i] = (sbyte)( ( patternIdx>>i ) & 1);
			}
			
			Init( bInOut[0], bInOut[1], bInOut[2], bInOut[3], bInOut[4], bInOut[5], bInOut[6], bInOut[7] );
		}
		public MB3DCubeInOut( sbyte a0, sbyte a1, sbyte a2, sbyte a3, sbyte a4, sbyte a5, sbyte a6, sbyte a7 )
		{
			Init( a0, a1, a2, a3, a4, a5, a6, a7 );
		}
		
		void Init( sbyte a0, sbyte a1, sbyte a2, sbyte a3, sbyte a4, sbyte a5, sbyte a6, sbyte a7 )
		{
			bFill[0,0,0] = a0;
			bFill[0,0,1] = a1;
			bFill[0,1,0] = a2;
			bFill[0,1,1] = a3;
			bFill[1,0,0] = a4;
			bFill[1,0,1] = a5;
			bFill[1,1,0] = a6;
			bFill[1,1,1] = a7;
			
			inCount = a0+a1+a2+a3+a4+a5+a6+a7;
		}
		
		public sbyte At( MB3DCubeVector point )
		{
			return bFill[point.z,point.y,point.x];
		}
	}
    
	struct MB3DCubePrimitivePattern
	{
		public MB3DCubeInOut InOut;
		
		public int [] IndexBuf;
        public int [] IndexBufAlter;
        public int IndexCount
        {
            get
            {
                if (IndexBuf != null)
                {
                    return IndexBuf.Length;
                }
                else
                {
                    return 0;
                }
            }
        }
        public int IndexCountAlter
        {
            get
            {
                if (IndexBufAlter != null)
                {
                    return IndexBufAlter.Length;
                }
                else
                {
                    return 0;
                }
            }
        }
	}
    
	class MB3DPatternMatchingInfo
    {
        // result, 0~14
        public int PrimaryPatternIndex;

        // 1: flip in/out
        public bool bFlipInOut;
        // 2: origin( point zero )
        public MB3DCubeVector Origin = new MB3DCubeVector();
        // 3: axis.
        public MB3DCubeVector[] Axis = new MB3DCubeVector[3];


		public void Match( MB3DCubeInOut src )
        {
	        // in count
	        PrimaryPatternIndex=-1;
	
	        bFlipInOut = src.inCount > 4;

	        for( int i=0; i<MB3D_PATTERN_COUNT; ++i )
	        {
		        MB3DCubeInOut tgt = __primitivePatterns[i].InOut;

		        if( bFlipInOut )
		        {
			        if( (8-src.inCount) != tgt.inCount )
			        {
				        continue;
			        }
		        }
		        else
		        {
			        if( src.inCount != tgt.inCount )
			        {
				        continue;
			        }
		        }

		        sbyte [] AxisDir = new sbyte[3];
		        for( Origin.x=0; Origin.x<2; ++Origin.x )
		        {
			        AxisDir[0] = (sbyte)( ( Origin.x != 0 ) ? -1 : 1 );
			        for( Origin.y=0; Origin.y<2; ++Origin.y )
			        {
				        AxisDir[1] = (sbyte)( ( Origin.y != 0 ) ? -1 : 1 );
				        for( Origin.z=0; Origin.z<2; ++Origin.z )
				        {
					        AxisDir[2] = (sbyte)( ( Origin.z != 0 ) ? -1 : 1 );

					        sbyte AxisOrder = (sbyte)( ( ( ( Origin.x + Origin.y + Origin.z ) % 2 ) != 0 ) ? 2 : 1 );
					
					        for( sbyte StartingAxis=0; StartingAxis<3; ++StartingAxis )
					        {
						        // ここで座標軸が決定
						        // 原点 : bStartingPeak[3]
						        // 軸1 : StartingAxis
						        // 軸2 : StartingAxis+AxisOrder;
						        // 軸3 : StartingAxis+AxisOrder+AxisOrder;
                                Axis[0] = new MB3DCubeVector();
                                Axis[1] = new MB3DCubeVector();
                                Axis[2] = new MB3DCubeVector();
						        Axis[0].SetAxisVector( StartingAxis, AxisDir[StartingAxis] );
						        Axis[1].SetAxisVector( (sbyte)( ( StartingAxis+AxisOrder ) % 3 ), (sbyte)( AxisDir[( StartingAxis+AxisOrder ) % 3] ) );
						        Axis[2].SetAxisVector( (sbyte)( ( StartingAxis+AxisOrder+AxisOrder ) % 3 ), (sbyte)( AxisDir[( StartingAxis+AxisOrder+AxisOrder ) % 3] ) );

						        // start matching
						        bool bMatch = true;
						        for( sbyte a0=0; a0<2; ++a0 )
						        {
							        for( sbyte a1=0; a1<2; ++a1 )
							        {
								        for( sbyte a2=0; a2<2; ++a2 )
								        {
									        MB3DCubeVector point = SampleVertex( new MB3DCubeVector(a0,a1,a2) );

									        if( ( bFlipInOut != ( src.At( point ) == tgt.bFill[a2,a1,a0] ) ) )
									        {
									        }
									        else
									        {
										        bMatch = false;
									        }
								        }
							        }
						        }

						        if( bMatch )
						        {
							        PrimaryPatternIndex = i;
							
							        return;
						        }
					        }
				        }
			        }
		        }
	        }
        }

        public int[] GetTargetPrimitiveIndexBuffer()
        {
            return (bFlipInOut && __primitivePatterns[PrimaryPatternIndex].IndexCountAlter > 0) ? __primitivePatterns[PrimaryPatternIndex].IndexBufAlter : __primitivePatterns[PrimaryPatternIndex].IndexBuf;
        }
		
		public MB3DCubeVector SampleVertex( MB3DCubeVector primaryPos )
        {
            return Origin + Axis[0] * primaryPos.x + Axis[1] * primaryPos.y + Axis[2] * primaryPos.z;
        }

		public void SampleSegment( sbyte primarySegmentID, out sbyte out_axis, out sbyte out_a_1, out sbyte out_a_2 )
        {
            sbyte primary_axis = (sbyte)( primarySegmentID / 4 );
            sbyte primary_a_1 = (sbyte)( primarySegmentID % 2 );
            sbyte primary_a_2 = (sbyte)( (primarySegmentID / 2) % 2 );

            out_axis = Axis[primary_axis].axisIdx;

//            sbyte a_1_idx = Axis[(primary_axis + 1) % 3].axisIdx;
//            sbyte a_2_idx = Axis[(primary_axis + 2) % 3].axisIdx;

            MB3DCubeVector pos = Origin + Axis[(primary_axis + 1) % 3] * primary_a_1 + Axis[(primary_axis + 2) % 3] * primary_a_2;

            //	out_a_1 = pos.e[a_1_idx];
            //	out_a_2 = pos.e[a_2_idx];

            sbyte primary_a_1_idx = (sbyte)( (out_axis + 1) % 3 );
            sbyte primary_a_2_idx = (sbyte)( (out_axis + 2) % 3 );
            out_a_1 = pos.e[primary_a_1_idx];
            out_a_2 = pos.e[primary_a_2_idx];
        }
		
	}

	class MB3DCubePattern
    {
        public int PatternIdx;
        public MB3DPatternMatchingInfo MatchingInfo = new MB3DPatternMatchingInfo();
        public int[] IndexBuf = new int[12];
        //		MB3DCubePrimitivePattern *TargetPattern;

        public void Init(int patternIdx)
        {
	        PatternIdx = patternIdx;

	        MB3DCubeInOut inOut = new MB3DCubeInOut( (sbyte)patternIdx);

	        MatchingInfo.Match( inOut );
	
	        // init index buffer
            int[] targetIdxBuffer = MatchingInfo.GetTargetPrimitiveIndexBuffer();// (MatchingInfo.bFlipInOut && __primitivePatterns[MatchingInfo.PrimaryPatternIndex].IndexCountAlter > 0) ? __primitivePatterns[MatchingInfo.PrimaryPatternIndex].IndexBufAlter : __primitivePatterns[MatchingInfo.PrimaryPatternIndex].IndexBuf;
            for (int i = 0; i < targetIdxBuffer.Length; ++i)
	        {
		        sbyte axis, a_1, a_2;
                MatchingInfo.SampleSegment((sbyte)(targetIdxBuffer[i]), out axis, out a_1, out a_2);

		        // reverse order if flip
                int targetIdx = MatchingInfo.bFlipInOut ? (targetIdxBuffer.Length- i - 1) : i;
		        IndexBuf[targetIdx] = axis*4+a_2*2+a_1;
	        }
        }
	};
    
	static bool __bCubePatternsInitialized=false;

	static MB3DCubePattern [] __cubePatterns = new MB3DCubePattern[256];

    static void __InitCubePatterns()
    {
        for (int i = 0; i < 256; ++i)
        {
            __cubePatterns[i] = new MB3DCubePattern();
            __cubePatterns[i].Init((sbyte)i);
        }
        __bCubePatternsInitialized = true;
    }
    
	const int MB3D_PATTERN_COUNT = 15;
	static MB3DCubePrimitivePattern [] __primitivePatterns = new MB3DCubePrimitivePattern[MB3D_PATTERN_COUNT]{

		// 0 points
        // #0
        new MB3DCubePrimitivePattern()
		{
			InOut = new MB3DCubeInOut(0, 0, 0, 0, 0, 0, 0, 0),
			IndexBuf = new int[]{}
		},
		
		// 1 points
        // #1
        new MB3DCubePrimitivePattern()
		{
			InOut = new MB3DCubeInOut(1, 0, 0, 0, 0, 0, 0, 0),
			IndexBuf = new int[]{0,4,8}
		},

		// 2 points
        // #2
        new MB3DCubePrimitivePattern()
		{
			//02
			InOut = new MB3DCubeInOut(1, 0, 1, 0, 0, 0, 0, 0),
			IndexBuf = new int[]{1,10,0,8,0,10}
		},
        // #3
        new MB3DCubePrimitivePattern()
		{
			//06
			InOut = new MB3DCubeInOut(1, 0, 0, 0, 0, 0, 1, 0),
			IndexBuf = new int[]{0,4,8,3,5,10},
            IndexBufAlter = new int[]{0,3,8,5,8,3,0,4,3,10,3,4}
		},
        // #4
        new MB3DCubePrimitivePattern()
		{
			//07
			InOut = new MB3DCubeInOut(1, 0, 0, 0, 0, 0, 0, 1),
			IndexBuf = new int[]{0,4,8,3,11,7}
		},

		// 3 points
        // #5
        new MB3DCubePrimitivePattern()
		{
			//123
			InOut = new MB3DCubeInOut(0, 1, 1, 1, 0, 0, 0, 0),
			IndexBuf = new int[]{10,4,0,10,0,9,10,9,11}
		},
        // #6
        new MB3DCubePrimitivePattern()
		{
			//027
			InOut = new MB3DCubeInOut(1, 0, 1, 0, 0, 0, 0, 1),
			IndexBuf = new int[]{1,10,0,8,0,10,3,11,7}
		},
        // #7
        new MB3DCubePrimitivePattern()
		{
			//247
			InOut = new MB3DCubeInOut(0, 0, 1, 0, 1, 0, 0, 1),
			IndexBuf = new int[]{10,4,1,2,8,5,3,11,7}
		},

		// 4 points
        // #8
        new MB3DCubePrimitivePattern()
		{
			//0123
			InOut = new MB3DCubeInOut(1, 1, 1, 1, 0, 0, 0, 0),
			IndexBuf = new int[]{10,8,11,9,11,8}
		},        
        // #9
        new MB3DCubePrimitivePattern()
		{
			//0135
			InOut = new MB3DCubeInOut(1, 1, 0, 1, 0, 1, 0, 0),
			IndexBuf = new int[]{2,7,8,8,7,4,4,7,11,4,11,1}
		},
        // #10
        new MB3DCubePrimitivePattern()
		{
			//0347
			InOut = new MB3DCubeInOut(1, 0, 0, 1, 1, 0, 0, 1),
			IndexBuf = new int[]{2,0,5,4,5,0,3,1,7,6,7,1}
		},
        // #11
        new MB3DCubePrimitivePattern()
		{
			//0137
			InOut = new MB3DCubeInOut(1, 1, 0, 1, 0, 0, 0, 1),
			IndexBuf = new int[]{8,9,4,4,9,3,7,3,9,1,4,3}
		},
        // #12
        new MB3DCubePrimitivePattern()
		{
			//1234
			InOut = new MB3DCubeInOut(0, 1, 1, 1, 1, 0, 0, 0),
			IndexBuf = new int[]{2,8,5,10,4,0,10,0,9,10,9,11}
		},
        // #13
        new MB3DCubePrimitivePattern()
		{
			//0356
			InOut = new MB3DCubeInOut(1, 0, 0, 1, 0, 1, 1, 0),
			IndexBuf = new int[]{0,4,8,3,5,10,2,7,9,11,1,6}
		},
        // #14
        new MB3DCubePrimitivePattern()
		{
			//1235
			InOut = new MB3DCubeInOut(0, 1, 1, 1, 0, 1, 0, 0),
			IndexBuf = new int[]{2,4,0,2,11,4,7,11,2,10,4,11}
		},
	};

    static MetaballGenerator()
    {
        __InitCubePatterns();
    }
    
    static float CalcPower( Vector3 relativePos, float radius, float density )
    {
	    return density * Mathf.Max( 1.0f - relativePos.magnitude/(radius), 0.0f );
    }

    static MetaballGenerator _instance;
    public static MetaballGenerator Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MetaballGenerator();
            }
            return _instance;
        }
    }

    public class MetaballPointInfo
    {
        public Vector3 center;
        public float radius;
        public float density;
    }

    public void CreateMesh(MetaballCellClusterInterface rootCell, Transform root, out Mesh out_mesh, MetaballCellObject cellObjPrefab = null, bool bReverse = false)
    {
        Mesh mesh = new Mesh();

        Bounds bounds;
        MetaballPointInfo[] points;

        AnalyzeCellTree(rootCell, root, out bounds, out points, cellObjPrefab);
        float cellSize = rootCell.BaseRadius * 0.3f;
        float powerThreshold = 0.4f;

        SetGeometory(mesh, bounds.center, bounds.extents, cellSize, points, powerThreshold, bReverse);

        // set boneweights

        out_mesh = mesh;
    }
#if false
    public void CreateMeshWithSkeleton(SkinnedMetaballCell rootCell/* MonsterIndividual_Tree monster*/, Transform root, out Mesh out_mesh, out Transform[] out_bones, MetaballCellObject cellObjPrefab = null, bool bReverse = false)
    {
        Mesh mesh = new Mesh();

        Bounds bounds;
        Transform [] bones;
        Matrix4x4 [] bindPoses;
        MetaballPointInfo [] points;

        AnalyzeCellTreeWithSkeleton(rootCell, root, out bounds, out bones, out bindPoses, out points, cellObjPrefab);
        float cellSize = rootCell.radius * 0.3f;
        float powerThreshold = 0.4f;

        mesh.bindposes = bindPoses;
        SetGeometory(mesh, bounds.center, bounds.extents, cellSize, points, powerThreshold, bReverse);

        // set boneweights

        out_mesh = mesh;
        out_bones = bones;
    }
#endif
    void AnalyzeCellTree(MetaballCellClusterInterface cellCluster, Transform root, out Bounds bounds, out MetaballPointInfo[] ballPoints, MetaballCellObject cellObjPrefab = null)
    {
        int cellCount = cellCluster.CellCount;

        Bounds tmpBounds = new Bounds(Vector3.zero, Vector3.zero);

        MetaballPointInfo[] tmpBallPoints = new MetaballPointInfo[cellCount];

        int cellIdx = 0;
        cellCluster.DoForeachCell((c) =>
        {
            // update bounds
            {
                for (int i = 0; i < 3; ++i)
                {
                    if (c.modelPosition[i] - c.radius < tmpBounds.min[i])
                    {
                        Vector3 tmp = tmpBounds.min;
                        tmp[i] = c.modelPosition[i] - c.radius;
                        tmpBounds.min = tmp;
                    }
                    if (c.modelPosition[i] + c.radius > tmpBounds.max[i])
                    {
                        Vector3 tmp = tmpBounds.max;
                        tmp[i] = c.modelPosition[i] + c.radius;
                        tmpBounds.max = tmp;
                    }
                }
            }

            // update skeleton
            {
                Vector3 pos = c.modelPosition;
                GameObject myBoneObj = null;

                if (cellObjPrefab)
                {
                    myBoneObj = (GameObject)GameObject.Instantiate(cellObjPrefab.gameObject);
                    myBoneObj.GetComponent<MetaballCellObject>().Setup(c);
                }
                else
                {
                    myBoneObj = new GameObject("Bone");
//                    myBoneObj.AddComponent<MonsterCellObject_Tree>().Setup(c as MonsterIndividual_Tree.Cell);
                }
                Transform myBone = myBoneObj.transform;

                {
                    myBone.parent = root;
                    myBone.localPosition = c.modelPosition;
                    myBone.localRotation = c.modelRotation;// Quaternion.identity;
                    myBone.localScale = Vector3.one;
                }
            }

            // update ball points
            {
                MetaballPointInfo point = new MetaballPointInfo();
                point.center = c.modelPosition;
                point.radius = c.radius;
                point.density = 1.0f;
                tmpBallPoints[cellIdx] = point;
            }

            ++cellIdx;
        });
        
        bounds = tmpBounds;
        ballPoints = tmpBallPoints;
    }

#if false
    void AnalyzeCellTreeWithSkeleton( SkinnedMetaballCell rootCell, Transform root, out Bounds bounds, out Transform [] bones, out Matrix4x4 [] bindPoses, out MetaballPointInfo[] ballPoints, MetaballCellObject cellObjPrefab=null)
    {
        int cellCount = rootCell.CellCount;

        Transform[] tmpBones = new Transform[cellCount];
        Matrix4x4[] tmpBindPoses = new Matrix4x4[cellCount];
        Bounds tmpBounds = new Bounds(Vector3.zero, Vector3.zero);

        MetaballPointInfo[] tmpBallPoints = new MetaballPointInfo[cellCount];

        Dictionary<SkinnedMetaballCell, int> boneDictionary = new Dictionary<SkinnedMetaballCell, int>();

        int cellIdx = 0;
        rootCell.DoForeachSkinnedCell((c) =>
        {
            // update bounds
            {
                for (int i = 0; i < 3; ++i)
                {
                    if (c.modelPosition[i] - c.radius < tmpBounds.min[i])
                    {
                        Vector3 tmp = tmpBounds.min;
                        tmp[i] = c.modelPosition[i] - c.radius;
                        tmpBounds.min = tmp;
                    }
                    if (c.modelPosition[i] + c.radius > tmpBounds.max[i])
                    {
                        Vector3 tmp = tmpBounds.max;
                        tmp[i] = c.modelPosition[i] + c.radius;
                        tmpBounds.max = tmp;
                    }
                }
            }

            // update skeleton
            {
                Vector3 pos = c.modelPosition;
                GameObject myBoneObj = null;

                if (cellObjPrefab)
                {
                    myBoneObj = (GameObject)GameObject.Instantiate(cellObjPrefab.gameObject);
                    myBoneObj.GetComponent<MetaballCellObject>().Setup(c);
                }
                else
                {
                    myBoneObj = new GameObject("Bone");
                    myBoneObj.AddComponent<MonsterCellObject_Tree>().Setup(c as MonsterIndividual_Tree.Cell);
                }
                Transform myBone = myBoneObj.transform;

                if (c.IsRoot)
                {
                    myBone.parent = root;
                    myBone.localPosition = Vector3.zero;
                    myBone.localRotation = c.modelRotation;// Quaternion.identity;
                    myBone.localScale = Vector3.one;
                }
                else
                {
                    Transform parentBone = tmpBones[boneDictionary[c.parent]];

                    myBone.parent = root;
                    myBone.localPosition = c.parent.modelPosition;
                    //myBone.localRotation = Quaternion.LookRotation(c.position - c.parent.position);
                    myBone.localRotation = c.modelRotation;
                    myBone.localScale = Vector3.one;

                    myBone.parent = parentBone;
                }

                tmpBones[cellIdx] = myBone;
                tmpBindPoses[cellIdx] = tmpBones[cellIdx].worldToLocalMatrix * root.localToWorldMatrix;

                boneDictionary.Add(c, cellIdx);
            }

            // update ball points
            {
                MetaballPointInfo point = new MetaballPointInfo();
                point.center = c.modelPosition;
                point.radius = c.radius;
                point.density = 1.0f;
                tmpBallPoints[cellIdx] = point;
            }

            ++cellIdx;
        });

        // set neighbors
        rootCell.DoForeachSkinnedCell((c) =>
        {
            MonsterCellObject_Tree myObj = tmpBones[boneDictionary[c]].GetComponent<MonsterCellObject_Tree>();

            foreach (MonsterIndividual_Tree.Cell tgt in c.links)
            {
                MonsterCellObject_Tree tgtObj = tmpBones[boneDictionary[tgt]].GetComponent<MonsterCellObject_Tree>();
                myObj.AddLink(tgtObj);
            }
        }
        );
        
        bounds = tmpBounds;
        bones = tmpBones;
        bindPoses = tmpBindPoses;
        ballPoints = tmpBallPoints;
    }
    
    Bounds CalcBounds(MonsterIndividual_Tree monster)
    {
        Bounds retval = new Bounds(Vector3.zero, Vector3.zero);
        monster.RootCell.DoForeachSkinnedCell((c) =>
        {
            for (int i = 0; i < 3; ++i)
            {
                if (c.modelPosition[i] - c.radius < retval.min[i])
                {
                    Vector3 tmp = retval.min;
                    tmp[i] = c.modelPosition[i] - c.radius;
                    retval.min = tmp;
                }
                if (c.modelPosition[i] + c.radius > retval.max[i])
                {
                    Vector3 tmp = retval.max;
                    tmp[i] = c.modelPosition[i] + c.radius;
                    retval.max = tmp;
                }
            }
        });
        return retval;
    }

    MetaballPointInfo [] CalcPoints(MonsterIndividual_Tree monster)
    {
        MetaballPointInfo[] points = new MetaballPointInfo[monster.CellCount];

        
        return points;
    }
#endif

    void SetGeometory( Mesh mesh, Vector3 center, Vector3 extent, float cellSize, MetaballPointInfo [] points, float powerThreshold, bool bReverse )
    {         
        int maxVertexCount = 30000;

	    if(!__bCubePatternsInitialized)
	    {
		    __InitCubePatterns();
	    }

//	    VerifyBuffers();

        int halfResolutionX = (int)Mathf.CeilToInt(extent.x/cellSize)+1;
        int halfResolutionY = (int)Mathf.CeilToInt(extent.y/cellSize)+1;
        int halfResolutionZ = (int)Mathf.CeilToInt(extent.z/cellSize)+1;

        int resolutionX = halfResolutionX*2;
        int resolutionY = halfResolutionY*2;
        int resolutionZ = halfResolutionZ*2;

        int gridResolutionX = resolutionX;
        int gridResolutionY = resolutionY;
        int gridResolutionZ = resolutionZ;

        Vector3 actualExtent = new Vector3(halfResolutionX*cellSize, halfResolutionY*cellSize, halfResolutionZ*cellSize );

	    Vector3 gridOrigin = center - actualExtent;


        float [] powerMap = new float[resolutionX*resolutionY*resolutionZ];
        Vector3 [] positionMap = new Vector3[resolutionX*resolutionY*resolutionZ];
        Vector3[] gradientMap = new Vector3[resolutionX * resolutionY * resolutionZ];
        int [] pointMap = new int[resolutionX*resolutionY*resolutionZ*3];
        bool [] inOutMap = new bool[resolutionX*resolutionY*resolutionZ];

        BoneWeight[] boneWeightMap = new BoneWeight[resolutionX * resolutionY * resolutionZ];
	
	    int gridStrideY = gridResolutionX;
	    int gridStrideZ = gridResolutionX*gridResolutionY;

	    int pointDirStride = gridResolutionX*gridResolutionY*gridResolutionZ;

	    for( int z=0; z<gridResolutionZ; ++z )
	    {
		    for( int y=0; y<gridResolutionY; ++y )
		    {
			    for( int x=0; x<gridResolutionX; ++x )
			    {
				    positionMap[x+y*gridStrideY+z*gridStrideZ] = gridOrigin + new Vector3(cellSize*x, cellSize*y, cellSize*z);
			    }
		    }
	    }
	
	    for( int i=0; i<3*gridResolutionZ*gridResolutionY*gridResolutionX; ++i )
	    {
		    pointMap[i] = -1;
	    }

        int pointIdx = 0;
        foreach( MetaballPointInfo pointInfo in points )
        {
            int minCellX = (int)Mathf.Floor((pointInfo.center.x - center.x - pointInfo.radius) / cellSize) + halfResolutionX;
            int minCellY = (int)Mathf.Floor((pointInfo.center.y - center.y - pointInfo.radius) / cellSize) + halfResolutionY;
            int minCellZ = (int)Mathf.Floor((pointInfo.center.z - center.z - pointInfo.radius) / cellSize) + halfResolutionZ;

            minCellX = Mathf.Max(0, minCellX);
            minCellY = Mathf.Max(0, minCellY);
            minCellZ = Mathf.Max(0, minCellZ);
         
            int maxCellX = (int)Mathf.Ceil((pointInfo.center.x - center.x + pointInfo.radius) / cellSize) + halfResolutionX;
            int maxCellY = (int)Mathf.Ceil((pointInfo.center.y - center.y + pointInfo.radius) / cellSize) + halfResolutionY;
            int maxCellZ = (int)Mathf.Ceil((pointInfo.center.z - center.z + pointInfo.radius) / cellSize) + halfResolutionZ;

            maxCellX = Mathf.Min(gridResolutionX - 1, maxCellX);
            maxCellY = Mathf.Min(gridResolutionY - 1, maxCellY);
            maxCellZ = Mathf.Min(gridResolutionZ - 1, maxCellZ);

            for( int cellZ = minCellZ; cellZ <= maxCellZ; ++cellZ )
            {
                for( int cellY = minCellY; cellY <= maxCellY; ++cellY )
                {
                    for( int cellX = minCellX; cellX <= maxCellX; ++cellX )
                    {
						Vector3 cellPos = positionMap[cellX + cellY*gridStrideY + cellZ*gridStrideZ];
                        
                        float power = CalcPower(cellPos - pointInfo.center, pointInfo.radius, pointInfo.density);

                        if( power > 0.0f )
                        {
						    powerMap[cellX + cellY*gridStrideY + cellZ*gridStrideZ] += power;

                            BoneWeight bw = boneWeightMap[cellX + cellY * gridStrideY + cellZ * gridStrideZ];
                            if (bw.weight0 < power || bw.weight1 < power)
                            {
                                if (bw.weight0 < bw.weight1)
                                {
                                    bw.weight0 = power;
                                    bw.boneIndex0 = pointIdx;
                                }
                                else
                                {
                                    bw.weight1 = power;
                                    bw.boneIndex1 = pointIdx;
                                }
                            }
                            boneWeightMap[cellX + cellY * gridStrideY + cellZ * gridStrideZ] = bw;
                        }
                    }
                }
            }
            ++pointIdx;
        }
	

	    // calc in/out
	    float threshold = powerThreshold;
        for( int i=0; i<gridResolutionX*gridResolutionY*gridResolutionZ; ++i )
        {
            inOutMap[i] = powerMap[i] >= threshold;

            if (inOutMap[i])
            {
                float powerEpsilon = 0.001f;
                if ( Mathf.Abs( powerMap[i] - threshold ) < powerEpsilon)
                {
                    if (powerMap[i] - threshold >= 0)
                    {
                        powerMap[i] = threshold + powerEpsilon;
                    }
                    else
                    {
                        powerMap[i] = threshold - powerEpsilon;
                    }
                }
            }
        }

	    // gradient
	    for( int z=1; z<gridResolutionZ-1; ++z )
	    {
		    for( int y=1; y<gridResolutionY-1; ++y )
		    {
			    for( int x=1; x<gridResolutionX-1; ++x )
			    {
				    gradientMap[x+y*gridStrideY+z*gridStrideZ].x = powerMap[(x+1)+y*gridStrideY+z*gridStrideZ]-powerMap[(x-1)+y*gridStrideY+z*gridStrideZ];
				    gradientMap[x+y*gridStrideY+z*gridStrideZ].y = powerMap[x+(y*1)*gridStrideY+z*gridStrideZ]-powerMap[x+(y-1)*gridStrideY+z*gridStrideZ];
				    gradientMap[x+y*gridStrideY+z*gridStrideZ].z = powerMap[x+y*gridStrideY+(z+1)*gridStrideZ]-powerMap[x+y*gridStrideY+(z-1)*gridStrideZ];

                    if (gradientMap[x + y * gridStrideY + z * gridStrideZ].sqrMagnitude > 0.001f)
                    {
                        gradientMap[x + y * gridStrideY + z * gridStrideZ].Normalize();
                    }
			    }
		    }
	    }

        int vertexCount = 0;
	    // create vertices
        List<Vector3> positionList = new List<Vector3>();
        List<Vector3> normalList = new List<Vector3>();
        List<BoneWeight> boneWeightList = new List<BoneWeight>();

	    for( int z=0; z<gridResolutionZ && vertexCount < maxVertexCount-1; ++z )
	    {
		    for( int y=0; y<gridResolutionY && vertexCount < maxVertexCount-1; ++y )
		    {
			    for( int x=0; x<gridResolutionX && vertexCount < maxVertexCount-1; ++x )
			    {
				    for( int dir=0; dir<3 && vertexCount < maxVertexCount-1; ++dir )
				    {
					    int dx = dir==0 ? 1 : 0;
					    int dy = dir==1 ? 1 : 0;
					    int dz = dir==2 ? 1 : 0;

					    if( z+dz < gridResolutionZ && y+dy < gridResolutionY && x+dx < gridResolutionX )
					    {
						    int idx0 = x+y*gridStrideY+z*gridStrideZ;
						    int idx1 = (x+dx)+(y+dy)*gridStrideY+(z+dz)*gridStrideZ;
						    float p0 = powerMap[idx0];
						    float p1 = powerMap[idx1];
						    if( ( p0 - threshold ) * ( p1 - threshold ) < 0.0f )
						    {
							    float alpha = ( threshold - p0 ) / ( p1 - p0 );
                                positionList.Add( positionMap[idx1]*alpha + positionMap[idx0]*(1.0f-alpha) );
							    //vertexBuffer[vertexCount].Color = FLinearColor(1.0f,1.0f,1.0f);
                                Vector3 tmpNormal = -(gradientMap[idx1] * alpha + gradientMap[idx0] * (1.0f - alpha)).normalized;
                                normalList.Add(bReverse ? -tmpNormal : tmpNormal);

                                BoneWeight sourceBW;
                                if (p0 > p1)
                                {
                                    sourceBW = boneWeightMap[idx0];
                                }
                                else
                                {
                                    sourceBW = boneWeightMap[idx1];
                                }

                                BoneWeight bw = new BoneWeight();
                                float weightSum = sourceBW.weight0+sourceBW.weight1;
                                if (weightSum > 0.0f)
                                {
                                    if (sourceBW.weight0 > sourceBW.weight1)
                                    {
                                        bw.weight0 = sourceBW.weight0 / weightSum;
                                        bw.boneIndex0 = sourceBW.boneIndex0;
                                        bw.weight1 = sourceBW.weight1 / weightSum;
                                        bw.boneIndex1 = sourceBW.boneIndex1;
                                    }
                                    else
                                    {
                                        bw.weight0 = sourceBW.weight1 / weightSum;
                                        bw.boneIndex0 = sourceBW.boneIndex1;
                                        bw.weight1 = sourceBW.weight0 / weightSum;
                                        bw.boneIndex1 = sourceBW.boneIndex0;
                                    }
                                }
                                else
                                {
                                    Debug.LogError("invalid boneweight");
                                }
                                boneWeightList.Add(bw);
//                            TODO : add to boneWeightList

							    pointMap[dir*pointDirStride+idx0] = vertexCount;

							    vertexCount++;
						    }
					    }
				    }
			    }
		    }
	    }

        int[] primaryPatternCounter = new int[MB3D_PATTERN_COUNT];

  //      Debug.Log("vertexCount=" + vertexCount);
   //     Debug.Log("boneCount=" + pointIdx);
	    // create indices
        int indexCount = 0;
        List<int> indexList = new List<int>();
	    if( vertexCount > 3 )
	    {
		    /*
		    for( unsigned int i=0; i<vertexCount-2 && indexCount < maxIndexCount-3; i+=3 )
		    {
			    indexBuffer[i] = i;
			    indexBuffer[i+1] = i+1;
			    indexBuffer[i+2] = i+2;

			    indexCount += 3;
		    }
		    */
		    for( int z=0; z<gridResolutionZ-1; ++z )
		    {
			    for( int y=0; y<gridResolutionY-1; ++y )
			    {
				    for( int x=0; x<gridResolutionX-1; ++x )
				    {
					    // inoutbuf->pattern idx
					    byte inOutBits=0;
					    for( int zoff=0; zoff<2; ++zoff )
					    {
						    for( int yoff=0; yoff<2; ++yoff )
						    {
							    for( int xoff=0; xoff<2; ++xoff )
							    {
								    if( inOutMap[(x+xoff)+(y+yoff)*gridStrideY+(z+zoff)*gridStrideZ] )
								    {
									    inOutBits |= (byte)( 1<<(zoff*4+yoff*2+xoff) );
								    }
							    }
						    }
					    }

					    /// create local vertex map
					    int [] localVertexMap = new int[12];
					    for( int dir=0; dir<3; ++dir )
					    {
						    for( int a_1=0; a_1<2; ++a_1 )
						    {
							    for( int a_2=0; a_2<2; ++a_2 )
							    {
								    int p_x, p_y, p_z;
								    switch( dir )
								    {
								    case 0:
									    p_x = x;
									    p_y = y+a_1;
									    p_z = z+a_2;
									    break;
								    case 1:
									    p_x = x+a_2;
									    p_y = y;
									    p_z = z+a_1;
									    break;
								    case 2:
									    p_x = x+a_1;
									    p_y = y+a_2;
									    p_z = z;
									    break;
								    default:
									    p_x=p_y=p_z=-1;
									    break;
								    }
								    int localIndex = dir*4+a_2*2+a_1;
								    localVertexMap[localIndex] = pointMap[dir*pointDirStride + p_x + p_y*gridStrideY + p_z*gridStrideZ];
							    }
						    }
					    }

                        int primaryPatternIdx = __cubePatterns[inOutBits].MatchingInfo.PrimaryPatternIndex;
                        primaryPatternCounter[primaryPatternIdx]++;

                        bool bErase = false;

                        /*
                        if (primaryPatternIdx == 3)
                        {
                            Debug.Log("grid:"+x+","+y+","+z);
                            bErase = true;  
                        }
                         */

                        if (!bErase)
                        {
//                            for (int idx = 0; idx < __primitivePatterns[__cubePatterns[inOutBits].MatchingInfo.PrimaryPatternIndex].IndexCount; ++idx)
                            for( int idx=0; idx<__cubePatterns[inOutBits].MatchingInfo.GetTargetPrimitiveIndexBuffer().Length; ++idx )
                            {
                                if (localVertexMap[__cubePatterns[inOutBits].IndexBuf[idx]] < 0)
                                {
                                    Debug.Log("(x,y,z)=" + x + "," + y + "," + z);
                                    Debug.Log("resolution=" + gridResolutionX + "," + gridResolutionY + "," + gridResolutionZ);
                                    {
                                        string tmp = "";
                                        for (int i = 0; i < 12; ++i)
                                        {
                                            tmp += (localVertexMap[i].ToString() + ",");
                                        }
                                        Debug.Log("localvtxmap=" + tmp);
                                    }
                                    Debug.Log("inout=" + System.Convert.ToString(inOutBits, 2));
                                    Debug.Log("idx=" + idx);
                                    Debug.Log("primaryPatternIdx=" + __cubePatterns[inOutBits].MatchingInfo.PrimaryPatternIndex);
                                    Debug.Log("indexCount=" + __primitivePatterns[__cubePatterns[inOutBits].MatchingInfo.PrimaryPatternIndex].IndexCount);

                                    {
                                        string tmp = "";
                                        for (int zoffset = 0; zoffset < 2; ++zoffset)
                                        {
                                            for (int yoffset = 0; yoffset < 2; ++yoffset)
                                            {
                                                for (int xoffset = 0; xoffset < 2; ++xoffset)
                                                {
                                                    int mapIdx = x + xoffset + (y + yoffset) * gridStrideY + (z + zoffset) * gridStrideZ;
                                                    tmp += (powerMap[mapIdx].ToString() + ",");
                                                }
                                            }
                                        }
                                        Debug.Log("powerMap=" + tmp);
                                    }
                                    throw new UnityException("vertex error");
                                }
                                indexList.Add(localVertexMap[__cubePatterns[inOutBits].IndexBuf[idx]]);
                                indexCount++;
                            }
                        }
				    }
			    }
		    }
	    }

        /*
        Debug.Log("dump positions");
        foreach (Vector3 pos in positionList)
        {
            Debug.Log(pos.ToString());
        }
        Debug.Log("dump normals");
        foreach (Vector3 n in normalList)
        {
            Debug.Log(n.ToString());
        }
        Debug.Log("dump triangles");
        foreach (int t in indexList)
        {
            Debug.Log(t.ToString());
        }
        Debug.Log("dump boneweight");
        foreach (BoneWeight w in boneWeightList)
        {
            Debug.Log(w.ToString());
        }
         */

        mesh.vertices = positionList.ToArray();
        mesh.normals = normalList.ToArray();
        if (!bReverse)
        {
            mesh.triangles = indexList.ToArray();
        }
        else
        {
            indexList.Reverse();
            mesh.triangles = indexList.ToArray();
        }
        mesh.boneWeights = boneWeightList.ToArray();

        /*
        Debug.Log("primary patterns");
        for (int i = 0; i < MB3D_PATTERN_COUNT; ++i)
        {
            Debug.Log(i.ToString()+":"+primaryPatternCounter[i]);
        }
         */
    }


#if false
	struct MB3DCubeVector
	{
		union
		{
			struct
			{
				char x, y, z;
			};
			char e[3];
		};
		
		char axisIdx;
		
		MB3DCubeVector()
		:x(0), y(0), z(0), axisIdx(-1)
		{
		}
		
		MB3DCubeVector( char x_, char y_, char z_ )
		:x(x_), y(y_), z(z_), axisIdx(-1)
		{
			CalcAxis();
		}
		
		MB3DCubeVector operator +( const MB3DCubeVector &rh )const
		{
			return MB3DCubeVector( x+rh.x, y+rh.y, z+rh.z );
		}
		
		void operator +=( const MB3DCubeVector &rh )
		{
			x += rh.x;
			y += rh.y;
			z += rh.z;
			
			CalcAxis();
		}
		
		void CalcAxis()
		{
			for( int i=0; i<3; ++i )
			{
				if( e[i] != 0 )
				{
					if( axisIdx==-1 )
					{
						axisIdx = i;
					}
					else
					{
						axisIdx = -1;
						break;
					}
				}
			}
		}
		
		void SetAxisVector( char axisIdx_, char value )
		{
			x=y=z=0;
			if( value == 0 )
			{
				axisIdx = -1;
			}
			else
			{
				axisIdx = axisIdx_;
				e[axisIdx] = value;
			}
		}
		
		MB3DCubeVector operator *( char rh )const
		{
			return MB3DCubeVector( x*rh, y*rh, z*rh );
		}
	};
	
	struct MB3DCubeInOut
	{
		// z, y, x
		char bFill[2][2][2];
		int inCount;
		
		MB3DCubeInOut()
		{
		}
		MB3DCubeInOut( unsigned char patternIdx )
		{
			char bInOut[8];
			for( int i=0; i<8; ++i )
			{
				bInOut[i] = ( patternIdx>>i ) & 1;
			}
			
			Init( bInOut[0], bInOut[1], bInOut[2], bInOut[3], bInOut[4], bInOut[5], bInOut[6], bInOut[7] );
		}
		MB3DCubeInOut( char a0, char a1, char a2, char a3, char a4, char a5, char a6, char a7 )
		{
			Init( a0, a1, a2, a3, a4, a5, a6, a7 );
		}
		
		void Init( char a0, char a1, char a2, char a3, char a4, char a5, char a6, char a7 )
		{
			bFill[0][0][0] = a0;
			bFill[0][0][1] = a1;
			bFill[0][1][0] = a2;
			bFill[0][1][1] = a3;
			bFill[1][0][0] = a4;
			bFill[1][0][1] = a5;
			bFill[1][1][0] = a6;
			bFill[1][1][1] = a7;
			
			inCount = a0+a1+a2+a3+a4+a5+a6+a7;
		}
		
		char At( const MB3DCubeVector &point )const
		{
			return bFill[point.z][point.y][point.x];
		}
	};
	
	struct MB3DCubePrimitivePattern
	{
		MB3DCubeInOut	InOut;
		
		int IndexBuf[12];
		int IndexCount;
	};
	
	struct MB3DPatternMatchingInfo
	{
	public:
			void Match( const MB3DCubeInOut &src );
		
		MB3DCubeVector SampleVertex( const MB3DCubeVector &primaryPos )const;
		void SampleSegment( char primarySegmentID, char &out_axis, char &out_a_1, char &out_a_2 )const;
		
	public:
			// result, 0~14
			int PrimaryPatternIndex;
		
		// 1: flip in/out
		bool bFlipInOut;
		// 2: origin( point zero )
		MB3DCubeVector Origin;
		// 3: axis.
		MB3DCubeVector Axis[3];
	};
	
	struct MB3DCubePattern
	{
		void Init(int patternIdx);
		
		int PatternIdx;
		MB3DPatternMatchingInfo MatchingInfo;
		int IndexBuf[12];
		//		MB3DCubePrimitivePattern *TargetPattern;
	};
	
	bool __bCubePatternsInitialized=false;
	
	MB3DCubePattern __cubePatterns[256];
	
	void __InitCubePatterns()
	{
		for( int i=0; i<256; ++i )
		{
			__cubePatterns[i].Init(i);
		}
		__bCubePatternsInitialized=true;
	}
	
	// IN側からみて反時計回りに頂点を指定しています
	const int MB3D_PATTERN_COUNT = 15;
	MB3DCubePrimitivePattern __primitivePatterns[MB3D_PATTERN_COUNT] = {
		
		// 0 points
		{
			MB3DCubeInOut(0, 0, 0, 0, 0, 0, 0, 0),
			{},
			0
		},
		
		// 1 points
		{
			MB3DCubeInOut(1, 0, 0, 0, 0, 0, 0, 0),
			{0,4,8},
			3
		},
		
		// 2 points
		{
			//02
			MB3DCubeInOut(1, 0, 1, 0, 0, 0, 0, 0),
			{1,10,0,8,0,10},
			6
		},
		{
			//06
			MB3DCubeInOut(1, 0, 0, 0, 0, 0, 1, 0),
			{0,4,8,3,5,10},
			6
		},
		{
			//07
			MB3DCubeInOut(1, 0, 0, 0, 0, 0, 0, 1),
			{0,4,8,3,11,7},
			6
		},
		
		// 3 points
		{
			//123
			MB3DCubeInOut(0, 1, 1, 1, 0, 0, 0, 0),
			{10,4,0,10,0,9,10,9,11},
			9
		},
		{
			//027
			MB3DCubeInOut(1, 0, 1, 0, 0, 0, 0, 1),
			{1,10,0,8,0,10,3,11,7},
			9
		},
		{
			//247
			MB3DCubeInOut(0, 0, 1, 0, 1, 0, 0, 1),
			{10,4,1,2,8,5,3,11,7},
			9
		},
		
		// 4 points
		{
			//0123
			MB3DCubeInOut(1, 1, 1, 1, 0, 0, 0, 0),
			{10,8,11,9,11,8},
			6
		},
		{
			//0135
			MB3DCubeInOut(1, 1, 0, 1, 0, 1, 0, 0),
			{2,7,8,8,7,4,4,7,11,4,11,1},
			12
		},
		{
			//0347
			MB3DCubeInOut(1, 0, 0, 1, 1, 0, 0, 1),
			{2,0,5,4,5,0,3,1,7,6,7,1},
			12
		},
		{
			//0137
			MB3DCubeInOut(1, 1, 0, 1, 0, 0, 0, 1),
			{8,9,4,4,9,3,7,3,9,1,4,3},
			12
		},
		{
			//1234
			MB3DCubeInOut(0, 1, 1, 1, 1, 0, 0, 0),
			{2,8,5,10,4,0,10,0,9,10,9,11},
			12
		},
		{
			//0356
			MB3DCubeInOut(1, 0, 0, 1, 0, 1, 1, 0),
			{0,4,8,3,5,10,2,7,9,11,1,6},
			12
		},
		{
			//1235
			MB3DCubeInOut(0, 1, 1, 1, 0, 1, 0, 0),
			{2,4,0,2,11,4,7,11,2,10,4,11},
			12
		},
	};
}

float CalcPower( const FVector relativePos, const float density )
{
	return density * FMath::Max( 1.0f - relativePos.SizeSquared()*0.01f, 0.0f );
}


void UB4BKinectMeshGeneratorMetaball3D::VerifyBuffers()
{
	if( gridResolutionX == cachedResX
	   && gridResolutionY == cachedResY
	   && gridResolutionZ == cachedResZ )
	{
		return;
	}
	else
	{
		const int cellCount = gridResolutionX*gridResolutionY*gridResolutionZ;
		powerMap.SetNum( cellCount );
		inOutMap.SetNum( cellCount );
		pointMap.SetNum( 3*cellCount );
		gradientMap.SetNum( cellCount );
		positionMap.SetNum( cellCount );
		
		cachedResX = gridResolutionX;
		cachedResY = gridResolutionY;
		cachedResZ = gridResolutionZ;
	}
}

void UB4BKinectMeshGeneratorMetaball3D::GenerateMesh( struct FKinectMeshVertex *vertexBuffer, uint32 &vertexCount, const uint32 maxVertexCount,
                                                     uint32 *indexBuffer, uint32 &indexCount, const uint32 maxIndexCount,
                                                     const uint16 *depthImage, const int imageSizeX, const int imageSizeY )
{
	SCOPE_CYCLE_COUNTER(B4BSTAT_METABALL3D);
	
	if(!__bCubePatternsInitialized)
	{
		__InitCubePatterns();
	}
	
	const int samplesX = 64;
	const int samplesY = 48;
	
	const int strideX = imageSizeX / samplesX;
	const int strideY = imageSizeY / samplesY;
	
	VerifyBuffers();
	/*
	const int gridResolutionX = 32;
	const int gridResolutionY = 32;
	const int gridResolutionZ = 32;


	float powerMap[gridResolutionZ][gridResolutionY][gridResolutionX];
	bool inOutMap[gridResolutionZ][gridResolutionY][gridResolutionX];


	int pointMap[3][gridResolutionZ][gridResolutionY][gridResolutionX];

	FVector gradientMap[gridResolutionZ][gridResolutionY][gridResolutionX];

	FVector positionMap[gridResolutionZ][gridResolutionY][gridResolutionX];

	const FVector gridCenter = FVector(200.0f, 0.0f, 0.0f);
	const FVector gridExtent = FVector(150.0f, 150.0f, 150.0f);
	*/
	
	const FVector gridOrigin = gridCenter-gridExtent;
	const FVector gridSize = gridExtent*2;
	
	const FVector cellSize = FVector( gridSize.X/(float)gridResolutionX, gridSize.Y/(float)gridResolutionY, gridSize.Z/(float)gridResolutionZ );
	
	const int maxRadius = 2;
	
	//	FMemory::Memzero( &powerMap[0], sizeof(float)*gridResolutionX*gridResolutionY*gridResolutionZ );
	FMemory::Memzero( &gradientMap[0], sizeof(FVector)*gridResolutionX*gridResolutionY*gridResolutionZ );
	
	const int gridStrideY = gridResolutionX;
	const int gridStrideZ = gridResolutionX*gridResolutionY;
	
	const int pointDirStride = gridResolutionX*gridResolutionY*gridResolutionZ;
	
	for( int z=0; z<gridResolutionZ; ++z )
	{
		for( int y=0; y<gridResolutionY; ++y )
		{
			for( int x=0; x<gridResolutionX; ++x )
			{
				positionMap[x+y*gridStrideY+z*gridStrideZ] = gridOrigin + FVector(cellSize.X*x, cellSize.Y*y, cellSize.Z*z);
			}
		}
	}
	
	for( int i=0; i<gridResolutionZ*gridResolutionY*gridResolutionX; ++i )
	{
		powerMap[i] *= FMath::Clamp( (1.0f-DeltaTime*AttenuationPerSecond), 0.0f, 0.99f );
	}
	for( int i=0; i<3*gridResolutionZ*gridResolutionY*gridResolutionX; ++i )
	{
		pointMap[i] = -1;
	}
	/*
	for( int i=0; i<3; ++i )
	{
		for( int z=0; z<gridResolutionZ; ++z )
		{
			for( int y=0; y<gridResolutionY; ++y )
			{
				for( int x=0; x<gridResolutionX; ++x )
				{
					pointMap[i*pointDirStride + x + y*gridStrideY + z*gridStrideZ] = -1;
				}
			}
		}
	}
	*/
	
	// powerMap
	for( int y=0; y<samplesY; ++y )
	{
		const int imageY = strideY * y;
		
		for( int x=0; x<samplesX; ++x )
		{
			const int imageX = strideX * x;
			
			const uint16 pixel = depthImage[imageX+imageY*imageSizeX];
			const uint32 playerIndex = pixel & NUI_IMAGE_PLAYER_INDEX_MASK;
			
			if( playerIndex > 0 && pixel <= NUI_IMAGE_DEPTH_MAXIMUM && pixel >= NUI_IMAGE_DEPTH_MINIMUM )// In_DepthTargetBits & (1<<playerIndex) )
			{				
				const Vector4 pos = NuiTransformDepthImageToSkeleton( imageX, imageY, pixel, NUI_IMAGE_RESOLUTION_640x480 );
				const FVector position = FVector( pos.z, pos.x, pos.y )*100.0f;
				
				const FVector relativePosition = position - gridOrigin;
				
				const FVector vCell = relativePosition / cellSize;
				
				const int gridX = FMath::Round(vCell.X);
				const int gridY = FMath::Round(vCell.Y);
				const int gridZ = FMath::Round(vCell.Z);
				
				for( int cellZ=FMath::Max(0,gridZ-maxRadius); cellZ <= FMath::Min(gridResolutionZ-1, gridZ+maxRadius); ++cellZ )
				{
					for( int cellY=FMath::Max(0,gridY-maxRadius); cellY <= FMath::Min(gridResolutionY-1, gridY+maxRadius); ++cellY )
					{
						for( int cellX=FMath::Max(0,gridX-maxRadius); cellX <= FMath::Min(gridResolutionX-1, gridX+maxRadius); ++cellX )
						{
							const FVector cellPos = positionMap[cellX + cellY*gridStrideY + cellZ*gridStrideZ];
							const float power = CalcPower( cellPos - position, position.X/100.0f );
							
							powerMap[cellX + cellY*gridStrideY + cellZ*gridStrideZ] += power;
						}
					}
				}
			}
		}
	}
	
	// calc in/out
	const float threshold = DensityThreshold;
	for( int z=0; z<gridResolutionZ; ++z )
	{
		for( int y=0; y<gridResolutionY; ++y )
		{
			for( int x=0; x<gridResolutionX; ++x )
			{
				if( powerMap[x+y*gridStrideY+z*gridStrideZ] == threshold )
				{
					powerMap[x+y*gridStrideY+z*gridStrideZ] += 0.001f;
				}
				
				inOutMap[x+y*gridStrideY+z*gridStrideZ] = powerMap[x+y*gridStrideY+z*gridStrideZ] > threshold;
			}
		}
	}
	
	// gradient
	for( int z=1; z<gridResolutionZ-1; ++z )
	{
		for( int y=1; y<gridResolutionY-1; ++y )
		{
			for( int x=1; x<gridResolutionX-1; ++x )
			{
				gradientMap[x+y*gridStrideY+z*gridStrideZ].X = powerMap[(x+1)+y*gridStrideY+z*gridStrideZ]-powerMap[(x-1)+y*gridStrideY+z*gridStrideZ];
				gradientMap[x+y*gridStrideY+z*gridStrideZ].Y = powerMap[x+(y*1)*gridStrideY+z*gridStrideZ]-powerMap[x+(y-1)*gridStrideY+z*gridStrideZ];
				gradientMap[x+y*gridStrideY+z*gridStrideZ].Z = powerMap[x+y*gridStrideY+(z+1)*gridStrideZ]-powerMap[x+y*gridStrideY+(z-1)*gridStrideZ];
				
				if( gradientMap[x+y*gridStrideY+z*gridStrideZ].SizeSquared() > 0.001f )
				{
					gradientMap[x+y*gridStrideY+z*gridStrideZ].Normalize();
				}
			}
		}
	}
	
	// create vertices
	for( int z=0; z<gridResolutionZ && vertexCount < maxVertexCount-1; ++z )
	{
		for( int y=0; y<gridResolutionY && vertexCount < maxVertexCount-1; ++y )
		{
			for( int x=0; x<gridResolutionX && vertexCount < maxVertexCount-1; ++x )
			{
				for( int dir=0; dir<3 && vertexCount < maxVertexCount-1; ++dir )
				{
					const int dx = dir==0 ? 1 : 0;
					const int dy = dir==1 ? 1 : 0;
					const int dz = dir==2 ? 1 : 0;
					
					if( z+dz < gridResolutionZ && y+dy < gridResolutionY && x+dx < gridResolutionX )
					{
						const int idx0 = x+y*gridStrideY+z*gridStrideZ;
						const int idx1 = (x+dx)+(y+dy)*gridStrideY+(z+dz)*gridStrideZ;
						const float p0 = powerMap[idx0];
						const float p1 = powerMap[idx1];
						if( ( p0 - threshold ) * ( p1 - threshold ) < 0.0f )
						{
							const float alpha = ( threshold - p0 ) / ( p1 - p0 );
							
							vertexBuffer[vertexCount].Position = positionMap[idx1]*alpha + positionMap[idx0]*(1.0f-alpha);
							vertexBuffer[vertexCount].Color = FLinearColor(1.0f,1.0f,1.0f);
							vertexBuffer[vertexCount].TangentZ = FPackedNormal( -(gradientMap[idx1]*alpha + gradientMap[idx0]*(1.0f-alpha)).SafeNormal() );
							
							pointMap[dir*pointDirStride+idx0] = vertexCount;
							
							vertexCount++;
						}
					}
				}
			}
		}
	}
	
	// create indices
	if( vertexCount > 3 )
	{
		/*
		for( unsigned int i=0; i<vertexCount-2 && indexCount < maxIndexCount-3; i+=3 )
		{
			indexBuffer[i] = i;
			indexBuffer[i+1] = i+1;
			indexBuffer[i+2] = i+2;

			indexCount += 3;
		}
		*/
		for( int z=0; z<gridResolutionZ-1; ++z )
		{
			for( int y=0; y<gridResolutionY-1; ++y )
			{
				for( int x=0; x<gridResolutionX-1; ++x )
				{
					// inoutbuf->pattern idx
					unsigned char inOutBits=0;
					for( int zoff=0; zoff<2; ++zoff )
					{
						for( int yoff=0; yoff<2; ++yoff )
						{
							for( int xoff=0; xoff<2; ++xoff )
							{
								if( inOutMap[(x+xoff)+(y+yoff)*gridStrideY+(z+zoff)*gridStrideZ] )
								{
									inOutBits |= (1<<(zoff*4+yoff*2+xoff));
								}
							}
						}
					}
					
					/// create local vertex map
					int localVertexMap[12];
					for( int dir=0; dir<3; ++dir )
					{
						for( int a_1=0; a_1<2; ++a_1 )
						{
							for( int a_2=0; a_2<2; ++a_2 )
							{
								int p_x, p_y, p_z;
								switch( dir )
								{
								case 0:
									p_x = x;
									p_y = y+a_1;
									p_z = z+a_2;
									break;
								case 1:
									p_x = x+a_2;
									p_y = y;
									p_z = z+a_1;
									break;
								case 2:
									p_x = x+a_1;
									p_y = y+a_2;
									p_z = z;
									break;
								default:
									p_x=p_y=p_z=-1;
									break;
								}
								const int localIndex = dir*4+a_2*2+a_1;
								localVertexMap[localIndex] = pointMap[dir*pointDirStride + p_x + p_y*gridStrideY + p_z*gridStrideZ];
							}
						}
					}
					
					for( int idx=0; idx<__primitivePatterns[__cubePatterns[inOutBits].MatchingInfo.PrimaryPatternIndex].IndexCount; ++idx )
					{
						if( localVertexMap[__cubePatterns[inOutBits].IndexBuf[idx]] < 0 )
						{
							throw "vertex error";
						}
						indexBuffer[indexCount] = localVertexMap[__cubePatterns[inOutBits].IndexBuf[idx]];
						indexCount++;
					}
				}
			}
		}
	}
}


void MB3DCubePattern::Init(int patternIdx)
{
	PatternIdx = patternIdx;
	
	MB3DCubeInOut inOut(patternIdx);
	
	MatchingInfo.Match( inOut );
	
	// init index buffer
	for( int i=0; i<__primitivePatterns[MatchingInfo.PrimaryPatternIndex].IndexCount; ++i )
	{
		char axis, a_1, a_2;
		MatchingInfo.SampleSegment( __primitivePatterns[MatchingInfo.PrimaryPatternIndex].IndexBuf[i], axis, a_1, a_2 );
		
		// reverse order if flip
		const int targetIdx = MatchingInfo.bFlipInOut ? (__primitivePatterns[MatchingInfo.PrimaryPatternIndex].IndexCount-i-1) : i;
		IndexBuf[targetIdx] = axis*4+a_2*2+a_1;
	}
}


void MB3DPatternMatchingInfo::Match( const MB3DCubeInOut &src )
{
	// in count
	PrimaryPatternIndex=-1;
	
	bFlipInOut = src.inCount > 4;
	
	for( int i=0; i<MB3D_PATTERN_COUNT; ++i )
	{
		const MB3DCubeInOut &tgt = __primitivePatterns[i].InOut;
		
		if( bFlipInOut )
		{
			if( (8-src.inCount) != tgt.inCount )
			{
				continue;
			}
		}
		else
		{
			if( src.inCount != tgt.inCount )
			{
				continue;
			}
		}
		
		char AxisDir[3];
		for( Origin.x=0; Origin.x<2; ++Origin.x )
		{
			AxisDir[0] = Origin.x ? -1 : 1;
			for( Origin.y=0; Origin.y<2; ++Origin.y )
			{
				AxisDir[1] = Origin.y ? -1 : 1;
				for( Origin.z=0; Origin.z<2; ++Origin.z )
				{
					AxisDir[2] = Origin.z ? -1 : 1;
					
					const int AxisOrder = ( ( Origin.x + Origin.y + Origin.z ) % 2 ) ? 2 : 1;
					
					for( int StartingAxis=0; StartingAxis<3; ++StartingAxis )
					{
						// ここで座標軸が決定
						// 原点 : bStartingPeak[3]
						// 軸1 : StartingAxis
						// 軸2 : StartingAxis+AxisOrder;
						// 軸3 : StartingAxis+AxisOrder+AxisOrder;
						
						Axis[0].SetAxisVector( StartingAxis, AxisDir[StartingAxis] );
						Axis[1].SetAxisVector( ( StartingAxis+AxisOrder ) % 3, AxisDir[( StartingAxis+AxisOrder ) % 3] );
						Axis[2].SetAxisVector( ( StartingAxis+AxisOrder+AxisOrder ) % 3, AxisDir[( StartingAxis+AxisOrder+AxisOrder ) % 3] );
						
						// start matching
						bool bMatch = true;
						for( char a0=0; a0<2; ++a0 )
						{
							for( char a1=0; a1<2; ++a1 )
							{
								for( char a2=0; a2<2; ++a2 )
								{
									const MB3DCubeVector point = SampleVertex( MB3DCubeVector(a0,a1,a2) );
									
									if( ( bFlipInOut != ( src.At( point ) == tgt.bFill[a2][a1][a0] ) ) )
									{
									}
									else
									{
										bMatch = false;
									}
								}
							}
						}
						
						if( bMatch )
						{
							PrimaryPatternIndex = i;
							
							return;
						}
					}
				}
			}
		}
	}


	MB3DCubeVector MB3DPatternMatchingInfo::SampleVertex( const MB3DCubeVector &primaryPos )const
	{
		return Origin + Axis[0]*primaryPos.x + Axis[1]*primaryPos.y + Axis[2]*primaryPos.z;
	}

	void MB3DPatternMatchingInfo::SampleSegment( char primarySegmentID, char &out_axis, char &out_a_1, char &out_a_2 )const
	{
		const char primary_axis = primarySegmentID / 4;
		const char primary_a_1 = primarySegmentID % 2;
		const char primary_a_2 = ( primarySegmentID / 2 ) % 2;
		
		out_axis = Axis[primary_axis].axisIdx;
		
		const char a_1_idx = Axis[(primary_axis+1)%3].axisIdx;
		const char a_2_idx = Axis[(primary_axis+2)%3].axisIdx;
		
		MB3DCubeVector pos = Origin + Axis[(primary_axis+1)%3]*primary_a_1 + Axis[(primary_axis+2)%3]*primary_a_2;
		
		//	out_a_1 = pos.e[a_1_idx];
		//	out_a_2 = pos.e[a_2_idx];
		
		const char primary_a_1_idx = ( out_axis+1 ) % 3;
		const char primary_a_2_idx = ( out_axis+2 ) % 3;
		out_a_1 = pos.e[primary_a_1_idx];
		out_a_2 = pos.e[primary_a_2_idx];
	}
#endif
}
