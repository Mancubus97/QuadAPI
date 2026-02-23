import React from 'react';
import { useEffect, useState, useCallback } from 'react';
import logo from './logo.svg';
import './App.css';
import TriviaPage from './components/TriviaPage';
import Question from './types/Question';




function App() {
  const [isLoading, setIsLoading] = useState<boolean>(false); // State to store an boolean to indicate if the data is loading
  const [questions, setQuestions] = useState<Array<Question>>([]); // State to store the questions data

   const fetchQuestions = useCallback(async () => {
    setIsLoading(true);
    try {
      const apiUrl = `http://localhost:5157/api/questions?amount=10`;

      const response = await fetch(apiUrl);

      if (!response.ok) {
        throw new Error('Network response was not ok');
      }

      const result = await response.json();

      // Set the fetched array of plants as the new state
      setQuestions(result);
    } catch (error) {
      console.error('Error fetching data:', error);
    } finally {
      setIsLoading(false);
    }
  }, []);

  return (
    <div className="App">
      {isLoading ? (
        <div>Loading...</div>
      ) : (
        <div>
          <button onClick={fetchQuestions}>Get Questions</button>
          <TriviaPage questions={questions}/>
        </div>
      )}
    </div>
  );
}

export type { Question };
export default App;
