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

    public Animator canvasAnimator;
    public Slider playerHealthSlider;
    public Slider fountainHealthSlider;
    public Text waveText;
    public Text waveOutOfText;
    public Text waveOutOfTextShadow;
    public Text gameOverText;

    Wave[] waves;
    Transform[] spawnPoints;
    int enemiesOnCamp = 0;
    bool isSpawning;
    bool isGameOver;
    bool fountainDead;
    bool wasLastWave;

    public CompleteProject.PlayerHealth ph;
    // persistent data
    
    int currentWave
    {
        get
        {
            return 9 - rylai.SecondDigit;
        }
        set
        {
            rylai.SecondDigit = 9 - value;
        }
    }

    int fountainHealth
    {
        get
        {
            return rylai.FourthDigit + 1;
        }
        set
        {
            rylai.FourthDigit = value - 1;
        }
    }

    //int _playerHealth;
    //public int playerHealth {
    //    get { 
    //        return _playerHealth; 
    //    }
    //    set { 
    //        if (_playerHealth == value) {
    //            return;
    //        }
    //        _playerHealth = value;
    //        SyncPlayerHealthSlider();
    //    }
    //}

    public int playerHealth
    {
        get
        {
            var fd = rylai.FirstDigit * 20;
            var td = 10 - rylai.ThirdDigit;

            return td + (fd > 80 ? fd - 90 : fd);
        }
        set
        {
            if (playerHealth == value)
            {
                return;
            }

            var v99 = value - 1;
            var uni = 9 - (v99 - (v99 / 10) * 10);
            
            var v90 = value - (10 - uni);
            var dec = ((v90 / 10) % 2 == 1 ? v90 + 90 : v90) / 20;

            rylai.FirstDigit = dec;
            rylai.ThirdDigit = uni;

            SyncPlayerHealthSlider();
        }
    }


    public static GameManager Instance { get; private set; }
    CycleManager rylai;

    void Awake()
    {
        Instance = this;
        rylai = CycleManager.Instance;
		playerHealth = 100;
		fountainHealth = 10;
		currentWave = 0;
		
        SyncFountainHealthSlider();
        SyncPlayerHealthSlider();

        fountainDead = false;

        spawnPoints = transform.Cast<Transform>().ToArray();

        waves = new[] {
            // #1
            new Wave {
                Groups = new [] {
                    new WaveGroup {
                        Delay = 0,
                        SpawnIndices = new [] { 0 },
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
        waveOutOfTextShadow.text = "Wave " + (currentWave + 1) + "/10";
        canvasAnimator.SetTrigger("Wave");

        foreach (var waveGroup in waves[currentWave].Groups)
        {
            // wait for wavegroup delay
            yield return new WaitForSeconds(waveGroup.Delay);

            var numSpawnIndices = waveGroup.SpawnIndices.Length;
            var spawnIndex = 0;
            var spawnSide = 1;
            // spawn group
            for (int i = 0; i < waveGroup.Size; i++)
            {
                //spawn enemy
                var spawn = spawnPoints[waveGroup.SpawnIndices[spawnIndex]];
                var position = spawn.position + (spawn.right * spawnSide * 3);

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
        if (fountainHealth == 1)
        {
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
            return true;
        }

        return false;
    }
}
