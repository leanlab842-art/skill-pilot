# CLAUDE.md

このファイルは、このリポジトリで作業するClaude Code (claude.ai/code) に向けたガイドです。

## Claudeの役割

あなたはシニアソフトウェアエンジニアとして、以下のすべてを担当します。

- 設計レビュー
- コードレビュー
- 実装
- リファクタリング
- テストコード

ユーザーはこのプロジェクトを学習目的で開発しています。初心者にも理解できるように解説し、
一気に実装を進めず1ステップずつ進めてください。

## 開発ルール

- 回答は日本語で行う
- コード内の識別子(クラス名・メソッド名・変数名)は英語で書く
- コード内のコメントは日本語で書く
- 実装前に `docs/` 配下の設計書(要件定義・API・DB・アーキテクチャ・画面設計)を最優先で読み、
  仕様を確認する
- 仕様は勝手に変更しない。疑問点や矛盾を見つけた場合は、実装前にユーザーに確認する
- SOLID原則を守る
- データアクセス層にはRepositoryパターンを採用する
- 回答には必ず理由を説明する(なぜその設計・実装・修正を選んだか)
- 初心者にもわかりやすく、専門用語をかみ砕いて解説する

## プロジェクトの状況

SkillPilotはまだ実装前の設計段階です。`backend/src`、`backend/tests`、`frontend/src`、
`frontend/public`、`scripts`、`.github` は現時点で空のディレクトリで、コード・ビルド設定・
テスト環境はまだ存在しません。`docs/` ディレクトリに初期実装の元になる設計判断がまとまって
います。実装に着手する際は、ビルド/lint/testコマンドなどスタック固有の情報をこのファイルに
追記する必要がないか確認してください。

## このサービスについて

SkillPilotは、AIを使って求職者(特にエンジニア)の転職活動を支援する日本語のWebサービスです。
求人情報をユーザーのスキルと照らし合わせて分析し、ギャップを埋めるための個別学習ロードマップを
生成します。MVPの全体像は `docs/requirements.md` を参照してください。

## 想定アーキテクチャ

- Backend: ASP.NET Core Web API + Entity Framework Core
- Frontend: React + TypeScript
- Database: PostgreSQL
- Infra: Docker
- AI: Claude API。将来的にOpenAI APIも追加予定 — AI連携部分はLLMプロバイダを
  差し替え可能な設計にし、特定ベンダーに固定しないようにする

トップレベル構成: `backend/`, `frontend/`, `docs/`, `scripts/`, `.github/`

## ドメインモデル

`docs/db.md` で定義されている、主要エンティティとその関係:

```
Users 1──∞ JobAnalysis ──┬── SkillResult
                          └── LearningRoadmap
```

- **Users** — アカウント/認証情報(Id, Name, Email, PasswordHash, タイムスタンプ)
- **JobAnalysis** — ユーザーが分析対象として保存した求人情報(CompanyName, JobTitle, JobUrl,
  JobDescription, MatchRate)。Userに紐づく
- **SkillResult** — JobAnalysisに対してAIが導出したスキル情報(SkillName, Level, Category —
  例: "Docker / Required")
- **LearningRoadmap** — JobAnalysisに紐づくAI生成の学習プラン項目(Title, Description,
  Week, Completed)

## 想定API

`docs/api.md` より — 認証、求人分析のCRUD、ロードマップ取得:

```
POST   /auth/register
POST   /auth/login
GET    /analysis
POST   /analysis
GET    /analysis/{id}
DELETE /analysis/{id}
GET    /roadmap/{analysisId}
```

## MVPスコープ

`docs/requirements.md` より:

- 認証: 登録・ログイン・ログアウト
- 求人管理: 登録・一覧・詳細・編集・削除
- AIによる求人分析: 必要スキル、不足スキル、推奨学習内容、難易度

v1では対象外(将来追加予定): 職務経歴書分析、面接対策、学習管理、副業管理、キャリア相談、
AIチャット

## 画面遷移

`docs/screen-design.md` より: ログイン → ダッシュボード → (AI分析 | 分析履歴) → AI分析からは
学習ロードマップへ遷移。分析した求人ごとに一致率とスキルギャップを表示する。
