using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<PlayerMovement> Players { get { return players; } }
    public static GameManager Instance { get; private set; }
    public int IsTurn { get; private set; }

    [SerializeField] private List<Transform> spawnPoints = new();

    [SerializeField] private List<PlayerMovement> players = new();

    public void ChangeTurn()
    {
        if (players.Count < 2) { return; }

        players[IsTurn - 1].CurrentTurn = false;

        IsTurn++;
        ResetIsTurn();

        players[IsTurn - 1].CurrentTurn = true;
    }

    private void ResetIsTurn()
    {
        if (IsTurn > players.Count)
        {
            IsTurn = 1;
        }
    }

    private void Start()
    {
        IsTurn = 1;
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

    private void OnStart()
    {
        foreach (PlayerMovement player in players)
        {
            if (player == players[0])
            {
                player.CurrentTurn = true;
            }
            else
            {
                player.CurrentTurn &= false;
            }
        }
        players[IsTurn - 1].CurrentTurn = true;
    }

    private void OnDisable()
    {
        Instance = null;
    }

    public void PlayerJoined()
    {
        PlayerMovement[] _players = FindObjectsOfType<PlayerMovement>();
        foreach (PlayerMovement player in _players)
        {
            if (players.Contains(player))
            {
                if (player == players[IsTurn - 1])
                {
                    player.CurrentTurn = true;
                }
                else
                {
                    player.CurrentTurn = false;
                }
                return;
            }
            players.Add(player);

            if (player == players[IsTurn - 1])
            {
                player.CurrentTurn = true;
            }
            else
            {
                player.CurrentTurn = false;
            }

            player.transform.position = spawnPoints[players.IndexOf(player)].position;
        }
    }
}