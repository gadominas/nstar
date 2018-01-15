using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NStar : MonoBehaviour {
  public LineRenderer lineRenderer;
  public CircleCollider2D starCollider;

  public GameObject selectedSprite;
  public GameObject starSprite;

  private ArrayList parentStars = new ArrayList();
  private ArrayList childStars = new ArrayList();

  private float starPositionInBubble;
  private Vector3 starPosition = new Vector3();
  private bool selected = false;
  private bool highlight = false;

  private Color normalStartColor;
  private Color normalEndColor;
  private Color highlightStartColor;
  private Color highlightEndColor;

  public void join(NStar parent) {
    starPositionInBubble = Random.Range(0, 360);

    if (parent != null) {
      parent.setUnSelected();
      parent.setChild(this);

      parentStars.Add(parent);
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

    if (parentStars.Count > 0) {
      foreach (NStar star in parentStars) {
        star.clearHighlightFromParentBranch();
      }
    }
  }

  public Vector3 getStarPosition() {
    return getStarPosition(starPositionInBubble);
  }

  public float getBubbleRadius() {
    return starCollider.radius;
  }

  public void setSelected() {
    selected = true;
  }

  public void setUnSelected() {
    selected = false;
  }

  public bool isSelected() {
    return selected;
  }

  void Start() {
    normalStartColor = lineRenderer.startColor;
    normalEndColor = lineRenderer.endColor;

    highlightStartColor = Color.blue;
    highlightEndColor = Color.green;
  }

  private Vector3 getStarPosition(float angle) {
    float radius = starCollider.radius * 0.6f;
    starPosition.x = transform.localPosition.x + radius * Mathf.Cos(angle);
    starPosition.y = transform.localPosition.y + radius * Mathf.Sin(angle);

    return starPosition;
  }

  void Update() {
    if (Input.GetMouseButtonDown(0)) {
      Vector2 touchPoint = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
      Vector3 worldPoint = Camera.main.ScreenToWorldPoint(touchPoint);

      bool overlaped = starCollider.OverlapPoint(worldPoint);
      highlightIfOverlaps(overlaped);
      selectIfOverlaps(overlaped);
    }

    selectedSprite.SetActive(selected);
  }

  private void highlightIfOverlaps(bool overlaped) {
    if (overlaped) {
      clearHighlightFromParentBranch();
      highlightChildBranch();
    }

    highlightConnections();
  }

  private void selectIfOverlaps(bool overlaped) {
    selected = overlaped;
  }

  void FixedUpdate() {
    updateConnections();
    starSprite.transform.position = getStarPosition();
  }

  private void updateConnections() {
    if (parentStars.Count > 0) {
      int lineCount = 0;

      foreach (NStar star in parentStars) {
        lineCount += 2;
        lineRenderer.positionCount = lineCount;
        lineRenderer.SetPosition(lineCount -2, getStarPosition());
        lineRenderer.SetPosition(lineCount -1, star.getStarPosition());
      }
    }
  }

  private void highlightConnections(){
    if (highlight) {
      lineRenderer.startColor = highlightStartColor;
      lineRenderer.endColor = highlightEndColor;
    } else {
      lineRenderer.startColor = normalStartColor;
      lineRenderer.endColor = normalEndColor;
    }
  }
}