using UnityEngine;
using UnityEditor;
using Team.Gameplay.GridSystem;

[CustomEditor(typeof(GridTile))]
public class GridTile_CustomInspector : Editor
{
    TileType newTileType = TileType.TILE;

    private void Reset()
    {
        newTileType = TileType.TILE;
    }
    public override void OnInspectorGUI()
    {
        GridTile Sc_gridTile = (GridTile)target;

        base.OnInspectorGUI();
        GUILayout.Space(10f);

        EditorGUILayout.HelpBox("Changing this field will set up the rest of the tile!", MessageType.Info);
        GUILayout.Space(2f);

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.LabelField("Tile Type Setter: ", EditorStyles.boldLabel);

        GUILayout.Space(10f);
        newTileType = (TileType)EditorGUILayout.EnumPopup(Sc_gridTile.myTileType);


        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(Sc_gridTile, "Change My Value");
            Sc_gridTile.myTileType = newTileType;
           /* if (Sc_gridTile)
            {
                EditorUtility.SetDirty(Sc_gridTile);
                return;
            }*/
        }
    }
}
