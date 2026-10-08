import { useEffect, useRef, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import {
  abandonInterview,
  completeInterview,
  generateNextQuestion,
  getInterviewProgress,
  submitAnswer,
  type GenerateInterviewQuestionResponse,
  type InterviewProgressResponse,
} from "../../api/interviewApi";

export function ActiveInterviewPage() {
  const { id: sessionId } = useParams<{ id: string }>();

  const [question, setQuestion] =
    useState<GenerateInterviewQuestionResponse | null>(null);

  const navigate = useNavigate();
  const questionLoadStarted = useRef(false);
  const progressRequestId = useRef(0);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");
  const [answer, setAnswer] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState("");
  const [hasAttemptedSubmit, setHasAttemptedSubmit] =
    useState(false);
  const [progress, setProgress] = useState<InterviewProgressResponse | null>(null);

  useEffect(() => {
    if (!sessionId) {
      setError("Interview session ID is missing.");
      setIsLoading(false);
      return;
    }

    if (questionLoadStarted.current) {
      return;
    }

    questionLoadStarted.current = true;

    const currentSessionId = sessionId;

    async function loadQuestion() {
      try {
        setError("");

        const response =
          await generateNextQuestion(currentSessionId);

        setQuestion(response);
        await loadProgress(currentSessionId);
      } catch {
        questionLoadStarted.current = false;

        setError(
          "Unable to load the interview question.",
        );
      } finally {
        setIsLoading(false);
      }
    }

    loadQuestion();
  }, [sessionId]);

  async function handleAbandonInterview() {
    if (!sessionId || isSubmitting) {
      return;
    }

    const shouldAbandon = window.confirm(
      "Are you sure you want to leave this interview? Your progress will be saved, but you will not be able to continue this interview later.",
    );

    if (!shouldAbandon) {
      return;
    }

    try {
      setError("");

      await abandonInterview(sessionId);

      navigate("/history");
    } catch {
      setError(
        "Unable to leave the interview. Please try again.",
      );
    }
  }

  async function handleSubmitAnswer() {
    if (!question || !sessionId) {
      return;
    }

    const trimmedAnswer = answer.trim();

    if (!trimmedAnswer) {
      setHasAttemptedSubmit(true);
      setSubmitError("Please enter your answer.");
      return;
    }

    setHasAttemptedSubmit(false);

    setIsSubmitting(true);
    setSubmitError("");

    try {
      await submitAnswer(
        question.questionId,
        trimmedAnswer,
      );

      if (question.questionNumber === question.totalQuestions) {
        await completeInterview(sessionId);

        navigate(`/report/${sessionId}`);
        return;
      }

      const nextQuestion =
        await generateNextQuestion(sessionId);

      setQuestion(nextQuestion);
      await loadProgress(sessionId);
      setAnswer("");
    } catch {
      setSubmitError(
        "Unable to submit your answer or load the next question.",
      );
    } finally {
      setIsSubmitting(false);
    }
  }

  async function loadProgress(currentSessionId: string) {
    const requestId = ++progressRequestId.current;

    try {
      const response =
        await getInterviewProgress(currentSessionId);

      if (requestId === progressRequestId.current) {
        setProgress(response);
      }
    } catch {
      // Progress is helpful UI information,
      // so don't block the interview if it fails.
    }
  }

  if (isLoading) {
    return (
      <div className="interview-page">
        <div className="interview-page-header">
          <p className="dashboard-eyebrow">
            AI INTERVIEW
          </p>

          <h1>Preparing your question...</h1>

          <p>
            The AI interviewer is generating your next
            question.
          </p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="interview-page">
        <div className="interview-page-header">
          <p className="dashboard-eyebrow">
            AI INTERVIEW
          </p>

          <h1>Unable to load interview</h1>

          <p className="setup-error">
            {error}
          </p>
        </div>
      </div>
    );
  }

  if (!question) {
    return null;
  }

  return (
    <div className="interview-page">
  <div className="interview-active-header">
    <div>
      <h1>Technical Interview</h1>

      <p>
        Take your time and explain your answer clearly.
      </p>
    </div>

    <button
      type="button"
      className="interview-leave-button"
      onClick={handleAbandonInterview}
      disabled={isSubmitting}
    >
      Leave Interview
    </button>
  </div>

      {progress && (
        <div className="interview-progress">
          <div className="interview-progress-header">
            <strong>
              Question {progress.currentQuestionNumber} of{" "}
              {progress.totalQuestions}
            </strong>

            <span>
              {progress.answeredQuestions} of{" "}
              {progress.totalQuestions} answered
            </span>
          </div>

          <div className="interview-progress-questions">
            {progress.questions.map((item) => {
              const isCurrent =
                item.questionNumber ===
                progress.currentQuestionNumber;

              return (
                <div
                  key={item.questionNumber}
                  className={`interview-progress-item ${item.answered
                    ? "answered"
                    : isCurrent
                      ? "current"
                      : "pending"
                    }`}
                >
                  <span>
                    {item.answered
                      ? "✓"
                      : isCurrent
                        ? "●"
                        : "○"}
                  </span>

                  <span>
                    Q{item.questionNumber}
                  </span>
                </div>
              );
            })}
          </div>
        </div>
      )}

      {hasAttemptedSubmit && !answer.trim() && (
        <div className="interview-warning">
          <strong>⚠ Answer required</strong>
          <p>
            Please enter your answer before continuing to
            the next question.
          </p>
        </div>
      )}

      {question.questionNumber === question.totalQuestions && (
        <div className="interview-warning">
          <strong>⚠ Final question</strong>
          <p>
            This is your final question. Make sure you have
            answered all questions before submitting.
          </p>
        </div>
      )}

      <div className="interview-question-card">
        <h2>{question.questionText}</h2>
      </div>

      <div className="interview-answer-section">
        <label htmlFor="answer">
          Your Answer
        </label>

        <textarea
          id="answer"
          placeholder="Type your answer here..."
          rows={10}
          value={answer}
          onChange={(event) => setAnswer(event.target.value)}
          disabled={isSubmitting}
        />

        <button
          type="button"
          onClick={handleSubmitAnswer}
          disabled={isSubmitting || !answer.trim()}
        >
          {isSubmitting
            ? "Submitting..."
            : question.questionNumber === question.totalQuestions
              ? "Submit & Finish Interview"
              : "Submit Answer"}
        </button>

        {submitError && (
          <p className="setup-error">
            {submitError}
          </p>
        )}
      </div>
    </div>
  );
}