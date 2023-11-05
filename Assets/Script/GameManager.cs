using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<PlayerMovement> Players { get { return players; } }
    public static GameManager Instance { get; private set; }
    public int IsTurn { get; private set; }

    [SerializeField] private List<Transform> spawnPoints = new();

    [SerializeField] private List<PlayerMovement> players = new();

    [Tooltip("Requires a rigidbody, I'd recommend adding some drag on the rigibody as well, otherwise it becomes very slippery.")]
    public List<GameObject> possibleObstacles = new();

    private float width, height;

    private void Start()
    {
        width = Screen.currentResolution.width;
        height = Screen.currentResolution.height;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
            Instance = this;
        }
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void OnDisable()
    {
        EventManager.ClearEvents(true);
        Instance = null;
    }

    public void PlayerJoined()
    {
        PlayerMovement[] _players = FindObjectsOfType<PlayerMovement>();
        foreach (PlayerMovement player in _players)
        {
            if (!players.Contains(player))
            {
                players.Add(player);
            }

            player.transform.position = spawnPoints[players.IndexOf(player)].position;
        }

        if (players.Count >= 2)
        {
            foreach (PlayerMovement player in players)
            {
                player.GameStarted = true;
            }
        }
    }
}