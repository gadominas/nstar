using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NStar : MonoBehaviour {
  public LineRenderer lineRenderer;
  public CircleCollider2D starCollider;

  public GameObject selectedSprite;
  public GameObject starSprite;

  private NStar parentStar;
  private ArrayList childStars = new ArrayList();

  private float starPositionInBubble;
  private Vector3 starPosition = new Vector3();
  private bool selected = false;
  public bool highlight = false;

  private Color normalStartColor;
  private Color normalEndColor;
  private Color highlightStartColor;
  private Color highlightEndColor;

  public void join(NStar parent) {
    starPositionInBubble = Random.Range(0, 360);

    // one time setup of parent
    if (parentStar == null && parent != null) {
      parentStar = parent;
    } 

    if (parentStar != null) {
      parentStar.setUnSelected();
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
    normalStartColor = Color.white;
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
  }

  void FixedUpdate() {
    updateConnections();
    highlightConnections();
    starSprite.transform.position = getStarPosition();
  }

  private void updateConnections() {
    if (childStars.Count > 0 && highlight) {
      int lineCount = 0;

      foreach (NStar star in childStars) {
        lineCount += 2;
        lineRenderer.positionCount = lineCount;
        lineRenderer.SetPosition(lineCount - 2, getStarPosition());
        lineRenderer.SetPosition(lineCount - 1, star.getStarPosition());
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