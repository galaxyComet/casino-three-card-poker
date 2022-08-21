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
public class DesignManager : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("material")]
    private Transform CardObject;
    public Material[] cardMaterial;
    public Transform prefab;
    private GameManager gameManager;
    private float[] cardX;
    private float cardY = -305.954f;
    private float cardZ = -882.74f;
    private float[] movecardX = new float[6] { 1676.835f, 1676.986f, 1677.135f, 1676.837f, 1676.987f, 1677.136f };
    private float movecardY = -305.9991f;
    private float[] movecardZ = new float[6] { -883.058f, -883.058f, -883.058f, -882.8187f, -882.8187f, -882.8187f };
    public int[] cardOrderArray;
    void Start()
    {
        cardX = new float[6];
        gameManager = FindObjectOfType<GameManager>();
    }

    public IEnumerator CardOder()
    {
        const float seconds = 0.1f;
        float time = 0;
        float before = 1677.483f;
        cardX[0] = before;
        while (time < seconds)
        {
            for (int i = 0; i < 52; i++)
            {
                float next = before + 0.0045f * i;
                CardObject = Instantiate(prefab, Vector3.Lerp(new Vector3(before, cardY, cardZ), new Vector3(next, cardY, cardZ), time / seconds), Quaternion.identity);
                if (i > 0 && i < 6)
                {
                    cardX[i] = next;
                }
                CardObject.name = "card" + (i + 1);
                CardObject.GetComponent<SpriteRenderer>().material = cardMaterial[cardOrderArray[i]];
                CardObject.transform.localScale = new Vector3(0.008599492f, -0.008231715f, 0.65701f);
                CardObject.transform.eulerAngles = new Vector3(30, 90, 90);
                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
        yield return new WaitForSeconds(1f);
        gameManager.betbtn.interactable = true;
        gameManager.foldbtn.interactable = true;
    }
    // Update is called once per frame
    void Update()
    {

    }
    public IEnumerator ThrowedCardClear(bool flag)
    {
        for (int i = 0; i < 52; i++)
        {
            string name = "card" + (i + 1);
            Destroy(GameObject.Find(name));
        }
        StartCoroutine(gameManager.firstServer());
        yield return new WaitForSeconds(1.5f);
        if (flag)
        {
            StartCoroutine(CardThrow(0, 6));
            StartCoroutine(gameManager.beginServer());
        }
    }
    public IEnumerator CardThrow(int from, int to)
    {
        for (int i = from; i < to; i++)
        {
            float time = 0;
            const float seconds = 0.15f;
            string name = "card" + (i + 1);
            while (time < seconds)
            {
                GameObject.Find(name).transform.position = Vector3.Lerp(new Vector3(cardX[i], cardY, cardZ), new Vector3(movecardX[i], movecardY, movecardZ[i]), time / seconds);
                if (i > 2 && i < 6)
                {
                    GameObject.Find(name).transform.rotation = Quaternion.Lerp(Quaternion.Euler(new Vector3(30, 90, 90)), Quaternion.Euler(new Vector3(90, 90, 90)), time / seconds);
                }
                else
                {
                    GameObject.Find(name).transform.rotation = Quaternion.Lerp(Quaternion.Euler(new Vector3(30, 90, 90)), Quaternion.Euler(new Vector3(-90, 90, 90)), time / seconds);
                }
                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            GameObject.Find(name).transform.position = new Vector3(movecardX[i], movecardY, movecardZ[i]);
            if (i > 2 && i < 6)
            {
                GameObject.Find(name).transform.rotation = Quaternion.Euler(new Vector3(90, 90, 90));
            }
            else
            {
                GameObject.Find(name).transform.rotation = Quaternion.Euler(new Vector3(-90, 90, 90));
            }
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(1.5f);
        gameManager.betbtn.interactable = true;
        gameManager.foldbtn.interactable = true;
    }
    public IEnumerator CardRotate(int from, int to)
    {
        yield return new WaitForSeconds(1f);
        for (int i = from; i < to; i++)
        {
            float time = 0;
            const float seconds = 0.15f;
            string name = "card" + i;
            while (time < seconds)
            {
                GameObject.Find(name).transform.rotation = Quaternion.Lerp(Quaternion.Euler(new Vector3(90, 90, 90)), Quaternion.Euler(new Vector3(-90, 90, 90)), time / seconds);
                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            GameObject.Find(name).transform.rotation = Quaternion.Euler(new Vector3(-90, 90, 90));
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(1f);
    }
}
