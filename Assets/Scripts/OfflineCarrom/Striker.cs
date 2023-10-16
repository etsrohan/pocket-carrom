using UnityEngine;
using UnityEngine.UI;

public class Striker : MonoBehaviour
{
    Rigidbody2D rigidBody;
    CircleCollider2D circleCollider;
    Transform selftrans;
    Vector2 startpos;
    public Slider strikerSlider;
    Vector2 direction;
    Vector3 touchPos;
    Vector3 touchPos2;
    SpriteRenderer strikerImage;
    public LineRenderer line;
    bool hasStriked = false;
    bool positionSet = false;
    bool strikeAble = true;
    readonly float maxDistance = 1f;
    readonly float minDistance = 0.2f;
    readonly int maxForce = 1000;
    readonly int minForce = 100;
    public GameObject board;
    float setTime;
    Touch touch;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        strikerImage = GetComponent<SpriteRenderer>();
        selftrans = transform;
        startpos = transform.position;
        circleCollider.isTrigger = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Get touch input
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
        }
        // Set touch position 2 to opposite of striket position
        touchPos = Camera.main.ScreenToWorldPoint(touch.position);
        touchPos2 = new Vector3(2 * transform.position.x - touchPos.x, 2 * transform.position.y - touchPos.y, touchPos.z);
        // Clamp max touch position 2 to max distance
        if (Vector2.Distance(transform.position, touchPos) > maxDistance)
        {
            direction = (Vector2)(touchPos2 - transform.position);
            direction.Normalize();
            direction *= maxDistance;
            direction += (Vector2)transform.position;
            touchPos2.x = direction.x;
            touchPos2.y = direction.y;
        }

        // Set slider position = striker position when position not set
        if (!hasStriked && !positionSet)
        {
            selftrans.position = new Vector2(strikerSlider.handleRect.position.x, startpos.y);
        }

        // Shoot striker
        if (Input.touchCount == 0 && rigidBody.velocity.magnitude == 0 && positionSet && strikeAble && Vector2.Distance(transform.position, touchPos) > minDistance)
        {
            shootStriker();
        }

        // Get touch input for striker touched and set position
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touch.position), Vector2.zero);
        if (hit.collider != null)
        {
            if (Input.touchCount > 0 && hit.collider.gameObject == gameObject && strikeAble)
            {
                if (!positionSet)
                {
                    positionSet = true;
                }
            }
        }

        // Reset position set if not touch input
        if (Input.touchCount == 0) positionSet = false;

        // Add line when ready to strike else disable line
        if (positionSet && rigidBody.velocity.magnitude == 0 && Vector2.Distance(transform.position, touchPos) > minDistance)
        {
            line.enabled = true;
            direction = (Vector2)(touchPos2 - transform.position);
            direction.Normalize();
            float magnitude = Vector2.Distance(selftrans.position, touchPos2);
            line.SetPosition(0, new Vector3(
                selftrans.position.x + direction.x * (0.2f),
                selftrans.position.y + direction.y * (0.2f),
                selftrans.position.z
            ));
            line.SetPosition(1, new Vector3(
                selftrans.position.x + (direction.x * (0.5f + magnitude * 3)),
                selftrans.position.y + (direction.y * (0.5f + magnitude * 3)),
                selftrans.position.z
            ));
        }
        else
        {
            line.enabled = false;
        }

        // Change turn after 3.5 seconds
        if (Time.time > (setTime + 3.5) && hasStriked)
        {
            if (board.GetComponent<GameLogic>().changePlayer)
            {
                board.GetComponent<GameLogic>().changeTurn();
            }
            strikerReset();
        }

        // Enable striker when not overlapping with another coin
        int layerMask = LayerMask.GetMask("Coins");
        if (circleCollider.IsTouchingLayers(layerMask) && circleCollider.isTrigger)
        {
            strikeAble = false;
            strikerImage.color = new Color(0.8f, 0.8f, 0.8f, 1);
        }
        else
        {
            strikeAble = true;
            strikerImage.color = new Color(1f, 1f, 1f, 1);
        }
    }

    public void shootStriker()
    {
        circleCollider.isTrigger = false;
        float x = 0;
        if (positionSet && rigidBody.velocity.magnitude == 0)
        {
            x = Vector2.Distance(transform.position, touchPos2);
        }
        direction = (Vector2)(touchPos2 - transform.position);
        direction.Normalize();
        float slope = (maxForce - minForce) / (maxDistance - minDistance);
        float force = slope * (x - minDistance) + minForce;
        rigidBody.AddForce(direction * force);
        hasStriked = true;
        positionSet = false;
        setTime = Time.time;
    }

    public void strikerReset()
    {
        circleCollider.isTrigger = true;
        rigidBody.velocity = Vector2.zero;
        hasStriked = false;
        board.GetComponent<GameLogic>().changePlayer = true;
        board.GetComponent<GameLogic>().sliderP1.value = 0;
        board.GetComponent<GameLogic>().sliderP2.value = 0;
        board.GetComponent<GameLogic>().putCoinsIntoStack();
    }
}
