using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Annotations/AnnotationSet")]
public class AnnotationSet : ScriptableObject
{
    public string SetName;
    public List<string> AnnotationNames;
}
