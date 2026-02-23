import { useState, useCallback } from "react";
import Question from "../types/Question";

type TriviaPageProps = {
  questions: Array<Question>;
};

type SelectedAnswer = {
  question: string;
  answer: string;
};

type CheckedAnswer = {
  question: string;
  isCorrect: boolean;
};

function TriviaPage({ questions }: TriviaPageProps) {
  const [selectedAnswers, setSelectedAnswers] = useState<SelectedAnswer[]>([]);
  const [checkedAnswers, setCheckedAnswers] = useState<CheckedAnswer[]>([]);
  const [isLoading, setIsLoading] = useState(false);

  // When user clicks an answer
  const handleAnswerClick = (question: string, answer: string) => {
    setSelectedAnswers((prev) => {
      const filtered = prev.filter((q) => q.question !== question);

      return [...filtered, { question, answer }];
    });
  };

  const checkAnswers = useCallback(async () => {
    setIsLoading(true);

    try {
      const response = await fetch(
        "http://localhost:5157/api/checkanswers",
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify(selectedAnswers),
        }
      );

      if (!response.ok) {
        throw new Error("Network response was not ok");
      }

      const result: CheckedAnswer[] = await response.json();
      setCheckedAnswers(result);
    } catch (error) {
      console.error("Error fetching data:", error);
    } finally {
      setIsLoading(false);
    }
  }, [selectedAnswers]);

  const getResultForQuestion = (question: string) => {
    return checkedAnswers.find((q) => q.question === question);
  };

  const getSelectedAnswer = (question: string) => {
    return selectedAnswers.find((q) => q.question === question)?.answer;
  };

  return (
    <div>
      {questions.map((question, index) => {
        const result = getResultForQuestion(question.question);

        return (
          <div key={index} style={{ marginBottom: "20px" }}>
            <h3>{question.question}</h3>

            <ul style={{ listStyle: "none", padding: 0 }}>
              {question.answers.map((answer, answerIndex) => {
                const isSelected =
                  getSelectedAnswer(question.question) === answer;

                return (
                  <li
                    key={answerIndex}
                    onClick={() =>
                      handleAnswerClick(question.question, answer)
                    }
                    style={{
                      cursor: "pointer",
                      padding: "8px",
                      marginBottom: "5px",
                      border: "1px solid #ccc",
                      backgroundColor: isSelected
                        ? "#d0e6ff"
                        : "white",
                    }}
                  >
                    {answer}
                  </li>
                );
              })}
            </ul>

            {result && (
              <div
                style={{
                  marginTop: "10px",
                  fontWeight: "bold",
                  color: result.isCorrect ? "green" : "red",
                }}
              >
                {result.isCorrect ? "Correct ✅" : "Incorrect ❌"}
              </div>
            )}
          </div>
        );
      })}

      <button onClick={checkAnswers} disabled={isLoading}>
        {isLoading ? "Checking..." : "Check Answers"}
      </button>
    </div>
  );
}

export default TriviaPage;