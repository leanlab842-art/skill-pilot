const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5147";

/** バックエンドの共通エラーレスポンス形式(docs/api.md参照)。 */
interface ApiErrorBody {
  error: {
    code: string;
    message: string;
  };
}

/** APIエラーをコード付きで扱えるようにするための例外型。 */
export class ApiError extends Error {
  readonly status: number;
  readonly code: string;

  constructor(status: number, code: string, message: string) {
    super(message);
    this.name = "ApiError";
    this.status = status;
    this.code = code;
  }
}

/**
 * orvalが生成するコードから呼び出される薄いfetchラッパー(mutator)。
 * Cookie(httpOnly JWT)を送受信するため常に `credentials: "include"` を付与する。
 */
export const apiClient = async <T>(url: string, options: RequestInit = {}): Promise<T> => {
  const response = await fetch(`${API_BASE_URL}${url}`, {
    ...options,
    credentials: "include",
    headers: {
      ...(options.body ? { "Content-Type": "application/json" } : {}),
      ...options.headers,
    },
  });

  if (!response.ok) {
    const body = (await response.json().catch(() => null)) as ApiErrorBody | null;
    throw new ApiError(
      response.status,
      body?.error.code ?? "UNKNOWN_ERROR",
      body?.error.message ?? response.statusText,
    );
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return (await response.json()) as T;
};

export default apiClient;
