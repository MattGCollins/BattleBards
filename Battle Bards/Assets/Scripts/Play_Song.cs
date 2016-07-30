using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Play_Song : MonoBehaviour {

    const int framesPerSec = 50;

    public GameObject MenuBar;
    public GameObject BeatBar;
    public static int maxHealth;
    public static int health;
    public static int enemyMaxHealth;
    public static int enemyHealth;
    public static int endState; // 0 = none, 1 = win, 2 = lose, 3 = flee

    private GameObject EndText;
    private GameObject canvas;
    private GameObject enemyHealthText;
    private GameObject playerHealthText;
    private GameObject enemyHealthBar;
    private GameObject playerHealthBar;
    private GameObject enemyText;
    private GameObject playerText;
    private int cycles = 0;
    private int time;
    private int iteration;
    private int prevInput;
    private int input;
    private int node;
    private int minCombo = 20;
    private int playing;
    private int[] combos;

    void Start()
    {
        canvas = GameObject.Find("Canvas");
        EndText = GameObject.Find("EndText");
        enemyHealthText = GameObject.Find("EnemyHealthText");
        playerHealthText = GameObject.Find("PlayerHealthText");
        enemyHealthBar = GameObject.Find("EnemyHealth");
        playerHealthBar = GameObject.Find("PlayerHealth");
        enemyText = GameObject.Find("EnemyText");
        playerText = GameObject.Find("KnightText");
        EndText.GetComponent<Text>().text = "";
        enemyText.GetComponent<TextMesh>().text = "";
        playerText.GetComponent<TextMesh>().text = "";
        time = 0;
        iteration = 0;
        prevInput = 0;
        input = 0;
        node = 0;
        playing = 1;
        endState = 0;
        maxHealth = 10;
        enemyMaxHealth = 10;
        health = 10;
        enemyHealth = 10;
        combos = new int[] {21, 27, 33, 48, 57, 59, 78, 497, 624, 782, 923, 1069, 1081, 1232,
                                7225, 9998, 12785, 17307, 18640, 20080};
        enemyHealthText.GetComponent<Text>().text = enemyHealth.ToString() + "/" + enemyMaxHealth.ToString();
        playerHealthText.GetComponent<Text>().text = health.ToString() + "/" + maxHealth.ToString();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // if on beat
        if (time % framesPerSec == 0)
        {
            // reset player and enemy texts
            enemyText.GetComponent<TextMesh>().text = "";
            playerText.GetComponent<TextMesh>().text = "";
            // if in combat
            if (playing == 1)
            {
                // if start of combo, send out first beat bar
                if (iteration == 0 && time > framesPerSec*4)
                {
                    Instantiate(BeatBar, transform.position + new Vector3(0, 1.5f, -2), Quaternion.Euler(0, 0, 0));
                    iteration++;
                }
                // otherwise, send out a bar and get input
                if (iteration > 0 && time > framesPerSec*4)
                {
                    Instantiate(BeatBar, transform.position + new Vector3(0, 1.5f, -2), Quaternion.Euler(0, 0, 0));
					input = 0;//MenuPress.choice;
                    node += input * (int)Mathf.Pow(4, iteration - 1);
                    Debug.Log(node);
                    // if a button is pressed calculate the node
                    if (input != 0)
                    {
                        iteration++;
                    }
                    // otherwise, check if node is in combos
                    if (input == 0)
                    {
                        // if not in combos and less than min combo, reset combo
                        // NOTE: using node <=  minCombo is a convoluted way of doing this; should probably use iterations > 1
                        if (!combos.Contains(node) && node <= minCombo)
                        {
                            // TODO: maybe take damage here and check if dead
                            iteration = 0;
                            node = 0;
                        }
                        // if not in combos, take damage and reset combo
                        if (!combos.Contains(node) && node > minCombo)
                        {
                            // subtract 1 from player health and scale proportionally 
                            health--;
                            playerText.GetComponent<TextMesh>().color = new Color(255, 0, 0, 255);
                            playerText.GetComponent<TextMesh>().text = "-1";
                            playerHealthBar.transform.localScale -= new Vector3(.09f*maxHealth/10, 0, 0);
                            playerHealthText.GetComponent<Text>().text = health.ToString() + "/10";
                            // IF YOU DIE
                            if (health == 0)
                            {
                                Debug.Log("You died");
                                Instantiate(EndText, new Vector3(0, 0, -3), Quaternion.Euler(0, 0, 0));
                                EndText.GetComponent<Text>().color = new Color(255, 0, 0, 255);
                                EndText.GetComponent<Text>().text = "YOU LOSE";
                                endState = 2;
                                playing = 0;
                                StartCoroutine(die());
                            }
                            iteration = 0;
                            node = 0;
                        }
                        // if yes, perform action and reset combo
                        if (combos.Contains(node))
                        {
                            switch (node)
                            {
                                // perform action
                                case 33:
                                    // Regular Attack 3
                                    attack(1, 0);
                                    break;
                                case 57:
                                    // Red Attack 3
                                    attack(1, 1);
                                    break;
                                case 48:
                                    // Yellow Attack 3
                                    attack(1, 2);
                                    break;
                                case 78:
                                    // Green Attack 3
                                    attack(1, 3);
                                    break;
                                case 27:
                                    // Blue Attack 3
                                    attack(1, 4);
                                    break;
                                case 59:
                                    // Heal 3
                                    heal(1);
                                    break;
                                case 21:
                                    Debug.Log("Help 3");
                                    break;
                                case 497:
                                    // Regular Attack 5
                                    attack(2, 0);
                                    break;
                                case 1081:
                                    // Red Attack 5
                                    attack(2, 1);
                                    break;
                                case 624:
                                    // Yellow Attack 5
                                    attack(2, 2);
                                    break;
                                case 782:
                                    // Green Attack 5
                                    attack(2, 3);
                                    break;
                                case 923:
                                    // Blue Attack 5
                                    attack(2, 4);
                                    break;
                                case 1232:
                                    // Heal 5
                                    heal(2);
                                    break;
                                case 1069:
                                    // IF YOU FLEE
                                    Debug.Log("Flee 5");
                                    Instantiate(EndText, new Vector3(0, 0, -3), Quaternion.Euler(0, 0, 0));
                                    EndText.GetComponent<Text>().color = new Color(0, 255, 0, 255);
                                    EndText.GetComponent<Text>().text = "YOU ESCAPED";
                                    endState = 3;
                                    playing = 0;
                                    StartCoroutine(flee());
                                    break;
                                case 12785:
                                    // Regular Attack 7
                                    attack(3, 0);
                                    break;
                                case 7225:
                                    // Red Attack 7
                                    attack(3, 1);
                                    break;
                                case 20080:
                                    // Yellow Attack 7
                                    attack(3, 2);
                                    break;
                                case 9998:
                                    // Green Attack 7
                                    attack(3, 3);
                                    break;
                                case 17307:
                                    // Blue Attack 7
                                    attack(3, 4);
                                    break;
                                case 18640:
                                    // Heal 7
                                    heal(3);
                                    break;
                            }
                            Debug.Log(node);
                            // IF YOU KILL ENEMY
                            if (enemyHealth == 0)
                            {
                                Debug.Log("You win!");
                                Instantiate(EndText, transform.position + new Vector3(0, 0, -3), Quaternion.Euler(0, 0, 0));
                                EndText.GetComponent<Text>().color = new Color(0, 255, 0, 255);
                                EndText.GetComponent<Text>().text = "YOU WIN";
                                endState = 1;
                                playing = 0;
                                StartCoroutine(win());
                            }
                            iteration = 0;
                            node = 0;
                        }
                    }
                }
            }
        }
    time++;
    }

    void attack(int dmg, int element)
    {
        Debug.Log("Attack " + dmg.ToString() + " " + element.ToString());
        // if regular attack
        if (element == 0)
        {
            // subtract dmg from enemy health and scale bar
            enemyHealth -= dmg;
            enemyText.GetComponent<TextMesh>().color = new Color(255, 0, 0, 255);
            enemyText.GetComponent<TextMesh>().text = "-" + dmg.ToString();
            if (enemyHealth < 0)
            {
                enemyHealth = 0;
                enemyHealthBar.transform.localScale = new Vector3(0, 0, 0);
            }
            else
            {
                enemyHealthBar.transform.localScale -= new Vector3(.09f * dmg * enemyMaxHealth / 10, 0, 0);
            }
            enemyHealthText.GetComponent<Text>().text = enemyHealth.ToString() + "/10";
        }
        // if elemental attack
        else if (element >= 1 && element <= 4)
        {
            // TODO add elemental attacks
            Debug.Log("Elemental Attack");
        }
    }

    void heal(int hp)
    {
        Debug.Log("Heal " + hp.ToString());
        // add dmg to player health and scale bar
        health += hp;
        playerText.GetComponent<TextMesh>().color = new Color(0, 255, 0, 255);
        playerText.GetComponent<TextMesh>().text = "+" + hp.ToString();
        if (health > 10)
        {
            playerHealthBar.transform.localScale = new Vector3(.9f, .9f, 1);
            health = 10;
        }
        else
        {
            playerHealthBar.transform.localScale += new Vector3(.09f * hp * maxHealth / 10, 0, 0);
        }
        playerHealthText.GetComponent<Text>().text = health.ToString() + "/10";
    }

    IEnumerator die()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("testScene");
    }

    IEnumerator flee()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("testScene");
    }

    IEnumerator win()
    {
        Debug.Log("1");
        yield return new WaitForSeconds(5);
        Debug.Log("2");
        SceneManager.LoadScene("testScene");
        Debug.Log("3");
    }
}
