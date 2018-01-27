using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFluidMove : MonoBehaviour {
  private Vector3 moveToSpot;

  public void moveCameraTo(Vector3 newSpot) {
    moveToSpot = newSpot;
  }

  void Start(){
    moveCameraTo(Camera.main.transform.position);
  }
	
  // Update is called once per frame
  void FixedUpdate() {
    Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, moveToSpot, Time.deltaTime);
  }
}
