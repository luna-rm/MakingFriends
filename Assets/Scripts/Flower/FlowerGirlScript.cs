using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class FlowerGirlScript : MonoBehaviour {
    [SerializeField] GameObject Player;
    [SerializeField] GameObject maze;

    private bool anim0Start = false;
    private bool anim2Start = false;

    [SerializeField] GameObject flower0;
    [SerializeField] GameObject flower1;

    private InteractionScript flower0Interaction;
    private InteractionScript flower1Interaction;

    private ImageScript flower0Image;
    private ImageScript flower1Image;

    [SerializeField] private TextScript flower0Text;
    [SerializeField] private TextScript flower1Text;

    [SerializeField] GameObject Wall0;

    [SerializeField] GameObject textObj;

    [SerializeField] GameObject otherSide;

    private bool canStartAnim0 = false;

    private void Awake() {
        flower0Interaction = flower0.GetComponent<InteractionScript>();
        flower1Interaction = flower1.GetComponent<InteractionScript>();

        flower0Image = flower0.GetComponent<ImageScript>();
        flower1Image = flower1.GetComponent<ImageScript>();
    }

    private void Update() {
        if(!anim0Start) {
            if (flower0Text.GetText() == "Olha pra trás!!") {
                StartCoroutine(startAnim0());
            }

            if(flower0Interaction.dialogueFinished || canStartAnim0) {
                StartCoroutine(anim0());
                anim0Start = true;
            }
        }

        if(!anim2Start) {
            if(flower1Interaction.dialogueFinished || GameEventManager.instance.canOpenMap) {
                StartCoroutine(anim2());
                anim2Start = true;
            }
        }
    }

    private IEnumerator anim0() {
        //GameEventManager.InputContext = InputContextEnum.LOCKED;
        //float elapsedTime = 0f;
        
       // Quaternion startRotation = Player.transform.rotation;

       // Vector3 directionToTarget = flower0.transform.position - Player.transform.position;
        //directionToTarget.y = 0;
        //Quaternion endRotation = Quaternion.LookRotation(directionToTarget);

        //while (elapsedTime < 0.5f) {
        //    float t = elapsedTime / 0.5f;

        //    Player.transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);

        //    elapsedTime += Time.deltaTime;
        //    yield return null;
        //}

        //Player.transform.rotation = endRotation;

        StartCoroutine(flower0Image.disappear());
        yield return new WaitForSeconds(flower0Image.animTime + 0.2f);

        StartCoroutine(anim1());

        //elapsedTime = 0f;
        
        //startRotation = Player.transform.rotation;

        //directionToTarget = flower1.transform.position - Player.transform.position;
        //directionToTarget.y = 0;
        //endRotation = Quaternion.LookRotation(directionToTarget);

        //while (elapsedTime < 1f) {
        //    float t = elapsedTime / 1f;

        //    Player.transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);

        //    elapsedTime += Time.deltaTime;
        //    yield return null;
        //}

        //Player.transform.rotation = endRotation;
        flower0Text.enabled = false;
        flower0.SetActive(false);
    }

    private IEnumerator startAnim0() {
        yield return new WaitForSeconds(0.75f);
        flower0Text.ResetPanel();
        canStartAnim0 = true;
    }

    private IEnumerator anim1() {
        flower1.SetActive (true);
        float elapsedTime = 0f;

        while (elapsedTime < 2f) {
            float newY = Mathf.Lerp(-5f, 0f, (elapsedTime / 2f));
            maze.transform.position = new Vector3(maze.transform.position.x, newY, maze.transform.position.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        maze.transform.position = new Vector3(maze.transform.position.x, 0f, maze.transform.position.z);

        StartCoroutine(flower1Image.appear());
        yield return new WaitForSeconds(flower0Image.animTime);
        flower1Interaction.enabled = true;
        flower1Text.enabled = true;

        Wall0.SetActive(false);

        GameEventManager.InputContext = InputContextEnum.DEFAULT;
    }

    private IEnumerator anim2() {
        StartCoroutine(flower1Image.disappear());
        yield return new WaitForSeconds(flower0Image.animTime + 0.2f);

        flower1Text.enabled = false;
        flower1.SetActive(false);
    }

}
