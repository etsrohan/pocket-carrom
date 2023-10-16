using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStriker : MonoBehaviour
{
    [SerializeField] private GameLogic logic;
    [SerializeField] private Slider slider;

    Rigidbody2D rb;
    CircleCollider2D circleCollider;
    bool isMoving;
    Vector2 startPos;


    private void Start()
    {
        isMoving = false;
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        startPos = transform.position;
    }

    private void Update()
    {
        // Check if the enemy striker has come to a near stop and is not moving
        if (rb.velocity.magnitude < 0.1f && !isMoving && logic.count % 2 == 1)
        {
            isMoving = true;
            logic.changePlayer = true;
            logic.putCoinsIntoStack();
            StartCoroutine(EnemyTurn());
        }
    }

    private void OnEnable()
    {
        // Reset the initial state of the enemy striker
        // CollisionSoundManager.shouldBeStatic = true;
        GetComponent<SpriteRenderer>().enabled = false;
        // transform.position = new Vector3(0, 3.45f, 0f);
    }

    IEnumerator EnemyTurn()
    {
        // Determine the target coin based on game logic
        // Find the closest coin to a pocket
        const int maxAttempts = 10;
        int attempts = 0;
        bool isObstructed;

        do
        {
            isObstructed = false;

            // Generate a random position within the board bounds
            float x = Random.Range(-1.55f, 1.55f);
            slider.value = x / 1.55f;
            transform.position = new Vector3(x, startPos.y, 0f);

            // Check if the generated position is obstructed by other coins or the striker
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f);

            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("black_coin") || collider.CompareTag("white_coin") || collider.CompareTag("red_coin"))
                {
                    isObstructed = true;
                    break;
                }
            }

            attempts++;
        }
        while (isObstructed && attempts < maxAttempts);

        if (isObstructed)
        {
            Debug.Log("Failed to find a valid position for the enemy striker.");
            transform.position = new Vector3(0f, startPos.y, 0f);
            isObstructed = false;
        }

        yield return new WaitWhile(() => isObstructed);
        GetComponent<SpriteRenderer>().enabled = true;

        yield return new WaitForSeconds(2f);
        // CollisionSoundManager.shouldBeStatic = false;
        yield return new WaitForSeconds(0.1f);

        // Find all the available black coins
        GameObject[] coins = GameObject.FindGameObjectsWithTag("black_coin");
        GameObject[] coins_white = GameObject.FindGameObjectsWithTag("white_coin");
        GameObject[] coins_red = GameObject.FindGameObjectsWithTag("red_coin");
        GameObject closestCoin = null;
        float closestDistance = Mathf.Infinity;

        if (coins.Length == 0)
        {
            Debug.Log("No coins left");
            yield break;
        }

        // Find the closest coin to a pocket
        foreach (GameObject coin in coins)
        {
            float distance = Vector3.Distance(coin.transform.position, GetClosestPocket(coin.transform.position));
            if (distance < closestDistance)
            {
                closestCoin = coin;
                closestDistance = distance;
            }
        }
        foreach (GameObject coin in coins_white)
        {
            float distance = Vector3.Distance(coin.transform.position, GetClosestPocket(coin.transform.position));
            if (distance < closestDistance)
            {
                closestCoin = coin;
                closestDistance = distance;
            }
        }
        foreach (GameObject coin in coins_red)
        {
            float distance = Vector3.Distance(coin.transform.position, GetClosestPocket(coin.transform.position));
            if (distance < closestDistance)
            {
                closestCoin = coin;
                closestDistance = distance;
            }
        }
        // Find solution to this later


        // Calculate the direction and speed of the striker based on the position of the target coin and the enemy's striker.
        Vector3 targetDirection = closestCoin.transform.position - transform.position;
        targetDirection.z = 0f;
        float targetSpeed = CalculateStrikerSpeed(targetDirection.magnitude);

        // Remove is trigger from circle collider
        circleCollider.isTrigger = false;
        // Apply the calculated force to the striker and end the enemy's turn.
        rb.AddForce(targetDirection.normalized * targetSpeed, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => rb.velocity.magnitude < 0.1f);
        yield return new WaitForSeconds(1.0f);
        isMoving = false;
        // StrikerController.playerTurn = true;
        // logic.changePlayer = true;
        circleCollider.isTrigger = true;
        if (logic.changePlayer)
        {
            logic.changeTurn();
            gameObject.SetActive(false);
        }
    }

    Vector3 GetClosestPocket(Vector3 position)
    {
        // Find the closest pocket to a given position
        Vector3 closestPocket = Vector3.zero;
        float closestDistance = Mathf.Infinity;
        GameObject[] pockets = GameObject.FindGameObjectsWithTag("hole");

        foreach (GameObject pocket in pockets)
        {
            float distance = Vector3.Distance(position, pocket.transform.position);
            if (distance < closestDistance)
            {
                closestPocket = pocket.transform.position;
                closestDistance = distance;
            }
        }

        return closestPocket;
    }

    float CalculateStrikerSpeed(float distance)
    {
        // Calculate the speed of the striker based on the distance to the target coin
        float maxDistance = 4.0f; // Maximum distance the striker can travel
        float minSpeed = 10f; // Minimum striker speed
        float maxSpeed = 30f; // Maximum striker speed

        float speed = Mathf.Lerp(minSpeed, maxSpeed, distance / maxDistance);
        return speed * 0.7f;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // Play the collision sound if the enemy striker collides with the board
        if (other.gameObject.CompareTag("border"))
        {
            // GetComponent<AudioSource>().Play();
        }
    }
}