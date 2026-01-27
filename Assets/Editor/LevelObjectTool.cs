using UnityEditor;
using UnityEngine;


public class LevelObjectTool : EditorWindow
{
    private int targetLayer = 0;
    private string targetTag = "unTagged.";
    private bool isStatic = false;



    [MenuItem("Tools/Level Object Tool")]
    public static void Open()
    {
        GetWindow<LevelObjectTool>("关卡对象工具");
    }

    private void OnGUI()
    {
        GUILayout.Label("关卡对象批量配置",EditorStyles.boldLabel);

        targetLayer = EditorGUILayout.LayerField("Layer", targetLayer);
        targetTag = EditorGUILayout.TagField("Tag",targetTag);
        isStatic = EditorGUILayout.Toggle("Static",isStatic);

        GUILayout.Space(10);

        if (GUILayout.Button("应用到选中物体"))
        {
            ApplySettings();
        }

        if (GUILayout.Button("校验选中物体"))
        {

        }
    }



    private void ApplySettings()
    {
        GameObject[] selected = Selection.gameObjects;

        if(selected.Length == 0)
        {
            Debug.LogWarning("Nothing to be selected.");
            return;
        }

        foreach(GameObject obj in selected)
        {
            Undo.RecordObject(obj,"Apply Level Settings");

            obj.layer = targetLayer;
            obj.tag = targetTag;
            obj.isStatic = isStatic;

            if (obj.GetComponent<Collider>() == null)
                Undo.AddComponent<Collider>(obj);

            Debug.Log("Evething is ready");
        }
    }

    private void ValidateObjects()
    {
        GameObject[] selected =Selection.gameObjects;

        if(selected.Length == 0)
        {
            Debug.LogWarning("Nothing to be selected.");
            return;
        }

        foreach(GameObject obj in selected)
        {
            if (obj.GetComponent<Collider>() == null)
                Debug.LogError($"{obj.name} missing collider");

            if (obj.layer != targetLayer)
            {
                Debug.LogWarning($"{obj.name} Layer isn't compliant");
            }

            if (obj.tag != targetTag)
            {
                Debug.LogWarning($"{obj.name} Tag isn't compliant");
            }
        }

        Debug.Log("Verification complete.");
    }

}
