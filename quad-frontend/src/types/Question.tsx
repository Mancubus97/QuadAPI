type Question = {
  quizId: number;
  category: string;
  type: string;
  difficulty: string;
  question: string;
  answers: string[];
  my_answer: string;
};

export default Question;