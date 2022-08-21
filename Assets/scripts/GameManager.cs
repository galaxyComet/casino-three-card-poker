using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Timers;
using Random = UnityEngine.Random;
using TMPro;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System.Linq;
using SimpleJSON;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Start is called before the first frame update
    private DesignManager designManager;
    private PokerControll pokerControll;
    private PairControll pairControll;
    private AntoControll antoControll;
    public TMP_Text totalPriceText;
    private float totalValue;
    private int loop = 0;
    private string FONflag = "NEW BET";
    public Button betbtn;
    public Button foldbtn;
    public bool clickflag = true;
    public static APIForm apiform;
    public static Globalinitial _global;
    [DllImport("__Internal")]
    private static extern void GameReady(string msg);
    BetPlayer _player;
    public void RequestToken(string data)
    {
        JSONNode usersInfo = JSON.Parse(data);
        _player.token = usersInfo["token"];
        _player.username = usersInfo["userName"];
        float i_balance = float.Parse(usersInfo["amount"]);
        totalValue = i_balance;
        totalPriceText.text = totalValue.ToString("F2");
    }
    void Start()
    {
        _player = new BetPlayer();
#if UNITY_WEBGL == true && UNITY_EDITOR == false
            GameReady("Ready");
#endif
        StartCoroutine(firstServer());
        designManager = FindObjectOfType<DesignManager>();
        pokerControll = FindObjectOfType<PokerControll>();
        pairControll = FindObjectOfType<PairControll>();
        antoControll = FindObjectOfType<AntoControll>();
        betbtn.interactable = false;
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void BetOrRebet()
    {
        if (pokerControll.AntoValue == 0)
        {
            StartCoroutine(alert("Set balance!", "other"));
        }
        else
        {
            if (totalValue >= pokerControll.AntoValue)
            {
                if (totalValue >= 5)
                {
                    betbtn.interactable = false;
                    foldbtn.interactable = false;
                    switch (loop)
                    {
                        case 0:
                            StartCoroutine(cardActiveClear());
                            StartCoroutine(UpdateCoinsAmount(totalValue, totalValue - pokerControll.PairValue - pokerControll.AntoValue));
                            StartCoroutine(designManager.CardThrow(0, 6));
                            clickflag = false;
                            foldbtn.transform.GetChild(0).GetComponent<TMP_Text>().text = "FOLD";
                            FONflag = "FOLD";
                            betbtn.transform.GetChild(0).GetComponent<TMP_Text>().text = "BET";
                            loop = loop + 1;
                            StartCoroutine(beginServer());
                            break;
                        case 1:
                            StartCoroutine(cardActiveClear());
                            StartCoroutine(pokerControll.betAction());
                            StartCoroutine(UpdateCoinsAmount(totalValue, totalValue - pokerControll.AntoValue));
                            foldbtn.transform.GetChild(0).GetComponent<TMP_Text>().text = "NEW BET";
                            FONflag = "NEW BET";
                            betbtn.transform.GetChild(0).GetComponent<TMP_Text>().text = "REBET";
                            loop = loop + 1;
                            break;
                        case 2:
                            StartCoroutine(betformat());
                            StartCoroutine(UpdateCoinsAmount(totalValue, totalValue - pokerControll.PairValue - pokerControll.AntoValue));
                            StartCoroutine(designManager.ThrowedCardClear(true));
                            clickflag = false;
                            foldbtn.transform.GetChild(0).GetComponent<TMP_Text>().text = "FOLD";
                            FONflag = "FOLD";
                            betbtn.transform.GetChild(0).GetComponent<TMP_Text>().text = "BET";
                            loop = 1;
                            break;
                    }
                }
                else
                {
                    StartCoroutine(alert("Insufficient balance!", "other"));
                }
            }
            else
            {
                StartCoroutine(alert("Insufficient balance!", "other"));
            }
        }
    }
    public void NewOrFold()
    {
        foldbtn.interactable = false;
        betbtn.transform.GetChild(0).GetComponent<TMP_Text>().text = "DEAL";
        loop = 0;
        switch (FONflag)
        {
            case "FOLD":
                foldbtn.transform.GetChild(0).GetComponent<TMP_Text>().text = "NEW BET";
                FONflag = "NEW BET";
                break;
            case "NEW BET":
                StartCoroutine(BetClear());
                break;
        }
        StartCoroutine(designManager.ThrowedCardClear(false));
    }
    public IEnumerator firstServer()
    {
        yield return new WaitForSeconds(0.5f);
        WWWForm form = new WWWForm();
        form.AddField("userName", _player.username);
        form.AddField("token", _player.token);
        _global = new Globalinitial();
        UnityWebRequest www = UnityWebRequest.Post(_global.BaseUrl + "api/CardOder", form);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            string strdata = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
            apiform = JsonUtility.FromJson<APIForm>(strdata);
            if (apiform.serverMsg == "Success")
            {
                designManager.cardOrderArray = apiform.cardOder;
                StartCoroutine(designManager.CardOder());
            }
            else
            {
                StartCoroutine(alert(apiform.serverMsg, "other"));
            }
        }
        else
        {
            StartCoroutine(alert("Can't find server!", "other"));
        }
    }
    public IEnumerator beginServer()
    {
        yield return new WaitForSeconds(0.5f);
        WWWForm form = new WWWForm();
        form.AddField("userName", _player.username);
        form.AddField("token", _player.token);
        form.AddField("antoAmount", pokerControll.AntoValue.ToString());
        form.AddField("pairAmount", pokerControll.PairValue.ToString());
        form.AddField("amount", totalValue.ToString("F2"));
        _global = new Globalinitial();
        UnityWebRequest www = UnityWebRequest.Post(_global.BaseUrl + "api/bet-threePoker", form);
        yield return new WaitForSeconds(0.00001f);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            string strdata = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
            apiform = JsonUtility.FromJson<APIForm>(strdata);
            if (apiform.serverMsg == "Success")
            {
                yield return new WaitForSeconds(0.5f);
                if (apiform.activeArray.Length > 0)
                {
                    if (apiform.msg != "")
                    {
                        StartCoroutine(alert(apiform.msg, "win"));
                    }
                    for (int i = 0; i < apiform.activeArray.Length; i++)
                    {
                        string name = "card" + (apiform.activeArray[i] + 1);
                        GameObject.Find(name).GetComponent<SpriteRenderer>().material.color = Color.yellow;
                    }
                }
                StartCoroutine(UpdateCoinsAmount(totalValue, apiform.total));
            }
            else
            {
                StartCoroutine(alert(apiform.serverMsg, "other"));
                StartCoroutine(UpdateCoinsAmount(totalValue, totalValue + pokerControll.PairValue + pokerControll.AntoValue));
            }
        }
        else
        {
            StartCoroutine(alert("Can't find server!", "other"));
            StartCoroutine(UpdateCoinsAmount(totalValue, totalValue + pokerControll.PairValue + pokerControll.AntoValue));
        }
    }
    public IEnumerator Server()
    {
        yield return new WaitForSeconds(0.5f);
        WWWForm form = new WWWForm();
        form.AddField("userName", _player.username);
        form.AddField("token", _player.token);
        form.AddField("betAmount", pokerControll.AntoValue.ToString());
        form.AddField("amount", totalValue.ToString("F2"));
        _global = new Globalinitial();
        UnityWebRequest www = UnityWebRequest.Post(_global.BaseUrl + "api/result-holdem", form);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            string strdata = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
            apiform = JsonUtility.FromJson<APIForm>(strdata);
            if (apiform.serverMsg == "Success")
            {
                if (apiform.gameResult == "winUser")
                {
                    StartCoroutine(alert(apiform.msg, "win"));
                }
                else
                {
                    StartCoroutine(alert(apiform.msg, "other"));
                }
                StartCoroutine(UpdateCoinsAmount(totalValue, apiform.total));
                yield return new WaitForSeconds(1f);
                for (int i = 0; i < apiform.activeArray.Length; i++)
                {
                    string name = "";
                    if (apiform.gameResult == "winUser")
                    {
                        name = "card" + (apiform.activeArray[i] + 1);
                    }
                    else
                    {
                        name = "card" + (apiform.activeArray[i] + 4);
                    }
                    GameObject.Find(name).GetComponent<SpriteRenderer>().material.color = Color.yellow;
                }
            }
            else
            {
                StartCoroutine(alert(apiform.serverMsg, "other"));
                StartCoroutine(UpdateCoinsAmount(totalValue, totalValue + pokerControll.AntoValue));
            }
            yield return new WaitForSeconds(1.5f);
        }
        else
        {
            StartCoroutine(alert("Can't find server!", "other"));
            StartCoroutine(UpdateCoinsAmount(totalValue, totalValue + pokerControll.AntoValue));
        }
    }
    public IEnumerator alert(string msg, string state)
    {
        if (state == "win")
        {
            AlertController.isWin = true;
        }
        else
        {
            AlertController.isLose = true;
        }
        GameObject.Find("alert").GetComponent<TMP_Text>().text = msg;
        yield return new WaitForSeconds(2.5f);
        AlertController.isWin = false;
        AlertController.isLose = false;
        yield return new WaitForSeconds(1.5f);
        betbtn.interactable = true;
        foldbtn.interactable = true;
    }
    private IEnumerator UpdateCoinsAmount(float preValue, float changeValue)
    {
        // Animation for increasing and decreasing of coins amount
        const float seconds = 0.2f;
        float elapsedTime = 0;
        while (elapsedTime < seconds)
        {
            totalPriceText.text = Mathf.Floor(Mathf.Lerp(preValue, changeValue, (elapsedTime / seconds))).ToString();
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        totalValue = changeValue;
        totalPriceText.text = totalValue.ToString();
    }
    public IEnumerator cardActiveClear()
    {
        for (int i = 0; i < 6; i++)
        {
            string name = "card" + (i + 1);
            GameObject.Find(name).GetComponent<SpriteRenderer>().material.color = Color.white;
        }
        yield return new WaitForSeconds(0.1f);
    }
    public IEnumerator BetClear()
    {
        clickflag = true;
        pokerControll.AntoValue = 0;
        pokerControll.PairValue = 0;
        pokerControll.BetValue = 0;
        pokerControll.AntoValueText.text = pokerControll.AntoValue.ToString();
        pokerControll.PairValueText.text = pokerControll.PairValue.ToString();
        pokerControll.BetValueText.text = pokerControll.BetValue.ToString();
        betbtn.transform.GetChild(0).GetComponent<TMP_Text>().text = "DEAL";
        loop = 0;
        StartCoroutine(cardActiveClear());
        for (int i = 0; i < pairControll.loop; i++)
        {
            string name = "pairPoker" + (i + 1);
            Destroy(GameObject.Find(name));
        }
        pairControll.loop = 0;
        for (int i = 0; i < antoControll.loop; i++)
        {
            string name = "antoPoker" + (i + 1);
            Destroy(GameObject.Find(name));
        }
        antoControll.loop = 0;
        for (int i = 0; i < pokerControll.betloop; i++)
        {
            string name = "betPoker" + (i + 1);
            Destroy(GameObject.Find(name));
        }
        pokerControll.betloop = 0;
        yield return new WaitForSeconds(0.5f);
    }
    public IEnumerator betformat()
    {
        StartCoroutine(cardActiveClear());
        for (int i = 0; i < pokerControll.betloop; i++)
        {
            string name = "betPoker" + (i + 1);
            Destroy(GameObject.Find(name));
        }
        pokerControll.betloop = 0;
        pokerControll.BetValue = 0;
        pokerControll.BetValueText.text = pokerControll.BetValue.ToString();
        yield return new WaitForSeconds(1f);
    }
}
public class BetPlayer
{
    public string username;
    public string token;
}
