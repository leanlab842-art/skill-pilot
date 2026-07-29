import { createContext, useContext, type ReactNode } from "react";
import { useQueryClient } from "@tanstack/react-query";
import {
  getGetApiV1UsersMeQueryKey,
  useGetApiV1UsersMe,
} from "@/api/generated/profile/profile";
import { postApiV1AuthLogin, postApiV1AuthLogout } from "@/api/generated/auth/auth";
import type { ProfileDto } from "@/api/generated/models";
import { ApiError } from "@/lib/apiClient";

interface AuthContextValue {
  /** ログイン中のユーザー情報。未ログインまたは判定中はundefined。 */
  user: ProfileDto | undefined;
  /** 初回のログイン状態確認(GET /users/me)が完了しているか。 */
  isLoading: boolean;
  isAuthenticated: boolean;
  login: (email: string, password: string) => Promise<void>;
  logout: () => Promise<void>;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

/**
 * ログイン状態を管理するProvider。
 * JWTはhttpOnly CookieのためJSからは読めない。そのため起動時に一度 `GET /users/me` を呼び、
 * 200なら認証済み・401なら未ログインとして状態を初期化する。
 */
export function AuthProvider({ children }: { children: ReactNode }) {
  const queryClient = useQueryClient();

  const {
    data: user,
    isLoading,
    isError,
    error,
  } = useGetApiV1UsersMe({
    query: {
      retry: false,
      staleTime: 5 * 60_000,
    },
  });

  // 401(未ログイン)はエラー表示すべき異常系ではなく「ログインしていない」という正常な状態。
  const isAuthenticated = !isLoading && !isError && user !== undefined;
  if (isError && !(error instanceof ApiError && error.status === 401)) {
    // 401以外(ネットワークエラー等)はコンソールに残しておく。UI側は「未ログイン」として扱う。
    console.error("Failed to fetch current user", error);
  }

  const login = async (email: string, password: string) => {
    await postApiV1AuthLogin({ email, password });
    await queryClient.invalidateQueries({ queryKey: getGetApiV1UsersMeQueryKey() });
  };

  const logout = async () => {
    await postApiV1AuthLogout();
    queryClient.setQueryData(getGetApiV1UsersMeQueryKey(), undefined);
    queryClient.clear();
  };

  return (
    <AuthContext.Provider value={{ user, isLoading, isAuthenticated, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext);
  if (!ctx) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return ctx;
}
