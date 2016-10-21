using UnityEngine;
using UnityEditor;
using Framework.Animation;

[CustomEditor(typeof(AnimationBase), true)]
public class AnimationBaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var t = target as AnimationBase;

        if (Application.isPlaying)
        {
            if (GUILayout.Button("Play"))
                t.Play();
        }
    }
}
