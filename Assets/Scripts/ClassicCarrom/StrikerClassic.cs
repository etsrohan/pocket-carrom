using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class StrikerClassic : NetworkBehaviour
{
    public NetworkVariable<bool> isActive = new NetworkVariable<bool>(false);
    private NetworkVariable<float> hostSliderValue = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<float> clientSliderValue = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    Rigidbody2D rigidBody;
    CircleCollider2D circleCollider;
    Transform selftrans;
    Vector2 startpos;
    private Slider strikerSlider;
    private Slider opponentSlider;
    Vector2 direction;
    Vector3 touchPos;
    Vector3 touchPos2;
    SpriteRenderer strikerImage;
    private GameObject board;
    public LineRenderer line;
    bool hasStriked = false;
    bool positionSet = false;
    bool strikeAble = true;
    readonly float maxDistance = 1f;
    readonly float minDistance = 0.2f;
    readonly int maxForce = 1000;
    readonly int minForce = 100;
    public GameLogicClassic logic;
    private float setTime;
    private GameObject CameraObj;
    Touch touch;
    // Start is called before the first frame update
    void Start()
    {

        string posTag = IsHost ? "p1_start" : "p2_start";
        transform.position = GameObject.FindGameObjectWithTag(posTag).transform.position;

        CameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        if (!IsHost) CameraObj.transform.rotation = Quaternion.Euler(0f, 0f, 180f);

        board = GameObject.FindGameObjectWithTag("board");
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<GameLogicClassic>();
        strikerSlider = GameObject.FindGameObjectWithTag("player_slider").GetComponent<Slider>();
        opponentSlider = GameObject.FindGameObjectWithTag("opponent_slider").GetComponent<Slider>();
        line = GameObject.FindGameObjectWithTag("line_renderer").GetComponent<LineRenderer>();

        rigidBody = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        strikerImage = GetComponent<SpriteRenderer>();
        selftrans = transform;
        startpos = transform.position;
        circleCollider.isTrigger = true;

        strikerSlider.interactable = true;
        opponentSlider.interactable = false;
    }

    public override void OnNetworkSpawn()
    {
        hostSliderValue.OnValueChanged += (float previousValue, float newValue) =>
        {
            if (!IsHost)
            {
                opponentSlider.handleRect.position = new Vector2(newValue, opponentSlider.handleRect.position.y);
            }
        };
        clientSliderValue.OnValueChanged += (float previousValue, float newValue) =>
        {
            if (IsHost)
            {
                opponentSlider.handleRect.position = new Vector2(newValue, opponentSlider.handleRect.position.y);
            }
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        if (IsHost)
        {
            if (logic.count % 2 == 0)
            {
                gameObject.SetActive(true);

            }
        }

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
            if (IsHost)
            {
                hostSliderValue.Value = strikerSlider.handleRect.position.x;
            }
            else
            {
                clientSliderValue.Value = strikerSlider.handleRect.position.x;
            }
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
                selftrans.position.x + direction.x * (magnitude - 0.2f),
                selftrans.position.y + direction.y * (magnitude - 0.2f),
                selftrans.position.z
            ));
            line.SetPosition(1, new Vector3(
                selftrans.position.x + (direction.x * (1.8f + magnitude)),
                selftrans.position.y + (direction.y * (1.8f + magnitude)),
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
            if (logic.changePlayer)
            {
                logic.changeTurn();
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
        logic.sliderP1.value = 0;
        logic.changePlayer = true;
        logic.sliderP2.value = 0;
        logic.putCoinsIntoStack();
    }
}
