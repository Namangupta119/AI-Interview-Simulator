import { useState, type FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import { createInterviewSession } from "../../api/interviewApi";

const ExperienceLevel = {
  Junior: 0,
  MidLevel: 1,
  Senior: 2,
  Lead: 3,
} as const;

type ExperienceLevel =
  (typeof ExperienceLevel)[keyof typeof ExperienceLevel];

const InterviewDifficulty = {
  Easy: 0,
  Medium: 1,
  Hard: 2,
} as const;

type InterviewDifficulty =
  (typeof InterviewDifficulty)[keyof typeof InterviewDifficulty];

const InterviewTopic = {
  CSharp: 0,
  DotNetCore: 1,
  EntityFrameworkCore: 2,
  SqlAndDatabases: 3,
  SystemDesign: 4,
  DataStructuresAndAlgorithms: 5,
  WebSecurity: 6,
  Microservices: 7,
  ReactAndFrontend: 8,
} as const;

type InterviewTopic =
  (typeof InterviewTopic)[keyof typeof InterviewTopic];

export function InterviewSetupPage() {
  const navigate = useNavigate();

  const [role, setRole] = useState("");
  const [experienceLevel, setExperienceLevel] =
    useState<ExperienceLevel>(ExperienceLevel.Junior);
  const [difficulty, setDifficulty] =
    useState<InterviewDifficulty>(InterviewDifficulty.Medium);
  const [totalQuestions, setTotalQuestions] = useState(10);

  const [topics, setTopics] = useState<InterviewTopic[]>([
    InterviewTopic.CSharp,
    InterviewTopic.DotNetCore,
    InterviewTopic.SqlAndDatabases,
  ]);

  const [customTopicInput, setCustomTopicInput] = useState("");
  const [customTopics, setCustomTopics] = useState<string[]>([]);

  const [error, setError] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  function toggleTopic(topic: InterviewTopic) {
    setTopics((current) =>
      current.includes(topic)
        ? current.filter((item) => item !== topic)
        : [...current, topic],
    );
  }

  function addCustomTopic() {
    const topic = customTopicInput.trim();

    if (!topic) {
      return;
    }

    if (topic.length > 100) {
      setError("Custom topic cannot exceed 100 characters.");
      return;
    }

    if (customTopics.length >= 5) {
      setError("You can add a maximum of 5 custom topics.");
      return;
    }

    if (
      customTopics.some(
        (item) => item.toLowerCase() === topic.toLowerCase(),
      )
    ) {
      setError("This custom topic has already been added.");
      return;
    }

    setCustomTopics((current) => [...current, topic]);
    setCustomTopicInput("");
    setError("");
  }

  function removeCustomTopic(topicToRemove: string) {
    setCustomTopics((current) =>
      current.filter((topic) => topic !== topicToRemove),
    );
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    setError("");

    if (!role.trim()) {
      setError("Please enter the interview role.");
      return;
    }

    if (topics.length === 0 && customTopics.length === 0) {
      setError("Please select or add at least one topic.");
      return;
    }

    setIsSubmitting(true);

    try {
      const response = await createInterviewSession({
        role: role.trim(),
        experienceLevel,
        difficulty,
        totalQuestions,
        topics,
        customTopics,
      });

      navigate(`/interview/${response.sessionId}`);
    } catch {
      setError("Unable to start the interview.");
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <div className="interview-setup">
      <div className="interview-setup-header">
        <p className="dashboard-eyebrow">NEW INTERVIEW</p>

        <h1>Set up your interview</h1>

        <p>
          Configure your interview before starting the AI-powered session.
        </p>
      </div>

      <form
        className="interview-setup-form"
        onSubmit={handleSubmit}
      >
        <div className="setup-field">
          <label htmlFor="role">Interview Role</label>

          <input
            id="role"
            type="text"
            placeholder="e.g. .NET Developer"
            value={role}
            onChange={(event) => setRole(event.target.value)}
            disabled={isSubmitting}
          />
        </div>

        <div className="setup-field">
          <label htmlFor="experienceLevel">
            Experience Level
          </label>

          <select
            id="experienceLevel"
            value={experienceLevel}
            onChange={(event) =>
              setExperienceLevel(
                Number(event.target.value) as ExperienceLevel,
              )
            }
            disabled={isSubmitting}
          >
            <option value={ExperienceLevel.Junior}>
              Junior
            </option>

            <option value={ExperienceLevel.MidLevel}>
              Mid Level
            </option>

            <option value={ExperienceLevel.Senior}>
              Senior
            </option>

            <option value={ExperienceLevel.Lead}>
              Lead
            </option>
          </select>
        </div>

        <div className="setup-field">
          <label htmlFor="difficulty">
            Difficulty
          </label>

          <select
            id="difficulty"
            value={difficulty}
            onChange={(event) =>
              setDifficulty(
                Number(event.target.value) as InterviewDifficulty,
              )
            }
            disabled={isSubmitting}
          >
            <option value={InterviewDifficulty.Easy}>
              Easy
            </option>

            <option value={InterviewDifficulty.Medium}>
              Medium
            </option>

            <option value={InterviewDifficulty.Hard}>
              Hard
            </option>
          </select>
        </div>

        <div className="setup-field">
          <label htmlFor="totalQuestions">
            Total Questions
          </label>

          <select
            id="totalQuestions"
            value={totalQuestions}
            onChange={(event) =>
              setTotalQuestions(Number(event.target.value))
            }
            disabled={isSubmitting}
          >
            <option value={5}>5</option>
            <option value={10}>10</option>
            <option value={15}>15</option>
            <option value={20}>20</option>
          </select>
        </div>

        <fieldset className="setup-topics">
          <legend>Topics</legend>

          {Object.entries(InterviewTopic)
            .filter(([, value]) => typeof value === "number")
            .map(([name, value]) => {
              const topic = value as InterviewTopic;

              return (
                <label
                  key={topic}
                  className="topic-option"
                >
                  <input
                    type="checkbox"
                    checked={topics.includes(topic)}
                    onChange={() => toggleTopic(topic)}
                    disabled={isSubmitting}
                  />

                  <span>{name}</span>
                </label>
              );
            })}
        </fieldset>

        <div className="setup-field">
          <label htmlFor="customTopic">
            Custom Topics
          </label>

          <div className="custom-topic-input">
            <input
              id="customTopic"
              type="text"
              placeholder="e.g. Azure, Docker, Redis"
              value={customTopicInput}
              onChange={(event) =>
                setCustomTopicInput(event.target.value)
              }
              maxLength={100}
              disabled={
                isSubmitting ||
                customTopics.length >= 5
              }
            />

            <button
              type="button"
              onClick={addCustomTopic}
              disabled={
                isSubmitting ||
                !customTopicInput.trim() ||
                customTopics.length >= 5
              }
            >
              Add
            </button>
          </div>

          {customTopics.length > 0 && (
            <div className="custom-topic-list">
              {customTopics.map((topic) => (
                <div
                  key={topic}
                  className="custom-topic-item"
                >
                  <span>{topic}</span>

                  <button
                    type="button"
                    onClick={() => removeCustomTopic(topic)}
                    disabled={isSubmitting}
                  >
                    Remove
                  </button>
                </div>
              ))}
            </div>
          )}

          <small>
            Add up to 5 custom topics.
          </small>
        </div>

        {error && (
          <p className="setup-error">
            {error}
          </p>
        )}

        <button
          type="submit"
          disabled={isSubmitting}
        >
          {isSubmitting
            ? "Starting..."
            : "Start Interview"}
        </button>
      </form>
    </div>
  );
}