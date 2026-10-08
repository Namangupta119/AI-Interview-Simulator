import { httpClient } from "./httpClient";

export interface CreateInterviewSessionRequest {
  role: string;
  experienceLevel: number;
  difficulty: number;
  totalQuestions: number;
  topics: number[];
  customTopics: string[];
}

export interface CreateInterviewSessionResponse {
  sessionId: string;
  role: string;
  experienceLevel: number;
  difficulty: number;
  totalQuestions: number;
  topics: number[];
  customTopics: string[];
  status: number;
  startedAtUtc: string;
}

export async function createInterviewSession(
  request: CreateInterviewSessionRequest
): Promise<CreateInterviewSessionResponse> {
  const response =
    await httpClient.post<CreateInterviewSessionResponse>(
      "/api/interviews/sessions",
      request
    );

  return response.data;
}

export interface GenerateInterviewQuestionResponse {
  questionId: string;
  questionNumber: number;
  totalQuestions: number;
  questionText: string;
  expectedAnswerPoints: string;
  topic: number | null;
  customTopic: string | null;
}

export async function generateNextQuestion(
  sessionId: string,
): Promise<GenerateInterviewQuestionResponse> {
  const response =
    await httpClient.post<GenerateInterviewQuestionResponse>(
      `/api/interviews/sessions/${sessionId}/questions/next`,
    );

  return response.data;
}

export interface SubmitAnswerRequest {
  answerText: string;
}

export async function submitAnswer(
  questionId: string,
  answerText: string,
): Promise<unknown> {
  const response = await httpClient.post(
    `/api/interviews/questions/${questionId}/answer`,
    {
      answerText,
    } satisfies SubmitAnswerRequest,
  );

  return response.data;
}

export async function completeInterview(
  sessionId: string,
): Promise<unknown> {
  const response =
    await httpClient.post(
      `/api/interviews/sessions/${sessionId}/complete`,
    );

  return response.data;
}

export async function getInterviewProgress(
  sessionId: string,
): Promise<InterviewProgressResponse> {
  const response =
    await httpClient.get<InterviewProgressResponse>(
      `/api/interviews/sessions/${sessionId}/progress`,
    );

  return response.data;
}

export interface InterviewQuestionProgress {
  questionNumber: number;
  answered: boolean;
}

export interface InterviewProgressResponse {
  totalQuestions: number;
  answeredQuestions: number;
  currentQuestionNumber: number;
  questions: InterviewQuestionProgress[];
}