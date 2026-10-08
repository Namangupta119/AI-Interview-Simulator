import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import {
    getInterviewReport,
    type InterviewReportResponse,
} from "../../api/reportApi";

export function ReportPage() {
    const { id: sessionId } = useParams<{ id: string }>();

    const [report, setReport] =
        useState<InterviewReportResponse | null>(null);

    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        if (!sessionId) {
            setError("Interview session ID is missing.");
            setIsLoading(false);
            return;
        }

        const currentSessionId = sessionId;

        async function loadReport() {
            try {
                setError("");

                const response =
                    await getInterviewReport(currentSessionId);

                setReport(response);
            } catch {
                setError(
                    "Unable to load the interview report.",
                );
            } finally {
                setIsLoading(false);
            }
        }

        loadReport();
    }, [sessionId]);

    if (isLoading) {
        return (
            <div className="report-page-header">
                <Link
                    to="/history"
                    className="report-back-link"
                >
                    ← Back to History
                </Link>

                <p className="dashboard-eyebrow">
                    INTERVIEW REPORT
                </p>

                <h1>Your Interview Report</h1>

                <p>
                    Here is a summary of your interview performance
                    and the areas you can improve.
                </p>
            </div>
        );
    }

    if (error) {
        return (
            <div className="report-page">
                <div className="report-page-header">
                    <Link
                        to="/history"
                        className="report-back-link"
                    >
                        ← Back to History
                    </Link>

                    <p className="dashboard-eyebrow">
                        INTERVIEW REPORT
                    </p>

                    <h1>Unable to load report</h1>

                    <p className="setup-error">
                        {error}
                    </p>
                </div>
            </div>
        );
    }

    if (!report) {
        return null;
    }

    const topicScores = JSON.parse(
        report.topicScoresJson,
    ) as Record<string, number>;

    const recommendedTopics =
        report.recommendedTopics
            .split(/\r?\n/)
            .map((topic) => topic.trim())
            .filter(Boolean);

    return (
        <div className="report-page">
            <div className="report-page-header">
    <div className="report-page-header-top">
        <div>
            <p className="dashboard-eyebrow">
                INTERVIEW REPORT
            </p>

            <h1>Your Interview Report</h1>

            <p>
                Here is a summary of your interview performance
                and the areas you can improve.
            </p>
        </div>

        <Link
            to="/history"
            className="report-back-link"
        >
            ← Back to History
        </Link>
    </div>
</div>

            <div className="report-score-card">
                <p>Overall Score</p>

                <strong>
                    {report.overallScore.toFixed(1)}
                    <span>/10</span>
                </strong>
            </div>

            <div className="report-section">
                <h2>Topic Performance</h2>

                <div className="report-topic-list">
                    {Object.entries(topicScores).map(
                        ([topic, score]) => (
                            <div
                                className="report-topic-item"
                                key={topic}
                            >
                                <div className="report-topic-header">
                                    <span>{topic}</span>

                                    <strong>
                                        {score.toFixed(1)}/10
                                    </strong>
                                </div>

                                <div className="report-topic-bar">
                                    <div
                                        className="report-topic-bar-fill"
                                        style={{
                                            width: `${Math.min(
                                                Math.max(score * 10, 0),
                                                100,
                                            )}%`,
                                        }}
                                    />
                                </div>
                            </div>
                        ),
                    )}
                </div>
            </div>

            <div className="report-section">
                <h2>Strengths</h2>

                <p>{report.strengthSummary}</p>
            </div>

            <div className="report-section">
                <h2>Areas to Improve</h2>

                <p>{report.weaknessSummary}</p>
            </div>

            <div className="report-section">
                <h2>Recommended Topics</h2>

                <div className="report-topic-tags">
                    {recommendedTopics.map((topic) => (
                        <span
                            className="report-topic-tag"
                            key={topic}
                        >
                            {topic}
                        </span>
                    ))}
                </div>
            </div>

            <div className="report-section">
                <h2>Improvement Plan</h2>

                <p>{report.improvementPlan}</p>
            </div>
        </div>
    );
}