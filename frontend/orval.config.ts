import { defineConfig } from "orval";

// バックエンド(SkillPilot.WebApi)のSwagger/OpenAPI定義から、型定義とTanStack Queryフックを
// 自動生成する。バックエンドをhttpプロファイルで起動した状態で `npm run generate:api` を実行する。
export default defineConfig({
  skillPilotApi: {
    input: {
      target: "http://localhost:5147/swagger/v1/swagger.json",
    },
    output: {
      mode: "tags-split",
      target: "src/api/generated/endpoints.ts",
      schemas: "src/api/generated/models",
      client: "react-query",
      httpClient: "fetch",
      override: {
        mutator: {
          path: "src/lib/apiClient.ts",
          name: "apiClient",
        },
        // mutator(apiClient)がエラー時に例外を投げ、成功時はレスポンスボディをそのまま返す
        // 設計のため、{ data, status, headers } でラップさせず素の型を返させる。
        fetch: {
          includeHttpResponseReturnType: false,
        },
      },
    },
  },
});
