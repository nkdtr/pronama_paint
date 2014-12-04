using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MetaballCell
{
    public Vector3 baseColor;
  //  public float resource;
    public string tag;

    public float radius;

    // calculated
    // position in the basis of root
    public Vector3 modelPosition;
    // rotation in the basis of root
    public Quaternion modelRotation;
}
