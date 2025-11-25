using System.Collections;
using UnityEngine;

public class drawMap : MonoBehaviour {

    [SerializeField] GameObject leftWall;
    [SerializeField] GameObject rightWall;
    [SerializeField] GameObject frontWall;
    [SerializeField] GameObject backWall;

    [SerializeField] GameObject leftLine;
    [SerializeField] GameObject rightLine;
    [SerializeField] GameObject frontLine;
    [SerializeField] GameObject backLine;

    private bool alreadyEnter = false;

    [SerializeField] bool mazeDepth = false;

    void Start() {
        leftLine.SetActive(false);    
        rightLine.SetActive(false);    
        frontLine.SetActive(false);    
        backLine.SetActive(false);    
    }

    private void OnTriggerEnter(Collider other) {
        if(other != null) {
            if(other.gameObject.name == "Player") {
                if (!alreadyEnter) {
                    alreadyEnter = true;
                    if(!mazeDepth) {
                        if (!leftWall.activeSelf) {
                            leftLine.SetActive(true);
                        }

                        if (!rightWall.activeSelf) {
                            rightLine.SetActive(true);
                        }

                        if (!frontWall.activeSelf) {
                            frontLine.SetActive(true);
                        }

                        if (!backWall.activeSelf) {
                            backLine.SetActive(true);
                        }
                    } else {
                        if (leftWall.activeSelf) {
                            leftLine.SetActive(true);
                        }

                        if (rightWall.activeSelf) {
                            rightLine.SetActive(true);
                        }

                        if (frontWall.activeSelf) {
                            frontLine.SetActive(true);
                        }

                        if (backWall.activeSelf) {
                            backLine.SetActive(true);
                        }
                    }                    
                }
            }
        }
    }

    private void OnTriggerStay(Collider other) {
        if(other != null) {
            if(other.gameObject.name == "Player") {
                GameEventManager.instance.canOpenMap = true;
            }
        }
    }

    private void OnTriggerExit(UnityEngine.Collider other) {
        if(other != null) {
            if(other.gameObject.name == "Player") {
                GameEventManager.instance.canOpenMap = false;
            }
        }
    }    
}
