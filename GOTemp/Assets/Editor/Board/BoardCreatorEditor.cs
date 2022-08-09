using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[CustomEditor(typeof(BoardCreator))]
public class BoardCreatorEditor : Editor
{
    private const int leftMouseBtnIndex = 0;
    private const int rightMouseBtnIndex = 1;
    private const int middleMouseBtnIndex = 2;
    private FieldGrid lastGridSelected = null;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BoardCreator myScript = (BoardCreator)target;

        var styleLabel = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };

        if (GUILayout.Button("SETUP GRID - RESET"))
        {
            myScript.SetupGrid();
            EditorUtility.SetDirty(myScript);
        }

        EditorGUILayout.LabelField("----------SETTINGS----------", styleLabel, GUILayout.ExpandWidth(true));

        if (GUILayout.Button("UPDATE TO NEW SETTINGS"))
        {
            myScript.UpdateSettings();
            EditorUtility.SetDirty(myScript);
        }

        if (GUILayout.Button("SET GRID VISIBLE (V)"))
        {
            myScript.SetGridVisible();
            EditorUtility.SetDirty(myScript);
        }

        if (GUILayout.Button("SET SPACE BETWEEN FIELDS"))
        {
            myScript.SetSpaceBetweenFields();
            EditorUtility.SetDirty(myScript);
        }
        
    }

    public void OnSceneGUI()
    {
        Event currentEvent = Event.current;
        BoardCreator myScript = (BoardCreator)target;
        Keyboard keyboard = Keyboard.current;

        switch (currentEvent.type)
        {
            case EventType.KeyDown:
                {
                    if (currentEvent.keyCode == KeyCode.V)
                    {
                        myScript.SetGridVisible();
                        currentEvent.Use();
                    }
                    break;
                }
            case EventType.KeyUp:
                {
                    lastGridSelected = null;
                    break;
                }
            case EventType.MouseDown:
                {
                    RaycastHit hit;
                    var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                    int controlId = GUIUtility.GetControlID(FocusType.Passive);
                    GUIUtility.hotControl = controlId;

                    //Fields
                    if (keyboard.leftCtrlKey.isPressed)
                    {
                        if (Physics.Raycast(ray, out hit))
                        {
                            if (hit.transform.parent.gameObject.TryGetComponent(out FieldGrid grid))
                            {
                                if(currentEvent.button == leftMouseBtnIndex)
                                {
                                    if(grid.Field == null)
                                    {
                                        CreateNewField(grid, myScript);
                                    }
                                }
                                else if (currentEvent.button == rightMouseBtnIndex)
                                {
                                    if (!CanRemoveField(grid))
                                    {
                                        return;
                                    }

                                    if (grid.Field.IsWonField)
                                    {
                                        grid.SetWonField();
                                    }

                                    grid.SetBusy();
                                    myScript.RemoveAllPathFromField(grid);
                                    myScript.RemoveAllPathFromField(grid.Field);
                                    grid.DestroyField();
                                }
                            }
                        }
                    }

                    //Path
                    if (keyboard.spaceKey.isPressed)
                    {
                        if (Physics.Raycast(ray, out hit))
                        {
                            if (hit.transform.parent.gameObject.TryGetComponent(out FieldGrid grid))
                            {
                                if (currentEvent.button == leftMouseBtnIndex)
                                {
                                    if (lastGridSelected == null)
                                    {
                                        lastGridSelected = grid;
                                        if (!grid.IsBusy)
                                        {
                                            CreateNewField(grid, myScript);
                                        }
                                    }
                                    else
                                    {
                                        EDirection pathDirection = lastGridSelected.IsANeighborGrid(grid.LogicPosition);
                                        if (pathDirection != EDirection.None)
                                        {
                                            if (!grid.IsBusy)
                                            {
                                                CreateNewField(grid, myScript);
                                            } 

                                            FieldPathGrid pathGrid = myScript.SetPathGrid(lastGridSelected.LogicPosition, grid.LogicPosition, pathDirection); 
                                            FieldPath fieldPath = myScript.SetPathField(lastGridSelected.LogicPosition, grid.LogicPosition, pathDirection);

                                            if(pathGrid == null || fieldPath == null)
                                            {
                                                //have path here
                                                lastGridSelected = grid;
                                                return;
                                            }

                                            pathGrid.SetFieldPath(fieldPath);
                                        }
                                        lastGridSelected = grid;
                                    }
                                }
                                else if (currentEvent.button == rightMouseBtnIndex)
                                {
                                    if (lastGridSelected == null)
                                    {
                                        if(grid.Field != null)
                                        {
                                            lastGridSelected = grid;
                                        }
                                    }
                                    else
                                    {
                                        if(lastGridSelected == grid || grid.Field == null)
                                        {
                                            return;
                                        }

                                        EDirection pathDirection = lastGridSelected.IsANeighborGrid(grid.LogicPosition);
                                        myScript.RemovePath(lastGridSelected, pathDirection);
                                        myScript.RemovePath(lastGridSelected.Field, pathDirection);
                                        lastGridSelected = grid;
                                    }
                                }
                            }
                        }
                    }

                    //Start field
                    if (keyboard.qKey.isPressed)
                    {
                        if (Physics.Raycast(ray, out hit))
                        {
                            if (hit.transform.parent.gameObject.TryGetComponent(out FieldGrid grid))
                            {
                                if (currentEvent.button == leftMouseBtnIndex)
                                {
                                    //Create field if grid is free
                                    if (!grid.IsBusy)
                                    {
                                        CreateNewField(grid, myScript);
                                    }

                                    //Remove old start grid
                                    FieldGrid oldStartPos = myScript.GetStartFieldGrid();
                                    oldStartPos.SetStartField();

                                    //Set new start field
                                    grid.SetStartField();

                                }
                                else if (currentEvent.button == rightMouseBtnIndex)
                                {
                                }
                            }
                        }
                    }

                    //Won field
                    if (keyboard.wKey.isPressed)
                    {
                        if (Physics.Raycast(ray, out hit))
                        {
                            if (hit.transform.parent.gameObject.TryGetComponent(out FieldGrid grid))
                            {
                                if (currentEvent.button == leftMouseBtnIndex)
                                {
                                    //Create field if grid is free
                                    if (!grid.IsBusy)
                                    {
                                        CreateNewField(grid, myScript);
                                    }

                                    if (grid.Field.IsWonField)
                                    {
                                        return;
                                    }
                                    //Set new start field
                                    grid.SetWonField();
                                }
                                else if (currentEvent.button == rightMouseBtnIndex)
                                {
                                    if (grid.Field.IsWonField)
                                    {
                                        grid.SetWonField();
                                    }
                                }
                            }
                        }
                    }

                    //Enemy spawner
                    if (keyboard.eKey.isPressed)
                    {
                        if (Physics.Raycast(ray, out hit))
                        {
                            if (hit.transform.parent.gameObject.TryGetComponent(out FieldGrid grid))
                            {
                                if (currentEvent.button == leftMouseBtnIndex)
                                {
                                    //Create field if grid is free
                                    if (!grid.IsBusy)
                                    {
                                        CreateNewField(grid, myScript);
                                    }

                                    if (grid.HasEnemySpawner())
                                    {
                                        return;
                                    }
                                    //Set enemy on field
                                    myScript.CreateEnemySpawner(grid);
                                    grid.SetEnemyOnField();
                                }
                                else if (currentEvent.button == rightMouseBtnIndex)
                                {
                                    if (grid.HasEnemySpawner())
                                    {
                                        myScript.DestroyEnemySpawner(grid);
                                        grid.SetEnemyOnField();
                                    }
                                }
                            }
                          
                        }
                    }

                    //Object spawner
                    if (keyboard.oKey.isPressed)
                    {
                        if (Physics.Raycast(ray, out hit))
                        {
                            if (hit.transform.parent.gameObject.TryGetComponent(out FieldGrid grid))
                            {
                                if (currentEvent.button == leftMouseBtnIndex)
                                {
                                    //Create field if grid is free
                                    if (!grid.IsBusy)
                                    {
                                        CreateNewField(grid, myScript);
                                    }

                                    if (grid.HasObjectSpawner())
                                    {
                                        return;
                                    }
                                    //Set object on field
                                    myScript.CreateObjectSpawner(grid);
                                    grid.SetObjectOnField();
                                }
                                else if (currentEvent.button == rightMouseBtnIndex)
                                {
                                    if (grid.HasObjectSpawner())
                                    {
                                        myScript.DestroyObjectSpawner(grid);
                                        grid.SetObjectOnField();
                                    }
                                }
                            }

                        }
                    }

                    //Path Object spawner
                    if (keyboard.iKey.isPressed)
                    {
                        if (Physics.Raycast(ray, out hit))
                        {
                            if (hit.transform.parent.gameObject.TryGetComponent(out FieldPathGrid path))
                            {
                                if (currentEvent.button == leftMouseBtnIndex)
                                {
                                    if (path.ObjectPathSpawner != null)
                                    {
                                        return;
                                    }
                                    //Set objec on field
                                    myScript.CreatePathObjectSpawner(path);
                                }
                                else if (currentEvent.button == rightMouseBtnIndex)
                                {
                                    if (path.ObjectPathSpawner != null)
                                    {
                                        myScript.DestroyPathObjectSpawner(path);
                                    }
                                }
                            }

                        }
                    }
                    currentEvent.Use();
                    EditorUtility.SetDirty(myScript);
                    break;
                }
        }
    }

    private void CreateNewField(FieldGrid grid, BoardCreator boardCreator)
    {
        grid.SetBusy();
        if (grid.IsBusy)
        {
            grid.SetField(boardCreator.CreateField(grid.transform, grid.LogicPosition));
        }
    }

    private bool CanRemoveField(FieldGrid grid)
    {
        if (grid.Field == null)
        {
            return false;
        }

        if (grid.Field.IsStartField)
        {
            Debug.Log("You cant remove start field! Create new start field to remove this.");
            return false;
        }

        if (grid.HasEnemySpawner())
        {
            Debug.Log("You cant remove field with Enemy! First remove enemy - press: (E+RMB).");
            return false;
        }

        return true;
    }
}
