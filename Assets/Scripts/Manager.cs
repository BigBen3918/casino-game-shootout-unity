using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Runtime.InteropServices;
using SimpleJSON;

public class Manager : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void GameReady(string msg);
    public TMP_InputField BetAmount;
    public TMP_InputField TotalAmount;
    public TMP_Text Alert;
    private int flag = 0;
    private int goal_number = 0;
    private float betAmount;
    private float totalAmount;
    public Button disable_Play;
    public Button disable_increase;
    public Button disable_random;
    public Button disable_cash;
    public Button disable_decrease;
    public static ReceiveJsonObject apiform;
    public GameObject ballbutton;
    public GameObject Ball;
    BetPlayer _player;
    private string BaseUrl = "http://83.136.219.243";
    void Start()
    {
        betAmount = 10f;
        BetAmount.text = betAmount.ToString("F2");
        ballbutton.SetActive(false);
        disable_random.interactable = false;
        disable_cash.interactable = false;
        _player = new BetPlayer();
#if UNITY_WEBGL == true && UNITY_EDITOR == false
        GameReady("Ready");
#endif
    }
    void Update()
    {
        if (goal_number == 0 || !disable_random.interactable)
        {
            disable_cash.interactable = false;
        }
        else
        {
            disable_cash.interactable = true;
        }
    }
    public void RequestToken(string data)
    {
        JSONNode usersInfo = JSON.Parse(data);
        _player.token = usersInfo["token"];
        totalAmount = float.Parse(usersInfo["amount"]);
        TotalAmount.text = totalAmount.ToString("F2");
    }
    public void double_increase()
    {
        StartCoroutine(_double_increase());
    }
    IEnumerator _double_increase()
    {
        if (flag == 0)
        {
            disable_increase.interactable = false;
            betAmount = float.Parse(BetAmount.text);
            if (totalAmount >= 2 * betAmount)
            {
                betAmount = 2 * betAmount;
            }
            else if (totalAmount < 10f)
            {
                Alert.text = "";
                Alert.text = "NOT ENOUGH BALANCE!";
                yield return new WaitForSeconds(2f);
                Alert.text = "";
                disable_increase.interactable = true;
            }
            else
            {
                betAmount = totalAmount;
                Alert.text = "";
                Alert.text = "MAXIMUM BET LIMIT " + totalAmount.ToString("F2") + "!";
                yield return new WaitForSeconds(2f);
                Alert.text = "";
                disable_increase.interactable = true;

            }
            BetAmount.text = betAmount.ToString("F2");
            disable_increase.interactable = true;
        }

    }
    public void double_decrease()
    {
        StartCoroutine(_double_decrease());
    }
    IEnumerator _double_decrease()
    {
        if (flag == 0)
        {
            disable_decrease.interactable = false;
            betAmount = float.Parse(BetAmount.text);
            if (totalAmount >= betAmount / 2)
            {
                if (betAmount / 2 >= 10f)
                {
                    betAmount = betAmount / 2;
                }
                else
                {
                    betAmount = 10f;
                    Alert.text = "";
                    Alert.text = "MINIMUM BET LIMIT" + " 10.00 !";
                    yield return new WaitForSeconds(2f);
                    Alert.text = "";
                    disable_decrease.interactable = true;

                }
            }
            else if (totalAmount < 10f)
            {
                betAmount = 10f;
                Alert.text = "";
                Alert.text = "MINIMUM BET LIMIT" + " 10.00 !";
                yield return new WaitForSeconds(2f);
                Alert.text = "";
                disable_decrease.interactable = true;
            }
            else
            {
                betAmount = totalAmount;
            }
            BetAmount.text = betAmount.ToString("F2");
            disable_decrease.interactable = true;
        }
    }
    public void Play()
    {

        disable_Play.interactable = false;

        Alert.text = "";
        flag = 1;
        betAmount = float.Parse(BetAmount.text);
        if (betAmount < 10)
        {
            betAmount = 10;
            Alert.text = "";
            Alert.text = "MINIMUM BET LIMIT 10.00!";
            goal_number = 0;
            flag = 0;
            disable_Play.interactable = true;
            disable_random.interactable = false;
        }
        else
        {
            if (totalAmount >= betAmount)
            {
                StartCoroutine(_Play());
            }
            else
            {
                Alert.text = "";
                Alert.text = "NOT ENOUGH BALANCE!";
                goal_number = 0;
                flag = 0;
                disable_Play.interactable = true;
                disable_random.interactable = false;
            }
        }
    }
    IEnumerator _Play()
    {
        betAmount = float.Parse(BetAmount.text);
        WWWForm form = new WWWForm();
        form.AddField("token", _player.token);
        form.AddField("betAmount", betAmount.ToString("F2"));
        UnityWebRequest www = UnityWebRequest.Post(BaseUrl + "/api/Play", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Alert.text = "";
            Alert.text = "CANNOT FIND SERVER!";
            goal_number = 0;
            flag = 0;
            disable_Play.interactable = true;
            disable_random.interactable = false;
        }
        else
        {

            string strdata = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
            apiform = JsonUtility.FromJson<ReceiveJsonObject>(strdata);
            if (apiform.Message == "SUCCESS!")
            {
                disable_random.interactable = true;
                ballbutton.SetActive(true);
            }
            else if (apiform.Message == "BET ERROR!")
            {
                Alert.text = "";
                Alert.text = "BET ERROR!";
                goal_number = 0;
                flag = 0;
                disable_Play.interactable = true;
                disable_random.interactable = false;
            }
        }
    }
    IEnumerator Server(int num)
    {
        WWWForm form = new WWWForm();
        form.AddField("num", num.ToString());
        UnityWebRequest www = UnityWebRequest.Post(BaseUrl + "/api/ball", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Alert.text = "";
            Alert.text = "CANNOT FIND SERVER!";
            goal_number = 0;
            flag = 0;
            disable_Play.interactable = true;
            disable_random.interactable = false;
        }
        else
        {
            string strdata = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
            apiform = JsonUtility.FromJson<ReceiveJsonObject>(strdata);
            if (apiform.Message == "SUCCESS!")
            {
                if (apiform.keeper_number == num)
                {
                    if (apiform.keeper_number == 0)
                    {
                        AnimatorController.iskeeper0 = true;
                        AnimatorController._isBall0 = true;
                    }
                    else if (apiform.keeper_number == 1)
                    {
                        AnimatorController.iskeeper1 = true;
                        AnimatorController._isBall1 = true;
                    }
                    else if (apiform.keeper_number == 2)
                    {
                        AnimatorController.iskeeper2 = true;
                        AnimatorController._isBall2 = true;
                    }
                    else if (apiform.keeper_number == 3)
                    {
                        AnimatorController.iskeeper3 = true;
                        AnimatorController._isBall3 = true;
                    }
                    else if (apiform.keeper_number == 4)
                    {
                        AnimatorController.iskeeper4 = true;
                        AnimatorController._isBall4 = true;
                    }

                    yield return new WaitForSeconds(1.2f);
                    goal_number = 0;
                    flag = 0;
                    ballbutton.SetActive(false);
                    Alert.text = "";
                    Alert.text = "NO GOAL";
                    AnimatorController.iskeeper0 = false;
                    AnimatorController.iskeeper1 = false;
                    AnimatorController.iskeeper2 = false;
                    AnimatorController.iskeeper3 = false;
                    AnimatorController.iskeeper4 = false;


                    AnimatorController._isBall0 = false;
                    AnimatorController._isBall1 = false;
                    AnimatorController._isBall2 = false;
                    AnimatorController._isBall3 = false;
                    AnimatorController._isBall4 = false;

                    Ball.SetActive(false);

                    yield return new WaitForSeconds(1f);

                    Ball.SetActive(true);
                    disable_Play.interactable = true;
                    Alert.text = "";
                    StartCoroutine(_cash());
                }
                else
                {
                    if (num == 0)
                    {
                        AnimatorController.isBall0 = true;
                    }
                    else if (num == 1)
                    {
                        AnimatorController.isBall1 = true;
                    }
                    else if (num == 2)
                    {
                        AnimatorController.isBall2 = true;
                    }
                    else if (num == 3)
                    {
                        AnimatorController.isBall3 = true;
                    }
                    else if (num == 4)
                    {
                        AnimatorController.isBall4 = true;
                    }


                    if (apiform.keeper_number == 0)
                    {
                        AnimatorController.iskeeper0 = true;
                    }
                    else if (apiform.keeper_number == 1)
                    {
                        AnimatorController.iskeeper1 = true;
                    }
                    else if (apiform.keeper_number == 2)
                    {
                        AnimatorController.iskeeper2 = true;
                    }
                    else if (apiform.keeper_number == 3)
                    {
                        AnimatorController.iskeeper3 = true;
                    }
                    else if (apiform.keeper_number == 4)
                    {
                        AnimatorController.iskeeper4 = true;
                    }

                    yield return new WaitForSeconds(1.2f);
                    goal_number += 1;
                    Alert.text = "";
                    Alert.text = "GOAL : " + goal_number.ToString();

                    AnimatorController.iskeeper0 = false;
                    AnimatorController.iskeeper1 = false;
                    AnimatorController.iskeeper2 = false;
                    AnimatorController.iskeeper3 = false;
                    AnimatorController.iskeeper4 = false;

                    AnimatorController.isBall0 = false;
                    AnimatorController.isBall1 = false;
                    AnimatorController.isBall2 = false;
                    AnimatorController.isBall3 = false;
                    AnimatorController.isBall4 = false;

                    Ball.SetActive(false);

                    yield return new WaitForSeconds(1f);

                    Ball.SetActive(true);
                    disable_random.interactable = true;
                    ballbutton.SetActive(true);
                    if (goal_number == 5)
                    {
                        StartCoroutine(_cash());
                    }
                }
            }
        }
    }
    IEnumerator _random()
    {
        disable_random.interactable = false;
        WWWForm form = new WWWForm();
        UnityWebRequest www = UnityWebRequest.Post(BaseUrl + "/api/_random", form);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Alert.text = "";
            Alert.text = "CANNOT FIND SERVER!";
            flag = 0;
            goal_number = 0;
            ballbutton.SetActive(false);
            disable_Play.interactable = true;
            disable_random.interactable = false;
        }
        else
        {
            string strdata = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
            apiform = JsonUtility.FromJson<ReceiveJsonObject>(strdata);
            if (apiform.Message == "SUCCESS!")
            {
                if (apiform.ball_number == apiform.keeper_number)
                {
                    if (apiform.keeper_number == 0)
                    {
                        AnimatorController.iskeeper0 = true;
                        AnimatorController._isBall0 = true;
                    }
                    else if (apiform.keeper_number == 1)
                    {
                        AnimatorController.iskeeper1 = true;
                        AnimatorController._isBall1 = true;
                    }
                    else if (apiform.keeper_number == 2)
                    {
                        AnimatorController.iskeeper2 = true;
                        AnimatorController._isBall2 = true;
                    }
                    else if (apiform.keeper_number == 3)
                    {
                        AnimatorController.iskeeper3 = true;
                        AnimatorController._isBall3 = true;
                    }
                    else if (apiform.keeper_number == 4)
                    {
                        AnimatorController.iskeeper4 = true;
                        AnimatorController._isBall4 = true;
                    }

                    yield return new WaitForSeconds(1f);
                    goal_number = 0;
                    flag = 0;
                    ballbutton.SetActive(false);
                    Alert.text = "";
                    Alert.text = "NO GOAL";

                    AnimatorController.iskeeper0 = false;
                    AnimatorController.iskeeper1 = false;
                    AnimatorController.iskeeper2 = false;
                    AnimatorController.iskeeper3 = false;
                    AnimatorController.iskeeper4 = false;

                    AnimatorController._isBall0 = false;
                    AnimatorController._isBall1 = false;
                    AnimatorController._isBall2 = false;
                    AnimatorController._isBall3 = false;
                    AnimatorController._isBall4 = false;

                    Ball.SetActive(false);

                    yield return new WaitForSeconds(1.2f);

                    Ball.SetActive(true);
                    disable_Play.interactable = true;
                    Alert.text = "";
                    StartCoroutine(_cash());
                }
                else
                {
                    if (apiform.ball_number == 0)
                    {
                        AnimatorController.isBall0 = true;
                    }
                    else if (apiform.ball_number == 1)
                    {
                        AnimatorController.isBall1 = true;
                    }
                    else if (apiform.ball_number == 2)
                    {
                        AnimatorController.isBall2 = true;
                    }
                    else if (apiform.ball_number == 3)
                    {
                        AnimatorController.isBall3 = true;
                    }
                    else if (apiform.ball_number == 4)
                    {
                        AnimatorController.isBall4 = true;
                    }


                    if (apiform.keeper_number == 0)
                    {
                        AnimatorController.iskeeper0 = true;
                    }
                    else if (apiform.keeper_number == 1)
                    {
                        AnimatorController.iskeeper1 = true;
                    }
                    else if (apiform.keeper_number == 2)
                    {
                        AnimatorController.iskeeper2 = true;
                    }
                    else if (apiform.keeper_number == 3)
                    {
                        AnimatorController.iskeeper3 = true;
                    }
                    else if (apiform.keeper_number == 4)
                    {
                        AnimatorController.iskeeper4 = true;
                    }

                    yield return new WaitForSeconds(1.2f);
                    goal_number += 1;
                    Alert.text = "";
                    Alert.text = "GOAL : " + goal_number.ToString();

                    AnimatorController.iskeeper0 = false;
                    AnimatorController.iskeeper1 = false;
                    AnimatorController.iskeeper2 = false;
                    AnimatorController.iskeeper3 = false;
                    AnimatorController.iskeeper4 = false;

                    AnimatorController.isBall0 = false;
                    AnimatorController.isBall1 = false;
                    AnimatorController.isBall2 = false;
                    AnimatorController.isBall3 = false;
                    AnimatorController.isBall4 = false;

                    Ball.SetActive(false);

                    yield return new WaitForSeconds(1f);

                    Ball.SetActive(true);
                    disable_random.interactable = true;
                    ballbutton.SetActive(true);
                    if (goal_number == 5)
                    {
                        StartCoroutine(_cash());
                    }
                }
            }
            else
            {
                Alert.text = "";
                Alert.text = "DATA ERROR!";
                goal_number = 0;
                flag = 0;
                ballbutton.SetActive(false);
                disable_Play.interactable = true;
                disable_random.interactable = false;
            }
        }
    }
    IEnumerator _cash()
    {
        WWWForm form = new WWWForm();
        form.AddField("token", _player.token);
        form.AddField("betAmount", betAmount.ToString("F2"));
        form.AddField("goalNumber", goal_number.ToString());
        UnityWebRequest www = UnityWebRequest.Post(BaseUrl + "/api/_cash", form);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Alert.text = "";
            Alert.text = "CANNOT FIND SERVER!";
            goal_number = 0;
            flag = 0;
            ballbutton.SetActive(false);
            disable_Play.interactable = true;
            disable_random.interactable = false;
        }
        else
        {
            string strdata = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
            apiform = JsonUtility.FromJson<ReceiveJsonObject>(strdata);
            if (apiform.Message == "SUCCESS!")
            {
                if (apiform.earnAmount == 0)
                {
                    totalAmount -= betAmount;
                }
                else
                {
                    totalAmount += apiform.earnAmount - betAmount;
                }
                TotalAmount.text = totalAmount.ToString("F2");
                Alert.text = "";
                if (apiform.earnAmount > 0)
                {
                    Alert.text = "REWARD : " + apiform.earnAmount.ToString("F2");
                }
                else
                {
                    Alert.text = "REWARD : 0.00";
                }
                goal_number = 0;
                flag = 0;
                ballbutton.SetActive(false);
                disable_Play.interactable = true;
                disable_random.interactable = false;
            }
            else if (apiform.Message == "SERVER ERROR!")
            {
                Alert.text = "";
                Alert.text = "SERVER ERROR!";
                goal_number = 0;
                flag = 0;
                ballbutton.SetActive(false);
                disable_Play.interactable = true;
                disable_random.interactable = false;
            }
            else
            {
                Alert.text = "";
                Alert.text = "DATA ERROR!";
                goal_number = 0;
                flag = 0;
                ballbutton.SetActive(false);
                disable_Play.interactable = true;
                disable_random.interactable = false;
            }
        }
    }
    public void left_up()
    {
        disable_random.interactable = false;
        ballbutton.SetActive(false);
        StartCoroutine(Server(2));
    }
    public void left_down()
    {
        disable_random.interactable = false;
        ballbutton.SetActive(false);
        StartCoroutine(Server(4));
    }
    public void middle()
    {
        disable_random.interactable = false;
        ballbutton.SetActive(false);
        StartCoroutine(Server(3));
    }
    public void right_up()
    {
        disable_random.interactable = false;
        ballbutton.SetActive(false);
        StartCoroutine(Server(0));
    }
    public void right_down()
    {
        disable_random.interactable = false;
        ballbutton.SetActive(false);
        StartCoroutine(Server(1));
    }
    public void random()
    {
        ballbutton.SetActive(false);
        StartCoroutine(_random());
    }
    public void cash()
    {
        StartCoroutine(_cash());
    }
}
public class BetPlayer
{
    public string token;
}
