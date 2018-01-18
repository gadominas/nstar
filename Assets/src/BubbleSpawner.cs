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

  private int numberOfStellarSystems = 1;
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
  private float radiusForNewPosition = 100.0f;

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
    if (Input.GetKeyUp(KeyCode.S) || Input.GetKey(KeyCode.B)) {
      spawnBuble();

      #if DEBUG
      starPopulation.text = "# of stars: [" + starMap.Count.ToString() + "] // # of stellars: [" + numberOfStellarSystems + "]";
      #endif
    } else if (Input.GetMouseButtonDown(0)) {
      #if DEBUG
      createNewStellarSytem();
      numberOfStellarSystems++;
    }

    if (Input.GetKeyUp(KeyCode.R)) {
      selectRandomStar();
      stellarSystemPreparedToCreate = true;
      numberOfStellarSystems++;
    }
    #endif

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
      angleForNewPosition = .0f;
      radiusForNewPosition = 100.0f;
    } 

    newPosition.x = deltaPos.x + radiusForNewPosition * Mathf.Cos(Random.Range(angleForNewPosition - .2f, angleForNewPosition + .2f));
    newPosition.y = deltaPos.y + radiusForNewPosition * Mathf.Sin(angleForNewPosition);

    angleForNewPosition += 0.5f;
    radiusForNewPosition -= 1.5f;

    return newPosition;
  }

  private Vector3 getNextStellarSystemPosition(Vector3 starPosition) {
    Vector3 minimumDistanceHitPt = new Vector3();

    Vector3 topHit = nearestPointOnFiniteLine(getUniverseTopLeft(), getUniverseTopRight(), starPosition);
    Vector3 bottomHit = nearestPointOnFiniteLine(getUniverseBottomLeft(), getUniverseBottomRight(), starPosition);
    Vector3 leftHit = nearestPointOnFiniteLine(getUniverseTopLeft(), getUniverseBottomLeft(), starPosition);
    Vector3 rightHit = nearestPointOnFiniteLine(getUniverseTopRight(), getUniverseBottomRight(), starPosition);

    print((int)Vector3.Distance(starPosition, topHit));
    print((int)Vector3.Distance(starPosition, bottomHit));
    print((int)Vector3.Distance(starPosition, leftHit));
    print((int)Vector3.Distance(starPosition, rightHit));

    float minDistance = Mathf.Min(Vector3.Distance(starPosition, topHit),
                          Mathf.Min(Vector3.Distance(starPosition, bottomHit),
                            Mathf.Min(Vector3.Distance(starPosition, leftHit),
                              Mathf.Min(Vector3.Distance(starPosition, rightHit)))));


    Dictionary<int,Vector3> hitPoints = 
      getHitPoint(starPosition, new Vector3[]{ topHit, bottomHit, leftHit, rightHit });
    
    if (hitPoints.TryGetValue(((int)minDistance), out minimumDistanceHitPt)) {
      Vector3 heading = minimumDistanceHitPt - universeBound.center;
      //heading = heading + 10.0f;
      return starPosition + heading;
    } else {
      return starPosition;
    }
  }

  private Dictionary<int,Vector3> getHitPoint(Vector3 starPosition, Vector3[] hitPoints) {
    Dictionary<int,Vector3> hitPointsSet = new Dictionary<int,Vector3>();

    foreach (Vector3 pt in hitPoints) {
      int distance = (int)Vector3.Distance(starPosition, pt);

      if (!hitPointsSet.ContainsKey(distance)) {
        hitPointsSet.Add(distance, pt);
      }
    }

    return hitPointsSet;
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

  private Vector3 nearestPointOnFiniteLine(Vector3 start, Vector3 end, Vector3 pnt) {
    var line = (end - start);
    var len = line.magnitude;
    line.Normalize();

    var v = pnt - start;
    var d = Vector3.Dot(v, line);
    d = Mathf.Clamp(d, 0f, len);
    return start + line * d;
  }

  private Vector3 getUniverseTopLeft() {
    return universeBound.min;
  }

  private Vector3 getUniverseBottomRight() {
    return universeBound.max;
  }

  private Vector3 getUniverseTopRight() {
    return new Vector3(getUniverseBottomRight().x, getUniverseTopLeft().y);
  }

  private Vector3 getUniverseBottomLeft() {
    return new Vector3(getUniverseTopLeft().x, getUniverseBottomRight().y);
  }

  #if DEBUG
  private void selectRandomStar() {
    foreach (NStar star in starMap) {
      star.setUnSelected();
      star.clearHighlightFromParentBranch();
    }

    ((NStar)starMap[Random.Range(0, starMap.Count - 1)]).setSelected();
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
    Debug.DrawLine(getUniverseTopLeft(), getUniverseTopRight(), Color.green);
    Debug.DrawLine(getUniverseTopRight(), getUniverseBottomRight(), Color.green);
    Debug.DrawLine(getUniverseBottomRight(), getUniverseBottomLeft(), Color.green);
    Debug.DrawLine(getUniverseBottomLeft(), getUniverseTopLeft(), Color.green);
  }
  #endif
}