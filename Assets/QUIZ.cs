using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms.Impl;

public class QUIZ : MonoBehaviour
{
    [Header("Questions")]
    [SerializeField] TextMeshProUGUI questionText;
    QuestionSO currentQuestion;
    [SerializeField] List<QuestionSO> questions = new List<QuestionSO>();

    [Header("Answers")]
    [SerializeField] GameObject[] answerButtons; 
    int correctAnswerIndex;
    bool hasAnsweredEarly = true;

    [Header("Button Colors")]
    [SerializeField] Sprite defaultAnswerSprite;
    [SerializeField] Sprite correctAnswerSprite;

    [Header("Timer")]
    [SerializeField] Image timerImage;
    Timer timer;

    [Header("Scoring")]
    [SerializeField] TextMeshProUGUI scoreText;
    ScoreKeeper scoreKeeper;

    [Header("Progress Bar")]
    [SerializeField] Slider progreeBar;

    public bool isComplete;

    void Awake()
    {
        timer = FindObjectOfType<Timer>();
        scoreKeeper = FindObjectOfType<ScoreKeeper>();
        progreeBar.maxValue = questions.Count;
        progreeBar.value = 0;
    }

    void Update() {
        timerImage.fillAmount = timer.fillFraction;
        if(timer.loadNextQuestion){
            if(progreeBar.value == progreeBar.maxValue){
                isComplete = true;
                return;
            }
            hasAnsweredEarly = false;
            GetNewQuestion();
            timer.loadNextQuestion = false;
        }
        else if(!hasAnsweredEarly && !timer.isAnsweringQuestion){
            DisplayAnswer(-1);
            SetButtonState(false);
        }
    }

    void DisplayAnswer(int index){
        Image buttonImage;

        if(index == currentQuestion.GetCorrectAnswerIndex())
        {
            questionText.text = "Correct!";
            buttonImage = answerButtons[index].GetComponent<Image>();
            buttonImage.sprite = correctAnswerSprite;
            scoreKeeper.IncrementCorrectAnswers();
        }
        else
        {
            correctAnswerIndex = currentQuestion.GetCorrectAnswerIndex();
            string correctAnswer = currentQuestion.GetAnswer(correctAnswerIndex);
            questionText.text = "Sorry, the correct answer was;\n" + correctAnswer;
            buttonImage = answerButtons[correctAnswerIndex].GetComponent<Image>();
            buttonImage.sprite = correctAnswerSprite;
        }
    }

    public void OnAnswerSelected(int index)
        {
            hasAnsweredEarly = true;
            DisplayAnswer(index);
            SetButtonState(false);
            timer.CancelTimer();
            scoreText.text = "Score :"+ scoreKeeper.CalculateScore() +"%";
        }

    void DisplayQuestions(){
        questionText.text = currentQuestion.GetQuestion();
         for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.GetAnswer(i);
        }
    }

    void GetNewQuestion(){
        if(questions.Count > 0){
        SetButtonState(true);
        SetDefaultButtonSprites();
        GetRandomQuestion();
        DisplayQuestions();
        progreeBar.value++;
        scoreKeeper.IncrementQuestionsSeen();
        }
    }

    void GetRandomQuestion(){
        int index = Random.Range(0,questions.Count);
        currentQuestion = questions[index];
        if(questions.Contains(currentQuestion)){
            questions.Remove(currentQuestion);
        }
    }

    void SetButtonState(bool state){
        for(int i=0;i<answerButtons.Length;i++){
            Button button = answerButtons[i].GetComponent<Button>();
            button.interactable = state;
        }
    }
    void SetDefaultButtonSprites(){
        for(int i=0;i<answerButtons.Length;i++){
            Image buttonImage = answerButtons[i].GetComponent<Image>();
            buttonImage.sprite = defaultAnswerSprite;
        }
    }
}



