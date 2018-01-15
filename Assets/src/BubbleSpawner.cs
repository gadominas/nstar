using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleSpawner : MonoBehaviour {
  public GameObject bubble;

  // DEBUG
  public GameObject lx;
  public GameObject ly;
  public GameObject rx;
  public GameObject ry;

  float vlx = .0f;
  float vly = .0f;
  float vrx = .0f;
  float vry = .0f;
  // DEBUG

  private NStar parentStar;
  private NStar currentStar;

  ArrayList starMap = new ArrayList();

  void Update() {
    if (Input.GetKey(KeyCode.R)) {
      parentStar = null;
      starMap.Clear();
      vlx = vly = vrx = vry = .0f;
    }

    if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.A)) {
      spawnBuble();
    }

    updateUniverseBoundMarker();
  }

  private updateqStarMap(){
    
      Vector2 touchPoint = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
      Vector3 worldPoint = Camera.main.ScreenToWorldPoint(touchPoint);

      bool overlaped = starCollider.OverlapPoint(worldPoint);
      highlightIfOverlaps(overlaped);
      selectIfOverlaps(overlaped);
 
  }

  private void spawnBuble() {
    Vector2 touchPoint = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    Vector3 worldPoint = Camera.main.ScreenToWorldPoint(touchPoint);
    worldPoint.z = .0f;

    currentStar = Instantiate(bubble, transform).GetComponent<NStar>();
    currentStar.transform.position = worldPoint;

    starMap.Add(currentStar.GetComponent<NStar>());
    currentStar.GetComponent<NStar>().join(getSelected());

    if (parentStar != null) {
      parentStar.setSelected();
    }

    parentStar = currentStar;
    updateUniverseBoundMarker();
  }

  private NStar getSelected(){
    foreach (NStar star in starMap) {
      if (star.isSelected()) {
        return star;
      }
    }
     
    return parentStar;
  }

  private void updateUniverseBoundMarker() {
    updateUniverseBound();

    lx.transform.position = new Vector3(vlx-5, vly+5);
    ly.transform.position = new Vector3(vlx-5, vry-5);
    rx.transform.position = new Vector3(vrx+5, vly+5);
    ry.transform.position = new Vector3(vrx+5, vry-5);
  }

  private void updateUniverseBound() {
    foreach (NStar star in starMap) {
      Vector3 position = star.transform.position;

      vlx = Mathf.Min(position.x, vlx);
      vly = Mathf.Max(position.y, vly);
      vrx = Mathf.Max(position.x, vrx);
      vry = Mathf.Min(position.y, vry);
    }
  }
}