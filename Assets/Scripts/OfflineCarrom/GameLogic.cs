using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
    public int points = 0;
    public TextMeshPro pointsText;
    public Slider sliderP1;
    public Image sliderCoinP1;
    public int points2 = 0;
    public TextMeshPro pointsText2;
    public Slider sliderP2;
    public Image sliderCoinP2;
    public int count = 0;
    public GameObject p1;
    public GameObject p2;
    public bool changePlayer = true;
    public bool queenPocketed = false;
    public bool strikerPocketed = false;
    public Queue<GameObject> coinsQueue;
    public Stack<GameObject> coinsStackP1;
    public Stack<GameObject> coinsStackP2;
    public GameObject gameOverScreen;
    public TextMeshProUGUI winnerText;

    void Start()
    {
        gameOverScreen.SetActive(false);
        p1.SetActive(true);
        sliderP1.interactable = true;
        sliderCoinP1.color = new Color(1f, 1f, 1f, 1);
        p2.SetActive(false);
        sliderP2.interactable = false;
        sliderCoinP2.color = new Color(0.2f, 0.2f, 0.2f, 1);

        coinsQueue = new Queue<GameObject>();
        coinsStackP1 = new Stack<GameObject>();
        coinsStackP2 = new Stack<GameObject>();
    }

    public void changeTurn()
    {
        // If Queen went in put queen back if no cover
        if (queenPocketed)
        {
            GameObject destackedCoin;
            if (count % 2 == 0)
            {
                if (coinsStackP1.Count > 0)
                {
                    destackedCoin = coinsStackP1.Pop();
                    destackedCoin.transform.position = transform.position;
                }
            }
            else
            {
                if (coinsStackP2.Count > 0)
                {
                    destackedCoin = coinsStackP2.Pop();
                    destackedCoin.transform.position = transform.position;
                }
            }
            queenPocketed = false;
        }

        // If Striker went in put coins back on table
        if (strikerPocketed)
        {
            while (coinsQueue.Count > 0)
            {
                GameObject dequedCoin = coinsQueue.Dequeue();
                if (dequedCoin.CompareTag("black_coin"))
                {
                    addPoints(-10);

                }
                else if (dequedCoin.CompareTag("white_coin"))
                {
                    addPoints(-20);
                }
                dequedCoin.transform.position = transform.position;
            }

            GameObject destackedCoin;
            if (count % 2 == 0)
            {
                if (coinsStackP1.Count > 0)
                {
                    destackedCoin = coinsStackP1.Pop();
                    if (destackedCoin.CompareTag("black_coin"))
                    {
                        addPoints(-10);

                    }
                    else if (destackedCoin.CompareTag("white_coin"))
                    {
                        addPoints(-20);
                    }
                    destackedCoin.transform.position = transform.position;
                }
            }
            else
            {
                if (coinsStackP2.Count > 0)
                {
                    destackedCoin = coinsStackP2.Pop();
                    if (destackedCoin.CompareTag("black_coin"))
                    {
                        addPoints(-10);

                    }
                    else if (destackedCoin.CompareTag("white_coin"))
                    {
                        addPoints(-20);
                    }
                    destackedCoin.transform.position = transform.position;
                }
            }
        }

        // Change turn
        count++;
        if (count % 2 == 0)
        {
            p1.SetActive(true);
            sliderP1.interactable = true;
            sliderCoinP1.color = new Color(1f, 1f, 1f, 1);
            p2.SetActive(false);
            sliderP2.interactable = false;
            sliderCoinP2.color = new Color(0.2f, 0.2f, 0.2f, 1);
        }
        else
        {
            p2.SetActive(true);
            sliderP2.interactable = true;
            sliderCoinP2.color = new Color(1f, 1f, 1f, 1);
            p1.SetActive(false);
            sliderP1.interactable = false;
            sliderCoinP1.color = new Color(0.2f, 0.2f, 0.2f, 1);
        }
        strikerPocketed = false;
    }

    [ContextMenu("Add Points")]
    public void addPoints(int add)
    {
        if (count % 2 == 0)
        {
            points += add;
            pointsText.text = points.ToString();
        }
        else
        {
            points2 += add;
            pointsText2.text = points2.ToString();
        }

    }

    public void putCoinsIntoStack()
    {
        // Put coins in queue into appropriate stack
        while (coinsQueue.Count > 0)
        {
            GameObject dequedCoin = coinsQueue.Dequeue();
            if (count % 2 == 0)
            {
                coinsStackP1.Push(dequedCoin);
            }
            else
            {
                coinsStackP2.Push(dequedCoin);
            }
        }
    }

    public void gameOver()
    {
        winnerText.text = points > points2 ? "player 1 wins!" : "player 2 wins!";
        gameOverScreen.SetActive(true);
        p1.SetActive(false);
        sliderP1.interactable = false;
        sliderCoinP1.color = new Color(0.2f, 0.2f, 0.2f, 1);
        p2.SetActive(false);
        sliderP2.interactable = false;
        sliderCoinP2.color = new Color(0.2f, 0.2f, 0.2f, 1);
    }

    public void restartGame()
    {
        SceneManager.LoadScene("OfflineCarrom");
    }

    public void mainMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
