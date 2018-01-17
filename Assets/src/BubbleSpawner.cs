using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BubbleSpawner : MonoBehaviour {
  #if DEBUG
  public GameObject boundaryCornerTempalte;
  public Text starPopulation;

  private GameObject lx;
  private GameObject ly;
  private GameObject rx;
  private GameObject ry;

  private GameObject universeCenter;
  private GameObject pt;
  #endif

  public float STAR_BUBBLE_RADIUS = 5.0f;
  public GameObject bubble;
 
  private float vlx = .0f;
  private float vly = .0f;
  private float vrx = .0f;
  private float vry = .0f;

  private Vector3 lxVector = new Vector3();
  private Vector3 lyVector = new Vector3();
  private Vector3 rxVector = new Vector3();
  private Vector3 ryVector = new Vector3();

  private Bounds universeBound = new Bounds();

  private float wRadius = .0f;
  private float hRadius = .0f;
  private float universeRadius = .0f;
  private Vector3 universeCenterPoint = new Vector3();
 
  private float angleForNewPosition = .0f;
  private bool stellarSystemPreparedToCreate = false;

  private NStar parentStar;
  private NStar currentStar;

  private ArrayList starMap = new ArrayList();

  void Start() {
    #if DEBUG
    lx = Instantiate(boundaryCornerTempalte, transform);
    ly = Instantiate(boundaryCornerTempalte, transform);
    rx = Instantiate(boundaryCornerTempalte, transform);
    ry = Instantiate(boundaryCornerTempalte, transform);

    universeCenter = Instantiate(boundaryCornerTempalte, transform);
    universeCenter.name = "universeCenterPoint";
    pt = Instantiate(boundaryCornerTempalte, transform);
    #endif
  }

  void Update() {
    if (Input.GetKeyUp(KeyCode.S)) {
      spawnBuble();

      #if DEBUG
      starPopulation.text = "# of stars: " + starMap.Count.ToString();
      #endif
    } else if (Input.GetMouseButtonDown(0)) {
      createNewStellarSytem();
    }

    if (Input.GetKey(KeyCode.R)) {
      
    }

    updateUniverseBoundaryMeta();
    #if DEBUG
    updateUniverseBoundMarker();
    updateCamera();
    #endif
  }

  private void spawnBuble() {
    currentStar = Instantiate(bubble, transform).GetComponent<NStar>();
    starMap.Add(currentStar.GetComponent<NStar>());

    NStar currentSelectedStar = getSelected();
    Vector3 currentStarPosition = new Vector3();

    if (currentSelectedStar != null) {
      currentStarPosition = currentSelectedStar.transform.position;
    }

    currentStar.GetComponent<NStar>().join(getSelected());
    currentStar.transform.position = getNextStarCoordinates(currentStarPosition);
    parentStar = currentStar;

    stellarSystemPreparedToCreate = false;
  }

  private void createNewStellarSytem() {
    stellarSystemPreparedToCreate = true;
    updateStarMap();
  }

  private Vector3 getNextStarCoordinates(Vector3 deltaPos) {
    Vector3 newPosition = new Vector3();

    if (stellarSystemPreparedToCreate) {
      deltaPos = getNextStellarSystemPosition(deltaPos);
      pt.transform.position = deltaPos;
    } 

    newPosition.x = deltaPos.x + STAR_BUBBLE_RADIUS * Mathf.Cos(angleForNewPosition);
    newPosition.y = deltaPos.y + STAR_BUBBLE_RADIUS * Mathf.Sin(angleForNewPosition);

    angleForNewPosition += 10.0f;

    return newPosition;
  }

  private Vector3 getNextStellarSystemPosition(Vector3 starPosition) {
    float d1 = Vector3.Distance(starPosition, universeBound.min);
    float d2 = Vector3.Distance(starPosition, universeBound.max);

    if (d1 > d2) {
      return universeBound.max;
    } else {
      return universeBound.min;
    }
  }

  private NStar getSelected() {
    foreach (NStar star in starMap) {
      if (star.isSelected()) {
        return star;
      }
    }
     
    return parentStar;
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

  private void updateUniverseBoundaryMeta() {
    if (starMap.Count > 0) {
      foreach (NStar star in starMap) {
        Vector3 position = star.transform.position;

        vlx = Mathf.Min(position.x, vlx);
        vly = Mathf.Max(position.y, vly);
        vrx = Mathf.Max(position.x, vrx);
        vry = Mathf.Min(position.y, vry);
      }

      wRadius = Mathf.Abs(vrx - vlx) / 2.0f;
      hRadius = Mathf.Abs(vly - vry) / 2.0f;
      universeRadius = Mathf.Max(wRadius, hRadius);

      updateUniverseBoundaryVectors();
      universeBound.SetMinMax(lxVector, ryVector);

      universeCenterPoint.x = vlx + wRadius;
      universeCenterPoint.y = vry + hRadius; 
      universeCenterPoint.z = .0f;
    } 
  }

  private void updateUniverseBoundaryVectors() {
    lxVector.Set(vlx - STAR_BUBBLE_RADIUS, vly + STAR_BUBBLE_RADIUS, .0f);
    lyVector.Set(vlx - STAR_BUBBLE_RADIUS, vry - STAR_BUBBLE_RADIUS, .0f);
    rxVector.Set(vrx + STAR_BUBBLE_RADIUS, vly + STAR_BUBBLE_RADIUS, .0f);
    ryVector.Set(vrx + STAR_BUBBLE_RADIUS, vry - STAR_BUBBLE_RADIUS, .0f);
  }

  #if DEBUG
  private void selectRandomStar(){
    foreach (NStar star in starMap) {
      star.setUnSelected();
      star.clearHighlightFromParentBranch();
    }

    starMap[0]
  }

  private void updateCamera() {
    if (universeRadius > .0f) {
      Camera.main.transform.position = universeBound.center + (new Vector3(.0f, .0f, -10.0f));
      if (universeRadius > 100.0f) {
        Camera.main.orthographicSize = universeRadius;
      } else {
        Camera.main.orthographicSize = 100.0f;
      }
    }
  }

  private void updateUniverseBoundMarker() {
    lx.transform.position = lxVector;
    ly.transform.position = lyVector;
    rx.transform.position = rxVector;
    ry.transform.position = ryVector;

    universeCenter.transform.position = universeBound.center + (new Vector3(.0f, .0f, -10.0f));

    drawGizmoAroundBound(universeBound);
  }

  private void drawGizmoAroundBound(Bounds bound) {
    Vector3 ptTopLeft = bound.min;
    Vector3 ptBottomRight = bound.max;
    Vector3 ptTopRight = new Vector3(ptBottomRight.x, ptTopLeft.y);
    Vector3 ptBottomLeft = new Vector3(ptTopLeft.x, ptBottomRight.y);

    Debug.DrawLine(ptTopLeft, ptTopRight, Color.green);
    Debug.DrawLine(ptTopRight, ptBottomRight, Color.green);
    Debug.DrawLine(ptBottomRight, ptBottomLeft, Color.green);
    Debug.DrawLine(ptBottomLeft, ptTopLeft, Color.green);
  }
  #endif
}