using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum Difficulty
{
    Easy = 0,
    Medium = 1,
    Hard = 2,
}
public enum AnimalType
{
    Elephant = 0,
    Tiger = 1,
    Monkey = 2,

}
public class GameManager : MonoBehaviour
{

    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<GameManager>();
            }
            if (instance == null)
            {
                GameObject obj = new();
                instance = obj.AddComponent<GameManager>();
            }
            return instance;
        }
    }
    #region Comparition numbers Refrennse
    [SerializeField] private Difficulty difficulty;
    [Header("Prefabs")]
    [SerializeField] private GameObject numberContainer;
    [SerializeField] private GameObject OperationContainer;
    [SerializeField] private GameObject numberPrefab;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private GameObject OperationPrefab;
    [SerializeField] private TMP_Text explainText;
    [SerializeField] int decimalPlaces = 3;
    [SerializeField] private string result;
    [SerializeField] private TMP_Text currentOperation;
    [SerializeField] GameObject NextButton;
    [SerializeField] private int level;

    //Comparation numbers 
    private float number1;
    private float number2;
    private string formattedNumber1;
    private string formattedNumber2;

    private Dictionary<string, bool> optionsDictionary = new();
    private List<string> availableOptions = new();
    #endregion

    #region Mini Game Refennse
    [SerializeField] private Camera cam;
    [SerializeField] GameObject animalPerfab;
    [SerializeField] int spawnNumber = 3;

    private float spawnPosY = -3f;

    private List<GameObject> animalList = new();
    #endregion
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
        optionsDictionary.Add("<", false);
        optionsDictionary.Add(">", false);
        optionsDictionary.Add("=", false);

        availableOptions = new List<string>(optionsDictionary.Keys);
    }
    // Start is called before the first frame update
    void Start()
    {
        //GenerateQuestion();
        SpwanAnimals();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GenerateQuestion();
        }
    }
    #region Comparition Methods
    public void GenerateQuestion()
    {
        GenerateNumbers(ref number1, ref formattedNumber1);

        if (level == (int)Difficulty.Easy)
        {
            Debug.Log("In E");
            GenerateEasyQuestion();
        }
        else if (level == (int)Difficulty.Medium)
        {
            Debug.Log("In M");

            GenerateMediumQuestion();
        }
        else
        {
            Debug.Log("In H");

            GenerateHardQuestion();
        }


        InitNumbers();
        InitOptions();
        GetResult();
    }

    private void GenerateEasyQuestion()
    {
        GenerateNumbers(ref number2, ref formattedNumber2);
    }
    private void GenerateMediumQuestion()
    {
        bool generateSame = Random.Range(0, 2) == 0;
        if (generateSame)
        {
            number2 = number1;
            formattedNumber2 = formattedNumber1;
        }
        else
        {
            GenerateNumbers(ref number2, ref formattedNumber2);
        }
    }
    private void GenerateHardQuestion()
    {
        float minOffset = Mathf.Pow(10, -decimalPlaces) * 25;
        float maxOffset = Mathf.Pow(10, -decimalPlaces) * 50;

        float newNumber;

        do
        {
            float offset = Random.Range(minOffset, maxOffset);
            newNumber = number1 + (Random.Range(0, 2) == 0 ? offset : -offset);
        }
        while (newNumber >= 10 || newNumber < 0);


        string formatSpecifier = "0." + new string('0', decimalPlaces);
        string formattedString = newNumber.ToString(formatSpecifier);

        if (formattedString.EndsWith("0"))
        {
            int newDigit = Random.Range(1, 10);
            formattedString = formattedString.Substring(0, formattedString.Length - 1) + newDigit;
        }

        Debug.Log($"Generated number2: {newNumber}, formatted: {formattedString}");

        number2 = float.Parse(formattedString);
        formattedNumber2 = formattedString;
    }
    private void GenerateNumbers(ref float number, ref string numberToString)
    {
        number = 0;

        float integerPart = Random.Range(0, 10);
        float decimalPart = Mathf.Round(
                                       Random.Range(0f, 1f) *
                                       Mathf.Pow(10, decimalPlaces)) /
                                       Mathf.Pow(10, decimalPlaces);

        float randomNumber = integerPart + decimalPart;

        string formatSpecifier = "0." + new string('0', decimalPlaces);
        string formattedNumber = randomNumber.ToString(formatSpecifier);

        if (formattedNumber.EndsWith("0"))
        {
            int newDigit = Random.Range(1, 10);
            formattedNumber = formattedNumber.Substring(0, formattedNumber.Length - 1) + newDigit;
        }

        number = float.Parse(formattedNumber);
        numberToString = formattedNumber;
    }
    private void GetResult()
    {
        result = "";
        if (number1 > number2)
        {
            result = ">";
        }
        else if (number1 < number2)
        {
            result = "<";
        }
        else
        {
            result = "=";
        }
    }
    private void InitNumbers()
    {
        foreach (Transform child in numberContainer.transform)
        {
            Destroy(child.gameObject);
        }
        explainText.transform.parent.gameObject.SetActive(false);

        NextButton.transform.gameObject.SetActive(false);

        availableOptions = new List<string>(optionsDictionary.Keys);

        var numPerfab1 = Instantiate(numberPrefab, numberContainer.transform);
        numPerfab1.GetComponent<TextReplacer>().text.text = formattedNumber1;

        var oprationPerfab = Instantiate(OperationPrefab, numberContainer.transform);
        currentOperation = oprationPerfab.GetComponent<TextReplacer>().text;
        currentOperation.text = "?";

        var numPerfab2 = Instantiate(numberPrefab, numberContainer.transform);
        numPerfab2.GetComponent<TextReplacer>().text.text = formattedNumber2;
    }
    private void InitOptions()
    {
        foreach (Transform child in OperationContainer.transform)
        {
            Destroy(child.gameObject);
        }

        while (availableOptions.Count > 0)
        {
            int randomIndex = Random.Range(0, availableOptions.Count);
            string randomOption = availableOptions[randomIndex];
            availableOptions.RemoveAt(randomIndex);

            var operation = Instantiate(OperationPrefab, OperationContainer.transform);
            operation.GetComponent<TextReplacer>().text.text = randomOption;
            var button = operation.GetComponent<Button>();
            string optionToCheck = randomOption;
            var buttonTextComponent = operation.GetComponent<TextReplacer>().text;
            button.onClick.AddListener(() =>
            {
                if (optionToCheck.Equals(result))
                {
                    AnswerCorrect(buttonTextComponent);
                }
                else
                {
                    AnswerIncorrect(buttonTextComponent);
                }
            });
        }
    }
    private void AnswerCorrect(TMP_Text text)
    {
        foreach (Transform child in OperationContainer.transform)
        {
            child.GetComponent<Button>().interactable = false;
        }
        text.color = Color.green;
        explainText.transform.parent.gameObject.SetActive(true);

        NextButton.transform.gameObject.SetActive(true);

        currentOperation.text = result;
        currentOperation.color = Color.green;

        explainText.text = $"Correct! \n {number1} {result} {number2}";

        level++;
    }

    private void AnswerIncorrect(TMP_Text text)
    {
        foreach (Transform child in OperationContainer.transform)
        {
            child.GetComponent<Button>().interactable = false;
        }
        text.color = Color.red;
        explainText.transform.parent.gameObject.SetActive(true);

        NextButton.transform.gameObject.SetActive(true);

        currentOperation.text = result;
        currentOperation.color = Color.green;

        explainText.text = $"Incorrect! \n" +
                           $"The correct answer is : {number1} {result} {number2}";
    }
    #endregion

    #region Mini Game Methods

    internal float GetRandomCameraPosition()
    {
        Vector3 screenBottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.transform.position.z));
        Vector3 screenTopRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.transform.position.z));

        float minX = screenBottomLeft.x;
        float maxY = screenTopRight.x;
        float newX = UnityEngine.Random.Range(minX, maxY);
        return newX;
    }
    private void SpwanAnimals()
    {
        while (spawnNumber > 0)
        {
            float randomX = GetRandomCameraPosition();
            Vector3 spawnPosition = new Vector3(randomX, spawnPosY, 0);
            var animal = Instantiate(animalPerfab, spawnPosition, Quaternion.identity);
            var randomAnimalType = GetRandomAnimalType();
            string lowerCaseAnimal = randomAnimalType.ToString().ToLower();
            animal.GetComponent<TextReplacer>().text.text = lowerCaseAnimal;
            animalList.Add(animal);
            spawnNumber--;
        }
    }

    private AnimalType GetRandomAnimalType()
    {
        var values = (AnimalType[])System.Enum.GetValues(typeof(AnimalType));
        int randomIndex =UnityEngine.Random.Range(0, values.Length);
        return values[randomIndex];
    }
    #endregion
}