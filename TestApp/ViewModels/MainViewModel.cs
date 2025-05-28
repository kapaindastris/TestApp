using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using TestApp.Commands;
using TestApp.Models;

namespace TestApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Question> Questions { get; set; } = new();
        public ObservableCollection<string> CurrentAnswers { get; set; } = new();

        private int currentIndex = -1;
        private string questionText;
        private string answer1, answer2, answer3, answer4;
        private string selectedRightAnswer;
        private string selectedUserAnswer;
        private Question currentQuestion;
        private bool isTesting = false;

        public string QuestionText { get => questionText; set { questionText = value; OnPropertyChanged(); } }
        public string Answer1 { get => answer1; set { answer1 = value; OnPropertyChanged(); } }
        public string Answer2 { get => answer2; set { answer2 = value; OnPropertyChanged(); } }
        public string Answer3 { get => answer3; set { answer3 = value; OnPropertyChanged(); } }
        public string Answer4 { get => answer4; set { answer4 = value; OnPropertyChanged(); } }
        public string SelectedRightAnswer { get => selectedRightAnswer; set { selectedRightAnswer = value; OnPropertyChanged(); } }
        public string SelectedUserAnswer { get => selectedUserAnswer; set { selectedUserAnswer = value; OnPropertyChanged(); } }
        public Question CurrentQuestion { get => currentQuestion; set { currentQuestion = value; OnPropertyChanged(); } }
        public bool IsTesting { get => isTesting; set { isTesting = value; OnPropertyChanged(); } }

        public ICommand AddQuestionCommand => new RelayCommand(AddQuestion);
        public ICommand StartTestCommand => new RelayCommand(StartTest);
        public ICommand SubmitAnswerCommand => new RelayCommand(SubmitAnswer);

        private int correctAnswers = 0;
        private int incorrectAnswers = 0;

        public int QuestionCount => Questions.Count;
        public int CorrectAnswers => correctAnswers;
        public int IncorrectAnswers => incorrectAnswers;

        private void AddQuestion()
        {
            // Валидация: проверка на заполненность всех вариантов
            var vars = new List<string> { Answer1, Answer2, Answer3, Answer4 };
            if (vars.Any(string.IsNullOrWhiteSpace))
            {
                MessageBox.Show("Все варианты ответа должны быть заполнены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Валидация: проверка на уникальность вариантов
            if (vars.Distinct().Count() != vars.Count)
            {
                MessageBox.Show("Варианты ответа должны быть уникальными.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Валидация: правильный ответ должен быть выбран из вариантов
            if (string.IsNullOrWhiteSpace(SelectedRightAnswer) || !vars.Contains(SelectedRightAnswer))
            {
                MessageBox.Show("Правильный ответ должен быть выбран из списка вариантов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Questions.Add(new Question
            {
                Number = Questions.Count + 1,
                Meaning = QuestionText,
                Variables = vars,
                RightAnswer = SelectedRightAnswer
            });

            // Очистка полей
            QuestionText = Answer1 = Answer2 = Answer3 = Answer4 = SelectedRightAnswer = string.Empty;
            OnPropertyChanged(nameof(QuestionCount));
        }

        private void StartTest()
        {
            if (Questions.Count == 0) return;
            currentIndex = 0;
            correctAnswers = incorrectAnswers = 0;
            IsTesting = true;
            LoadQuestion();
        }

        private void LoadQuestion()
        {
            if (currentIndex < Questions.Count)
            {
                CurrentQuestion = Questions[currentIndex];
                CurrentAnswers.Clear();
                foreach (var a in CurrentQuestion.Variables)
                    CurrentAnswers.Add(a);
                SelectedUserAnswer = null;
            }
        }

        private void SubmitAnswer()
        {
            if (SelectedUserAnswer == CurrentQuestion.RightAnswer)
                correctAnswers++;
            else
                incorrectAnswers++;

            currentIndex++;
            if (currentIndex < Questions.Count)
                LoadQuestion();
            else
                IsTesting = false;

            OnPropertyChanged(nameof(CorrectAnswers));
            OnPropertyChanged(nameof(IncorrectAnswers));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}