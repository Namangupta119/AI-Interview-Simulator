import { httpClient } from "./httpClient";

export interface InterviewHistoryItemResponse {
  sessionId: string;
  role: string;
  experienceLevel: number;
  difficulty: number;
  totalQuestions: number;
  status: number;
  overallScore: number | null;
  startedAtUtc: string;
  completedAtUtc: string | null;
  topics: number[];
  customTopics: string[];
  hasReport: boolean;
}

export interface InterviewHistoryResponse {
  items: InterviewHistoryItemResponse[];
}

export async function getInterviewHistory(): Promise<InterviewHistoryResponse> {
  const response =
    await httpClient.get<InterviewHistoryResponse>(
      "/api/interviews/sessions/history",
    );

  return response.data;
}