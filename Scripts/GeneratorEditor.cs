using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor (typeof (MapGenerator))]
public class generatorEditor : Editor {
    public override void OnInspectorGUI() {
        MapGenerator gen = (MapGenerator)target; 
        
        if (GUILayout.Button("Generate")) {
            gen.Generate();
            gen.BuildMesh();

        } 
        if (GUILayout.Button("Simulate Rainfall")) {
            gen.Generate();
            gen.Rainfall();
            gen.BuildMesh();           
        }

    }
}
