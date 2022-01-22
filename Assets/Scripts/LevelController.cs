using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{

  public static LevelController lv;

  //  internal spaceshipBuilt parameter and externally accessible getter
  private static bool _spaceshipBuilt = false;
  public static bool SpaceshipBuilt
  {
    set { _spaceshipBuilt = value; }
    get { return _spaceshipBuilt; }
  }

  private static string _nextSpaceshipPart = "SpaceshipPart2";
  public static string NextSpaceshipPart
  {
    set { _nextSpaceshipPart = value; }
    get { return _nextSpaceshipPart; }
  }

  private static bool _spacesmanInside = false;
  public static bool SpacemanInside
  {
    set { _spacesmanInside = value; }
    get { return _spacesmanInside; }
  }

  private static bool _fuelCellActive = false;
  public static bool FuelCellActive
  {
    set { _fuelCellActive = value; }
    get { return _fuelCellActive; }
  }

  public Transform playerSpawnPoint;
  public Transform fuelCell;
  public Transform[] spawnPoints;
  public Transform[] spaceshipParts;
  public Transform spaceshipTopPart;
  public Transform[] collectables;

  public GameObject spaceship;
  public GameObject liftOffEntrance;
  public GameObject spaceman;

  private int fuelCellsNeeded;
  private int fuelCellsDropped;

  private bool rocketReadyForTakeOff;
  private bool rocketLaunched;
  private Animator spaceshipAnim;

  private int collectablesCounter;                                           //  keep track of the number of colectables in the scene
  private int maxColectablesInScene = 3;
  private bool spawningCollectable;

  private int currentLevelLoop;
  private int maxLevelLoops = 3;
  private bool readyForNextLevel = false;
  private bool newLoopStarted;



  //  S  T  A  R  T
  //  ---------------------------------------------------------------------------------------
  void Start()
  {

    //    make sure we have a LevelController object in the scene
    if (lv == null)
      lv = GameObject.FindGameObjectWithTag("LV").GetComponent<LevelController>();

    spaceshipAnim = spaceship.GetComponent<Animator>();
    if (spaceshipAnim == null)
      Debug.LogError("You need to put the spaceship on the LevelController");

    readyForNextLevel = true;
    currentLevelLoop = 1;

    rocketLaunched = false;
    newLoopStarted = true;

    //  initialise properties
    fuelCellsNeeded = 6;
    fuelCellsDropped = 0;


    collectablesCounter = 0;
    spawningCollectable = false;

    StartCoroutine(DropCollectable(newLoopStarted));
    newLoopStarted = false;
  }

  //  U  P  D  A  T  E
  //  ---------------------------------------------------------------------------------------
  void Update()
  {
    // theNextPart = _nextSpaceshipPart;

    //    if the spaceship is build and we are not dropping a cell and there are cells left to drop
    if (_spaceshipBuilt && !_fuelCellActive && fuelCellsDropped < fuelCellsNeeded)
    {
      _fuelCellActive = true;

      //  start the dropping cell co-routine.
      StartCoroutine(DropFuelCell(newLoopStarted));
    }

    //  once all the fuel is onboard we can prepare the rocket for take-off by setting all the parts to
    //  kinetic and isTrigger so the spaceman can walk it it. Start at the top and work your way doen oto the base otherwise
    //  the parts above will fall through the floor.
    //  We also need to activate the lift-off trigger for the spaceman to walk into
    if (fuelCellsDropped == fuelCellsNeeded && !rocketReadyForTakeOff)
    {
      // for (int i = spaceshipParts.Length - 1; i >= 0; i--)
      // {
      //   spaceshipParts[i].GetComponent<Rigidbody2D>().isKinematic = true;
      //   spaceshipParts[i].GetComponent<BoxCollider2D>().isTrigger = true;
      // }
      rocketReadyForTakeOff = true;
      liftOffEntrance.SetActive(true);
    }

    //  if the spaceman is inside the rocket then we can launch
    if (_spacesmanInside && !rocketLaunched)
    {
      Debug.Log("StartCoroutine(RocketLaunch());");
      StartCoroutine(RocketLaunch());
    }


    //  if the max collectable is not yet hit AND we're not currently spawing a collectable AND
    //  the rocket is currently launch then start spawning another collectable.
    if (collectablesCounter < maxColectablesInScene && !spawningCollectable && !rocketLaunched)
      StartCoroutine(DropCollectable(newLoopStarted));

  }


  private void FixedUpdate()
  {
    if (readyForNextLevel && currentLevelLoop <= maxLevelLoops)
    {
      rocketLaunched = false;
      _spacesmanInside = false;
      rocketReadyForTakeOff = false;

      if (currentLevelLoop == 1)
      {
        _spaceshipBuilt = false;

      }
      else
      {
        CleanupLeftoverCollectables();

        //  run spaceship landing animation
        //  before we land the spaceship we need to make it all white again
        for (int i = 0; i < spaceshipParts.Length; i++)
        {
          SpriteRenderer sr = spaceshipParts[i].GetComponent<SpriteRenderer>();
          sr.color = Color.white;
        }

        //  now land the spaceship
        spaceshipAnim.SetBool("LaunchSpaceship", false);
        spaceshipAnim.SetBool("LandSpaceship", true);


        //  as the spaceship is landing we can start the countdown to spawn the spaceman
        StartCoroutine(SpawnSpaceman());

        //  initialise a few things
        newLoopStarted = true;
        _spaceshipBuilt = true;
        fuelCellsDropped = 0;
        _nextSpaceshipPart = null;

      }
    }

    readyForNextLevel = false;


  }


  //  F  u  e  l  C  e  l  l  L  a  n  d  e  d
  //  ------------------------------------------------------------------------------------------------------------
  //
  //  publically available method for flagging that a fuel cell has landed on the spaceship
  //
  public static void FuelCellLanded()
  {
    lv._fuelCellLanded();
  }

  //  actual method that controls what happens once a fuel cell has landed.
  private void _fuelCellLanded()
  {

    //  before we increment the fuelCellsDropped counter we can use it's value to get the correct spaceship part to change the colour off
    SpriteRenderer sr = spaceshipParts[fuelCellsDropped].GetComponent<SpriteRenderer>();
    if (sr != null)
      sr.color = Color.magenta;

    fuelCellsDropped++;
    _fuelCellActive = false;
  }


  public static void DecrementCollectablesCounter()
  {
    lv.collectablesCounter--;
  }

  //  D  r  o  p  F  u  e  l  C  e  l  l
  //  ----------------------------------------------------------------------------------------------------------------
  //
  //  waits between 3 and 6 second then spawns a new fuel cell
  //
  IEnumerator DropFuelCell(bool newLoopStarted)
  {
    int waitPeriod;

    //  get a randon wait periodbefore we spawn. If it's the firest collectable of the level then
    //  wait an extra 5s to make sure the ship lands or we have a bit more time to build the ship
    waitPeriod = Random.Range(3, 10);
    if (newLoopStarted)
      waitPeriod += 5;

    //  wait for a bit
    yield return new WaitForSeconds(waitPeriod);

    //  find a spawn point
    Transform _sp = spawnPoints[Random.Range(0, spawnPoints.Length)];

    //  and create a fuel cell to drop
    Instantiate(fuelCell, _sp.position, _sp.rotation);
  }


  //  D  r  o  p  C  o  l  l  e  c  t  a  b  l  e
  //  ----------------------------------------------------------------------------------------------------------------
  //
  //  waits between 3 and 6 second then spawns a new collectable
  //
  IEnumerator DropCollectable(bool newLoopStarted)
  {
    int waitPeriod;

    //  set the spawning flag to true so we don't spawn shite loads more whlie waiting for this one to spawn
    spawningCollectable = true;

    //  get a randon wait periodbefore we spawn. If it's the firest collectable of the level then
    //  wait an extra 5s to make sure the ship lands or we have a bit more time to build the ship
    waitPeriod = Random.Range(3, 10);
    if (newLoopStarted)
      waitPeriod += 5;

    //  wait for a bit
    yield return new WaitForSeconds(waitPeriod);

    collectablesCounter++;

    //  find a spawn point
    Transform _sp = spawnPoints[Random.Range(0, spawnPoints.Length)];

    //  choose a collectable to drop
    Transform _collectable = collectables[Random.Range(0, collectables.Length)];

    //  and create a collectable cell to drop
    Instantiate(_collectable, _sp.position, _sp.rotation);

    spawningCollectable = false;
  }



  //  R  o  c  k  e  t  L  a  u  n  c  h
  //  --------------------------------------------------------------------------------------------------------------------
  //
  //  starts the launch rocket animation
  //
  IEnumerator RocketLaunch()
  {
    rocketLaunched = true;

    //  launch the spaceship
    spaceshipAnim.SetBool("LandSpaceship", false);
    spaceshipAnim.SetBool("LaunchSpaceship", true);

    //  wait for 5 seconds then go onto the next level
    yield return new WaitForSeconds(5.0f);
    currentLevelLoop++;
    readyForNextLevel = true;



  }


  private void CleanupLeftoverCollectables()
  {

    //  go through all the game object and if any of them are collectables destroy them
    List<GameObject> rootObjects = new List<GameObject>();
    Scene scene = SceneManager.GetActiveScene();
    scene.GetRootGameObjects(rootObjects);

    // iterate root objects and do something
    for (int i = 0; i < rootObjects.Count; ++i)
    {
      GameObject gameObject = rootObjects[i];
      if (gameObject.tag == "Collectable")
        Destroy(gameObject);
    }

    collectablesCounter = 0;
  }


  IEnumerator SpawnSpaceman()
  {
    yield return new WaitForSeconds(5.5f);

    Debug.Log("Spawning in LevelController");
    //  TODO: only spawn the player if there are no enemies nearby. Try using raycast to see what's happening around you.
    //  SPAWN THE PLAYER AS HE'S BEEN DESTROYED AT THE END OF THE PREVIOUS LOOP
    Instantiate(spaceman, playerSpawnPoint.transform.position, Quaternion.identity);

  }

}
