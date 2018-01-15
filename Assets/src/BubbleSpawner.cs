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
  public LineRenderer minDistanceLine;
  // DEBUG

  float vlx = .0f;
  float vly = .0f;
  float vrx = .0f;
  float vry = .0f;

  int cornerIndex = 0;
  float angleForNewPosition = .0f;
  Vector3 starSpawnPositionDelta = new Vector3();
  bool branchSpawned = false;

  private NStar parentStar;
  private NStar currentStar;

  ArrayList starMap = new ArrayList();

  void Update() {
    if (Input.GetKey(KeyCode.A)) {
      spawnBuble();
    } else if (Input.GetMouseButtonDown(0)) {
      updateStarMap();
      updateStarSpawnPositionDelta();
    }
  }

  private void updateStarMap() {
    Vector2 touchPoint = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    Vector3 worldPoint = Camera.main.ScreenToWorldPoint(touchPoint);
    NStar selectedStar = null;

    bool overlaped = false;

    foreach (NStar star in starMap) {
      if (star.isOverlapPoint(worldPoint) && !overlaped) {
        overlaped = true;
        selectedStar = star;
        selectedStar.setSelected();
      } else {
        star.setUnSelected();
      }

      star.clearHighlightFromParentBranch();
    }

    if (selectedStar != null) {
      selectedStar.highlightChildBranch();
    }
  }

  private void updateStarSpawnPositionDelta() {
    if (++cornerIndex > 3) {
      cornerIndex = 0;
    }
    
    switch (cornerIndex) {
      case 0:
        starSpawnPositionDelta = rx.transform.position;
        break;
      case 1:
        starSpawnPositionDelta = ry.transform.position;
        break;
      case 2:
        starSpawnPositionDelta = ly.transform.position;
        break;
      case 3:
        starSpawnPositionDelta = lx.transform.position;
        break;
    }

    branchSpawned = false;
  }

  private void spawnBuble() {
    currentStar = Instantiate(bubble, transform).GetComponent<NStar>();
    starMap.Add(currentStar.GetComponent<NStar>());

    currentStar.GetComponent<NStar>().join(getSelected());
    updateUniverseBoundMarker();

    currentStar.transform.position = getNextStarCoordiantes();
    parentStar = currentStar;
  }

  private Vector3 getNextStarCoordiantes() {
    NStar activeStar = getSelected();
    Vector3 newPosition = new Vector3();

    if (activeStar != null) {
      Vector3 deltaPos = activeStar.getStarPosition();

      if (!branchSpawned) {
        branchSpawned = true;
        deltaPos = starSpawnPositionDelta;
      }

      newPosition.x = deltaPos.x + 5.0f * Mathf.Cos(angleForNewPosition);
      newPosition.y = deltaPos.y + 5.0f * Mathf.Sin(angleForNewPosition);

      angleForNewPosition += 10.0f;
      updateMinLineGuide(activeStar.getStarPosition(), newPosition);
    }

    print("Star population: " + starMap.Count);

    return newPosition;
  }

  private NStar getSelected() {
    foreach (NStar star in starMap) {
      if (star.isSelected()) {
        return star;
      }
    }
     
    return parentStar;
  }

  private void updateUniverseBoundMarker() {
    updateUniverseBound();

    lx.transform.position = new Vector3(vlx - 5, vly + 5);
    ly.transform.position = new Vector3(vlx - 5, vry - 5);
    rx.transform.position = new Vector3(vrx + 5, vly + 5);
    ry.transform.position = new Vector3(vrx + 5, vry - 5);
  }

  private void updateMinLineGuide(Vector3 fromPt, Vector3 toPt) {
    minDistanceLine.positionCount = 2;
    minDistanceLine.SetPosition(0, fromPt);
    minDistanceLine.SetPosition(1, toPt);
  }

  private void updateUniverseBound() {
    if (starMap.Count > 0) {
      foreach (NStar star in starMap) {
        Vector3 position = star.transform.position;

        vlx = Mathf.Min(position.x, vlx);
        vly = Mathf.Max(position.y, vly);
        vrx = Mathf.Max(position.x, vrx);
        vry = Mathf.Min(position.y, vry);
      }
    } 
  }
}