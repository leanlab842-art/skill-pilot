import { QueryClient } from "@tanstack/react-query";
import { ApiError } from "@/lib/apiClient";

export const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      // 401(未ログイン)はリトライしても無駄なので、それ以外だけ1回リトライする。
      retry: (failureCount, error) => {
        if (error instanceof ApiError && error.status === 401) return false;
        return failureCount < 1;
      },
      staleTime: 30_000,
    },
  },
});
