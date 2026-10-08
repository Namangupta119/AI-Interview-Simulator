import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import {
    getInterviewHistory,
    type InterviewHistoryItemResponse,
} from "../../api/historyApi";

const experienceLabels: Record<number, string> = {
    0: "Junior",
    1: "Mid Level",
    2: "Senior",
    3: "Lead",
};

const difficultyLabels: Record<number, string> = {
    0: "Easy",
    1: "Medium",
    2: "Hard",
};

const statusLabels: Record<number, string> = {
    0: "In Progress",
    1: "Completed",
    2: "Abandoned",
};

const topicLabels: Record<number, string> = {
    0: "C#",
    1: ".NET Core",
    2: "Entity Framework Core",
    3: "SQL & Databases",
    4: "System Design",
    5: "Data Structures & Algorithms",
    6: "Web Security",
    7: "Microservices",
    8: "React & Frontend",
};

function formatDate(date: string): string {
    return new Date(date).toLocaleDateString("en-IN", {
        day: "2-digit",
        month: "short",
        year: "numeric",
    });
}

function getScoreClass(score: number | null): string {
    if (score === null) {
        return "history-score-pending";
    }

    if (score >= 7) {
        return "history-score-good";
    }

    if (score >= 5) {
        return "history-score-average";
    }

    return "history-score-low";
}

function getTopicName(topic: number): string {
    return topicLabels[topic] ?? `Topic ${topic}`;
}

function getStatusName(status: number): string {
    return statusLabels[status] ?? "Unknown";
}

function HistoryCard({
    interview,
}: {
    interview: InterviewHistoryItemResponse;
}) {
    const topics = [
        ...interview.topics.map(getTopicName),
        ...interview.customTopics,
    ];

    const isCompleted = interview.status === 1;
    const isInProgress = interview.status === 0;

    const statusClass =
        interview.status === 1
            ? "history-status-completed"
            : interview.status === 2
                ? "history-status-abandoned"
                : "history-status-progress";

    return (
        <article className="history-card">
            <div className="history-card-header">
                <div>
                    <p className="history-card-role">{interview.role}</p>

                    <p className="history-card-date">
                        {formatDate(interview.startedAtUtc)}
                    </p>
                </div>

                <span className={`history-status ${statusClass}`}>
                    {getStatusName(interview.status)}
                </span>
            </div>

            <div className="history-card-details">
                <div>
                    <span>Experience</span>
                    <strong>
                        {experienceLabels[interview.experienceLevel] ?? "Unknown"}
                    </strong>
                </div>

                <div>
                    <span>Difficulty</span>
                    <strong>
                        {difficultyLabels[interview.difficulty] ?? "Unknown"}
                    </strong>
                </div>

                <div>
                    <span>Questions</span>
                    <strong>{interview.totalQuestions}</strong>
                </div>

                <div>
                    <span>Score</span>
                    <strong className={getScoreClass(interview.overallScore)}>
                        {interview.overallScore !== null
                            ? `${interview.overallScore.toFixed(1)}/10`
                            : "Pending"}
                    </strong>
                </div>
            </div>

            {topics.length > 0 && (
                <div className="history-topics">
                    {topics.map((topic) => (
                        <span className="history-topic-tag" key={topic}>
                            {topic}
                        </span>
                    ))}
                </div>
            )}

            <div className="history-card-footer">
                {interview.hasReport ? (
                    <Link
                        to={`/report/${interview.sessionId}`}
                        className="history-card-action"
                    >
                        View Report →
                    </Link>
                ) : isInProgress ? (
                    <Link
                        to={`/interview/${interview.sessionId}`}
                        className="history-card-action"
                    >
                        Continue Interview →
                    </Link>
                ) : isCompleted ? (
                    <span className="history-card-muted">
                        Report unavailable
                    </span>
                ) : (
                    <span className="history-card-muted">
                        Interview abandoned
                    </span>
                )}
            </div>
        </article>
    );
}

export function HistoryPage() {
    const [interviews, setInterviews] = useState<
        InterviewHistoryItemResponse[]
    >([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        async function loadHistory() {
            try {
                setError("");

                const response = await getInterviewHistory();

                setInterviews(response.items);
            } catch {
                setError("Unable to load your interview history.");
            } finally {
                setIsLoading(false);
            }
        }

        loadHistory();
    }, []);

    if (isLoading) {
        return (
            <div className="history-page">
                <div className="history-page-header">
                    <p className="dashboard-eyebrow">INTERVIEW HISTORY</p>
                    <h1>Your Interview History</h1>
                    <p>
                        Review your previous interviews and track your progress.
                    </p>
                </div>

                <div className="history-state">
                    Loading your interview history...
                </div>
            </div>
        );
    }

    if (error) {
        return (
            <div className="history-page">
                <div className="history-page-header">
                    <p className="dashboard-eyebrow">INTERVIEW HISTORY</p>
                    <h1>Your Interview History</h1>
                </div>

                <div className="history-state history-state-error">
                    {error}
                </div>
            </div>
        );
    }

    return (
        <div className="history-page">
            <div className="history-page-header">
                <div>
                    <p className="dashboard-eyebrow">INTERVIEW HISTORY</p>

                    <h1>Your Interview History</h1>

                    <p>
                        Review your previous interviews and track your performance
                        over time.
                    </p>
                </div>

                <Link
                    to="/interview/setup"
                    className="history-new-interview"
                >
                    + New Interview
                </Link>
            </div>

            {interviews.length === 0 ? (
                <div className="history-state history-empty">
                    <div className="history-empty-icon">🎯</div>

                    <h2>No interviews yet</h2>

                    <p>
                        Start your first AI-powered interview to see your
                        performance history here.
                    </p>

                    <Link
                        to="/interview/setup"
                        className="history-card-action"
                    >
                        Start Your First Interview →
                    </Link>
                </div>
            ) : (
                <div className="history-list">
                    {interviews.map((interview) => (
                        <HistoryCard
                            key={interview.sessionId}
                            interview={interview}
                        />
                    ))}
                </div>
            )}
        </div>
    );
}