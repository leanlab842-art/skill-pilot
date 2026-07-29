# API設計

ベースパス: `/api/v1`

---

## 共通仕様

### 認証・認可

- JWTアクセストークンをhttpOnly Cookie(`access_token`、Secure、SameSite=Strict、
  有効期限1時間)で発行・検証する。XSSによるトークン窃取を防ぐため、フロントのJSからは
  参照できない形にする。
- `Users/Me` 系・`Analyses` 系のエンドポイントはすべて認証必須。
- 認可はJWTから取得したUserIdを起点に行う。自分が所有するリソースのみ操作可能。
  他人のリソースIDを指定した場合は `404 Not Found` を返す(存在を推測させないため
  `403` ではなく `404` を用いる)。

### エラーレスポンス形式

```json
{
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "human readable message",
    "details": [{ "field": "email", "message": "..." }]
  }
}
```

### ページネーション

一覧系エンドポイントは `?page=1&pageSize=20` クエリパラメータを受け付け、
レスポンスは以下の形式とする。

```json
{ "items": [], "page": 1, "pageSize": 20, "totalCount": 0 }
```

---

## 認証

| Method | Path | 説明 | 認証 |
|---|---|---|---|
| POST | /api/v1/auth/register | ユーザー登録 | 不要 |
| POST | /api/v1/auth/login | ログイン。Cookieにアクセストークンを発行 | 不要 |
| POST | /api/v1/auth/logout | ログアウト。Cookieを削除 | 必須 |

---

## プロフィール・保有スキル [新規]

| Method | Path | 説明 | 認証 |
|---|---|---|---|
| GET | /api/v1/users/me | 自分のプロフィール取得 | 必須 |
| PUT | /api/v1/users/me | 自分のプロフィール更新(Name) | 必須 |
| GET | /api/v1/users/me/skills | 保有スキル一覧取得 | 必須 |
| POST | /api/v1/users/me/skills | 保有スキル登録 | 必須 |
| PUT | /api/v1/users/me/skills/{skillId} | 保有スキル編集 | 必須 |
| DELETE | /api/v1/users/me/skills/{skillId} | 保有スキル削除 | 必須 |

---

## 求人分析

| Method | Path | 説明 | 認証 |
|---|---|---|---|
| GET | /api/v1/analyses | 求人分析一覧(ページネーション) | 必須 |
| POST | /api/v1/analyses | 求人登録 + AI分析実行(同期) | 必須 |
| GET | /api/v1/analyses/{analysisId} | 求人詳細(SkillResult・LearningRoadmapを含む) | 必須 |
| PUT | /api/v1/analyses/{analysisId} | 求人編集。JobDescription変更時は自動で再分析する | 必須 |
| DELETE | /api/v1/analyses/{analysisId} | 求人削除(論理削除) | 必須 |
| PATCH | /api/v1/analyses/{analysisId}/roadmap/{roadmapId} | 学習ロードマップ項目の完了フラグ更新 | 必須 |

### POST /api/v1/analyses リクエスト例

```json
{
  "companyName": "string",
  "jobTitle": "string",
  "jobUrl": "string | null",
  "jobDescription": "string"
}
```

### GET /api/v1/analyses/{analysisId} レスポンス例

```json
{
  "id": "uuid",
  "companyName": "string",
  "jobTitle": "string",
  "jobUrl": "string | null",
  "status": "Pending | Completed | Failed",
  "matchRate": 72,
  "skillResults": [
    { "id": "uuid", "skillName": "Docker", "level": "Intermediate", "category": "Required", "isMissing": false }
  ],
  "roadmap": [
    { "id": "uuid", "skillResultId": "uuid | null", "title": "string", "week": 1, "completed": false }
  ],
  "createdAt": "ISO8601",
  "updatedAt": "ISO8601"
}
```

### PATCH /api/v1/analyses/{analysisId}/roadmap/{roadmapId} リクエスト例

```json
{ "completed": true }
```

---

## v1で採用した設計方針(前回レビュー反映)

- 全エンドポイントに `/api/v1` プレフィックスを付与し、将来の破壊的変更に備えた
- 求人編集(`PUT /analyses/{id}`)、ログアウト、ロードマップ完了フラグ更新
  (`PATCH .../roadmap/{id}`)を追加し、要件のCRUDとAPIの不整合を解消した
- `/roadmap/{analysisId}` というフラットな設計を `/analyses/{analysisId}/roadmap/{roadmapId}`
  に変更し、リソース階層をRESTfulに整理した
- スキル情報は `/users/me/skills` としてユーザー配下にネストし、所有者が明確な設計にした
- 共通のエラーレスポンス形式・ページネーション仕様・認可ルール(404によるリソース秘匿)を明文化した

---

## 未確定事項(要確認)

- 一覧・詳細レスポンスに含める項目の最終確定(上記は暫定)
- バリデーションルールの詳細(文字数上限、必須項目)
