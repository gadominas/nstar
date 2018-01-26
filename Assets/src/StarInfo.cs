using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarInfo {
  public string starId = "undefined";
  public string starName = "undefined";
  public string starDescription = "undefined";
  public bool visited = false;

  #if DEBUG
  public void randomizeInfo() {
    System.Guid myGUID = System.Guid.NewGuid();
    starId = myGUID.ToString();

    starName = "name_" + starId;
    starDescription = "starDescription_" + starId;
    visited = false;
  }
  #endif

  public override string ToString() {
    return "id:{" + starId + "} name:{" + starName + "} description:{" + starDescription + "} visited:{" + visited + "}";
  }
}
