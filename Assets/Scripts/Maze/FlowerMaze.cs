using System.Collections.Generic;
using UnityEngine;

public class FlowerMaze : MazeCell {

    [SerializeField] private GameObject leftWall;
    [SerializeField] private GameObject rightWall;
    [SerializeField] private GameObject frontWall;
    [SerializeField] private GameObject backWall;

    [SerializeField] private GameObject floor;
    [SerializeField] private GameObject exit;

    [SerializeField] private GameObject firefly;
    [SerializeField] private List<GameObject> plants = new List<GameObject>();

    public override void Visit() {
        base.Visit();
        floor.SetActive(true);

        int numFirefly = Random.Range(1, 4);
        for (int i = 0; i < numFirefly; i++) { 
            Instantiate(firefly, transform.position + new Vector3(0f, -0.4f, 0f), Quaternion.identity, transform);    
        }

        int numPlants = Random.Range(0, 2);
        for (int i = 0; i < numPlants; i++) {
            Instantiate(plants[Random.Range(0, 2)], transform.position + new Vector3(Random.Range(-1.5f, 1.5f), -0.4f, Random.Range(-1.5f, 1.5f)), Quaternion.identity, transform);    
        }

        int numGrass = Random.Range(1, 4);
        for (int i = 0; i < numGrass; i++) {
            Instantiate(plants[Random.Range(2, 4)], transform.position + new Vector3(Random.Range(-1.5f, 1.5f), -0.4f, Random.Range(-1.5f, 1.5f)), Quaternion.identity, transform);    
        }
    }

    public override void ClearLeftWall() {
        leftWall.SetActive(false);
    }

    public override void ClearRightWall() {
        rightWall.SetActive(false);
    }

    public override void ClearFrontWall() {
        frontWall.SetActive(false);
    }

    public override void ClearBackWall() {
        backWall.SetActive(false);
    }

    public override void SetAsExit() {
        base.SetAsExit();

        floor.SetActive(false);
        exit.SetActive(true);
    }
}
