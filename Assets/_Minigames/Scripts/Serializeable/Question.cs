namespace Structures
{
    [System.Serializable]
    public class Question
    {
        public bool success;
        public QuestionData[] data; 
    }

    [System.Serializable]
    public class QuestionData
    {
        public int id;
        public string sub_master_value;
        public string type;
        public string game_type;
        public string media;
        public string audio;
        public string media_type;
        public string question_type;
        public string question;
        public Answers[] answers;
        public Answers answer_correct;
    }
}