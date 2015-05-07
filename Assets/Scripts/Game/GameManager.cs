using UnityEngine;
using System.Linq;
using System.Collections;
using Game;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject Hellephant;
    public GameObject ZomBunny;
    public GameObject ZomBear;
    public float spawnDelay = .5f;
    public float staminaDropRate = 2f;
    public float staminaRegenRate = 1f; 

    public Animator canvasAnimator;
    public Slider playerHealthSlider;
    public Slider fountainHealthSlider;
    public Slider staminaSlider;
    public Text waveText;
    public Text waveOutOfText;
    public Text gameOverText;

    Wave[] waves;
    Transform[] spawnPoints;
    int enemiesOnCamp = 0;
    bool isSpawning;
    bool isGameOver;
    bool fountainDead;
    bool wasLastWave;

    public bool IsConsumingStamina { get; set; }

    public CompleteProject.PlayerHealth ph;
    // persistent data
    
	int currentWave;

	int fountainHealth;
    private int _playerHealth;

    public int playerHealth
    {
        get { return _playerHealth; }
        set {
            if (_playerHealth == value)
            {
                return;
            }

            _playerHealth = value;
            SyncPlayerHealthSlider();
        }
    }

    private float _stamina;

    public float stamina
    {
        get { return _stamina; }
        set
        {
            if (_stamina == value)
            {
                return;
            }

            _stamina = value;
            SyncStaminaSlider();
        }
    }



    public static GameManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
		playerHealth = 100;
		fountainHealth = 10;
        stamina = 100;
		currentWave = 0;
		
        SyncFountainHealthSlider();
        SyncPlayerHealthSlider();
        SyncStaminaSlider();

        fountainDead = false;

        spawnPoints = transform.Cast<Transform>().ToArray();

        waves = new[] {
            // #1
            new Wave {
                Groups = new [] {
                    new WaveGroup {
                        Delay = 0,
                        SpawnIndices = new [] { 0, 1 },
                        Size = 4,
                        EnemyPrefab = ZomBear,
                    },
                    new WaveGroup {
                        Delay = 5,
                        SpawnIndices = new [] { 1 },
                        Size = 4,
                        EnemyPrefab = ZomBear,
                    },
                    new WaveGroup {
                        Delay = 5,
                        SpawnIndices = new [] { 2 },
                        Size = 4,
                        EnemyPrefab = ZomBear,
                    },
                    new WaveGroup {
                        Delay = 5,
                        SpawnIndices = new [] { 3 },
                        Size = 4,
                        EnemyPrefab = ZomBear,
                    },
                }
            },
            // #2
            new Wave {
                Groups = new [] {
                    new WaveGroup {
                        Delay = 0,
                        SpawnIndices = new [] { 0 },
                        Size = 1,
                        EnemyPrefab = ZomBunny,
                    },
					new WaveGroup {
						Delay = 0,
						SpawnIndices = new [] { 1 },
						Size = 1,
						EnemyPrefab = ZomBunny,
					},
					new WaveGroup {
						Delay = 0,
						SpawnIndices = new [] { 2 },
						Size = 1,
						EnemyPrefab = ZomBunny,
					},
					new WaveGroup {
						Delay = 0,
						SpawnIndices = new [] { 3 },
						Size = 1,
						EnemyPrefab = ZomBunny,
					},
                 }
            },
            // #3
			new Wave {
				Groups = new [] {
					new WaveGroup {
						Delay = 0,
						SpawnIndices = new [] { 0, 2 },
						Size = 4,
						EnemyPrefab = ZomBear,
					},
					new WaveGroup {
						Delay = 3,
						SpawnIndices = new [] { 1, 3 },
						Size = 2,
						EnemyPrefab = ZomBunny,
					},
					
				}
			},
            // #4
			new Wave {
				Groups = new [] {
					new WaveGroup {
						Delay = 0,
						SpawnIndices = new [] { 0 },
						Size = 2,
						EnemyPrefab = ZomBear,
					},
					new WaveGroup {
						Delay = 0,
						SpawnIndices = new [] { 0 },
						Size = 1,
						EnemyPrefab = ZomBunny,
					},
					new WaveGroup {
						Delay = 3,
						SpawnIndices = new [] { 2 },
						Size = 2,
						EnemyPrefab = ZomBear,
					},
					new WaveGroup {
						Delay = 3,
						SpawnIndices = new [] { 2 },
						Size = 1,
						EnemyPrefab = ZomBunny,
					},
					new WaveGroup {
						Delay = 3,
						SpawnIndices = new [] { 1 },
						Size = 2,
						EnemyPrefab = ZomBear,
					},
					new WaveGroup {
						Delay = 3,
						SpawnIndices = new [] { 1 },
						Size = 1,
						EnemyPrefab = ZomBunny,
					},
					new WaveGroup {
						Delay = 3,
						SpawnIndices = new [] { 3 },
						Size = 2,
						EnemyPrefab = ZomBear,
					},
					new WaveGroup {
						Delay = 3,
						SpawnIndices = new [] { 3 },
						Size = 1,
						EnemyPrefab = ZomBunny,
					},
					
				}
			},
            // #5
             new Wave {
                Groups = new [] {
                    new WaveGroup {
                        Delay = 0,
                        SpawnIndices = new [] { 1 },
                        Size = 2,
                        EnemyPrefab = Hellephant,
                    },
					new WaveGroup {
						Delay = 4,
						SpawnIndices = new [] { 2 },
						Size = 2,
						EnemyPrefab = Hellephant,
					},
					new WaveGroup {
						Delay = 4,
						SpawnIndices = new [] { 0 },
						Size = 2,
						EnemyPrefab = Hellephant,
					},
					new WaveGroup {
						Delay = 4,
						SpawnIndices = new [] { 3 },
						Size = 2,
						EnemyPrefab = Hellephant,
					},
                }
            },
            // #6
             new Wave {
                Groups = new [] {
					new WaveGroup {
						Delay = 0,
						SpawnIndices = new [] { 0 },
						Size = 1,
						EnemyPrefab = Hellephant,
					},
					new WaveGroup {
						Delay = 0,
						SpawnIndices = new [] { 0 },
						Size = 2,
						EnemyPrefab = ZomBear,
					},
					new WaveGroup {
						Delay = 0,
						SpawnIndices = new [] { 0 },
						Size = 1,
						EnemyPrefab = ZomBunny,
					},
					new WaveGroup {
						Delay = 2,
						SpawnIndices = new [] { 2 },
						Size = 1,
						EnemyPrefab = Hellephant,
					},
					new WaveGroup {
						Delay = 0,
						SpawnIndices = new [] { 2 },
						Size = 2,
						EnemyPrefab = ZomBear,
					},
					new WaveGroup {
						Delay = 0,
						SpawnIndices = new [] { 2 },
						Size = 1,
						EnemyPrefab = ZomBunny,
					},
					
                }
            },
            // #7
             new Wave {
                Groups = new [] {
					new WaveGroup {
						Delay = 0,
						SpawnIndices = new [] { 1 },
						Size = 3,
						EnemyPrefab = Hellephant,
					},
					new WaveGroup {
						Delay = 2,
						SpawnIndices = new [] { 3 },
						Size = 6,
						EnemyPrefab = ZomBear,
					},
					new WaveGroup {
						Delay = 2,
						SpawnIndices = new [] { 0, 2 },
						Size = 2,
						EnemyPrefab = ZomBunny,
					},
                }
            },
            // #8
             new Wave {
                Groups = new [] {
					new WaveGroup {
						Delay = 0,
						SpawnIndices = new [] { 1, 3 },
						Size = 8,
						EnemyPrefab = Hellephant,
					},
					new WaveGroup {
						Delay = 4,
						SpawnIndices = new [] { 0, 2 },
						Size = 4,
						EnemyPrefab = ZomBunny,
					},
                }
            },
            // #9
             new Wave {
				Groups = new [] {
					new WaveGroup {
						Delay = 0,
						SpawnIndices = new [] { 0, 1 },
						Size = 8,
						EnemyPrefab = ZomBear,
					},
					new WaveGroup {
						Delay = 0,
						SpawnIndices = new [] { 0, 1 },
						Size = 4,
						EnemyPrefab = Hellephant,
					},
					new WaveGroup {
						Delay = 6,
						SpawnIndices = new [] { 2, 3 },
						Size = 8,
						EnemyPrefab = ZomBear,
					},
					new WaveGroup {
						Delay = 0,
						SpawnIndices = new [] { 2, 3 },
						Size = 4,
						EnemyPrefab = Hellephant,
					},
				}
            },
            // #10
             new Wave {
                Groups = new [] {
					new WaveGroup {
						Delay = 0,
						SpawnIndices = new [] { 0, 1, 2, 3 },
						Size = 8,
						EnemyPrefab = Hellephant,
					},
					new WaveGroup {
						Delay = 4,
						SpawnIndices = new [] { 0, 1, 2, 3 },
						Size = 12,
						EnemyPrefab = ZomBear,
					},
					new WaveGroup {
						Delay = 4,
						SpawnIndices = new [] { 0, 1, 2, 3 },
						Size = 4,
						EnemyPrefab = ZomBunny,
					},
                }
            },
        };

        
    }

    void Start()
    {
        SpawnWave(.5f);
    }


    void Update()
    {
        stamina = Mathf.Clamp(stamina + (Time.deltaTime * (IsConsumingStamina ? -staminaDropRate : staminaRegenRate)), 0, 100);

        if (!isGameOver && !isSpawning && enemiesOnCamp == 0)
        {
            if (currentWave == 9)
            {
                // win state
                gameOverText.text = "Congratulations!";
                isGameOver = true;
            }
            else
            {
                currentWave += 1;
                SpawnWave(1.5f);
            }
        }


    }


    void SpawnWave(float delay = 0)
    {
        isSpawning = true;
        StartCoroutine(SpawnWaveCoroutine(delay));
    }

    IEnumerator SpawnWaveCoroutine(float startDelay)
    {
        yield return new WaitForSeconds(startDelay);
        waveText.text = "Wave #" + (currentWave + 1);
        waveOutOfText.text = "Wave " + (currentWave + 1) + "/10";
        canvasAnimator.SetTrigger("Wave");

        foreach (var waveGroup in waves[currentWave].Groups)
        {
            // wait for wavegroup delay
            yield return new WaitForSeconds(waveGroup.Delay);

            var numSpawnIndices = waveGroup.SpawnIndices.Length;
            var spawnIndex = 0;
            var spawnSide = 1;
            bool[] played = new bool[numSpawnIndices];
            
            // spawn group
            for (int i = 0; i < waveGroup.Size; i++)
            {
                //spawn enemy
                var spawn = spawnPoints[waveGroup.SpawnIndices[spawnIndex]];
                var position = spawn.position + (spawn.right * spawnSide * 3);

                if (!played[spawnIndex])
                {
                    var ps = spawn.GetChild(0).GetComponent<ParticleSystem>();
                    ps.Play();
                    played[spawnIndex] = true;
                }

                var enemy = (GameObject)Instantiate(waveGroup.EnemyPrefab, position, spawn.rotation);

                enemiesOnCamp += 1;
                var enemyHealth = enemy.GetComponent<CompleteProject.EnemyHealth>();
                enemyHealth.EnemyDied += (go) => enemiesOnCamp -= 1;

                spawnIndex = (spawnIndex + 1) % numSpawnIndices;
                if (spawnIndex == 0)
                {
                    spawnSide *= -1;
                }

                yield return new WaitForSeconds(spawnDelay/numSpawnIndices);
            }

        }

        isSpawning = false;
    
    }

    public void DamageFountain()
    {
        if (fountainHealth < 1)
        {
            fountainHealth = 0;
            fountainDead = true;
            return;
        }

        fountainHealth -= 1;
        SyncFountainHealthSlider();
    }


    private void SyncPlayerHealthSlider()
    {
        SyncSlider(playerHealthSlider, playerHealth);
    }


    private void SyncStaminaSlider()
    {
        SyncSlider(staminaSlider, (int)stamina);
    }


    private void SyncFountainHealthSlider()
    {
        SyncSlider(fountainHealthSlider, fountainHealth);
    }

    private void SyncSlider(Slider slider, int value)
    {
        slider.value = value;
    }

    public bool CheckGameOver()
    {
        if (isGameOver)
        {
            return true;
        }

        if (ph.isDead || fountainDead)
        {
            isGameOver = true;
            SyncFountainHealthSlider();
            SyncPlayerHealthSlider();   
            return true;
        }

        return false;
    }
}
