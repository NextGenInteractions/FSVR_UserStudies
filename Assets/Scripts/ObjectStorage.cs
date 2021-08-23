using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectStorage : MonoBehaviour {
  

  public List<ObjectStorageState> states = new List<ObjectStorageState>();
  [System.NonSerialized]
  public int state = 0;
  /// <summary>
  /// Add a new state to state list
  /// Name: "State X" where X is the index number of the state in list
  /// If state list is more than one, the object list from last state is cloned to the new one
  /// </summary>
  public void AddNewState () {
    int count = states.Count;
    ObjectStorageState newState = new ObjectStorageState();
    newState.stateName = "State" + (count + 1);
    if (count > 0) {
      Transform[] clonedList = new Transform[states[count - 1].savedObjectList.Count];
      states[state].savedObjectList.CopyTo(clonedList);
      newState.savedObjectList = new List<Transform>(clonedList);
    }
    states.Add(newState);
    state = states.Count - 1;
  }

  /// <summary>
  /// Delete current state
  /// by calling DeleteCertainState
  /// </summary>
  public void DeleteCurrentState () {
    DeleteCertainState(state);
  }

  /// <summary>
  /// Delete All states
  /// </summary>
  public void DeleteAll() {
    for(int i = states.Count - 1; i >= 0; --i) {
      DeleteCertainState(i);
    }
  }

  /// <summary>
  /// Delete the state provided by the index
  /// </summary>
  /// <param name="index"> index of state in the list</param>
  public void DeleteCertainState (int index) {
    states[index].ClearObjectLists();
    states.RemoveAt(state);
    state = (states.Count > 0 && index > 0) ? index - 1 : 0;
  }

  /// <summary>
  /// Reset current state
  /// by calling ResetCertainState
  /// </summary>
  public void ResetCurrentState() {
    ResetCertainState(state);
  }

  /// <summary>
  /// Reset to the provided state
  /// </summary>
  /// <param name="index"> index of state in the list</param>
  public void ResetCertainState(int index) {
    if (states.Count <= index) {
      Debug.LogError("[Can't reset inexisting state] input state index: " + index + "state count: " + states.Count);
      return;
    }
    states[index].RestoreSceneObjects();
  }

  /// <summary>
  /// Save current state
  /// </summary>
  public void SaveCurrentState() {
    states[state].SaveSceneObjects();
  }

  /// <summary>
  /// Save all states
  /// </summary>
  public void SaveAllStates() {
    foreach (ObjectStorageState oss in states)
      oss.SaveSceneObjects();
  }

  /// <summary>
  /// Clear object list in current state
  /// </summary>
  public void ClearObjectListsinCurrentState () {
    states[state].ClearObjectLists();
  }

  private void Update() {
    // key binding that triggers reset to certain state
    foreach(ObjectStorageState oss in states) {
      if (Input.GetKeyDown(oss.keyBinding))
        oss.RestoreSceneObjects();
    }
  }

}

[System.Serializable]
public class ObjectStorageState {
  [Header("General Information")]
  public string stateName;
  public string stateNote;
  [Header("Stored Obj/Info")]
  public List<Transform> savedObjectList = new List<Transform>();
  public List<SavedObjectInfo> infoList = new List<SavedObjectInfo>();
  // [System.NonSerialized]
  [Header("Settings")]
  public bool posCheck = true;
  public bool rotCheck = true, scaleCheck = true, hierCheck = false, activityCheck = true, childrenCheck = false;
  public KeyCode keyBinding;

  public ObjectStorageState(bool pos = true, bool rot = true, bool scale = true, bool hier = false, bool activity = true, bool children = false)
  {
    posCheck = pos;
    rotCheck = rot;
    scaleCheck = scale;
    hierCheck = hier;
    activityCheck = activity;
    childrenCheck = children;
  }

  /// <summary>
  /// Reset the listed objects to its saved state
  /// based on the field checked from InfoList
  /// </summary>
  public void RestoreSceneObjects() {
    for (int i = 0; i < savedObjectList.Count; ++i) {
      if (posCheck) savedObjectList[i].position = infoList[i].position;
      if (rotCheck) savedObjectList[i].rotation = infoList[i].rotation;
      if (hierCheck) savedObjectList[i].parent = infoList[i].parent;
      if (scaleCheck) savedObjectList[i].localScale = infoList[i].scale;
      if (activityCheck) savedObjectList[i].gameObject.SetActive(infoList[i].activity);
      if (childrenCheck)
      {
        infoList[i].objSS.RestoreSceneObjects();
      }
    }
  }

  /// <summary>
  /// Save the fields of the list objects based on the field checks
  /// Generate info to the Infolist as storage
  /// </summary>
  public void SaveSceneObjects() {
    infoList.Clear();
    for (int i = 0; i < savedObjectList.Count; ++i) {
      infoList.Add(new SavedObjectInfo());
      if (hierCheck) infoList[i].parent = savedObjectList[i].parent;
      if (posCheck) infoList[i].position = savedObjectList[i].position;
      if (rotCheck) infoList[i].rotation = savedObjectList[i].rotation;
      if (scaleCheck) infoList[i].scale = savedObjectList[i].localScale;
      if (activityCheck) infoList[i].activity = savedObjectList[i].gameObject.activeSelf;
      if (childrenCheck)
      {
        infoList[i].objSS = new ObjectStorageState(posCheck, rotCheck, scaleCheck, hierCheck, activityCheck, false);
        Transform[] allChildren = savedObjectList[i].GetComponentsInChildren<Transform>();
        foreach(Transform child in allChildren)
          infoList[i].objSS.savedObjectList.Add(child);
        infoList[i].objSS.SaveSceneObjects();
      }
    }
  }

  /// <summary>
  /// Clear objectlist and infolist
  /// </summary>
  public void ClearObjectLists() {
    savedObjectList.Clear();
    infoList.Clear();
  }
}

[System.Serializable]
public class SavedObjectInfo {
    public Vector3 position, scale;
    public Quaternion rotation;
    public Transform parent;
    public bool activity;
    [HideInInspector]
    public ObjectStorageState objSS;
}
