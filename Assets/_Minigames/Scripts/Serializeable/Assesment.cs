namespace Structures
{
    [System.Serializable]
    public class Assesment
    {
        public bool success;
        public QuestionData[] data; 
    }

    [System.Serializable]
    public class AssesmentData
    {
        public int id;
        public string master_value;
        public string question;
        public Answers[] answers;
        public Answers answers_correct;
    }
}