using UnityEngine;


public class CoinCollector : MonoBehaviour
{
    public GameLogic logic;
    private int pointsThreshold = 160;
    // Start is called before the first frame update
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<GameLogic>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("black_coin"))
        {
            collision.gameObject.transform.position = new Vector3(-1000, 0, 0);
            if (logic.queenPocketed)
            {
                logic.addPoints(50);
                logic.queenPocketed = false;
            }
            logic.addPoints(10);

            if (!logic.strikerPocketed) logic.changePlayer = false;

        }
        else if (collision.gameObject.CompareTag("white_coin"))
        {
            collision.gameObject.transform.position = new Vector3(-1000, 0, 0);
            if (logic.queenPocketed)
            {
                logic.addPoints(50);
                logic.queenPocketed = false;
            }
            logic.addPoints(20);

            if (!logic.strikerPocketed) logic.changePlayer = false;

        }
        else if (collision.gameObject.CompareTag("red_coin"))
        {
            collision.gameObject.transform.position = new Vector3(-1000, 0, 0);
            logic.queenPocketed = true;
            if (!logic.strikerPocketed) logic.changePlayer = false;
        }
        else if (collision.gameObject.CompareTag("striker_coin"))
        {
            if (logic.count % 2 == 0)
            {
                logic.p1.gameObject.transform.position = new Vector3(-1000, 0, 0);
            }
            else
            {
                logic.p2.gameObject.transform.position = new Vector3(-1000, 0, 0);
            }
            logic.strikerPocketed = true;
            logic.changePlayer = true;
        }

        if (!collision.gameObject.CompareTag("striker_coin"))
        {
            logic.coinsQueue.Enqueue(collision.gameObject);
        }

        if (logic.points >= pointsThreshold || logic.points2 >= pointsThreshold)
        {
            // Game over screen
            logic.gameOver();
        }
    }
}
