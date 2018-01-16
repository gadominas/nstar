using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BubbleSpawner : MonoBehaviour {
  public float STAR_BUBBLE_RADIUS = 5.0f;
  public GameObject bubble;

  // DEBUG
  public GameObject boundaryCornerTempalte;
  public LineRenderer lineRenderTemplate;

  public LineRenderer minDistanceLine;
  public LineRenderer boundaryLineRender;
  public LineRenderer newStellarSystemSpawnDirection;
  public Text starPopulation;

  private GameObject lx;
  private GameObject ly;
  private GameObject rx;
  private GameObject ry;

  private GameObject universeCenter;
  private GameObject tmpPt;
  // DEBUG

  private float vlx = .0f;
  private float vly = .0f;
  private float vrx = .0f;
  private float vry = .0f;

  private Vector3 lxVector = new Vector3();
  private Vector3 lyVector = new Vector3();
  private Vector3 rxVector = new Vector3();
  private Vector3 ryVector = new Vector3();

  private float wRadius = .0f;
  private float hRadius = .0f;
  private float universeRadius = .0f;
  private Vector3 universeCenterPoint = new Vector3();
  private float newStellarSystemSpawnDirectionAngle = .0f;

  private int cornerIndex = 0;
  private float spiralSize = .0f;
  private float angleForNewPosition = .0f;
  private bool stellarSystemPreparedToCreate = false;

  private NStar parentStar;
  private NStar currentStar;

  private ArrayList starMap = new ArrayList();

  void Start() {
    lx = Instantiate(boundaryCornerTempalte, transform);
    ly = Instantiate(boundaryCornerTempalte, transform);
    rx = Instantiate(boundaryCornerTempalte, transform);
    ry = Instantiate(boundaryCornerTempalte, transform);

    minDistanceLine = Instantiate(lineRenderTemplate, transform); 
    boundaryLineRender = Instantiate(lineRenderTemplate, transform); 
    newStellarSystemSpawnDirection = Instantiate(lineRenderTemplate, transform); 

    universeCenter = Instantiate(boundaryCornerTempalte, transform);
    tmpPt = Instantiate(boundaryCornerTempalte, transform);
    universeCenter.name = "universeCenterPoint";
  }

  void Update() {
    if (Input.GetKey(KeyCode.A)) {
      spawnBuble();
      starPopulation.text = "# of stars: " + starMap.Count.ToString();
    } else if (Input.GetMouseButtonDown(0)) {
      createNewStellarSytem();
    }

    if (Input.GetKey(KeyCode.R)) {
      newStellarSystemSpawnDirectionAngle += Time.deltaTime;

      Vector3 zAxis = new Vector3(0, 0, 1.0f);
      tmpPt.transform.RotateAround(universeCenterPoint, zAxis, newStellarSystemSpawnDirectionAngle);

      print(universeCenter.transform.position + " " + spiralSize);
    }

    updateUniverseBoundaryMeta();
    updateUniverseBoundMarker();
    updateCamera();
  }

  private void createNewStellarSytem() {
    stellarSystemPreparedToCreate = true;

    updateStarMap();
  }

  private void spawnBuble() {
    currentStar = Instantiate(bubble, transform).GetComponent<NStar>();
    starMap.Add(currentStar.GetComponent<NStar>());

    currentStar.GetComponent<NStar>().join(getSelected());
    currentStar.transform.position = getNextStarCoordinates();
    parentStar = currentStar;

    stellarSystemPreparedToCreate = false;
  }

  private Vector3 getNextStarCoordinates() {
    NStar activeStar = getSelected();
    Vector3 newPosition = new Vector3();

    if (activeStar != null) {
      Vector3 deltaPos = activeStar.getStarPosition();

      if (stellarSystemPreparedToCreate) {
        deltaPos = getNextStellarSystemPosition();
      }

      newPosition.x = deltaPos.x + STAR_BUBBLE_RADIUS * Mathf.Cos(angleForNewPosition);
      newPosition.y = deltaPos.y + STAR_BUBBLE_RADIUS * Mathf.Sin(angleForNewPosition);

      angleForNewPosition += 10.0f;
      updateMinLineGuide(activeStar.getStarPosition(), newPosition);
    }

    return newPosition;
  }


  private Vector3 getNextStellarSystemPosition() {
    if (++cornerIndex > 3) {
      cornerIndex = 0;
    }
    
    switch (cornerIndex) {
      case 0:
        return ryVector;
      case 1:
        return lyVector;
      case 2:
        return lxVector;
      case 3:
        return rxVector;
      default:
        return new Vector3();
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

  private void updateUniverseBoundMarker() {
    updateUniverseBoundaryMeta();

    lx.transform.position = lxVector;
    ly.transform.position = lyVector;
    rx.transform.position = rxVector;
    ry.transform.position = ryVector;

    universeCenter.transform.position = universeCenterPoint;

    newStellarSystemSpawnDirection.positionCount = 2;
    newStellarSystemSpawnDirection.SetPosition(0, universeCenterPoint);
    newStellarSystemSpawnDirection.SetPosition(1, new Vector3(
      universeCenterPoint.x + universeRadius * Mathf.Cos(newStellarSystemSpawnDirectionAngle),
      universeCenterPoint.y + universeRadius * Mathf.Sin(newStellarSystemSpawnDirectionAngle),
      .0f
    ));

    boundaryLineRender.positionCount = 5;
    boundaryLineRender.SetPosition(0, lxVector);
    boundaryLineRender.SetPosition(1, rxVector);
    boundaryLineRender.SetPosition(2, ryVector);
    boundaryLineRender.SetPosition(3, lyVector);
    boundaryLineRender.SetPosition(4, lxVector);
  }

  private void updateCamera() {
    if (universeRadius > .0f) {
      Camera.main.transform.position.Set(universeCenterPoint.x, universeCenterPoint.y, -10.0f);
      Camera.main.orthographicSize = universeRadius;
    }
  }

  private void updateMinLineGuide(Vector3 fromPt, Vector3 toPt) {
    minDistanceLine.positionCount = 2;
    minDistanceLine.SetPosition(0, fromPt);
    minDistanceLine.SetPosition(1, toPt);
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

      universeCenterPoint.x = vlx + wRadius;
      universeCenterPoint.y = vry + hRadius; 
      universeCenterPoint.z = .0f;

      // update universe boundary vectors
      updateUniverseBoundaryVectors();
    } 
  }

  private void updateUniverseBoundaryVectors() {
    lxVector.Set(vlx - STAR_BUBBLE_RADIUS, vly + STAR_BUBBLE_RADIUS, .0f);
    lyVector.Set(vlx - STAR_BUBBLE_RADIUS, vry - STAR_BUBBLE_RADIUS, .0f);
    rxVector.Set(vrx + STAR_BUBBLE_RADIUS, vly + STAR_BUBBLE_RADIUS, .0f);
    ryVector.Set(vrx + STAR_BUBBLE_RADIUS, vry - STAR_BUBBLE_RADIUS, .0f);
  }
}