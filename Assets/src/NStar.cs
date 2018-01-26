using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NStar : MonoBehaviour {
  public LineRenderer lineRenderer;
  public CircleCollider2D starCollider;

  public GameObject selectedSprite;
  public GameObject visitedSprite;
  public GameObject currentSprite;
  public GameObject starSprite;
  public GameObject starInfoDialog;

  private NStar parentStar;
  private ArrayList childStars = new ArrayList();

  private float starPositionInBubble;
  private Vector3 starPosition = new Vector3();

  // main states of the star
  private bool selected = false;
  private bool current = false;
  private bool visited = false;
  public bool highlight = false;

  private Color normalStartColor;
  private Color normalEndColor;
  private Color highlightStartColor;
  private Color highlightEndColor;

  private StarInfo starInfo;

  public void join(NStar parent, StarInfo starInfo) {
    this.starInfo = starInfo;
    starPositionInBubble = Random.Range(0, 360);

    // one time setup of parent
    if (parentStar == null && parent != null) {
      parentStar = parent;
    } 

    if (parentStar != null) {
      parentStar.clearSelected();
      parentStar.setChild(this);
    }
  }

  public void setChild(NStar child) {
    childStars.Add(child);
  }

  public void highlightChildBranch() {
    highlight = true;

    if (childStars.Count > 0) {
      foreach (NStar star in childStars) {
        star.highlightChildBranch();
      }
    }
  }

  public void clearHighlightFromParentBranch() {
    highlight = false;

    if (parentStar != null) {
      parentStar.clearHighlightFromParentBranch();
    }
  }

  public bool isOverlapPoint(Vector3 point) {
    return starCollider.OverlapPoint(point);
  }

  public Vector3 getStarPosition() {
    return getStarPosition(starPositionInBubble);
  }

  public float getBubbleRadius() {
    return starCollider.radius;
  }

  // selected status
  public void setSelected() {
    selected = true;
  }

  public void clearSelected() {
    selected = false;
  }

  public bool isSelected() {
    return selected;
  }

  // current status
  public void setCurrent(){
    current = true;
  }

  public void clearCurrent(){
    current = false;
  }

  public bool isCurrent(){
    return current;
  }

  // visited status
  public bool isVisited(){
    return visited;
  }

  public void setVisited(){
    visited = true;
  }

  public override string ToString(){
    return starInfo.ToString();
  }

  public string getStarId(){
    return starInfo.starId;
  }

  void Start() {
    normalStartColor = Color.gray;
    normalEndColor = Color.gray;

    highlightStartColor = Color.white;
    highlightEndColor = Color.green;
  }

  private Vector3 getStarPosition(float angle) {
    float radius = starCollider.radius * 0.6f;
    starPosition.x = transform.localPosition.x + radius * Mathf.Cos(angle);
    starPosition.y = transform.localPosition.y + radius * Mathf.Sin(angle);

    return starPosition;
  }

  void Update() {
    selectedSprite.SetActive(selected);
    currentSprite.SetActive(current);
    visitedSprite.SetActive(visited);
    starInfoDialog.SetActive(selected);
  }

  void FixedUpdate() {
    updateConnections();
    highlightConnections();
    starSprite.transform.position = getStarPosition();
  }

  private void updateConnections() {
    if (childStars.Count > 0 && visited) {
      int lineCount = 0;

      foreach (NStar star in childStars) {
        lineCount += 2;
        lineRenderer.positionCount = lineCount;
        lineRenderer.SetPosition(lineCount - 2, getStarPosition());
        lineRenderer.SetPosition(lineCount - 1, star.getStarPosition());
      }

      if (highlight) {
        lineRenderer.startWidth = 3;
        lineRenderer.endWidth = 0;
      } else {
        lineRenderer.startWidth = 2.0f;
        lineRenderer.endWidth = 1.0f;
      }
    } else {
      lineRenderer.positionCount = 0;
    }
  }

  private void highlightConnections() {
    if (highlight) {
      lineRenderer.startColor = highlightStartColor;
      lineRenderer.endColor = highlightEndColor;
    } else {
      lineRenderer.startColor = normalStartColor;
      lineRenderer.endColor = normalEndColor;
    }
  }
}