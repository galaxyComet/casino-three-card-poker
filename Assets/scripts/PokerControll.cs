using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Timers;
using Random = UnityEngine.Random;
using TMPro;
using UnityEngine.Networking;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using System.Linq;
using UnityEngine.SceneManagement;
using SimpleJSON;
public class PokerControll : MonoBehaviour 
{
    private AntoControll antoControll;
    private DesignManager designManager;
    private GameManager gameManager;
    public int loop = 0;
    public int betloop = 0;
    public bool clickAble = true;
    public Transform Poker;
    public Transform prefab;
    private Transform PokerPieces;
    public TMP_Text everyBetAmountText;
    public int everyBetAmount = 5;
    public int AntoValue = 0;
    public TMP_Text AntoValueText;
    public int PairValue = 0;
    public TMP_Text PairValueText;
    public int BetValue = 0;
    public TMP_Text BetValueText;
    public Color betColor = Color.black;
    // Start is called before the first frame update
    void Start()
    {
        antoControll = FindObjectOfType<AntoControll>();
        designManager = FindObjectOfType<DesignManager>();
        gameManager = FindObjectOfType<GameManager>();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnMouseDown()
    {
        switch (loop)
        {
            case 0:
                Poker.GetComponent<MeshRenderer>().materials[0].color = Color.white;
                Poker.GetComponent<MeshRenderer>().materials[1].color = Color.cyan;
                everyBetAmount = 10;
                betColor = Color.cyan;
                loop = loop + 1;
                break;
            case 1:
                Poker.GetComponent<MeshRenderer>().materials[0].color = Color.white;
                Poker.GetComponent<MeshRenderer>().materials[1].color = Color.magenta;
                everyBetAmount = 15;
                betColor = Color.magenta;
                loop = loop + 1;
                break;
            case 2:
                Poker.GetComponent<MeshRenderer>().materials[0].color = Color.white;
                Poker.GetComponent<MeshRenderer>().materials[1].color = Color.yellow;
                everyBetAmount = 20;
                betColor = Color.yellow;
                loop = loop + 1;
                break;
            case 3:
                Poker.GetComponent<MeshRenderer>().materials[0].color = Color.white;
                Poker.GetComponent<MeshRenderer>().materials[1].color = Color.blue;
                betColor = Color.blue;
                everyBetAmount = 25;
                loop = loop + 1;
                break;
            case 4:
                Poker.GetComponent<MeshRenderer>().materials[0].color = Color.white;
                Poker.GetComponent<MeshRenderer>().materials[1].color = Color.green;
                betColor = Color.green;
                everyBetAmount = 50;
                loop = loop + 1;
                break;
            case 5:
                Poker.GetComponent<MeshRenderer>().materials[0].color = Color.white;
                Poker.GetComponent<MeshRenderer>().materials[1].color = Color.red;
                everyBetAmount = 100;
                betColor = Color.red;
                loop = loop + 1;
                break;
            case 6:
                Poker.GetComponent<MeshRenderer>().materials[0].color = Color.white;
                Poker.GetComponent<MeshRenderer>().materials[1].color = Color.black;
                everyBetAmount = 5;
                betColor = Color.black;
                loop = 0;
                break;
        }
        everyBetAmountText.text = everyBetAmount.ToString();
    }
    public IEnumerator pokerOder(float x,float y,float z,string name, int n)
    {
        if (name == "antoPoker")
        {
            AntoValue = AntoValue + everyBetAmount;
            AntoValueText.text = AntoValue.ToString();
        }
        else
        {
            PairValue = PairValue + everyBetAmount;
            PairValueText.text = PairValue.ToString();
        }
        PokerPieces = Instantiate(prefab, new Vector3(1676.992f, -305.9f, -883.3063f), Quaternion.identity);
        PokerPieces.name = name + n;
        PokerPieces.GetComponent<MeshRenderer>().materials[0].color = Color.white;
        PokerPieces.GetComponent<MeshRenderer>().materials[1].color = Poker.GetComponent<MeshRenderer>().materials[1].color;
        PokerPieces.transform.GetChild(0).GetComponent<TMP_Text>().text = everyBetAmount.ToString();
        PokerPieces.transform.localScale = new Vector3(5f, 5f, 1f);
        const float seconds = 0.3f;
        float time = 0;
        float yy = y + (0.0083f * (n - 1));
        while (time < seconds)
        {
            PokerPieces.transform.position = Vector3.Lerp(new Vector3(1676.992f, -305.9f, -883.3063f),
                new Vector3(x, yy, z), time / seconds);
            PokerPieces.transform.localScale = new Vector3(5f, 5f, 1f);
            PokerPieces.transform.rotation = Quaternion.Lerp(Quaternion.Euler(new Vector3(-128.999f, 0, 0)), Quaternion.Euler(new Vector3(-90, 0, 0)), time / seconds);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        PokerPieces.transform.position = new Vector3(x, yy, z);
        yield return new WaitForSeconds(0.0001f);
        clickAble = true;
    }
    public IEnumerator betAction() {
        for (int i = 0; i < antoControll.loop; i++)
        {
            float yz = -305.9865f + (0.0083f * (antoControll.loop - 1));
            PokerPieces = Instantiate(prefab, new Vector3(1676.579f, yz, -882.707f), Quaternion.identity);
            PokerPieces.name = "betPoker" + (i + 1);
            string name = "antoPoker" + (i + 1);
            PokerPieces.GetComponent<MeshRenderer>().materials[0].color = Color.white;
            PokerPieces.GetComponent<MeshRenderer>().materials[1].color = GameObject.Find(name).GetComponent<MeshRenderer>().materials[1].color;
            PokerPieces.transform.GetChild(0).GetComponent<TMP_Text>().text = GameObject.Find(name).transform.GetChild(0).GetComponent<TMP_Text>().text;
            PokerPieces.transform.localScale = new Vector3(5f, 5f, 1f);

            const float seconds = 0.3f;
            float time = 0;
            float yy = -305.96f + (0.0083f * (i - 1));
            while (time < seconds)
            {
                PokerPieces.transform.position = Vector3.Lerp(new Vector3(1676.579f, -305.9865f, -882.707f),
                    new Vector3(1677.482f, yy, -883.007f), time / seconds);

                PokerPieces.transform.localScale = new Vector3(5f, 5f, 1f);
                PokerPieces.transform.rotation = Quaternion.Lerp(Quaternion.Euler(new Vector3(-128.999f, 0, 0)), Quaternion.Euler(new Vector3(-90, 0, 0)), time / seconds);
                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            PokerPieces.transform.position = new Vector3(1677.482f, yy, -883.007f);
            yield return new WaitForSeconds(0.0001f);
        }
        betloop = antoControll.loop;
        BetValue = AntoValue;
        BetValueText.text = BetValue.ToString();
        yield return new WaitForSeconds(0.0001f);
        StartCoroutine(designManager.CardRotate(4, 7));
        StartCoroutine(gameManager.Server());
    }
}
