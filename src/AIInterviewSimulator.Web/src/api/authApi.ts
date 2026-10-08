import { httpClient } from "./httpClient";

export interface RegisterRequest {
  email: string;
  username: string;
  fullName: string;
  password: string;
}

export interface LoginRequest {
  identifier: string;
  password: string;
}

export interface AuthResponse {
  userId: string;
  email: string;
  username: string;
  roles: string[];
  accessToken: string;
  expiresAtUtc: string;
}

export interface CurrentUserResponse {
  userId: string;
  email: string;
  username: string;
  fullName: string;
}

export async function register(
  request: RegisterRequest,
): Promise<void> {
  await httpClient.post("/api/auth/register", request);
}

export async function login(
  request: LoginRequest,
): Promise<AuthResponse> {
  const response = await httpClient.post<AuthResponse>(
    "/api/auth/login",
    request,
  );

  return response.data;
}

export async function getCurrentUser(): Promise<CurrentUserResponse> {
  const response = await httpClient.get<CurrentUserResponse>(
    "/api/auth/me",
  );

  return response.data;
}