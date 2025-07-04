using UnityEngine;
using UnityEditor;
using Team.Gameplay.GridSystem;

[CustomEditor(typeof(OilTile))]
public class OilTile_CustomInspector : Editor
{
    TileType newTileType = TileType.TILE;

    private void Reset()
    {
        newTileType = TileType.TILE;
    }
    public override void OnInspectorGUI()
    {
        GridTile Sc_gridTile = (GridTile)target;

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.LabelField("Tile Type Setter: ", EditorStyles.boldLabel);

        GUILayout.Space(10f);
        newTileType = (TileType)EditorGUILayout.EnumPopup(Sc_gridTile.myTileType);

        GUILayout.Space(2f);
        EditorGUILayout.HelpBox("Changing this field will set up the rest of the tile!", MessageType.Info);


        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(Sc_gridTile, "Change My Value");
            Sc_gridTile.myTileType = newTileType;
            EditorUtility.SetDirty(Sc_gridTile);
        }
        GUILayout.Space(10f);

        base.OnInspectorGUI();

    }
}
