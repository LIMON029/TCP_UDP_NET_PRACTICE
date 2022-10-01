using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // For singleton
    public static GameManager instance;
    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();
    public static Dictionary<int, ItemSpawner> itemSpawners = new Dictionary<int, ItemSpawner>();
    public static Dictionary<int, ProjectileManager> projectiles = new Dictionary<int, ProjectileManager>();

    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;
    public GameObject itemSpawnerPrefab;
    public GameObject projectilePrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance is alreay exist, destoying object.");
            Destroy(this);
        }
    }

    public void SpawnPlayer(int _id, string _username, Vector3 _position, Quaternion _quaternion)
    {
        GameObject _player;
        if(_id == Client.instance.myId)
        {
            _player = Instantiate(localPlayerPrefab, _position, _quaternion);
        }
        else
        {
            _player = Instantiate(playerPrefab, _position, _quaternion);
        }

        _player.GetComponent<PlayerManager>().Initialize(_id, _username);
        players.Add(_id, _player.GetComponent<PlayerManager>());
    }

    public void CreateItemSpawner(int _spawnerId, Vector3 _position, bool _hasItem)
    {
        GameObject _spawner = Instantiate(itemSpawnerPrefab, _position, itemSpawnerPrefab.transform.rotation);
        _spawner.GetComponent<ItemSpawner>().Initialize(_spawnerId, _hasItem);
        itemSpawners.Add(_spawnerId, _spawner.GetComponent<ItemSpawner>());
    }

    public void SpawnProjectile(int _id, Vector3 _position)
    {
        GameObject _projectile = Instantiate(projectilePrefab, _position, Quaternion.identity);
        _projectile.GetComponent<ProjectileManager>().Initialize(_id);
        projectiles.Add(_id, _projectile.GetComponent<ProjectileManager>());
    }
}
