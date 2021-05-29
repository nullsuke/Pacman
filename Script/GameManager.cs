using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Pacman pacmanPrefab = default;
    [SerializeField] private Maze[] mazePrefabs = default;
    [SerializeField] private Dot dotPrefab = default;
    [SerializeField] private PowerCokie powerCokiePrefab = default;
    [SerializeField] private Akabei akabeiPrefab = default;
    [SerializeField] private Pinky pinkyPrefab = default;
    [SerializeField] private Aosuke aosukePrefab = default;
    [SerializeField] private Guzuta guzutaPrefab = default;
    [SerializeField] private Scorer scorer = default;
    [SerializeField] private Text scoreText = default;
    //Readyの表示。
    [SerializeField] private Text readyText = default;
    //ゲームオーバーの表示。
    [SerializeField] private GameObject overPanel = default;
    [SerializeField] private GameObject allClearPanel = default;
    [SerializeField] private GameObject livesPanel = default;
    [SerializeField] private float scareSpan;
    [SerializeField] private float eatSpan;
    [SerializeField] private float eatenSpan;
    //"Ready"を表示する期間。
    [SerializeField] private float readySpan;
    [SerializeField] private float nextSpan;
    private List<AGhost> ghosts;
    private Pacman pacman;
    private Maze maze;
    private Akabei akabei;
    private Pinky pinky;
    private Aosuke aosuke;
    private Guzuta guzuta;
    private FruitsCreator fruitsCreator;
    private Fruits fruits;
    private SoundManager soundManager;
    private bool isScare;
    private float scareEndTime;
    private int mazeIndex;
    private int score;
    private int lives;
    //一度の恐慌状態で食べたゴーストの数。
    private int eatenGhost;
    //食べたエサの数。
    private int eatenDot;

    public void Continue()
    {
        Destroy(maze.gameObject);
        overPanel.SetActive(false);
        Start();
    }

    public void BackToTitle()
    {
        SceneManager.LoadScene("Title");
    }

    private void Start()
    {
        lives = 2;
        score = 0;

        maze = Instantiate(mazePrefabs[mazeIndex]);
        
        StartCoroutine(CoroutineStart(SetGame));
    }

    private void FixedUpdate()
    {
        if (isScare && scareEndTime < Time.fixedTime )
        {
            isScare = false;
            eatenGhost = 0;
            ghosts.ForEach(g => g.Calm());

            //BGMを再生。
            soundManager.StopBGM();
            soundManager.PlayBGM("GhostNormal", 0.6f);
        }
    }

    //Readyを一定時間表示した後、パックマン、ゴーストをActiveにする。
    private IEnumerator CoroutineStart(Action callback)
    {
        readyText.gameObject.SetActive(true);
        callback();

        yield return new WaitForSeconds(readySpan);

        readyText.gameObject.SetActive(false);

        pacman.Run();
        ghosts.ForEach(g => g.Run());

        //BGMを再生。
        soundManager.PlayBGM("GhostNormal", 0.6f);
    }
    
    private IEnumerator CoroutineNext()
    {
        yield return new WaitForSeconds(nextSpan);

        NextMaze();
    }

    private void Awake()
    {
        SetAudio();
    }

    //ステージ開始時の初期化。
    private void SetGame()
    {
        //パックマンの作成。
        pacman = Instantiate(pacmanPrefab, maze.transform);
        //パックマンの初期位置は必ず整数でないと移動中に引っかかる。
        pacman.Initialize(maze.PacmanStartPosition);
        //パックマン死亡時のイベントを登録。
        pacman.OnDead += (s, e) => CheckLives();

        //ゴーストの作成。
        PlaceGhost();
        //エサの作成。
        PlaceDot();
        //パワーエサの作成。
        PlacePowerCokie();
        //残機の更新。
        UpdateLives();

        fruitsCreator = GetComponent<FruitsCreator>();
        fruitsCreator.Initialize();

        scareEndTime = 0;
        eatenGhost = 0;
        eatenDot = 0;
    }

    //パックマンが死亡後の初期化。
    private void ResetGame()
    {
        pacman = Instantiate(pacmanPrefab, maze.transform);
        pacman.Initialize(maze.PacmanStartPosition);
        pacman.OnDead += (s, e) => CheckLives();

        ghosts.ForEach(g => Destroy(g.gameObject));
        PlaceGhost();
        UpdateLives();

        scareEndTime = 0;
        eatenGhost = 0;
    }

    private void NextMaze()
    {
        mazeIndex++;

        if (mazePrefabs.Length <= mazeIndex)
        {
            allClearPanel.SetActive(true);
        }
        else
        {
            Destroy(maze.gameObject);
            maze = Instantiate(mazePrefabs[mazeIndex]);
            StartCoroutine(CoroutineStart(SetGame));
        }
    }

    private void PlaceGhost()
    {
        akabei = Instantiate(akabeiPrefab, maze.transform);
        akabei.Initialize(maze, pacman);

        pinky = Instantiate(pinkyPrefab, maze.transform);
        pinky.Initialize(maze, pacman);

        aosuke = Instantiate(aosukePrefab, maze.transform);
        aosuke.Initialize(maze, pacman, akabei);

        guzuta = Instantiate(guzutaPrefab, maze.transform);
        guzuta.Initialize(maze, pacman);

        ghosts = new List<AGhost>
        { akabei, pinky, aosuke, guzuta };

        ghosts.ForEach(g =>
        {
            //ゴーストが食べられたときの処理。
            g.OnEaten += (s, e) =>
            {
                //パックマン、ゴーストを一時停止。
                PauseAll(eatSpan);

                //食べたゴーストの数により、点数が変化する。
                var scr = g.Score * (1 << eatenGhost);
                //得点の更新。
                score += scr;
                scoreText.text = score.ToString();
                //得点を表示。
                scorer.Set(scr, g.transform.position, eatSpan);
                
                eatenGhost++;

                soundManager.PlaySE("EatGhost", 0.6f);
            };

            //ゴーストが食べたときの処理。
            g.OnEat += (s, e) =>
            {
                pacman.Dead();
                //ゴーストを停止。
                ghosts.ForEach(g2 => g2.Stop());
                isScare = false;
                lives--;

                if (fruits != null) fruits.Destroy();

                soundManager.PlaySE("Dead", 0.6f);
                soundManager.StopBGM();
            };
        });
    }

    private void PlaceDot()
    {
        var poses = maze.DotPositions;
        int all = poses.Count;
        
        poses.ForEach(p =>
        {
            var dot = Instantiate(dotPrefab, maze.transform, false);
            dot.transform.localPosition = p;

            //パックマンが食べたときの処理。
            dot.OnEaten += (s, e) =>
            {
                //得点の更新。
                var d = (Dot)s;
                score += d.Score;
                scoreText.text = score.ToString();

                eatenDot++;
                //フルーツの作成。
                CreateFruits();

                //SEの再生。
                soundManager.PlaySE("EatDot", 0.4f);

                all--;

                //エサを全部食べた。
                if (all == 0)
                {
                    //パックマン、ゴーストの停止。
                    StopAll();
                    //BGM、SEの停止。
                    soundManager.StopBGM();
                    soundManager.StopSE();
                    //迷路を点滅。
                    maze.Blinking();
                    //次のステージを開始。
                    StartCoroutine(CoroutineNext());
                }
            };
        });
    }

    private void PlacePowerCokie()
    {
        var poses = maze.PowerCokiePositions;

        poses.ForEach(p =>
        {
            var pow = Instantiate(powerCokiePrefab, maze.transform, false);
            pow.transform.localPosition = p;

            //パックマンが食べたときの処理。
            pow.OnEaten += (s, e) =>
            {
                //ゴーストを恐慌状態にする。
                isScare = true;
                scareEndTime = Time.fixedTime + scareSpan;
                ghosts.ForEach(g => g.Scare(scareSpan));

                //得点の更新。
                var pc = (PowerCokie)s;
                score += pc.Score;
                scoreText.text = score.ToString();

                eatenDot++;
                //フルーツの作成。
                CreateFruits();

                //SE、BGMの再生。
                soundManager.PlaySE("EatPowerCokie", 0.2f);
                soundManager.StopBGM();
                soundManager.PlayBGM("GhostScare", 0.4f);
            };
        });
    }

    private void PauseAll(float span)
    {
        pacman.Pause(span);
        ghosts.ForEach(g => g.Pause(span));
    }

    private void StopAll()
    {
        pacman.Stop();
        ghosts.ForEach(g => g.Stop());
        isScare = false;
    }

    private void CheckLives()
    {
        if (lives >= 0)
        {
            StartCoroutine(CoroutineStart(ResetGame));
        }
        else
        {
            overPanel.SetActive(true);
        }
    }

    private void CreateFruits()
    {
        if (fruitsCreator.HasEatenDotEnough(eatenDot))
        {
            fruits = fruitsCreator.Create(maze, mazeIndex);

            //パックマンが食べたときの処理。
            fruits.OnEaten += (s, e) =>
            {
                var f = (Fruits)s;
                //得点の更新。
                score += f.Score;
                scoreText.text = score.ToString();
                //得点を表示。
                scorer.Set(f.Score, f.transform.position, eatSpan);

                //SEを再生。
                soundManager.PlaySE("EatPowerCokie", 0.2f);
            };
        }
    }

    private void UpdateLives()
    {
        var p = livesPanel.transform;

        for (int i = 0; i < p.childCount;i++)
        {
            if (i < lives) p.GetChild(i).gameObject.SetActive(true);
            else p.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void SetAudio()
    {
        soundManager = SoundManager.Instance;
        soundManager.LoadSE("EatDot", "Byu");
        soundManager.LoadSE("EatPowerCokie", "ByChance");
        soundManager.LoadSE("EatGhost", "Byuu");
        soundManager.LoadSE("Dead", "Qyurururu");
        soundManager.LoadBGM("GhostNormal", "Panic");
        soundManager.LoadBGM("GhostScare", "LFO");
        soundManager.LoadSE("GhostDead", "Obake");
    }
}
