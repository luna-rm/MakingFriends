using System.Collections.Generic;
using UnityEngine;

public class DepthMaze : MazeCell {

    [SerializeField] private GameObject leftWall;
    [SerializeField] private GameObject rightWall;
    [SerializeField] private GameObject frontWall;
    [SerializeField] private GameObject backWall;

    [SerializeField] private GameObject floor;

    [SerializeField] private GameObject protection;

    private static int howManySinceLast = 5;
    [SerializeField] int howMany = 5;

    [SerializeField] float toEmission = 0.75f;
    [SerializeField] private ParticleSystem centerP;
    [SerializeField] private ParticleSystem leftP;
    [SerializeField] private ParticleSystem rightP;
    [SerializeField] private ParticleSystem frontP;
    [SerializeField] private ParticleSystem backP;

    public override void Visit() {
        base.Visit();
        floor.SetActive(true);

        howManySinceLast++;
        if (howManySinceLast > howMany) {
            howManySinceLast = 0;
            protection.SetActive(true);
        }
    }

    public override void ClearLeftWall() {
        leftWall.SetActive(true);
    }

    public override void ClearRightWall() {
        rightWall.SetActive(true);
    }

    public override void ClearFrontWall() {
        frontWall.SetActive(true);
    }

    public override void ClearBackWall() {
        backWall.SetActive(true);
    }

    private void Update() {     

        if (DepthManager.instance.startParticles) {
            var emission = centerP.emission;
            emission.rateOverTime = toEmission;
            centerP.gameObject.SetActive(true);

            if (leftWall.activeSelf) {
                emission = leftP.emission;
                emission.rateOverTime = toEmission;
                leftP.gameObject.SetActive(true);
            }

            if (rightWall.activeSelf) {
                emission = rightP.emission;
                emission.rateOverTime = toEmission;
                rightP.gameObject.SetActive(true);
            }

            if (frontWall.activeSelf) {
                emission = frontP.emission;
                emission.rateOverTime = toEmission;
                frontP.gameObject.SetActive(true);
            }

            if (backWall.activeSelf) {
                emission = backP.emission;
                emission.rateOverTime = toEmission;
                backP.gameObject.SetActive(true);
            }
        } else {
            var emission = centerP.emission;
            emission.rateOverTime = 0;
            centerP.gameObject.SetActive(false);
            
            emission = leftP.emission;
            emission.rateOverTime = 0;
            leftP.gameObject.SetActive(false);

            emission = rightP.emission;
            emission.rateOverTime = 0;
            rightP.gameObject.SetActive(false);

            emission = frontP.emission;
            emission.rateOverTime = 0;
            frontP.gameObject.SetActive(false);

            emission = backP.emission;
            emission.rateOverTime = 0;
            backP.gameObject.SetActive(false);
        }
    }

    public override void SetAsExit() {
        base.SetAsExit();
        ClearRightWall();
    }
}
