import React from 'react';
import { useState, useCallback } from 'react';
import './App.css';
import TriviaPage from './components/TriviaPage';
import Question from './types/Question';




function App() {
  const [isLoading, setIsLoading] = useState<boolean>(false); // State to store an boolean to indicate if the data is loading
  const [questions, setQuestions] = useState<Array<Question>>([]); // State to store the questions data
  const [amount, setAmount] = useState<number>(10);


   const fetchQuestions = async () => {
    setIsLoading(true);
    try {
      const apiUrl = `http://localhost:5157/api/questions?amount=${amount}`;

      const response = await fetch(apiUrl);

      if (!response.ok) {
        throw new Error('Network response was not ok');
      }

      const result = await response.json();

      setQuestions(result);
    } catch (error) {
      console.error('Error fetching data:', error);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="App">
      {isLoading ? (
        <div>Loading...</div>
      ) : (
        <div>
          <input
          type="number"
          min="1"
          value={amount}
          onChange={(e) => setAmount(Number(e.target.value))}/>

          <button onClick={fetchQuestions}>Get Questions</button>

          <TriviaPage questions={questions}/>
        </div>
      )}
    </div>
  );
}

export type { Question };
export default App;
